using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wireform.Sketch
{
    public class WireformSketchProperties
    {
        /// <summary>
        /// width of document subimage Mat when transformed
        /// </summary>
        public int DocWidth { get; set; } = 720;

        /// <summary>
        /// height of document subimage Mat when transformed
        /// </summary>
        public int DocHeight { get; set; } = 556;

        /// <summary>
        /// margin pixels of document to ignore
        /// </summary>
        public int DocMargin { get; set; } = 10;

        /// <summary>
        /// The epsilon value for the approximation of the document contour in frame.
        /// </summary>
        public double DocumentApproxEpsilon { get; set; } = .02;

        /// <summary>
        /// The outline color to draw around the detected document. Null if disabled
        /// </summary>
        public MCvScalar? DocumentOutlineColor { get; set; } = new MCvScalar(0, 255, 0);

        /// <summary>
        /// The lower bound color (HSV) for the document detection InRange.
        /// </summary>
        public MCvScalar DocumentLowerBound { get; set; } = new MCvScalar(0, 0, 100);

        /// <summary>
        /// The upper bound color (HSV) for the document detection InRange.
        /// </summary>
        public MCvScalar DocumentUpperBound { get; set; } = new MCvScalar(255, 50, 255);

        /// <summary>
        /// The upper bound for lightness of pixel before it is considered a gate pixel.
        /// </summary>
        public double GateThreshold { get; set; } = 110;



        /// <summary>
        /// the amount of dilation to happen on the detected gate contours
        /// </summary>
        public int GateDilationCount { get; set; } = 1;

        /// <summary>
        /// The epsilon value for the approximation of the gate contours in document.
        /// </summary>
        public double GateApproxEpsilon { get; set; } = .04;

        /// <summary>
        /// The minimum percent area for a child contour of a gate to be considered a child and not just noise.
        /// </summary>
        public double GateChildMinAreaPercent { get; set; } = .005;

        /// <summary>
        /// true if the gate children should be drawn to the screen.
        /// </summary>
        public bool DrawGateChildren { get; set; } = true;

        /// <summary>
        /// The lower bound of the wire color detection (hsv).
        /// </summary>
        public MCvScalar WireColorLower { get; set; } = new MCvScalar(160 / 2, 255 / 4, 255 / 2);
        /// <summary>
        /// The upper bound of the wire color detection (hsv).
        /// </summary>
        public MCvScalar WireColorUpper { get; set; } = new MCvScalar(274 / 2, 255, 255);

        /// <summary>
        /// The true (unmodified by arclength) epsilon value to aproximate the wires.
        /// </summary>
        public double WireApproxTrueEpsilon { get; set; } = 5;
        /// <summary>
        /// The amount of pixels off from square a bit source is allowed to be.
        /// </summary>
        public int BitSourceSizeTolerance { get; set; } = 10;

        /// <summary>
        /// The amount of pixels away from a gatePin a wire can be before it snaps to that pin
        /// </summary>
        public double GatePinSnapRange { get; set; } = 100;
        public bool DebugDrawCv { get; set; } = false;
        public bool DebugDrawWireform { get; set; } = false;
    }
}
