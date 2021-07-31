using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Data.Bits;
using Wireform.Circuitry.Gates;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils.Collision;
using Wireform.Sketch.Data;
using Wireform.Sketch.Utils;
using Wireform.Sketch.WireformExtras;

namespace Wireform.Sketch
{
    public partial class SketchForm : Form
    {
        VideoCapture capture;
        public SketchForm()
        {
            InitializeComponent();
        }

        public static ImageBox Imagebox { get; private set; }

        readonly WireformSketch sketcher = new WireformSketch(new WireformSketchProperties()
        {
            
            //DebugDrawWireform = true,
            //DebugDrawCv = true
        });

        private void Form1_Load(object sender, EventArgs e)
        {
            capture = new VideoCapture(1);
            ExposureBar_Scroll(this, null);

            //set properties
            Imagebox = imageBox2;
            capture.Set(CapProp.FrameWidth, 1280);
            capture.Set(CapProp.FrameHeight, 720);

            documentLowerBound.BackColor = sketcher.Props.DocumentHsvLowerBound.ColorFromHSV();
            documentUpperBound.BackColor = sketcher.Props.DocumentHsvUpperBound.ColorFromHSV();

            wireLowerBound.BackColor = sketcher.Props.WireHsvLowerBound.ColorFromHSV();
            wireUpperBound.BackColor = sketcher.Props.WireHsvUpperBound.ColorFromHSV();

            gateLowerBound.BackColor = sketcher.Props.GateHsvLowerBound.ColorFromHSV();
            gateUpperBound.BackColor = sketcher.Props.GateHsvUpperBound.ColorFromHSV();

            onePanel.BackColor = sketcher.Props.BitColors.One.ToColor();
            zeroPanel.BackColor = sketcher.Props.BitColors.Zero.ToColor();
            nothingPanel.BackColor = sketcher.Props.BitColors.Nothing.ToColor();
            errorPanel.BackColor = sketcher.Props.BitColors.Error.ToColor();
           
            //attach event
            Application.Idle += LoadFrame;
        }

        #region Wireform
        bool captureWireform = false;
        private void LoadFrame(object sender, EventArgs e)
        {
            //load a frame from the camera
            using Mat frame = capture.QueryFrame();
            if (frame == null)
            {
                capture.Stop();
                capture.Start();
                return;
            }

            sketcher.ProcessFrame(frame, captureWireform);
            if (captureWireform)
            {
                DrawPanel();
                captureWireform = false;
            }

            imageBox1.SetImageBox(frame);
        }

        private void WireformPanel_Click(object sender, EventArgs e)
        {
            var args = (e as MouseEventArgs);
            Point position = args.Location;
            position = new Point((int)(position.X * 1), (int)(position.Y * 1));

            var mouseCollider = new BoxCollider(position.X, position.Y, 1, 1);
            mouseCollider.GetIntersections(sketcher.boardStack.CurrentState, (true, false, false), out _, out var intersectedObjects, false);

            foreach (var obj in intersectedObjects)
            {
                if (obj is BitSource bit)
                {
                    bit.currentValue = bit.currentValue == BitValue.One ? BitValue.Zero : BitValue.One;
                }
            }

            //sketcher.boardStack.CurrentState.Gates.Add(new BitSource(position.ToVec2(), Direction.Up));

            sketcher.boardStack.CurrentState.Propogate();
            DrawPanel();
        }



        const float scaleDiv = 20;
        private void DrawPanel()
        {
            Bitmap buffer;
            buffer = new Bitmap(this.Width, this.Height);
            //start an async task
            _ = Task.Factory.StartNew(async () =>
            {
                using Graphics g = Graphics.FromImage(buffer);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                PainterScope painter = new PainterScope(new WinformsPainter(g), scaleDiv);
                await Draw(sketcher.boardStack.CurrentState, painter).ConfigureAwait(false);
                //invoke an action against the main thread to draw the buffer to the background image of the main form.
                Invoke(new Action(() => wireformPanel.BackgroundImage = buffer));
            });

        }

