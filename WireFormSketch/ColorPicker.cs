using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Wireform.Sketch
{
    public partial class ColorPicker : Form
    {
        SketchForm parent;
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

        private void okButton_Click(object sender, EventArgs e)
        {
            colorPanel.BackColor = color.ColorFromHSV();
            parent.dialogColor = color;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
            => Close();

        private void colorChange_Scroll(object sender, EventArgs e)
        {
            color = new MCvScalar(hueBar.Value, saturationBar.Value, valueBar.Value);
            colorPanel.BackColor = color.ColorFromHSV();
        }
    }
}
