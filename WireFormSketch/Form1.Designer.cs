
namespace Wireform.Sketch
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.CaptureButton = new System.Windows.Forms.Button();
            this.imageBox2 = new Emgu.CV.UI.ImageBox();
            this.DisplayGroupBox = new System.Windows.Forms.GroupBox();
            this.WireformButton = new System.Windows.Forms.RadioButton();
            this.CVDebugbutton = new System.Windows.Forms.RadioButton();
            this.outputButton = new System.Windows.Forms.RadioButton();
            this.wireformPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
            this.DisplayGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageBox1
            // 
            this.imageBox1.Location = new System.Drawing.Point(140, 21);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(921, 678);
            this.imageBox1.TabIndex = 2;
            this.imageBox1.TabStop = false;
            // 
            // CaptureButton
            // 
            this.CaptureButton.Location = new System.Drawing.Point(12, 21);
            this.CaptureButton.Name = "CaptureButton";
            this.CaptureButton.Size = new System.Drawing.Size(94, 29);
            this.CaptureButton.TabIndex = 3;
            this.CaptureButton.Text = "Capture";
            this.CaptureButton.UseVisualStyleBackColor = true;
            this.CaptureButton.Click += new System.EventHandler(this.CaptureButton_Click);
            // 
            // imageBox2
            // 
            this.imageBox2.Location = new System.Drawing.Point(1081, 472);
            this.imageBox2.Name = "imageBox2";
            this.imageBox2.Size = new System.Drawing.Size(486, 334);
            this.imageBox2.TabIndex = 4;
            this.imageBox2.TabStop = false;
            // 
            // DisplayGroupBox
            // 
            this.DisplayGroupBox.Controls.Add(this.WireformButton);
            this.DisplayGroupBox.Controls.Add(this.CVDebugbutton);
            this.DisplayGroupBox.Controls.Add(this.outputButton);
            this.DisplayGroupBox.Location = new System.Drawing.Point(12, 56);
            this.DisplayGroupBox.Name = "DisplayGroupBox";
            this.DisplayGroupBox.Size = new System.Drawing.Size(122, 117);
            this.DisplayGroupBox.TabIndex = 5;
            this.DisplayGroupBox.TabStop = false;
            this.DisplayGroupBox.Text = "Display";
            // 
            // WireformButton
            // 
            this.WireformButton.AutoSize = true;
            this.WireformButton.Location = new System.Drawing.Point(3, 53);
            this.WireformButton.Name = "WireformButton";
            this.WireformButton.Size = new System.Drawing.Size(93, 24);
            this.WireformButton.TabIndex = 2;
            this.WireformButton.Text = "Wireform";
            this.WireformButton.UseVisualStyleBackColor = true;
            this.WireformButton.CheckedChanged += new System.EventHandler(this.Display_CheckedChanged);
            // 
            // CVDebugbutton
            // 
            this.CVDebugbutton.AutoSize = true;
            this.CVDebugbutton.Location = new System.Drawing.Point(3, 83);
            this.CVDebugbutton.Name = "CVDebugbutton";
            this.CVDebugbutton.Size = new System.Drawing.Size(95, 24);
            this.CVDebugbutton.TabIndex = 1;
            this.CVDebugbutton.Text = "Cv Debug";
            this.CVDebugbutton.UseVisualStyleBackColor = true;
            this.CVDebugbutton.CheckedChanged += new System.EventHandler(this.Display_CheckedChanged);
            // 
            // outputButton
            // 
            this.outputButton.AutoSize = true;
            this.outputButton.Checked = true;
            this.outputButton.Location = new System.Drawing.Point(3, 23);
            this.outputButton.Name = "outputButton";
            this.outputButton.Size = new System.Drawing.Size(76, 24);
            this.outputButton.TabIndex = 0;
            this.outputButton.TabStop = true;
            this.outputButton.Text = "Output";
            this.outputButton.UseVisualStyleBackColor = true;
            this.outputButton.CheckedChanged += new System.EventHandler(this.Display_CheckedChanged);
            // 
            // wireformPanel
            // 
            this.wireformPanel.Location = new System.Drawing.Point(1067, 21);
            this.wireformPanel.Name = "wireformPanel";
            this.wireformPanel.Size = new System.Drawing.Size(703, 445);
            this.wireformPanel.TabIndex = 6;
            this.wireformPanel.Click += new System.EventHandler(this.wireformPanel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1782, 853);
            this.Controls.Add(this.wireformPanel);
            this.Controls.Add(this.DisplayGroupBox);
            this.Controls.Add(this.imageBox2);
            this.Controls.Add(this.CaptureButton);
            this.Controls.Add(this.imageBox1);
            this.Location = new System.Drawing.Point(0, 150);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
            this.DisplayGroupBox.ResumeLayout(false);
            this.DisplayGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox1;
        private System.Windows.Forms.Button CaptureButton;
        private Emgu.CV.UI.ImageBox imageBox2;
        private System.Windows.Forms.GroupBox DisplayGroupBox;
        private System.Windows.Forms.RadioButton WireformButton;
        private System.Windows.Forms.RadioButton CVDebugbutton;
        private System.Windows.Forms.RadioButton outputButton;
        private System.Windows.Forms.Panel wireformPanel;
    }
}

