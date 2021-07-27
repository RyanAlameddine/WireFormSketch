using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using OneOf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WireFormSketch
{
    public class WireFormSketch
    {
        public readonly WireFormSketchProperties Props;
        public WireFormSketch(WireFormSketchProperties properties)
        {
            Props = properties;
            //gateRegistries = new List<GateFitnessFunc>()
            //{
            //    AndGate,
            //    NotGate,
            //    OrGate
            //};
        }


        List<(GateTraits gate, ContourData contourData)> gateRecord = new List<(GateTraits gate, ContourData contourData)>();

        /// <summary>
        /// The main function of this library. 
        /// Takes in an image representing the current frame to process and draw gates and wires drawn onto.
        /// </summary>
        /// <param name="frame">The current input from the camera (or image loaded in)</param>
        /// <param name="readGates">Whether or not gates should be read on this frame.</param>
        public string ProcessFrame(Mat frame, bool readGates)
        {

            var docRet = FindDocument(frame);

            if (docRet.IsT0) return docRet.AsT0; //if error message, return error
            using DocumentData doc = docRet.AsT1; //get document data
            //(Mat document, Mat f_DocumentMask, Mat f_Transformation) = documentData;


            if (readGates)
            {
                var gateRet = FindGates(doc);
                if (gateRet.IsT0) return gateRet.AsT0; //if get error message, return error
                gateRecord = gateRet.AsT1; //record gate data
            }

            foreach (var gate in gateRecord)
            {
                CvInvoke.Rectangle(doc.Document, gate.contourData.boundingRect, new MCvScalar(), 3);
                CvInvoke.PutText(doc.Document, gate.gate.ToString(), new Point((int)gate.contourData.centroid.X, (int)gate.contourData.centroid.Y), FontFace.HersheySimplex, 1, new MCvScalar(0, 0, 255), 3);
            }


            using Mat documentUnWarped = new Mat(frame.Size, frame.Depth, frame.NumberOfChannels);
            CvInvoke.WarpPerspective(doc.D_Untrimmed, documentUnWarped, doc.F_Transformation, frame.Size, warpType: Warp.InverseMap);

            documentUnWarped.CopyTo(frame, doc.F_DocumentMask);

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
        private OneOf<string, List<(GateTraits gate, ContourData contourData)>> FindGates(DocumentData doc)
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
            List<(GateTraits gate, ContourData contourData)> gates = new List<(GateTraits gate, ContourData contourData)>();
            List<ContourData> modifiers = new List<ContourData>(); //things that aren't gates (like xor bars)
            
            for (int i = 0; i != -1; i = d_hierarchy[i, 0])
            {
                //load contour data from contour and hierarchy
                ContourData contourData = ContourData.From(d_gateContours, i, d_hierarchy, Props);


                //if it is on the edge (might be a border problem with the paper itself).
                var rect = contourData.boundingRect;
                if (rect.X <= 1 || rect.Y <= 1 || rect.Width + rect.X >= d_GateMask.Width - 1 || rect.Height + rect.Y >= d_GateMask.Height - 1)
                {
                    continue;
                }

                //if it is a modifier:
                if (contourData.children.Count == 0)
                {
                    modifiers.Add(contourData);
                    continue;
                }

                //draw children for debugging.
                if (Props.DrawGateChildren)
                {
                    int j = 1;
                    foreach (var child in contourData.children)
                    {
                        CvInvoke.Polylines(doc.Document, child.contour, true, new MCvScalar(255 / j, 0, 0), 2);
                        j++;
                    }
                }

                gates.Add((GetTraits(contourData), contourData));
            }

            //check modifiers and link them to their respective gates:
            foreach(var modifier in modifiers)
            {
                bool isXorBar = modifier.boundingRect.Height > 2 * modifier.boundingRect.Width;
                if (!isXorBar) continue;

                //a point that should be near the centroid of the gate to link it to. For example:
                //|  |- - -\
                //|  | x o |    (x is the linker point, o is the centroid of the contour
                //|  |- - -/
                
                Point linkerPoint = new Point((int) (modifier.centroid.X + modifier.boundingRect.Height), (int)modifier.centroid.Y);

                //the maximum allowed distance from the linker point
                double maxDistSqr = modifier.boundingRect.Height * modifier.boundingRect.Height;

                //find the centroid of the gate that is closest to our linker point
                double minDistSqr = double.MaxValue;
                int minDistIndex = -1;
                for(int i = 0; i < gates.Count; i++)
                {
                    (GateTraits gate, ContourData contourData) = gates[i];

                    double distSqr = contourData.centroid.DistanceSqr(modifier.centroid);
                    if (distSqr > maxDistSqr || gate.HasFlag(GateTraits.XorBar)) continue;

                    if(distSqr < minDistSqr)
                    {
                        minDistSqr = distSqr;
                        minDistIndex = i;
                    }
                }

                if(minDistIndex == -1)
                {
                    CvInvoke.Polylines(doc.Document, modifier.contour, true, new MCvScalar(0, 0, 255), 3);
                    continue;
                }

                //add xor flag and update bounding rect
                (GateTraits minGate, ContourData minCont) = gates[minDistIndex];

                Rectangle unionedRect = minCont.boundingRect.Union(modifier.boundingRect);

                gates[minDistIndex] = (minGate | GateTraits.XorBar, new ContourData(minCont.contour, minCont.approxC, unionedRect, minCont.children, minCont.centroid, minCont.arcLength, minCont.leftEdge));
            
            }

            return gates;
        }

        /// <summary>
        /// Matches the gate traits of the specified gate contour data.
        /// </summary>
        private GateTraits GetTraits(ContourData contourData)
        {
            GateTraits traits = GateTraits.NoTraits;
            if (contourData.children.Count == 0) return traits;
            if (contourData.children.Count == 1) traits |= GateTraits.NotDotted;
            if (contourData.children.Count == 2) traits |= GateTraits.Dotted;
            if (contourData.leftEdge.Count() == 2) traits |= GateTraits.FlatLeftEdge;
            if (contourData.children.Min.approxC.Length == 3) traits |= GateTraits.FirstChildTriangular;
            //XOrBar is added by modifier handler

            return traits;
        }
    }
}
