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
    /// <summary>
    /// Stores the data and Mats of the document loaded by WireFormSketch.FindDocument
    /// </summary>
    public readonly struct DocumentData : IDisposable
    {
        /// <summary>
        /// The document Mat, untrimmed.
        /// </summary>
        public readonly Mat D_Untrimmed;
        /// <summary>
        /// The document Mat.
        /// </summary>
        public readonly Mat Document;
        /// <summary>
        /// The mask of the document on the original frame mat.
        /// </summary>
        public readonly Mat F_DocumentMask;
        /// <summary>
        /// The perspective transformation of the document performed on the original mat to get DocumentUnTrimmed.
        /// </summary>
        public readonly Mat F_Transformation;

        public DocumentData(Mat document, Mat d_Untrimmed, Mat f_DocumentMask, Mat f_Transformation)
        {
            Document = document;
            D_Untrimmed = d_Untrimmed;
            F_DocumentMask = f_DocumentMask;
            F_Transformation = f_Transformation;
        }

        //public void Deconstruct(out Mat document, out Mat f_DocumentMask, out Mat f_Transformation)
        //{
        //    document = Document;
        //    f_DocumentMask = F_DocumentMask;
        //    f_Transformation = F_Transformation;
        //}

        public void Dispose()
        {
            Document.Dispose();
            D_Untrimmed.Dispose();
            F_DocumentMask.Dispose();
            F_Transformation.Dispose();
        }
    }

    public class WireFormSketch
    {
        public readonly WireFormSketchProperties Props;
        public WireFormSketch(WireFormSketchProperties properties)
        {
            Props = properties;
            gateRegistries = new List<GateFitnessFunc>()
            {
                AndGate,
                NotGate,
                OrGate
            };
        }



        /// <summary>
        /// Finds a white, landscape, rectangular document within the input image.
        /// </summary>
        /// <returns>DocumentData if success, string with error message if failure.</returns>
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
                CvInvoke.Line(frame, tL, tR, color, 3);
                CvInvoke.Line(frame, tR, bR, color, 3);
                CvInvoke.Line(frame, bR, bL, color, 3);
                CvInvoke.Line(frame, bL, tL, color, 3);

                CvInvoke.PutText(frame, $"Document has {f_documentContour.Size} verticies!", new Point(10, 10), FontFace.HersheyPlain, 1, color);
            }


            //a mask with only the document contour on the frame.
            Mat f_documentOnlyMask = new Mat(frame.Size, DepthType.Cv8U, 1);
            f_documentOnlyMask.SetTo(new MCvScalar());
            CvInvoke.FillPoly(f_documentOnlyMask, f_documentContour, new MCvScalar(255, 255, 255));

            return new DocumentData(document, d_untrimmed, f_documentOnlyMask, f_transformation);
        }


        private OneOf<string, List<(GateEnum gate, ContourData contourData)>> FindGates(DocumentData doc)
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
            List<(GateEnum gate, ContourData contourData)> gates = new List<(GateEnum gate, ContourData contourData)>();
            for (int i = 0; i != -1; i = d_hierarchy[i, 0])
            {
                //load contour data from contour and hierarchy
                ContourData contourData = ContourData.From(d_gateContours, i, d_hierarchy);


                //if it is on the edge (might be a border problem with the paper itself).
                var rect = contourData.boundingRect;
                if (rect.X <= 1 || rect.Y <= 1 || rect.Width + rect.X >= d_GateMask.Width - 1 || rect.Height + rect.Y >= d_GateMask.Height - 1)
                {
                    continue;
                }


                //run fitness func on each gate
                var pairs = gateRegistries.SelectMany(func => func(contourData));

                //select highest fitness value
                var (gate, fitness) = pairs.Aggregate((x, acc) => x.fitness > acc.fitness ? x : acc);

                //add gate to registry
                gates.Add((gate, contourData));
            }

            return gates;
        }





        List<(GateEnum gate, ContourData contourData)> gateRecord = new List<(GateEnum gate, ContourData contourData)>();

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
                CvInvoke.PutText(doc.Document, gate.gate.ToString(), new Point((int)gate.contourData.centroid.X, (int)gate.contourData.centroid.Y), FontFace.HersheySimplex, 2, new MCvScalar(0, 0, 255), 3);
            }


            using Mat documentUnWarped = new Mat(frame.Size, frame.Depth, frame.NumberOfChannels);
            CvInvoke.WarpPerspective(doc.D_Untrimmed, documentUnWarped, doc.F_Transformation, frame.Size, warpType: Warp.InverseMap);

            documentUnWarped.CopyTo(frame, doc.F_DocumentMask);


            //SetImageBox(imageBox2, docGateMask);
            //SetImageBox(imageBox1, frame);
            return null;
        }




        delegate List<(GateEnum gate, double fitness)> GateFitnessFunc(ContourData data);

        readonly List<GateFitnessFunc> gateRegistries;

        private List<(GateEnum gate, double fitness)> AndGate(ContourData data)
        {
            double fitness = 0;
            if (data.leftEdge.Count() == 2) //flat left edge
            {
                fitness++;
            }

            if (data.approxC.Length == 4) //four points
            {
                fitness++;

                //get corner points
                (Point topLeft, Point topRight, Point bottomLeft, Point bottomRight) = data.approxC.GetCornerPoints();

                //if has curved front shape
                if (topLeft.Y < topRight.Y) fitness++;
                if (bottomLeft.Y > bottomRight.Y) fitness++;
            }


            return new List<(GateEnum gate, double fitness)>() { (GateEnum.And, fitness) };
        }

        private List<(GateEnum gate, double fitness)> OrGate(ContourData data)
        {
            double fitness = 0;
            if (data.leftEdge.Count() >= 3) //not flat left edge
            {
                fitness += 4;
            }
            return new List<(GateEnum gate, double fitness)>() { (GateEnum.Or, fitness) };
        }

        private List<(GateEnum gate, double fitness)> NotGate(ContourData data)
        {
            double fitness = 0;
            if (data.leftEdge.Count() == 2 && data.children >= 2) //flat left edge
            {
                fitness = 100;
            }

            return new List<(GateEnum gate, double fitness)>() { (GateEnum.Not, fitness) };
        }
    }
}
