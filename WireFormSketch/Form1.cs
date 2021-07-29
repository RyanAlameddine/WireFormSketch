using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Wireform.Sketch
{
    public partial class Form1 : Form
    {
        VideoCapture capture;
        public Form1()
        {
            InitializeComponent();
        }

        public static ImageBox imagebox;

        readonly WireformSketch sketcher = new WireformSketch(new WireformSketchProperties());

        private void Form1_Load(object sender, EventArgs e)
        {
            capture = new VideoCapture(1);
            capture.Set(CapProp.FrameWidth, 1280);
            capture.Set(CapProp.FrameHeight, 720);
            imagebox = imageBox2;


            Application.Idle += LoadFrame;
        }

        private void LoadFrame(object sender, EventArgs e)
        {
            //load a frame from the camera
            using Mat frame = capture.QueryFrame();
            if (frame == null) return;

            sketcher.ProcessFrame(frame, false);

            SetImageBox(imageBox1, frame);
        }

        private void CaptureButton_Click(object sender, EventArgs e)
        {
            //load a frame from the camera
            using Mat frame = capture.QueryFrame();
            if (frame == null) return;

            sketcher.ProcessFrame(frame, true);

            SetImageBox(imageBox1, frame);
        }

        void SetImageBox(ImageBox imageBox, Mat image)
        {
            imageBox.Image?.Dispose();
            imageBox.Image = image;
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