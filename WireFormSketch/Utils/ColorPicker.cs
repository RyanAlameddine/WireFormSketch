using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Wireform.Sketch.Utils
{
    public partial class ColorPicker : Form
    {
        readonly SketchForm parent;
        MCvScalar color;
        public ColorPicker()
        {
            InitializeComponent();
        }

        public void DisplayAsDialog(MCvScalar startColor)
        {
            colorPanel.BackColor = startColor.ColorFromHSV();
            parent.dialogColor = startColor;
            hueBar.Value = (int)startColor.V0;
            saturationBar.Value = (int)startColor.V1;
            valueBar.Value = (int)startColor.V2;
            ShowDialog();
        }

        public ColorPicker(SketchForm sketchForm) : this()
        {
            parent = sketchForm;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            colorPanel.BackColor = color.ColorFromHSV();
            parent.dialogColor = color;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
            => Close();

        private void ColorChange_Scroll(object sender, EventArgs e)
        {
            color = new MCvScalar(hueBar.Value, saturationBar.Value, valueBar.Value);
            colorPanel.BackColor = color.ColorFromHSV();
        }
    }
}
