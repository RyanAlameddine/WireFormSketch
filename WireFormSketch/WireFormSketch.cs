using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using OneOf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Wireform;
using System.Text;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.MathUtils;

namespace Wireform.Sketch
{
    public class WireformSketch
    {
        public readonly WireformSketchProperties Props;
        public WireformSketch(WireformSketchProperties properties)
        {
            Props = properties;
            //gateRegistries = new List<GateFitnessFunc>()
            //{
            //    AndGate,
            //    NotGate,
            //    OrGate
            //};
        }

        static readonly Dictionary<GateTraits, string> gatePaths = new()
        {
            { GateTraits.And, "Logic/And"  },
            { GateTraits.Or , "Logic/Or"  },
            { GateTraits.Not, "Logic/Not"  },
        };

        readonly List<(GateTraits gate, ContourData contourData)> gateRecord = new();
        readonly List<(Point[] contour, Point[] approx)> wireRecord = new();
        readonly BoardStack boardStack = new BoardStack(new DebugSaver());

        /// <summary>
        /// The main function of this library. 
        /// Takes in an image representing the current frame to process and draw gates and wires drawn onto.
        /// </summary>
        /// <param name="frame">The current input from the camera (or image loaded in)</param>
        /// <param name="readCircuit">Whether or not gates/wires should be read on this frame.</param>
        public string ProcessFrame(Mat frame, bool readCircuit)
        {

            var docRet = FindDocument(frame);

            if (docRet.IsT0) return docRet.AsT0; //if error message, return error
            using DocumentData doc = docRet.AsT1; //get document data
            //(Mat document, Mat f_DocumentMask, Mat f_Transformation) = documentData;


            if (readCircuit)
            {
                string gateError = FindGates(doc);
                if (gateError is not null) return gateError; //if get error message, return error

                string wireError = FindWires(doc);
                if (wireError is not null) return wireError; //if get error message, return error

                LoadWireform();
            }

            DrawOutput(frame, doc);

            return null;
        }


