using System;
using System.Collections.Generic;
using System.Text;

namespace WireFormSketch
{
    /// <summary>
    /// The core enum of this algorithm. Defines the traits on which the gates are matched and read.
    /// </summary>
    [Flags]
    public enum GateTraits
    {
        NoTraits = 0,

        NotDotted = 0b00000001, //one open region in contour
        Dotted = 0b00000010, //two open regions in contour
        FlatLeftEdge = 0b00000100, //the left edge is a flat line
        FirstChildTriangular = 0b00001000, //the leftmost open region is a triangle
        XorBar = 0b00010000, //whether it is to the right of a xor bar


        Or = NotDotted,
        Xor = Or | XorBar,
        And = Or | FlatLeftEdge,


        NOr = Dotted,
        XNor = NOr | XorBar,
        NAnd = NOr | FlatLeftEdge,
        Not = NAnd | FirstChildTriangular,

    }
}
