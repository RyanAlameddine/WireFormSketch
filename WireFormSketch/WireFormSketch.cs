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
using Wireform.Circuitry;
using Wireform.Circuitry.Data.Bits;

namespace Wireform.Sketch
{
    public class WireformSketch
    {
        public readonly WireformSketchProperties Props;
        public WireformSketch(WireformSketchProperties properties)
        {
            Props = properties;
            _ = GateCollection.GatePaths; //to temporarily fix a bug in wireform 
        }

        static readonly Dictionary<GateTraits, string> gatePaths = new()
        {
            { GateTraits.Tunnel, "Tunnel" },
            { GateTraits.Bit, "BitSource" },
            { GateTraits.And, "Logic/AND" },
            { GateTraits.Or , "Logic/OR"  },
            { GateTraits.Xor, "Logic/XOR" },
            { GateTraits.Not, "Logic/NOT" },
            { GateTraits.NAnd, "Logic/NAND" },
            { GateTraits.NOr , "Logic/NOR"  },
            { GateTraits.XNor, "Logic/XNOR" },
        };

        public readonly BoardStack boardStack = new BoardStack(new DebugSaver());

        readonly List<(GateTraits gate, ContourData contourData)> gateRecord = new();
        readonly List<(Point[] contour, Point[] approx)> wireRecord = new();

        /// <summary>
        /// Each index here corresponds to an index in <see cref="wireRecord"/>
        /// </summary>
        readonly List<WireLine> wirePairs = new();

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
            
            if (readCircuit)
            {
                string gateError = FindGates(doc);
                if (gateError is not null) return gateError; //if get error message, return error

                string wireError = FindWires(doc);
                if (wireError is not null) return wireError; //if get error message, return error

                LoadWireform();

                boardStack.CurrentState.Propogate();
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
            using Mat d_blurred = doc.Document.Clone();
            CvInvoke.GaussianBlur(d_blurred, d_blurred, new Size(7, 7), 0);
            //find the gates using an inrange in hsv
            using Mat d_hsv = new Mat();
            CvInvoke.CvtColor(d_blurred, d_hsv, ColorConversion.Bgr2Hsv);
            using Mat d_GateMask = new Mat();
            CvInvoke.InRange(d_hsv, (ScalarArray) Props.GateHsvLowerBound, (ScalarArray) Props.GateHsvUpperBound, d_GateMask);

            //dilate to increase clarity of shapes detected and minimize chance of disconnect
            using Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Dilate(d_GateMask, d_GateMask, element, new Point(-1, -1), Props.GateDilationCount, BorderType.Constant, new MCvScalar(0, 0, 0));

            Form1.imagebox.SetImageBox(d_GateMask);

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
                bool isTunnel = width > 1.3 * height;// && modifier.ApproxC.Length <= 4;
                bool isXorBar = height > 1.3 * width;// && modifier.ApproxC.Length > 4;
                bool isBitPin = Math.Abs(height - width) < Props.BitSourceSizeTolerance && width * height > 100;
                if (isXorBar)
                {
                    int minDistIndex = FindLinker(modifier, gateRecord.Select(g => g.contourData.Centroid.ToPoint()).ToArray(), true); //.Where(g => !g.gate.HasFlag(GateTraits.XorBar))
                    if (minDistIndex == -1) continue;

                    //add xor flag and union bounding rects
                    (GateTraits minGate, ContourData minCont) = gateRecord[minDistIndex];
                    Rectangle unionedRect = minCont.BoundingRect.Union(modifier.BoundingRect);
                    gateRecord[minDistIndex] = (minGate | GateTraits.Bar, new ContourData(minCont.Contour, minCont.ApproxC, unionedRect, minCont.Children, minCont.Centroid, minCont.ArcLength, minCont.LeftEdge));

                } 
                else if (isBitPin)
                {
                    gateRecord.Add((GateTraits.Bit, modifier));
                }
                else if (isTunnel)
                {
                    //find all other modifiers and find closest modifier
                    var modifiersWithoutMe = modifiers.Where(m => m.Contour != modifier.Contour);
                    int minDistIndex = FindLinker(modifier, modifiersWithoutMe
                        .Select(m => m.Centroid.ToPoint())
                        .ToArray(), false); //.Where(g => !g.gate.HasFlag(GateTraits.XorBar))
                    if (minDistIndex == -1) continue;

                    var targetModifier = modifiersWithoutMe.ElementAt(minDistIndex);
                    if (modifier.Centroid.Y > targetModifier.Centroid.Y) continue;

                    //add a tunnel gate with the two unioned
                    Rectangle unionedRect = targetModifier.BoundingRect.Union(modifier.BoundingRect);
                    gateRecord.Add((GateTraits.Tunnel, new ContourData(targetModifier.Contour, targetModifier.ApproxC, unionedRect, targetModifier.Children, targetModifier.Centroid, targetModifier.ArcLength, targetModifier.LeftEdge)));

                }
            }

