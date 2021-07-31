using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using Wireform.Sketch.Utils;

namespace Wireform.Sketch.Data
{
    public readonly struct ContourData : IComparable<ContourData>
    {
        /// <summary>
        /// The contour of interest
        /// </summary>
        public readonly Point[] Contour { get; init; }
        /// <summary>
        /// The bounding rect surrounding the contour of interest
        /// </summary>
        public readonly Rectangle BoundingRect { get; init; }
        /// <summary>
        /// The centroid of the contour of interest
        /// </summary>
        public readonly MCvPoint2D64f Centroid { get; init; }
        /// <summary>
        /// The arc length of the contour of interest
        /// </summary>
        public readonly double ArcLength { get; init; }
        /// <summary>
        /// The approximated contour of interest with epsilon <see cref="WireformSketchProperties.GateApproxEpsilon"/>
        /// </summary>
        public readonly Point[] ApproxC { get; init; }
        /// <summary>
        /// The left edge of points on the contour (those with X < centroid.X).
        /// </summary>
        public readonly IEnumerable<Point> LeftEdge { get; init; }
        /// <summary>
        /// The child contours of the contour of interest (sorted by centroid from left to right then top to bottom)
        /// </summary>
        public readonly SortedSet<ContourData> Children { get; init; }

        public ContourData(Point[] contour, Point[] approxC, Rectangle boundingRect, SortedSet<ContourData> children, MCvPoint2D64f centroid, double arcLength, IEnumerable<Point> leftEdge)
        {
            this.Contour = contour;
            this.BoundingRect = boundingRect;
            this.Centroid = centroid;
            this.ArcLength = arcLength;
            this.ApproxC = approxC;
            this.LeftEdge = leftEdge;
            this.Children = children;
        }

        public static ContourData From(VectorOfVectorOfPoint contours, int i, HierarchyMatrix hierarchy, WireformSketchProperties props)
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
                if (child.BoundingRect.Width * child.BoundingRect.Height < props.GateChildMinAreaPercent * rect.Width * rect.Height) continue;

                children.Add(child);
            }

            return new ContourData(contour.ToArray(), approxC.ToArray(), rect, children, centroid, arclength, leftEdge);
        }

        public int CompareTo([AllowNull] ContourData other)
        {
            var xCompare = Centroid.X.CompareTo(other.Centroid.X);
            return xCompare != 0 ? xCompare : Centroid.Y.CompareTo(other.Centroid.Y);
        }
    }
}
