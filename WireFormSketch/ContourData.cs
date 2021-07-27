using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WireFormSketch
{
    public readonly struct ContourData : IComparable<ContourData>
    {
        /// <summary>
        /// The contour of interest
        /// </summary>
        public readonly Point[] contour;
        /// <summary>
        /// The bounding rect surrounding the contour of interest
        /// </summary>
        public readonly Rectangle boundingRect;
        /// <summary>
        /// The centroid of the contour of interest
        /// </summary>
        public readonly MCvPoint2D64f centroid;
        /// <summary>
        /// The arc length of the contour of interest
        /// </summary>
        public readonly double arcLength;
        /// <summary>
        /// The approximated contour of interest with epsilon <see cref="WireFormSketchProperties.GateApproxEpsilon"/>
        /// </summary>
        public readonly Point[] approxC;
        /// <summary>
        /// The left edge of points on the contour (those with X < centroid.X).
        /// </summary>
        public readonly IEnumerable<Point> leftEdge;
        /// <summary>
        /// The child contours of the contour of interest (sorted by centroid from left to right then top to bottom)
        /// </summary>
        public readonly SortedSet<ContourData> children;

        public ContourData(Point[] contour, Point[] approxC, Rectangle boundingRect, SortedSet<ContourData> children, MCvPoint2D64f centroid, double arcLength, IEnumerable<Point> leftEdge)
        {
            this.contour = contour;
            this.boundingRect = boundingRect;
            this.centroid = centroid;
            this.arcLength = arcLength;
            this.approxC = approxC;
            this.leftEdge = leftEdge;
            this.children = children;
        }

        public static ContourData From(VectorOfVectorOfPoint contours, int i, HierarchyMatrix hierarchy, WireFormSketchProperties props)
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
            CvInvoke.ApproxPolyDP(contour, approxC, arclength * props.GateApproxEpsilon, true);

            //the left edge of the contour (<center)
            IEnumerable<Point> leftEdge = approxC.ToArray().Where((point) => point.X < centroid.X);

            //find child contours of the gate contour
            SortedSet<ContourData> children = new SortedSet<ContourData>();
            for (int j = hierarchy[i, 2]; j != -1; j = hierarchy[j, 0])
            {
                ContourData child = From(contours, j, hierarchy, props);

                //if the child is too small to be considered a child
                if (child.boundingRect.Width * child.boundingRect.Height < props.GateChildMinAreaPercent * rect.Width * rect.Height) continue;

                children.Add(child);
            }

            return new ContourData(contour.ToArray(), approxC.ToArray(), rect, children, centroid, arclength, leftEdge);
        }

        public int CompareTo([AllowNull] ContourData other)
        {
            var xCompare = centroid.X.CompareTo(other.centroid.X);
            return xCompare != 0 ? xCompare : centroid.Y.CompareTo(other.centroid.Y);
        }
    }
}