            return null;
        }

        /// <summary>
        /// Finds the nearest point in the given linker direction. See comment inside for full explanation.
        /// </summary>
        private int FindLinker(ContourData modifier, Point[] targetPoints, bool vertical)
        {
            //a point that should be near the centroid of the gate to link it to. For example:
            //|  |- - -\
            //|  | x o |    (x is the linker point, o is the centroid of the contour
            //|  |- - -/

            //Point linkerPoint = new Point((int)modifier.Centroid.X + (vertical ? modifier.BoundingRect.Height : 0), (int)modifier.Centroid.Y + (!vertical ? modifier.BoundingRect.Width : 0));

            //the maximum allowed distance from the linker point
            double maxDistSqr = vertical ? Math.Pow(modifier.BoundingRect.Height, 2) : Math.Pow(modifier.BoundingRect.Width, 2);
            

            //find the centroid of the gate that is closest to our linker point
            int minDistIndex = targetPoints.ClosestInRange(modifier.Centroid.ToPoint(), maxDistSqr*1.5);

            return minDistIndex;
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

            using VectorOfVectorOfPoint d_wireContours = new VectorOfVectorOfPoint();
            using Mat d_wireHierarchy = new Mat();
            CvInvoke.FindContours(d_wireMask, d_wireContours, d_wireHierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            wireRecord.Clear();
            wirePairs.Clear();
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
            var state = boardStack.CurrentState;
            state.Connections.Clear();
            state.Gates.Clear();
            state.Wires.Clear();

            state.Gates.AddRange(gateRecord.Select(ToGate).Where(g => g is not null));

            //TODO: optimize this from n^2 to something more reasonable (quadtree?)
            var allPins = state.Gates.SelectMany(gate => gate.Inputs.Append(gate.Outputs[0]));
            foreach(var pin in allPins)
            {
                //pair each point on each wire with the index of it's corresponding wire in wireRecord
                IEnumerable<(Point point, int wireIndex, int indexInWire)> wirePoints = wireRecord
                    .Select(wireRec => wireRec.approx)
                    .Zip(Enumerable.Range(0, wireRecord.Count))
                    .SelectMany(((Point[] approx, int i) val) 
                        => val.approx.Zip3(
                            Enumerable.Repeat(val.i, val.approx.Length),
                            Enumerable.Range(0, val.approx.Length)));

                //the index in wireRecord of the wire point closest to the pin
                int index = wirePoints
                    .Select(p => p.point)
                    .ToArray()
                    .ClosestInRange(pin.StartPoint.ToPoint(), Props.GatePinSnapRange);

                if (index == -1) continue;

                (_, int wireIndex, int indexInWire) = wirePoints.ElementAt(index);

                wireRecord[wireIndex].approx[indexInWire] = pin.StartPoint.ToPoint();
            }

            //register wires and add pairs
            state.Wires.AddRange(wireRecord.SelectMany(ToWire));
            for(int i = 0; i < wireRecord.Count; i++)
            {
                var wires = ToWire(wireRecord[i]);
                wirePairs.Add(wires[0]);
                state.Wires.AddRange(wires);
            }

            //add tunnel wires
            state.Wires.AddRange(state.Gates.Where(g => g is TunnelGate).Select(g => new WireLine(g.Inputs[0].StartPoint, g.Outputs[0].StartPoint, true)));

             state.Wires.ForEach(wire => wire.AddConnections(state.Connections));
            
        }

        /// <summary>
        /// Converts a wire data object to a set of wires
        /// </summary>
        private List<WireLine> ToWire((Point[] contour, Point[] approx) wireData)
        {
            (_, Point[] approx) = wireData;
            List<WireLine> wires = new List<WireLine>();

            if (approx.Length == 0) return wires;

            wires.Add(new WireLine(approx[0].ToVec2(), approx[^1].ToVec2(), true));

            for(int i = 0; i < approx.Length - 1; i++)
            {
                wires.Add(new WireLine(approx[i].ToVec2(), approx[i + 1].ToVec2(), true));
            }
            return wires;
        }


        /// <summary>
        /// Converts a gate data object to a Gate.
        /// </summary>
        /// <returns>null if the gate cannot be instantiated</returns>
        private Gate ToGate((GateTraits gate, ContourData contourData) data)
        {
            (GateTraits gate, ContourData contourData) = data;

            if(gatePaths.TryGetValue(gate, out string path))
            {

                Gate newGate = path != "Tunnel" ? GateCollection.CreateGate(path, Vec2.Zero) : new TunnelGate(Vec2.Zero, 0);

                //set gate to center of gate hitbox
                Vec2 centroid = new Vec2((int)contourData.Centroid.X, (int)contourData.Centroid.Y);
                var gateOffsetFromCenterX = (new Vec2(newGate.HitBox.X, newGate.HitBox.Y)*2 + new Vec2(newGate.HitBox.Width, newGate.HitBox.Height))/2;
                newGate.SetPosition(centroid - gateOffsetFromCenterX);

                //Set gate input positions (evenly space the corners and inputs along the left edge) | * * * |
                Rectangle rect = contourData.BoundingRect;
                var inputs = newGate.Inputs;
                int offset = contourData.BoundingRect.Height / (inputs.Length + 1);
                for(int i = 0; i < inputs.Length; i++)
                {
                    inputs[i].SetPosition(new Vec2(rect.X, rect.Y + offset * (i + 1)));
                }

                newGate.Outputs[0].SetPosition(new Vec2(rect.X + rect.Width, rect.Y + rect.Height / 2));

                newGate.AddConnections(boardStack.CurrentState.Connections);

                newGate.LocalHitbox.X = -contourData.BoundingRect.Width / 2;
                newGate.LocalHitbox.Y = -contourData.BoundingRect.Height / 2;
                newGate.LocalHitbox.Width = contourData.BoundingRect.Width;
                newGate.LocalHitbox.Height = contourData.BoundingRect.Height;

                //special case for tunnel
                if(gate == GateTraits.Tunnel)
                {
                    newGate.Inputs[0].SetPosition(new Vec2(rect.X + rect.Width / 2, rect.Y));
                    newGate.Outputs[0].SetPosition(new Vec2(rect.X + rect.Width / 2, rect.Y + rect.Height));
                }

                return newGate;
            }
            return null;
        }

        /// <summary>
        /// Draws all the output onto frame through the document ROI.
        /// </summary>
        private void DrawOutput(Mat frame, DocumentData doc)
        {
            if (Props.DebugDrawCv)
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
            }

            if (Props.DebugDrawWireform)
            {
                
                foreach (var connection in boardStack.CurrentState.Connections)
                {
                    if (connection.Value.Count <= 1 || !connection.Value.Where(x => x is GatePin).Any()) continue;
                    CvInvoke.DrawMarker(doc.Document, new Point((int)connection.Key.X, (int)connection.Key.Y), new MCvScalar(0, 255, 255), MarkerTypes.Star, thickness: 3);
                }

                foreach (var gate in boardStack.CurrentState.Gates)
                {
                    CvInvoke.PutText(doc.Document, gate.GetType().Name.ToString(), new Point((int)gate.StartPoint.X, (int)gate.StartPoint.Y), FontFace.HersheyComplex, 1, new MCvScalar());
                    foreach (var input in gate.Inputs)
                    {
                        CvInvoke.DrawMarker(doc.Document, new Point((int)input.StartPoint.X, (int)input.StartPoint.Y), new MCvScalar(0, 255, 0), MarkerTypes.Cross, thickness: 3);
                    }
                    foreach (var output in gate.Outputs)
                    {
                        CvInvoke.DrawMarker(doc.Document, new Point((int)output.StartPoint.X, (int)output.StartPoint.Y), new MCvScalar(0, 255, 0), MarkerTypes.Diamond, thickness: 3);
                    }
                    CvInvoke.Rectangle(doc.Document, new Rectangle((int)gate.HitBox.X, (int)gate.HitBox.Y, (int)gate.HitBox.Width, (int)gate.HitBox.Height), new MCvScalar(100, 100, 100), 2);
                }

                foreach (var wire in boardStack.CurrentState.Wires)
                {
                    CvInvoke.Line(doc.Document, wire.StartPoint.ToPoint(), wire.EndPoint.ToPoint(), new MCvScalar(100, 100, 100), 3);
                }
            }

            if (Props.DrawOutput)
            {
                for(int i = 0; i < wirePairs.Count; i++)
                {
                    WireLine wire = wirePairs[i];
                    (Point[] contour, _) = wireRecord[i];
                    MCvScalar color = wire.Values[0].Selected switch
                    {
                        BitValue.Error   => Props.BitColors.Error,
                        BitValue.Nothing => Props.BitColors.Nothing,
                        BitValue.One     => Props.BitColors.One,
                        BitValue.Zero    => Props.BitColors.Zero,

                        _ => throw new Exception("BitValue undefined")
                    }; 

                    using VectorOfPoint cont = new VectorOfPoint(contour);
                    CvInvoke.FillPoly(doc.Document, cont, color);
                }
            }

            using Mat documentUnWarped = new Mat(frame.Size, frame.Depth, frame.NumberOfChannels);
            CvInvoke.WarpPerspective(doc.D_Untrimmed, documentUnWarped, doc.F_Transformation, frame.Size, warpType: Warp.InverseMap);

            documentUnWarped.CopyTo(frame, doc.F_DocumentMask);
        }
    }
}
