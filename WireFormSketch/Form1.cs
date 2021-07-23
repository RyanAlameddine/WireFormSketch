using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireFormSketch
{
    enum GateEnum
    {
        And,
        Or,
        Not
    }

    readonly struct ContourData
    {
        public readonly VectorOfPoint contour;
        public readonly Rectangle boundingRect;
        public readonly Mat gateROI;
        public readonly MCvPoint2D64f centroid;
        public readonly double arcLength;
        public readonly VectorOfPoint approxC;
        public readonly IEnumerable<Point> leftEdge;
        public readonly int children;

        public ContourData(VectorOfPoint contour, VectorOfPoint approxC, Rectangle boundingRect, Mat gateROI, int children, MCvPoint2D64f centroid, double arcLength, IEnumerable<Point> leftEdge)
        {
            this.contour = contour;
            this.boundingRect = boundingRect;
            this.centroid = centroid;
            this.gateROI = gateROI;
            this.arcLength = arcLength;
            this.approxC = approxC;
            this.leftEdge = leftEdge;
            this.children = children;
        }
    }

    public partial class Form1 : Form
    {
        delegate List<(GateEnum gate, double fitness)> GateFitnessFunc(ContourData data);

        readonly List<GateFitnessFunc> gateRegistries;

        VideoCapture capture;
        public Form1()
        {
            InitializeComponent();

            gateRegistries = new List<GateFitnessFunc>()
            {
                AndGate,
                NotGate,
                OrGate
            };
        }

        private List<(GateEnum gate, double fitness)> AndGate(ContourData data)
        {
            double fitness = 0;
            if (data.leftEdge.Count() == 2) //flat left edge
            {
                fitness++;
            }

            if (data.approxC.Size == 4) //four points
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

                //using VectorOfPoint approxC = new VectorOfPoint();
                //CvInvoke.ApproxPolyDP(data.contour, approxC, data.arcLength * 0.04, true);

                //IEnumerable<Point> leftEdge = approxC.ToArray().Where((point) => point.X < data.centroid.X);

                //this almost worked:
                //using VectorOfPoint approxC = new VectorOfPoint();
                //CvInvoke.ApproxPolyDP(data.contour, approxC, data.arcLength * 0.005, true);

                ////VectorOfPoint rightEdge = new VectorOfPoint(data.contour.ToArray().Where((point) => point.X > data.centroid.X).ToArray());
                ////double rightAlen = CvInvoke.ArcLength(rightEdge, true);

                //using VectorOfInt hull = new VectorOfInt();
                ////using Mat convexityDefect = new Mat();
                //CvInvoke.ConvexHull(approxC, hull);
                ////CvInvoke.ConvexityDefects(rightEdge, hull, convexityDefect);

                //Debug.WriteLine((approxC.Size - hull.Size) / (float)approxC.Size);

                ////fitness += (data.contour.Size - hull.Size) / data.arcLength * 5;
                //if((approxC.Size - hull.Size) / (float)approxC.Size > .5f)
                //{
                //    fitness += 5;
                //}







                //if (!convexityDefect.IsEmpty)
                //{
                //    ////Data from Mat are not directly readable so we convert it to Matrix<>
                //    //Matrix<int> m = new Matrix<int>(convexityDefect.Rows, convexityDefect.Cols,
                //    //   convexityDefect.NumberOfChannels);
                //    //convexityDefect.CopyTo(m);

                //    Debug.WriteLine(convexityDefect.Rows + " " + convexityDefect.Cols);
                //}



                //using VectorOfPoint approxC2 = new VectorOfPoint();
                //Point[] points = new Point[approxC.Size];
                //for (int i = 0; i < approxC.Size; i++)
                //{
                //    points[i] = new Point(approxC[i].X - data.boundingRect.X, approxC[i].Y - data.boundingRect.Y);
                //}
                ////Point[] points = rightEdge.Select(x => new Point(x.X - data.boundingRect.X, x.Y - data.boundingRect.Y)).ToArray();

                //approxC2.Push(points);

                //Mat mat = new Mat(data.boundingRect.Size, DepthType.Cv8U, 3);
                //mat.SetTo(new MCvScalar());
                //CvInvoke.FillPoly(mat, approxC2, new MCvScalar(255, 255, 255), LineType.FourConnected);
                //for (int i = 0; i < approxC2.Size; i++)
                //{
                //    CvInvoke.DrawMarker(mat, approxC2[i], new MCvScalar(0, 0, 255), MarkerTypes.Cross);
                //}
                //imageBox3.Image?.Dispose();
                //imageBox3.Image = mat;



                //var rightEdge = data.contour.ToArray().Where((point) => point.X > data.centroid.X).Select(x => new PointF(x.X, x.Y)).ToArray();

                //using VectorOfInt hull = new VectorOfInt();
                //using Mat convexityDefect = new Mat();
                //VectorOfPoint rightEdgePts = new VectorOfPoint(CvInvoke.ConvexHull(rightEdge).Select(x => new Point((int)x.X, (int)x.Y)).ToArray());
                //CvInvoke.ConvexityDefects(rightEdgePts, hull, convexityDefect);

                //if (!convexityDefect.IsEmpty)
                //{
                //    ////Data from Mat are not directly readable so we convert it to Matrix<>
                //    //Matrix<int> m = new Matrix<int>(convexityDefect.Rows, convexityDefect.Cols,
                //    //   convexityDefect.NumberOfChannels);
                //    //convexityDefect.CopyTo(m);

                //    Debug.WriteLine(convexityDefect.Rows + " " + convexityDefect.Cols);
                //}

            }
            //double fitness = 0;

            return new List<(GateEnum gate, double fitness)>() { (GateEnum.Not, fitness) };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            capture = new VideoCapture(1);
            capture.Set(CapProp.FrameWidth, 1280);
            capture.Set(CapProp.FrameHeight, 720);


            Application.Idle += button1_Click;
        }

        //width and height of document subimage
        const int docWidth = 720;
        const int docHeight = 556;

        //margin pixels of document to ignore
        const int docMargin = 10;

        //the amount of dilation to happen on the detected gate contours
        const int dilationCount = 1;
        private void button1_Click(object sender, EventArgs e)
        {
            using Mat frame = capture.QueryFrame();
            if (frame == null) return;

            //CvInvoke.Normalize(frame, frame, 0, 255, NormType.MinMax);

            //blur slightly for document detection
            using Mat blurredFrame = frame.Clone();
            CvInvoke.GaussianBlur(blurredFrame, blurredFrame, new Size(7, 7), 0);

            //convert to HSV for easier color detection
            using Mat hsvFrame = new Mat();
            CvInvoke.CvtColor(blurredFrame, hsvFrame, ColorConversion.Bgr2Hsv);

            //in range to detect sheet of paper

            //in the future: make this a dynamic threshold?
            using Mat documentMask = new Mat();
            CvInvoke.InRange(hsvFrame, (ScalarArray)new MCvScalar(0, 0, 100), (ScalarArray)new MCvScalar(255, 50, 255), documentMask);

            using VectorOfVectorOfPoint frameContours = new VectorOfVectorOfPoint();
            using Mat h1 = new Mat();
            CvInvoke.FindContours(documentMask, frameContours, h1, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            //SetImageBox(imageBox2, documentMask);
            if (frameContours.Size == 0)
            {
                SetImageBox(imageBox1, frame);
                return;
            }

            //find contour with the largest area
            double largestArea = double.MinValue;
            VectorOfPoint documentContour = frameContours[0];

            for (int i = 0; i < frameContours.Size; i++)
            {
                VectorOfPoint contour = frameContours[i];
                if (contour.Size == 0) continue;
                double area = CvInvoke.ContourArea(contour);
                if (area < largestArea) continue;

                largestArea = area;
                documentContour = contour;
            }

            //a mask with only the document contour on the frame.
            using Mat documentOnlyMask = new Mat(frame.Size, DepthType.Cv8U, 1);
            documentOnlyMask.SetTo(new MCvScalar());
            CvInvoke.FillPoly(documentOnlyMask, documentContour, new MCvScalar(255, 255, 255));

            double docALen = CvInvoke.ArcLength(documentContour, true);
            //get bounds of the document for perspective transformation
            //TODO: make this adaptive
            CvInvoke.ApproxPolyDP(documentContour, documentContour, .02 * docALen, true);

            //get the corners of the conour (square).
            var (tL, tR, bL, bR) = documentContour.GetCornerPoints();

            PointF[] initialDoc = new PointF[] { tL, tR, bL, bR };

            ////transformation with 10 pixel padding
            //PointF[] extendedDoc = new PointF[]
            //{
            //    new PointF(-10        , -10),
            //    new PointF(docWidth+10, -10),
            //    new PointF(-10        , docHeight+10),
            //    new PointF(docWidth+10, docHeight+10)
            //};
            //perfect transformation
            PointF[] transformedDoc = new PointF[]
            {
                new PointF(0       , 0),
                new PointF(docWidth, 0),
                new PointF(0       , docHeight),
                new PointF(docWidth, docHeight)
            };
            using Mat transformation = CvInvoke.GetPerspectiveTransform(initialDoc, transformedDoc);

            using Mat docUnTrimmed = new Mat();
            CvInvoke.WarpPerspective(frame, docUnTrimmed, transformation, new Size(docWidth, docHeight));
            using Mat document = new Mat(docUnTrimmed, new Rectangle(docMargin, docMargin, docUnTrimmed.Width - 2 * docMargin, docUnTrimmed.Height - 2 * docMargin));

            //print bounds of document for debugging
            CvInvoke.Line(frame, tL, tR, new MCvScalar(0, 255, 0), 3);
            CvInvoke.Line(frame, tR, bR, new MCvScalar(0, 255, 0), 3);
            CvInvoke.Line(frame, bR, bL, new MCvScalar(0, 255, 0), 3);
            CvInvoke.Line(frame, bL, tL, new MCvScalar(0, 255, 0), 3);

            CvInvoke.PutText(frame, $"Document has {documentContour.Size} verticies!", new Point(10, 10), FontFace.HersheyPlain, 1, new MCvScalar(0, 255, 0));

            //Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));

            //CvInvoke.Dilate(document, document, element, new Point(-1, -1), 6, BorderType.Constant, new MCvScalar(0, 0, 0));

            //using Mat docHsv = new Mat();
            //CvInvoke.CvtColor(document, docHsv, ColorConversion.Bgr2Hsv);

            //using Mat docGateMask = new Mat();
            //CvInvoke.InRange(docHsv, (ScalarArray)new MCvScalar(0, 0, 0), (ScalarArray)new MCvScalar(255, 255, 255*2/3), docGateMask);


            //find the gates using a threshold
            using Mat docGray = new Mat();
            CvInvoke.CvtColor(document, docGray, ColorConversion.Bgr2Gray);
            using Mat docGateMask = new Mat();
            CvInvoke.Threshold(docGray, docGateMask, 110, 255, ThresholdType.BinaryInv);

            //dilate to increase clarity of shapes detected and minimize chance of disconnect
            using Mat element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Dilate(docGateMask, docGateMask, element, new Point(-1, -1), dilationCount, BorderType.Constant, new MCvScalar(0, 0, 0));

            //using VectorOfVectorOfPoint gateContours = new VectorOfVectorOfPoint();
            //using Mat hierarchy = new Mat();
            //CvInvoke.FindContours(docGateMask, gateContours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);

            //full set of gate contours (including inner contours)
            using VectorOfVectorOfPoint gateContours = new VectorOfVectorOfPoint();
            using Mat hierarchy = new Mat();
            CvInvoke.FindContours(docGateMask, gateContours, hierarchy, RetrType.Ccomp, ChainApproxMethod.ChainApproxNone);


            if (gateContours.Size == 0)
            {
                SetImageBox(imageBox2, docGateMask);
                SetImageBox(imageBox1, frame);
                return;
            }
            //CvInvoke.PutText(document, "test text", new Point(10, 100), FontFace.HersheySimplex, 3, new MCvScalar(0, 255, 0), 3);
            for (int i = 0; i != -1; i = Get(hierarchy, 0, i))
            {
                VectorOfPoint contour = gateContours[i]; //external gate contour (the outline)
                Rectangle rect = CvInvoke.BoundingRectangle(contour); //bounding box of the gate contour
                using Mat gateROI = new Mat(docGateMask, rect); //ROI containing the gate contour
                using Moments moments = CvInvoke.Moments(contour); //Contour moments of gate contour
                MCvPoint2D64f centroid = moments.GravityCenter; //center of gate contour
                double arclength = CvInvoke.ArcLength(contour, true); //arc length of gate contour

                using VectorOfPoint approxC = new VectorOfPoint(); //the approximated polygon gate contour
                CvInvoke.ApproxPolyDP(contour, approxC, arclength * 0.04, true);

                IEnumerable<Point> leftEdge = approxC.ToArray().Where((point) => point.X < centroid.X); //the left edge of the contour (<center)

                int children = 0;
                for (int j = Get(hierarchy, 2, i); j != -1; j = Get(hierarchy, 0, j), children++);

                //run fitness func on each gate
                var pairs = gateRegistries.SelectMany(func => func(new ContourData(contour, approxC, rect, gateROI, children, centroid, arclength, leftEdge)));

                //select highest value
                var (gate, fitness) = pairs.Aggregate((x, acc) => x.fitness > acc.fitness ? x : acc);

                CvInvoke.Rectangle(document, rect, new MCvScalar(), 3);
                CvInvoke.PutText(document, gate.ToString(), new Point((int)centroid.X, (int)centroid.Y), FontFace.HersheySimplex, 2, new MCvScalar(0, 0, 255), 3);
                //CvInvoke.PutText(document, children.ToString(), new Point((int)centroid.X, (int)centroid.Y), FontFace.HersheySimplex, 2, new MCvScalar(0, 0, 255), 3);
            }

            using Mat documentUnWarped = new Mat(frame.Size, frame.Depth, frame.NumberOfChannels);
            CvInvoke.WarpPerspective(docUnTrimmed, documentUnWarped, transformation, frame.Size, warpType: Warp.InverseMap);

            documentUnWarped.CopyTo(frame, documentOnlyMask);


            SetImageBox(imageBox2, docGateMask);
            SetImageBox(imageBox1, frame);
        }

        void SetImageBox(ImageBox imageBox, Mat image)
        {
            imageBox.Image?.Dispose();
            imageBox.Image = image;
        }


        /// <param name="component">next, previous, child, parent</param>
        /// <param name="contourIndex">contour to read from</param>
        public int Get(Mat Hierarchy, int component, int contourIndex)
        {
            long elementStride = Hierarchy.ElementSize / sizeof(Int32);
            var offset = (long)component + contourIndex * elementStride;
            if (0 <= offset && offset < Hierarchy.Total.ToInt64() * elementStride)
            {
                unsafe
                {
                    return *((int*)Hierarchy.DataPointer.ToPointer() + offset);
                }
            }
            else
            {
                return -1;
            }
        }
    }
}
