using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wireform.Sketch.Utils
{
    public class HierarchyMatrix : IDisposable
    {
        public readonly Mat Matrix;

        public HierarchyMatrix()
        {
            Matrix = new Mat();
        }

        /// <param name="component">next, previous, child, parent</param>
        /// <param name="contourIndex">contour to read from</param>
        public int this[int contourIndex, int component] => Get(component, contourIndex);


        /// <param name="contourIndex">contour to read from</param>
        public (int next, int previous, int firstChild, int parent) this[int contourIndex] => (this[contourIndex, 0], this[contourIndex, 1], this[contourIndex, 2], this[contourIndex, 3]);

        public void Dispose()
        {
            Matrix.Dispose();
        }

        /// <param name="component">next, previous, child, parent</param>
        /// <param name="contourIndex">contour to read from</param>
        int Get(int component, int contourIndex)
        {
            long elementStride = Matrix.ElementSize / sizeof(Int32);
            var offset = (long)component + contourIndex * elementStride;
            if (0 <= offset && offset < Matrix.Total.ToInt64() * elementStride)
            {
                unsafe
                {
                    return *((int*)Matrix.DataPointer.ToPointer() + offset);
                }
            }
            else
            {
                return -1;
            }
        }
    }
}
