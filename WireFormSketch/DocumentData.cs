using Emgu.CV;
using System;
using System.Collections.Generic;
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
}
