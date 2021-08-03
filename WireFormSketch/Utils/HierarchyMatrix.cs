using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wireform.Sketch.Utils
{
    public class HierarchyMatrix : IDisposable, IOutputArray
    {
        /// <summary>
        /// The backing Mat that stores the actual hierarchy data
        /// </summary>
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


        /// <param name="component">next, previous, child, parent</param>
        /// <param name="contourIndex">contour to read from</param>
        int Get(int component, int contourIndex)
        {
            //element stride is the amount of ints wide each element is
            long elementStride = Matrix.ElementSize / sizeof(Int32);
            //offset is the position within the matrix 
            //(think a 2D array of a type with a size of elementStride.
            var offset = component + contourIndex * elementStride;

            //if the index is within bounds
            if (offset >= 0 && offset < Matrix.Total.ToInt64() * elementStride)
            {
                unsafe
                {
                    //get the value that is at the matrix array pointer + offset
                    return *((int*)Matrix.DataPointer.ToPointer() + offset);
                }
            }
            return -1;
        }

        public OutputArray GetOutputArray() => ((IOutputArray)Matrix).GetOutputArray();

        public InputArray GetInputArray() => ((IInputArray)Matrix).GetInputArray();

        public void Dispose() => Matrix.Dispose();
    }
}
