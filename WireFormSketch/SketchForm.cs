﻿using Emgu.CV;
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
        VideoCapture capture = null;
        public SketchForm()
        {
            InitializeComponent();
        }


        readonly WireformSketch sketcher = new WireformSketch(new WireformSketchProperties()
        {
            
            //DebugDrawWireform = true,
            //DebugDrawCv = true
        });

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckCameras();
            CreateCapture();

            //debuggerDisplayBox.DisplayMember = "Key";
            //debuggerDisplayBox.ValueMember = "Key";

            //set properties
            documentLowerBound.BackColor = sketcher.Props.DocumentHsvLowerBound.ColorFromHSV();
            documentUpperBound.BackColor = sketcher.Props.DocumentHsvUpperBound.ColorFromHSV();

            wireLowerBound.BackColor = sketcher.Props.WireHsvLowerBound.ColorFromHSV();
            wireUpperBound.BackColor = sketcher.Props.WireHsvUpperBound.ColorFromHSV();

            gateLowerBound.BackColor = sketcher.Props.GateHsvLowerBound.ColorFromHSV();
            gateUpperBound.BackColor = sketcher.Props.GateHsvUpperBound.ColorFromHSV();

            onePanel    .BackColor = sketcher.Props.BitColors.One    .ToColor();
            zeroPanel   .BackColor = sketcher.Props.BitColors.Zero   .ToColor();
            nothingPanel.BackColor = sketcher.Props.BitColors.Nothing.ToColor();
            errorPanel  .BackColor = sketcher.Props.BitColors.Error  .ToColor();
           
            //attach event
            Application.Idle += LoadFrame;
        }

        /// <summary>
        /// Finds all valid cameras
        /// </summary>
        private void CheckCameras()
        {
            bool valid = true;
            for (int i = 0; valid; i++)
            {
                using VideoCapture testCap = new VideoCapture(i);
                if (testCap.IsOpened) cameraIdBox.Items.Add(i);
                else valid = false;
            }
            cameraIdBox.SelectedIndex = 0;
        }

        private void CreateCapture()
        {
            capture?.Dispose();
            //capture = new VideoCapture(1);
            capture = new VideoCapture(int.Parse(cameraIdBox.SelectedItem.ToString()));
            ExposureBar_Scroll(this, null);
            ResolutionChanged(this, null);
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

                var selectedKey = debuggerDisplayBox.SelectedItem;
                debuggerDisplayBox.DataSource = sketcher.snapshot.Keys.ToArray();
                if (selectedKey is not null && sketcher.snapshot.ContainsKey(selectedKey.ToString())) debuggerDisplayBox.SelectedItem = selectedKey;
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
            Bitmap buffer = new Bitmap(wireformPanel.Width, wireformPanel.Height);
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
        private void CameraIdBox_SelectedIndexChanged(object sender, EventArgs e) => CreateCapture();

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
        private MCvScalar GetColorProperty(MCvScalar currentColor, Panel panel, bool hsv)
        {
            ColorPicker colorPicker = new ColorPicker(this);
            colorPicker.DisplayAsDialog(currentColor);
            panel.BackColor = hsv ? dialogColor.ColorFromHSV() : dialogColor.ToColor();
            return dialogColor;
        }
        private void DocumentLowerBound_Click(object sender, EventArgs e)
            => sketcher.Props.DocumentHsvLowerBound = GetColorProperty(sketcher.Props.DocumentHsvLowerBound, documentLowerBound, true);
        private void DocumentUpperBound_Click(object sender, EventArgs e)
            => sketcher.Props.DocumentHsvUpperBound = GetColorProperty(sketcher.Props.DocumentHsvUpperBound, documentUpperBound, true);
        private void GateLowerBound_Click(object sender, EventArgs e)
            => sketcher.Props.GateHsvLowerBound = GetColorProperty(sketcher.Props.GateHsvLowerBound, gateLowerBound, true);
        private void GateUpperBound_Click(object sender, EventArgs e)
            => sketcher.Props.GateHsvUpperBound = GetColorProperty(sketcher.Props.GateHsvUpperBound, gateUpperBound, true);
        private void WireLowerBound_Click(object sender, EventArgs e)
            => sketcher.Props.WireHsvLowerBound = GetColorProperty(sketcher.Props.WireHsvLowerBound, wireLowerBound, true);
        private void WireUpperBound_Click(object sender, EventArgs e)
            => sketcher.Props.WireHsvUpperBound = GetColorProperty(sketcher.Props.WireHsvUpperBound, wireUpperBound, true);
        private void OnePanel_Click(object sender, EventArgs e)
            => sketcher.Props.BitColors = sketcher.Props.BitColors with { One = GetColorProperty(sketcher.Props.BitColors.One, onePanel, false) };
        private void ZeroPanel_Click(object sender, EventArgs e)
            => sketcher.Props.BitColors = sketcher.Props.BitColors with { Zero = GetColorProperty(sketcher.Props.BitColors.Zero, zeroPanel, false) };
        private void NothingPanel_Click(object sender, EventArgs e)
            => sketcher.Props.BitColors = sketcher.Props.BitColors with { Nothing = GetColorProperty(sketcher.Props.BitColors.Nothing, nothingPanel, false) };
        private void ErrorPanel_Click(object sender, EventArgs e)
            => sketcher.Props.BitColors = sketcher.Props.BitColors with { Error = GetColorProperty(sketcher.Props.BitColors.Error, errorPanel, false) };
        private void ResolutionChanged(object sender, EventArgs e)
        {
            capture.Set(CapProp.FrameWidth, (int)resolutionWidthBox.Value);
            capture.Set(CapProp.FrameHeight, (int)resolutionHeightBox.Value);
        }

        private void DebuggerDisplayBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            debuggerNumberPicker.Value = 0;
            debuggerNumberPicker.Maximum = sketcher.snapshot[debuggerDisplayBox.SelectedItem.ToString()].Count - 1;
            LoadDebugImage();
        }

        private void DebuggerNumberPicker_ValueChanged(object sender, EventArgs e)
        {
            LoadDebugImage();
        }

        private void LoadDebugImage()
        {
            imageBox2.Image = sketcher.snapshot[debuggerDisplayBox.SelectedItem.ToString()][(int)debuggerNumberPicker.Value];
        }

        #endregion Winforms

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                foreach (var key in sketcher.snapshot.Keys)
                {
                    var mats = sketcher.snapshot[key];
                    string nameEdited = key.Replace('/', '.');
                    for (int i = 0; i < mats.Count; i++)
                    {
                        string name = $"{fbd.SelectedPath}\\{nameEdited}{i}.png";
                        CvInvoke.Imwrite(name, mats[i]);
                    }
                }
            }

            using (Mat frame = capture.QueryFrame())
            {
                sketcher.Props.DebugDrawCv = false;
                sketcher.Props.DebugDrawWireform = false;
                sketcher.Props.DrawOutput = true;
                sketcher.ProcessFrame(frame, false);
                string name = $"{fbd.SelectedPath}\\out.png";
                CvInvoke.Imwrite(name, frame);
            };

            using (Mat frame = capture.QueryFrame())
            {
                sketcher.Props.DebugDrawCv = false;
                sketcher.Props.DebugDrawWireform = true;
                sketcher.Props.DrawOutput = false;
                sketcher.ProcessFrame(frame, false);
                string name = $"{fbd.SelectedPath}\\outWireform.png";
                CvInvoke.Imwrite(name, frame);
            };

            using (Mat frame = capture.QueryFrame())
            {
                sketcher.Props.DebugDrawCv = true;
                sketcher.Props.DebugDrawWireform = false;
                sketcher.Props.DrawOutput = false;
                sketcher.ProcessFrame(frame, false);
                string name = $"{fbd.SelectedPath}\\outCv.png";
                CvInvoke.Imwrite(name, frame);
            };

            Display_CheckedChanged(this, null);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            capture.Dispose();
            sketcher.Dispose();
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