        /// <summary>
        /// Finds a white, landscape, rectangular document within the input image.
        /// </summary>
        /// <returns><see cref="DocumentData"/> if success, string with error message if failure.</returns>
        private OneOf<string, DocumentData> FindDocument(Mat frame)
        {
            if (frame is null) return "Inputted frame was null";

            //blur slightly for document detection
            using Mat f_blurredFrame = frame.Clone();
            CvInvoke.GaussianBlur(f_blurredFrame, f_blurredFrame, new Size(7, 7), 0);

            //convert to HSV for easier color detection
            using Mat f_hsvFrame = new Mat();
            CvInvoke.CvtColor(f_blurredFrame, f_hsvFrame, ColorConversion.Bgr2Hsv);

            //in range to detect sheet of paper:
            //in the future: make this a dynamic threshold or canny edge or hough line transform?
            using Mat f_documentMask = new Mat();
            CvInvoke.InRange(f_hsvFrame, (ScalarArray)Props.DocumentLowerBound, (ScalarArray)Props.DocumentUpperBound, f_documentMask);

            using VectorOfVectorOfPoint f_contours = new VectorOfVectorOfPoint();
            using Mat f_hierarchy = new Mat();
            CvInvoke.FindContours(f_documentMask, f_contours, f_hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            if (f_contours.Size == 0) return "No possible document contours found. Try changing the document range!";

            //find contour with the largest area
            double largestArea = double.MinValue;
            VectorOfPoint f_documentContour = f_contours[0];

            for (int i = 0; i < f_contours.Size; i++)
            {
                VectorOfPoint f_contour = f_contours[i];
                if (f_contour.Size == 0) continue;
                double area = CvInvoke.ContourArea(f_contour);
                if (area < largestArea) continue;

                largestArea = area;
                f_documentContour = f_contour;
            }

            //get bounds of the document for perspective transformation
            //TODO: make this adaptive
            double docALen = CvInvoke.ArcLength(f_documentContour, true);
            CvInvoke.ApproxPolyDP(f_documentContour, f_documentContour, Props.DocumentApproxEpsilon * docALen, true);

            //get the corners of the conour (square).
            var (tL, tR, bL, bR) = f_documentContour.ToArray().GetCornerPoints();

            PointF[] f_initialDoc = new PointF[] { tL, tR, bL, bR };

            //perfect transformation (padding is now done as Mat ROI)
            PointF[] f_transformedDoc = new PointF[]
            {
                new PointF(0             , 0),
                new PointF(Props.DocWidth, 0),
                new PointF(0             , Props.DocHeight),
                new PointF(Props.DocWidth, Props.DocHeight)
            };
            Mat f_transformation = CvInvoke.GetPerspectiveTransform(f_initialDoc, f_transformedDoc);

            Mat d_untrimmed = new Mat();
            CvInvoke.WarpPerspective(frame, d_untrimmed, f_transformation, new Size(Props.DocWidth, Props.DocHeight));
            Rectangle trimBounds = new Rectangle(Props.DocMargin, Props.DocMargin, d_untrimmed.Width - 2 * Props.DocMargin, d_untrimmed.Height - 2 * Props.DocMargin);
            Mat document = new Mat(d_untrimmed, trimBounds);


            if(Props.DocumentOutlineColor is not null)
            {
                var color = Props.DocumentOutlineColor.Value;
                //CvInvoke.Line(frame, tL, tR, color, 3);
                //CvInvoke.Line(frame, tR, bR, color, 3);
                //CvInvoke.Line(frame, bR, bL, color, 3);
                //CvInvoke.Line(frame, bL, tL, color, 3);

                CvInvoke.PutText(frame, $"Document has {f_documentContour.Size} verticies!", new Point(10, 10), FontFace.HersheyPlain, 1, color);
            }


            //a mask with only the document contour on the frame.
            Mat f_documentOnlyMask = new Mat(frame.Size, DepthType.Cv8U, 1);
            f_documentOnlyMask.SetTo(new MCvScalar());
            CvInvoke.FillPoly(f_documentOnlyMask, f_documentContour, new MCvScalar(255, 255, 255));

            return new DocumentData(document, d_untrimmed, f_documentOnlyMask, f_transformation);
        }

        /// <summary>
        /// Reads the gates from the loaded document.
        /// </summary>
        private string FindGates(DocumentData doc)
        {
            //find the gates using a threshold
            using Mat d_Gray = new Mat();
            CvInvoke.CvtColor(doc.Document, d_Gray, ColorConversion.Bgr2Gray);
            using Mat d_GateMask = new Mat();
            CvInvoke.Threshold(d_Gray, d_GateMask, Props.GateThreshold, 255, ThresholdType.BinaryInv);

            //dilate to increase clarity of shapes detected and minimize chance of disconnect
            using Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Dilate(d_GateMask, d_GateMask, element, new Point(-1, -1), Props.GateDilationCount, BorderType.Constant, new MCvScalar(0, 0, 0));


            //full set of gate contours (including inner contours)
            using VectorOfVectorOfPoint d_gateContours = new VectorOfVectorOfPoint();
            using HierarchyMatrix d_hierarchy = new HierarchyMatrix();
            CvInvoke.FindContours(d_GateMask, d_gateContours, d_hierarchy.Matrix, RetrType.Ccomp, ChainApproxMethod.ChainApproxNone);


            if (d_gateContours.Size == 0) return "No Gate contours found";

            //now that we have found each gate contour, loop through them and process and register each one
            gateRecord.Clear();
            List<ContourData> modifiers = new List<ContourData>(); //things that aren't gates (like xor bars)
            
            for (int i = 0; i != -1; i = d_hierarchy[i, 0])
            {
                //load contour data from contour and hierarchy
                ContourData contourData = ContourData.From(d_gateContours, i, d_hierarchy, Props);


                //if it is on the edge (might be a border problem with the paper itself).
                var rect = contourData.BoundingRect;
                if (rect.X <= 1 || rect.Y <= 1 || rect.Width + rect.X >= d_GateMask.Width - 1 || rect.Height + rect.Y >= d_GateMask.Height - 1)
                {
                    continue;
                }

                //if it is a modifier:
                if (contourData.Children.Count == 0)
                {
                    modifiers.Add(contourData);
                    continue;
                }

                gateRecord.Add((GetTraits(contourData), contourData));
            }

            //check modifiers and link them to their respective gates:
            foreach(var modifier in modifiers)
            {
                int height = modifier.BoundingRect.Height;
                int width = modifier.BoundingRect.Width;
                bool isXorBar = height > 2 * width;
                bool isBitPin = Math.Abs(height - width) < Props.BitSourceSizeTolerance && width * height > 100;
                if (isXorBar)
                {
                    //a point that should be near the centroid of the gate to link it to. For example:
                    //|  |- - -\
                    //|  | x o |    (x is the linker point, o is the centroid of the contour
                    //|  |- - -/
                
                    Point linkerPoint = new Point((int) (modifier.Centroid.X + modifier.BoundingRect.Height), (int)modifier.Centroid.Y);

                    //the maximum allowed distance from the linker point
                    double maxDistSqr = modifier.BoundingRect.Height * modifier.BoundingRect.Height;

                    //find the centroid of the gate that is closest to our linker point
                    int minDistIndex = gateRecord
                        .Where(g => g.gate.HasFlag(GateTraits.XorBar))
                        .Select(g => g.contourData.Centroid)
                        .ToArray()
                        .ClosestInRange(modifier.Centroid, maxDistSqr);

                    if(minDistIndex == -1) continue;

                    //add xor flag and update bounding rect
                    (GateTraits minGate, ContourData minCont) = gateRecord[minDistIndex];

                    Rectangle unionedRect = minCont.BoundingRect.Union(modifier.BoundingRect);
                    gateRecord[minDistIndex] = (minGate | GateTraits.XorBar, new ContourData(minCont.Contour, minCont.ApproxC, unionedRect, minCont.Children, minCont.Centroid, minCont.ArcLength, minCont.LeftEdge));

                } 
                else if (isBitPin)
                {
                    gateRecord.Add((GateTraits.Bit, modifier));
                }                            
            }

            return null;
        }

        /// <summary>
        /// Matches the gate traits of the specified gate contour data.
        /// </summary>
        private static GateTraits GetTraits(ContourData contourData)
        {
            GateTraits traits = GateTraits.NoTraits;
            if (contourData.Children.Count == 0) return traits;
            if (contourData.Children.Count == 1) traits |= GateTraits.NotDotted;
            if (contourData.Children.Count == 2) traits |= GateTraits.Dotted;
            if (contourData.LeftEdge.Count() == 2) traits |= GateTraits.FlatLeftEdge;
            if (contourData.Children.Min.ApproxC.Length == 3) traits |= GateTraits.FirstChildTriangular;
            //XOrBar and Bit are added by modifier handler

            return traits;
        }

        /// <summary>
        /// Loads each wire from the document image
        /// </summary>
        private string FindWires(DocumentData doc)
        {
            using Mat d_blurred = doc.Document.Clone();
            CvInvoke.GaussianBlur(d_blurred, d_blurred, new Size(7, 7), 0);

            using Mat d_hsv = new Mat();
            CvInvoke.CvtColor(d_blurred, d_hsv, ColorConversion.Bgr2Hsv);

            using Mat d_wireMask = new Mat();
            CvInvoke.InRange(d_hsv, (ScalarArray)Props.WireColorLower, (ScalarArray)Props.WireColorUpper, d_wireMask);

            Form1.imagebox.Image?.Dispose();
            Form1.imagebox.Image = d_wireMask;

            using VectorOfVectorOfPoint d_wireContours = new VectorOfVectorOfPoint();
            using Mat d_wireHierarchy = new Mat();
            CvInvoke.FindContours(d_wireMask, d_wireContours, d_wireHierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            wireRecord.Clear();
            for(int i = 0; i < d_wireContours.Size; i++)
            {
                using VectorOfPoint approx = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(d_wireContours[i], approx, Props.WireApproxTrueEpsilon, true);
                wireRecord.Add((d_wireContours[i].ToArray(), approx.ToArray()));
            }

            return null;
        }

        /// <summary>
        /// Loads data into Wireform library classes.
        /// </summary>
        private void LoadWireform()
        {
            boardStack.CurrentState.Connections.Clear();
            boardStack.CurrentState.Gates.Clear();
            boardStack.CurrentState.Wires.Clear();
            boardStack.CurrentState.Gates.AddRange(gateRecord.Select(ToGate).Where(g => g is not null));
        }

        private Gate ToGate((GateTraits gate, ContourData contourData) data)
        {
            (GateTraits gate, ContourData contourData) = data;

            if(gatePaths.TryGetValue(gate, out string path))
            {
                Vec2 centroid = new Vec2((int)contourData.Centroid.X, (int)contourData.Centroid.Y);
                Gate newGate = GateCollection.CreateGate(path, new Vec2(0, 0));
                //center of gate hitbox
                var gateOffsetFromCenterX = (new Vec2(newGate.HitBox.X, newGate.HitBox.Y)*2 + new Vec2(newGate.HitBox.Width, newGate.HitBox.Height))/2;
                newGate.SetPosition(centroid - gateOffsetFromCenterX);

                return newGate;
            }
            return null;
        }

        /// <summary>
        /// Draws all the output onto frame through the document ROI.
        /// </summary>
        private void DrawOutput(Mat frame, DocumentData doc)
        {
            foreach (var gate in gateRecord)
            {
                CvInvoke.Rectangle(doc.Document, gate.contourData.BoundingRect, new MCvScalar(), 3);
                CvInvoke.PutText(doc.Document, gate.gate.ToString(), new Point((int)gate.contourData.Centroid.X, (int)gate.contourData.Centroid.Y), FontFace.HersheySimplex, 1, new MCvScalar(0, 0, 255), 3);

                CvInvoke.Polylines(doc.Document, gate.contourData.ApproxC, true, new MCvScalar(0, 0, 0), 2);
                //draw children for debugging.
                if (Props.DrawGateChildren)
                {
                    int j = 1;
                    foreach (var child in gate.contourData.Children)
                    {
                        CvInvoke.Polylines(doc.Document, child.ApproxC, true, new MCvScalar(255 / j, 0, 0), 2);
                        j++;
                    }
                }
            }

            foreach ((var contour, var approx) in wireRecord)
            {
                CvInvoke.Polylines(doc.Document, contour, true, new MCvScalar(255, 0, 0), 2);
                CvInvoke.Polylines(doc.Document, approx, true, new MCvScalar(255, 255, 0), 2);
            }

            using Mat documentUnWarped = new Mat(frame.Size, frame.Depth, frame.NumberOfChannels);
            CvInvoke.WarpPerspective(doc.D_Untrimmed, documentUnWarped, doc.F_Transformation, frame.Size, warpType: Warp.InverseMap);

            documentUnWarped.CopyTo(frame, doc.F_DocumentMask);
        }
    }
}
