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

    public partial class Form1 : Form
    {
        delegate List<(GateEnum gate, double fitness)> GateFitnessFunc(VectorOfPoint contour, Rectangle boundingRect, MCvPoint2D64f centroid);
        List<GateFitnessFunc> gateRegistries;

        VideoCapture capture;
        public Form1()
        {
            InitializeComponent();
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
        private void button1_Click(object sender, EventArgs e)
        {
            using Mat frame = capture.QueryFrame();

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

            for(int i = 0; i < frameContours.Size; i++)
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
            CvInvoke.FillPoly(documentOnlyMask, documentContour, new MCvScalar(255, 255, 255));

            double docALen = CvInvoke.ArcLength(documentContour, true);
            //get bounds of the document for perspective transformation
            //TODO: make this adaptive
            CvInvoke.ApproxPolyDP(documentContour, documentContour, .02 * docALen, true);

            //get the corners of the conour (square).
            var (tL, tR, bL, bR) = documentContour.GetCornerPoints(frame.Width, frame.Height);

            PointF[] initialDoc = new PointF[] { tL, tR, bL, bR };

            //PointF[] transformedDoc = new PointF[]
            //{
            //    new PointF(-10        , -10),
            //    new PointF(docWidth+10, -10),
            //    new PointF(-10        , docHeight+10),
            //    new PointF(docWidth+10, docHeight+10)
            //};
            PointF[] transformedDoc = new PointF[]
            {
                new PointF(0        , 0),
                new PointF(docWidth, 0),
                new PointF(0        , docHeight+0),
                new PointF(docWidth, docHeight+0)
            };
            using Mat transformation = CvInvoke.GetPerspectiveTransform(initialDoc, transformedDoc);

            using Mat document = new Mat();
            CvInvoke.WarpPerspective(frame, document, transformation, new Size(docWidth, docHeight));


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
            CvInvoke.Dilate(docGateMask, docGateMask, element, new Point(-1, -1), 4, BorderType.Constant, new MCvScalar(0, 0, 0));

            using VectorOfVectorOfPoint gateContours = new VectorOfVectorOfPoint();
            using Mat h2 = new Mat();
            CvInvoke.FindContours(documentMask, frameContours, h2, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            if (frameContours.Size == 0)
            {
                SetImageBox(imageBox2, docGateMask);
                SetImageBox(imageBox1, frame);
                return;
            }
            CvInvoke.PutText(document, "test text", new Point(10, 100), FontFace.HersheySimplex, 3, new MCvScalar(0, 255, 0), 3);
            for (int i = 0; i < gateContours.Size; i++)
            {
                VectorOfPoint contour = frameContours[i];
                Rectangle rect = CvInvoke.BoundingRectangle(contour);
                Moments moments = CvInvoke.Moments(contour);
                MCvPoint2D64f centroid = moments.GravityCenter;

                var pairs = gateRegistries.SelectMany(func => func(contour, rect, centroid));

                var maxFitness = pairs.Aggregate((x, acc) => x.fitness > acc.fitness ? x : acc);

                CvInvoke.PutText(document, "test text", new Point(0, 0), FontFace.HersheySimplex, 2, new MCvScalar(0, 255, 0), 3);


            }

            using Mat documentUnWarped = new Mat(frame.Size, frame.Depth, frame.NumberOfChannels);
            CvInvoke.WarpPerspective(document, documentUnWarped, transformation, frame.Size, warpType: Warp.InverseMap);

            documentUnWarped.CopyTo(frame, documentOnlyMask);


            SetImageBox(imageBox2, documentOnlyMask);
            SetImageBox(imageBox1, frame);
        }

        void SetImageBox(ImageBox imageBox, Mat image)
        {
            imageBox.Image?.Dispose();
            imageBox.Image = image;
        }
    }
}
