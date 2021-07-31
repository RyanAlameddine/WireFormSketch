using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Sketch.WireformExtras
{
    /// <summary>
    /// A tunnel gate that lets you jump wires over other wires
    /// </summary>
    [Gate("Tunnel")]
    class TunnelGate : Gate
    {
        public TunnelGate(Vec2 position, Direction direction) : base(position, direction, new BoxCollider(0, 0, 0, 0))
        {
            Inputs = new GatePin[] { new GatePin(this, Vec2.Zero) };
            Outputs = new GatePin[] { new GatePin(this, Vec2.Zero) };
        }

        public override BoardObject Copy() => new TunnelGate(StartPoint, Direction);

        protected override void Compute() { }//=> Outputs[0] = Inputs[0];

        protected override async Task DrawGate(PainterScope painter) => await painter.DrawLine(Color.Black, 3, Inputs[0].StartPoint, Outputs[0].StartPoint);
    }
}
