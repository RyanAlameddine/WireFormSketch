﻿
namespace Wireform.Sketch
{
    partial class SketchForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.DebuggerDisplay = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.exposureBar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gateUpperBound = new System.Windows.Forms.Panel();
            this.gateLowerBound = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.wireUpperBound = new System.Windows.Forms.Panel();
            this.wireLowerBound = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.documentUpperBound = new System.Windows.Forms.Panel();
            this.documentLowerBound = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.zeroPanel = new System.Windows.Forms.Panel();
            this.onePanel = new System.Windows.Forms.Panel();
            this.nothingPanel = new System.Windows.Forms.Panel();
            this.errorPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
            this.DisplayGroupBox.SuspendLayout();
            this.DebuggerDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.exposureBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.zeroPanel.SuspendLayout();
            this.onePanel.SuspendLayout();
            this.nothingPanel.SuspendLayout();
            this.errorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageBox1
            // 
            this.imageBox1.Location = new System.Drawing.Point(12, 56);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(1037, 678);
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
            this.imageBox2.Location = new System.Drawing.Point(1206, 491);
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
            this.DisplayGroupBox.Location = new System.Drawing.Point(1070, 472);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1067, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Click on a bitsource here to toggle it!";
            // 
            // DebuggerDisplay
            // 
            this.DebuggerDisplay.Controls.Add(this.radioButton1);
            this.DebuggerDisplay.Controls.Add(this.radioButton2);
            this.DebuggerDisplay.Controls.Add(this.radioButton3);
            this.DebuggerDisplay.Location = new System.Drawing.Point(1067, 595);
            this.DebuggerDisplay.Name = "DebuggerDisplay";
            this.DebuggerDisplay.Size = new System.Drawing.Size(122, 117);
            this.DebuggerDisplay.TabIndex = 6;
            this.DebuggerDisplay.TabStop = false;
            this.DebuggerDisplay.Text = "Debugger";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(3, 53);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(93, 24);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.Text = "Wireform";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(3, 83);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(95, 24);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Cv Debug";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Checked = true;
            this.radioButton3.Location = new System.Drawing.Point(3, 23);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(76, 24);
            this.radioButton3.TabIndex = 0;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Output";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // exposureBar
            // 
            this.exposureBar.Location = new System.Drawing.Point(6, 25);
            this.exposureBar.Minimum = -10;
            this.exposureBar.Name = "exposureBar";
            this.exposureBar.Size = new System.Drawing.Size(130, 56);
            this.exposureBar.TabIndex = 7;
            this.exposureBar.Value = -10;
            this.exposureBar.Scroll += new System.EventHandler(this.exposureBar_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.exposureBar);
            this.groupBox1.Location = new System.Drawing.Point(12, 740);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 82);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exposure";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.gateUpperBound);
            this.groupBox2.Controls.Add(this.gateLowerBound);
            this.groupBox2.Location = new System.Drawing.Point(312, 743);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(140, 82);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Gate Color Range";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "->";
            // 
            // gateUpperBound
            // 
            this.gateUpperBound.Location = new System.Drawing.Point(97, 26);
            this.gateUpperBound.Name = "gateUpperBound";
            this.gateUpperBound.Size = new System.Drawing.Size(37, 47);
            this.gateUpperBound.TabIndex = 1;
            this.gateUpperBound.Click += new System.EventHandler(this.gateUpperBound_Click);
            // 
            // gateLowerBound
            // 
            this.gateLowerBound.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.gateLowerBound.Location = new System.Drawing.Point(6, 26);
            this.gateLowerBound.Name = "gateLowerBound";
            this.gateLowerBound.Size = new System.Drawing.Size(37, 47);
            this.gateLowerBound.TabIndex = 0;
            this.gateLowerBound.Click += new System.EventHandler(this.gateLowerBound_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.wireUpperBound);
            this.groupBox3.Controls.Add(this.wireLowerBound);
            this.groupBox3.Location = new System.Drawing.Point(458, 743);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(140, 82);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Wire Color Range";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "->";
            // 
            // wireUpperBound
            // 
            this.wireUpperBound.Location = new System.Drawing.Point(97, 26);
            this.wireUpperBound.Name = "wireUpperBound";
            this.wireUpperBound.Size = new System.Drawing.Size(37, 47);
            this.wireUpperBound.TabIndex = 1;
            this.wireUpperBound.Click += new System.EventHandler(this.wireUpperBound_Click);
            // 
            // wireLowerBound
            // 
            this.wireLowerBound.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.wireLowerBound.Location = new System.Drawing.Point(6, 26);
            this.wireLowerBound.Name = "wireLowerBound";
            this.wireLowerBound.Size = new System.Drawing.Size(37, 47);
            this.wireLowerBound.TabIndex = 0;
            this.wireLowerBound.Click += new System.EventHandler(this.wireLowerBound_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.documentUpperBound);
            this.groupBox4.Controls.Add(this.documentLowerBound);
            this.groupBox4.Location = new System.Drawing.Point(166, 743);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(140, 82);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Doc Color Range";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 20);
            this.label4.TabIndex = 2;
            this.label4.Text = "->";
            // 
            // documentUpperBound
            // 
            this.documentUpperBound.Location = new System.Drawing.Point(97, 26);
            this.documentUpperBound.Name = "documentUpperBound";
            this.documentUpperBound.Size = new System.Drawing.Size(37, 47);
            this.documentUpperBound.TabIndex = 1;
            this.documentUpperBound.Click += new System.EventHandler(this.documentUpperBound_Click);
            // 
            // documentLowerBound
            // 
            this.documentLowerBound.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.documentLowerBound.Location = new System.Drawing.Point(6, 26);
            this.documentLowerBound.Name = "documentLowerBound";
            this.documentLowerBound.Size = new System.Drawing.Size(37, 47);
            this.documentLowerBound.TabIndex = 0;
            this.documentLowerBound.Click += new System.EventHandler(this.documentLowerBound_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.errorPanel);
            this.groupBox5.Controls.Add(this.nothingPanel);
            this.groupBox5.Controls.Add(this.zeroPanel);
            this.groupBox5.Controls.Add(this.onePanel);
            this.groupBox5.Location = new System.Drawing.Point(858, 743);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(179, 82);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Wire Colors";
            // 
            // zeroPanel
            // 
            this.zeroPanel.Controls.Add(this.label9);
            this.zeroPanel.Location = new System.Drawing.Point(49, 26);
            this.zeroPanel.Name = "zeroPanel";
            this.zeroPanel.Size = new System.Drawing.Size(37, 47);
            this.zeroPanel.TabIndex = 1;
            this.zeroPanel.Click += new System.EventHandler(this.zeroPanel_Click);
            // 
            // onePanel
            // 
            this.onePanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.onePanel.Controls.Add(this.label6);
            this.onePanel.Controls.Add(this.label5);
            this.onePanel.Location = new System.Drawing.Point(6, 26);
            this.onePanel.Name = "onePanel";
            this.onePanel.Size = new System.Drawing.Size(37, 47);
            this.onePanel.TabIndex = 0;
            this.onePanel.Click += new System.EventHandler(this.onePanel_Click);
            // 
            // nothingPanel
            // 
            this.nothingPanel.Controls.Add(this.label10);
            this.nothingPanel.Location = new System.Drawing.Point(92, 26);
            this.nothingPanel.Name = "nothingPanel";
            this.nothingPanel.Size = new System.Drawing.Size(37, 47);
            this.nothingPanel.TabIndex = 2;
            this.nothingPanel.Click += new System.EventHandler(this.nothingPanel_Click);
            // 
            // errorPanel
            // 
            this.errorPanel.Controls.Add(this.label11);
            this.errorPanel.Location = new System.Drawing.Point(135, 26);
            this.errorPanel.Name = "errorPanel";
            this.errorPanel.Size = new System.Drawing.Size(37, 47);
            this.errorPanel.TabIndex = 3;
            this.errorPanel.Click += new System.EventHandler(this.errorPanel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "1";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 20);
            this.label6.TabIndex = 1;
            this.label6.Text = "1";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "1";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 20);
            this.label8.TabIndex = 2;
            this.label8.Text = "1";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 20);
            this.label9.TabIndex = 2;
            this.label9.Text = "0";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(20, 20);
            this.label10.TabIndex = 3;
            this.label10.Text = "N";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 13);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 20);
            this.label11.TabIndex = 4;
            this.label11.Text = "E";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SketchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1782, 828);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.DebuggerDisplay);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.wireformPanel);
            this.Controls.Add(this.imageBox1);
            this.Controls.Add(this.DisplayGroupBox);
            this.Controls.Add(this.imageBox2);
            this.Controls.Add(this.CaptureButton);
            this.Location = new System.Drawing.Point(0, 150);
            this.Name = "SketchForm";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
            this.DisplayGroupBox.ResumeLayout(false);
            this.DisplayGroupBox.PerformLayout();
            this.DebuggerDisplay.ResumeLayout(false);
            this.DebuggerDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.exposureBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.zeroPanel.ResumeLayout(false);
            this.zeroPanel.PerformLayout();
            this.onePanel.ResumeLayout(false);
            this.onePanel.PerformLayout();
            this.nothingPanel.ResumeLayout(false);
            this.nothingPanel.PerformLayout();
            this.errorPanel.ResumeLayout(false);
            this.errorPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox DebuggerDisplay;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.TrackBar exposureBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel gateUpperBound;
        private System.Windows.Forms.Panel gateLowerBound;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel wireUpperBound;
        private System.Windows.Forms.Panel wireLowerBound;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel documentUpperBound;
        private System.Windows.Forms.Panel documentLowerBound;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Panel zeroPanel;
        private System.Windows.Forms.Panel onePanel;
        private System.Windows.Forms.Panel errorPanel;
        private System.Windows.Forms.Panel nothingPanel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}