        private async Task Draw(BoardState boardState, PainterScope painter)
        {
            //scale objects from paper scale for drawing
            foreach (var boardobj in boardState.BoardObjects)
            {
                boardobj.StartPoint /= scaleDiv;
                if (boardobj is Gate gate) foreach (GatePin pin in gate.Inputs.Union(gate.Outputs)) pin.LocalPoint /= scaleDiv;
                if (boardobj is WireLine wire) wire.EndPoint /= scaleDiv;
            }
            //Draw board objects
            foreach (BoardObject obj in boardState.BoardObjects)
            {
                await obj.Draw(painter, boardState);
            }
            //reset scales
            foreach (var boardobj in boardState.BoardObjects)
            {
                boardobj.StartPoint *= scaleDiv;
                if (boardobj is Gate gate) foreach (GatePin pin in gate.Inputs.Union(gate.Outputs)) pin.LocalPoint *= scaleDiv;
                if (boardobj is WireLine wire) wire.EndPoint *= scaleDiv;
            }
        }
        #endregion

        #region WinformsInput
        /// <summary>
        /// Capture gates next frame
        /// </summary>
        private void CaptureButton_Click(object sender, EventArgs e) => captureWireform = true;

        private void Display_CheckedChanged(object sender, EventArgs e)
        {
            sketcher.Props.DrawOutput = outputButton.Checked;
            sketcher.Props.DebugDrawCv = CVDebugbutton.Checked;
            sketcher.Props.DebugDrawWireform = WireformButton.Checked;
        }

        private void ExposureBar_Scroll(object sender, EventArgs e)
            => capture.Set(CapProp.Exposure, exposureBar.Value);

        public MCvScalar dialogColor = new MCvScalar();
        /// <summary>
        /// Loads a color from the dialog and returns a new one
        /// </summary>
        private MCvScalar GetColorProperty(MCvScalar currentColor, Panel panel)
        {
            ColorPicker colorPicker = new ColorPicker(this);
            colorPicker.DisplayAsDialog(currentColor);
            panel.BackColor = dialogColor.ColorFromHSV();
            return dialogColor;
        }
        private void DocumentLowerBound_Click(object sender, EventArgs e)
            => sketcher.Props.DocumentHsvLowerBound = GetColorProperty(sketcher.Props.DocumentHsvLowerBound, documentLowerBound);
        private void DocumentUpperBound_Click(object sender, EventArgs e)
            => sketcher.Props.DocumentHsvUpperBound = GetColorProperty(sketcher.Props.DocumentHsvUpperBound, documentUpperBound);
        private void GateLowerBound_Click(object sender, EventArgs e)
            => sketcher.Props.GateHsvLowerBound = GetColorProperty(sketcher.Props.GateHsvLowerBound, gateLowerBound);
        private void GateUpperBound_Click(object sender, EventArgs e)
            => sketcher.Props.GateHsvUpperBound = GetColorProperty(sketcher.Props.GateHsvUpperBound, gateUpperBound);
        private void WireLowerBound_Click(object sender, EventArgs e)
            => sketcher.Props.WireHsvLowerBound = GetColorProperty(sketcher.Props.WireHsvLowerBound, wireLowerBound);
        private void WireUpperBound_Click(object sender, EventArgs e)
            => sketcher.Props.WireHsvUpperBound = GetColorProperty(sketcher.Props.WireHsvUpperBound, wireUpperBound);
        private void OnePanel_Click(object sender, EventArgs e)
            => sketcher.Props.BitColors = sketcher.Props.BitColors with { One = GetColorProperty(sketcher.Props.BitColors.One, onePanel) };
        private void ZeroPanel_Click(object sender, EventArgs e)
            => sketcher.Props.BitColors = sketcher.Props.BitColors with { Zero = GetColorProperty(sketcher.Props.BitColors.Zero, zeroPanel) };
        private void NothingPanel_Click(object sender, EventArgs e)
            => sketcher.Props.BitColors = sketcher.Props.BitColors with { Nothing = GetColorProperty(sketcher.Props.BitColors.Nothing, nothingPanel) };
        private void ErrorPanel_Click(object sender, EventArgs e)
            => sketcher.Props.BitColors = sketcher.Props.BitColors with { Error = GetColorProperty(sketcher.Props.BitColors.Error, errorPanel) };


        #endregion Winforms

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            capture.Dispose();
        }
    }
}

//To fix a bug with c# 9 before net5.0:
//TODO: deleteme
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETCOREAPP3_0 || NETCOREAPP3_1 || NET45 || NET451 || NET452 || NET6 || NET461 || NET462 || NET47 || NET471 || NET472 || NET48

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit { }
}

#endif