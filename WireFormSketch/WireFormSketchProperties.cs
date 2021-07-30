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
        public MCvScalar DocumentHsvLowerBound { get; set; } = new MCvScalar(0, 0, 100);

        /// <summary>
        /// The upper bound color (HSV) for the document detection InRange.
        /// </summary>
        public MCvScalar DocumentHsvUpperBound { get; set; } = new MCvScalar(180, 50, 255);

        /// <summary>
        /// The lower bound for the gate color dection (hsv).
        /// </summary>
        public MCvScalar GateHsvLowerBound { get; set; } = new MCvScalar(0, 0, 0);
        /// <summary>
        /// The upper bound for the gate color dection (hsv).
        /// </summary>
        public MCvScalar GateHsvUpperBound { get; set; } = new MCvScalar(180, 255, 100);

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
        public MCvScalar WireHsvLowerBound { get; set; } = new MCvScalar(160 / 2, 255 / 4, 255 / 2);
        /// <summary>
        /// The upper bound of the wire color detection (hsv).
        /// </summary>
        public MCvScalar WireHsvUpperBound { get; set; } = new MCvScalar(274 / 2, 255, 255);

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
        public double GatePinSnapRange { get; set; } = 200;

        /// <summary>
        /// If true, will draw contour object outlines, positions, and gate traits.
        /// </summary>
        public bool DebugDrawCv { get; set; } = false;

        /// <summary>
        /// If true, will draw output confirming what the wireform library has processed about the cv data.
        /// Also displays connection information.
        /// </summary>
        public bool DebugDrawWireform { get; set; } = false;

        /// <summary>
        /// If true, will draw the regular output onto the document mat on the frame.
        /// </summary>
        public bool DrawOutput { get; set; } = true;

        /// <summary>
        /// The colors of each bit.
        /// </summary>
        public BitColors BitColors { get; set; }
            = new BitColors(Error: new MCvScalar(0, 0, 255/3), Nothing: new MCvScalar(105, 105, 105), One: new MCvScalar(180, 40, 199), Zero: new MCvScalar(94, 27, 66));
    }

    public record BitColors(MCvScalar Error, MCvScalar Nothing, MCvScalar One, MCvScalar Zero);
}
