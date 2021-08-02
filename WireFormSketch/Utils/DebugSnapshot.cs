using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Wireform.Sketch.Data;

namespace Wireform.Sketch.Utils
{
    /// <summary>
    /// Takes copies of Mats and stores them for debug viewing purposes.
    /// Only records registers when the readCircuit flag is true
    /// </summary>
    public class DebugSnapshot : IDisposable, IReadOnlyDictionary<string, List<Mat>>
    {
        private readonly Dictionary<string, List<Mat>> snapshots = new();

        private bool readCircuits;

        public List<Mat> this[string key] => ((IReadOnlyDictionary<string, List<Mat>>)snapshots)[key];

        public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, List<Mat>>)snapshots).Keys;

        public IEnumerable<List<Mat>> Values => ((IReadOnlyDictionary<string, List<Mat>>)snapshots).Values;

        public int Count => ((IReadOnlyCollection<KeyValuePair<string, List<Mat>>>)snapshots).Count;

        public bool ContainsKey(string key)=> ((IReadOnlyDictionary<string, List<Mat>>)snapshots).ContainsKey(key);

        public IEnumerator<KeyValuePair<string, List<Mat>>> GetEnumerator() 
            => ((IEnumerable<KeyValuePair<string, List<Mat>>>)snapshots).GetEnumerator();

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out List<Mat> value) 
            => ((IReadOnlyDictionary<string, List<Mat>>)snapshots).TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)snapshots).GetEnumerator();

        /// <summary>
        /// Adds name and mat to snapshot data
        /// </summary>
        /// <param name="name">the name of the mat</param>
        /// <param name="mat">mat to be cloned and stored</param>
        public void RegisterMat(string name, Mat mat)
        {
            if (!readCircuits) return;
            if (!snapshots.ContainsKey(name)) snapshots.Add(name, new List<Mat>());
            snapshots[name].Add(mat.Clone());
        }
        /// <summary>
        /// Adds name and contour data onto <paramref name="on"/> and into snapshot data.
        /// </summary>
        /// <param name="on">The mat to record the contour data on</param>
        public void RegisterContours(string name, VectorOfVectorOfPoint contours, Mat on)
        {
            if (!readCircuits) return;
            Mat target = on.Clone();

            for (int i = 0; i < contours.Size; i++)
            {
                CvInvoke.DrawContours(target, contours, i, new MCvScalar(0, 0, 255), 3);
            }
            RegisterMat(name, target);
        }

        /// <summary>
        /// Adds name and contour data onto <paramref name="on"/> and into snapshot data.
        /// </summary>
        /// <param name="on">The mat to record the contour data on</param>
        public void RegisterContourData(string name, ContourData contourData, Mat on)
        {
            if (!readCircuits) return;
            Mat target = on.Clone();
            CvInvoke.Polylines(target, contourData.Contour, true, new MCvScalar(0, 0, 255), 3);
            CvInvoke.Polylines(target, contourData.ApproxC, true, new MCvScalar(255, 0, 0), 1);
            CvInvoke.Rectangle(target, contourData.BoundingRect, new MCvScalar(), 2);
            CvInvoke.DrawMarker(target, contourData.Centroid.ToPoint(), new MCvScalar(), MarkerTypes.TiltedCross, 2);

            RegisterMat(name, target);

            //TODO REGISTER CHILDREN
            //foreach (var child in contourData.Children)
            //{
            //    CvInvoke.Polylines(target, contourData.ApproxC, true, new MCvScalar(255, 0, 0), 1);
            //}
        }



        /// <summary>
        /// Clears the snapshot if readCircuits.
        /// </summary>
        /// <param name="readCircuits">whether or not inputs should be ignored</param>
        public void Clear(bool readCircuits)
        {
            this.readCircuits = readCircuits;
            if (!readCircuits) return;
            foreach (var mat in snapshots.Values.SelectMany(x => x))
            {
                mat.Dispose();
            }
            snapshots.Clear();
        }

        public void Dispose() => Clear(true);
    }
}
