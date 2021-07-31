using System.Drawing;
using System.Threading.Tasks;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;

namespace Wireform.Sketch.WireformExtras
{
    /// <summary>
    /// A tool used to draw on the circuit board.
    /// Each painter contains a reference to the current Graphics class
    /// as well as the current zoom value.
    /// Painters also contain an internal offset value which will be applied to all position elements
    /// and a multiplier which determines rotational values of drawn gates
    /// </summary>
    class WinformsPainter : IPainter
    {
        readonly Graphics gfx;
        public WinformsPainter(Graphics gfx)
        {
            this.gfx = gfx;
        }

        Pen GetPen(Color color, int width)
        {
            return new Pen(color, width);
        }

        Brush GetEmptyBrush(Color color)
        {
            return (new Pen(color, 1)).Brush;
        }

        public Task DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint)
        {
            gfx.DrawLine(GetPen(color, penWidth), startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
            return Task.CompletedTask;
        }

        public Task DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            gfx.DrawArc(GetPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y, startAngle, sweepAngle);
            return Task.CompletedTask;
        }

        public Task DrawEllipse(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            gfx.DrawEllipse(GetPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y);
            return Task.CompletedTask;
        }

        public Task FillEllipse(Color color, Vec2 startPoint, Vec2 size)
        {
            gfx.FillEllipse(GetEmptyBrush(color), startPoint.X, startPoint.Y, size.X, size.Y);
            return Task.CompletedTask;
        }

        public Task DrawRectangle(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            gfx.DrawRectangle(GetPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y);
            return Task.CompletedTask;
        }

        public Task FillRectangle(Color color, Vec2 startPoint, Vec2 size)
        {
            gfx.FillRectangle(GetEmptyBrush(color), startPoint.X, startPoint.Y, size.X, size.Y);
            return Task.CompletedTask;
        }

        public Task DrawString(string s, Color color, Vec2 startPoint, float scale)
        {
            Font font = new Font(FontFamily.GenericMonospace, scale, FontStyle.Bold);
            gfx.DrawString(s, font, new Pen(color, 10).Brush, startPoint.X, startPoint.Y);
            return Task.CompletedTask;
        }

        public Task<Vec2> MeasureString(string s, float zoom, float scale)
        {
            Font font = new Font(FontFamily.GenericMonospace, scale, FontStyle.Bold);
            var size = gfx.MeasureString(s, font);
            return Task.FromResult(new Vec2(size.Width, size.Height));
        }
    }
}