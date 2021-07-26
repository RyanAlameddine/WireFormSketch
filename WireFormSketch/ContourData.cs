using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WireFormSketch
{
    public readonly struct ContourData
    {
        public readonly Point[] contour;
        public readonly Rectangle boundingRect;
        public readonly MCvPoint2D64f centroid;
        public readonly double arcLength;
        public readonly Point[] approxC;
        public readonly IEnumerable<Point> leftEdge;
        public readonly int children;

        public ContourData(VectorOfPoint contour, VectorOfPoint approxC, Rectangle boundingRect, int children, MCvPoint2D64f centroid, double arcLength, IEnumerable<Point> leftEdge)
        {
            this.contour = contour.ToArray();
            this.boundingRect = boundingRect;
            this.centroid = centroid;
            this.arcLength = arcLength;
            this.approxC = approxC.ToArray();
            this.leftEdge = leftEdge;
            this.children = children;
        }

        public static ContourData From(VectorOfVectorOfPoint contours, int i, HierarchyMatrix hierarchy)
        {
            using VectorOfPoint contour = contours[i]; //external gate contour (the outline)
            Rectangle rect = CvInvoke.BoundingRectangle(contour); //bounding box of the gate contour

            //Contour moments of gate contour
            using Moments moments = CvInvoke.Moments(contour);

            //center of gate contour
            MCvPoint2D64f centroid = moments.GravityCenter;

            //arc length of gate contour
            double arclength = CvInvoke.ArcLength(contour, true);

            //the approximated polygon gate contour
            using VectorOfPoint approxC = new VectorOfPoint();
            CvInvoke.ApproxPolyDP(contour, approxC, arclength * 0.04, true);

            //the left edge of the contour (<center)
            IEnumerable<Point> leftEdge = approxC.ToArray().Where((point) => point.X < centroid.X);

            //find child contours of the gate contour
            int children = 0;
            for (int j = hierarchy[i, 2]; j != -1; j = hierarchy[j, 0], children++) ;

            return new ContourData(contour, approxC, rect, children, centroid, arclength, leftEdge);
        }
    }
}
