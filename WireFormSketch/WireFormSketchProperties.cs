using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace WireFormSketch
{
    public class WireFormSketchProperties
    {
        /// <summary>
        /// width of document subimage Mat when transformed
        /// </summary>
        public int DocWidth { get; init; } = 720;

        /// <summary>
        /// height of document subimage Mat when transformed
        /// </summary>
        public int DocHeight { get; init; } = 556;

        /// <summary>
        /// margin pixels of document to ignore
        /// </summary>
        public int DocMargin { get; init; } = 10;

        /// <summary>
        /// The epsilon value for the approximation of the document contour in frame.
        /// </summary>
        public double DocumentApproxEpsilon { get; init; } = .02;

        /// <summary>
        /// The outline color to draw around the detected document. Null if disabled
        /// </summary>
        public MCvScalar? DocumentOutlineColor { get; init; } = new MCvScalar(0, 255, 0);

        /// <summary>
        /// The lower bound color (HSV) for the document detection InRange.
        /// </summary>
        public MCvScalar DocumentLowerBound { get; init; } = new MCvScalar(0, 0, 100);

        /// <summary>
        /// The upper bound color (HSV) for the document detection InRange.
        /// </summary>
        public MCvScalar DocumentUpperBound { get; init; } = new MCvScalar(255, 50, 255);

        /// <summary>
        /// The upper bound for lightness of pixel before it is considered a gate pixel.
        /// </summary>
        public double GateThreshold { get; init; } = 110;



        /// <summary>
        /// the amount of dilation to happen on the detected gate contours
        /// </summary>
        public int GateDilationCount { get; init; } = 1;

        /// <summary>
        /// The epsilon value for the approximation of the gate contours in document.
        /// </summary>
        public double GateApproxEpsilon { get; init; } = .04;

        /// <summary>
        /// The minimum percent area for a child contour of a gate to be considered a child and not just noise.
        /// </summary>
        public double GateChildMinAreaPercent { get; init; } = .005;

        /// <summary>
        /// true if the gate children should be drawn to the screen.
        /// </summary>
        public bool DrawGateChildren { get; init; } = true;
    }
}
