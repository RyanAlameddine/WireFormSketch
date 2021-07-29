using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Wireform.MathUtils;

namespace Wireform.Sketch
{
    static class Extensions
    {
        public static (Point topLeft, Point topRight, Point bottomLeft, Point bottomRight) GetCornerPoints(this Point[] points)
        {
            Point topLeft;
            Point topRight;
            Point bottomLeft;
            Point bottomRight;

            //split to left and right edges by X coordinate (because paper is assumed to be horizontal)
            //this will solve problem of the paper being oriented toward a corner 
            //(a case which makes distance from corner not work as a valid metric)
            static int comparison(Point p1, Point p2) => p1.X.CompareTo(p2.X);
            Array.Sort(points, comparison);

            var lefts  = points.Take(points.Length / 2);
            var rights = points.Skip(points.Length / 2);

            topLeft     = lefts .Aggregate((min, current) => current.Y < min.Y ? current : min);
            topRight    = rights.Aggregate((min, current) => current.Y < min.Y ? current : min);
            bottomLeft  = lefts .Aggregate((max, current) => current.Y > max.Y ? current : max);
            bottomRight = rights.Aggregate((max, current) => current.Y > max.Y ? current : max);

            return (topLeft, topRight, bottomLeft, bottomRight);
        }

        public static double DistanceSqr(this Point point, Point other)
        {
            return Math.Pow(point.X - other.X, 2) + Math.Pow(point.Y - other.Y, 2);
        }

        public static double DistanceSqr(this MCvPoint2D64f point, MCvPoint2D64f other)
        {
            return Math.Pow(point.X - other.X, 2) + Math.Pow(point.Y - other.Y, 2);
        }

        public static Point Add(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }

        public static Rectangle Union(this Rectangle r1, Rectangle r2) {
            int x = Math.Min(r1.X, r2.X);
            int y = Math.Min(r1.Y, r2.Y);
            int xT = Math.Max(r1.X + r1.Width, r2.X + r2.Width); 
            int yT = Math.Max(r1.Y + r1.Height, r2.Y + r2.Height);

            return new Rectangle(x, y, xT - x, yT - y);
        }

        public static int ClosestInRange(this IList<Point> points, Point target, double maxRangeSqrd)
        {
            double minDistSqr = double.MaxValue;
            int minDistIndex = -1;
            for (int i = 0; i < points.Count; i++)
            {

                double distSqr = points[i].DistanceSqr(target);
                if (distSqr > maxRangeSqrd) continue;

                if (distSqr < minDistSqr)
                {
                    minDistSqr = distSqr;
                    minDistIndex = i;
                }
            }

            return minDistIndex;
        }

        public static Vec2 ToVec2(this Point point) => new Vec2(point.X, point.Y);
        public static Point ToPoint(this Vec2 point) => new Point((int) point.X, (int)point.Y);
        public static Point ToPoint(this MCvPoint2D64f point) => new Point((int) point.X, (int)point.Y);

        public static IEnumerable<(T1, T2, T3)> Zip3<T1, T2, T3>(this IEnumerable<T1> source, IEnumerable<T2> second, IEnumerable<T3> third)
        {
            using var e1 = source.GetEnumerator();
            using var e2 = second.GetEnumerator();
            using var e3 = third.GetEnumerator();
            while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
                yield return (e1.Current, e2.Current, e3.Current);
        }
    }
}
