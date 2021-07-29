using System;
using System.Collections.Generic;
using System.Text;

namespace Wireform.Sketch
{
    /// <summary>
    /// The core enum of this algorithm. Defines the traits on which the gates are matched and read.
    /// </summary>
    [Flags]
    public enum GateTraits
    {
        NoTraits = 0,


        FilledIn             = 0b00000001, ///0 open regions in contour
        NotDotted            = 0b00000010, ///1 open regions in contour
        Dotted               = 0b00000100, ///2 open regions in contour
        FlatLeftEdge         = 0b00001000, ///the left edge is a flat line
        FirstChildTriangular = 0b00010000, ///the leftmost open region is a triangle
        XorBar               = 0b00100000, ///whether it is to the right of a xor bar


        Bit = FilledIn,

        Or  = NotDotted,
        Xor = Or | XorBar,
        And = Or | FlatLeftEdge,


        NOr  = Dotted,
        XNor = NOr  | XorBar,
        NAnd = NOr  | FlatLeftEdge,
        Not  = NAnd | FirstChildTriangular,

    }
}
