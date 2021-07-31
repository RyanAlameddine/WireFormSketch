
namespace Wireform.Sketch.Utils
{
    partial class ColorPicker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hueBar = new System.Windows.Forms.TrackBar();
            this.colorPanel = new System.Windows.Forms.Panel();
            this.huePanel = new System.Windows.Forms.GroupBox();
            this.saturationPanel = new System.Windows.Forms.GroupBox();
            this.saturationBar = new System.Windows.Forms.TrackBar();
            this.valuePanel = new System.Windows.Forms.GroupBox();
            this.valueBar = new System.Windows.Forms.TrackBar();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.hueBar)).BeginInit();
            this.huePanel.SuspendLayout();
            this.saturationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.saturationBar)).BeginInit();
            this.valuePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valueBar)).BeginInit();
            this.SuspendLayout();
            // 
            // hueBar
            // 
            this.hueBar.Location = new System.Drawing.Point(6, 22);
            this.hueBar.Maximum = 180;
            this.hueBar.Name = "hueBar";
            this.hueBar.Size = new System.Drawing.Size(130, 56);
            this.hueBar.TabIndex = 1;
            this.hueBar.Scroll += new System.EventHandler(this.ColorChange_Scroll);
            // 
            // colorPanel
            // 
            this.colorPanel.Location = new System.Drawing.Point(12, 12);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(446, 74);
            this.colorPanel.TabIndex = 2;
            // 
            // huePanel
            // 
            this.huePanel.Controls.Add(this.hueBar);
            this.huePanel.Location = new System.Drawing.Point(6, 92);
            this.huePanel.Name = "huePanel";
            this.huePanel.Size = new System.Drawing.Size(147, 81);
            this.huePanel.TabIndex = 3;
            this.huePanel.TabStop = false;
            this.huePanel.Text = "Hue";
            // 
            // saturationPanel
            // 
            this.saturationPanel.Controls.Add(this.saturationBar);
            this.saturationPanel.Location = new System.Drawing.Point(159, 92);
            this.saturationPanel.Name = "saturationPanel";
            this.saturationPanel.Size = new System.Drawing.Size(147, 81);
            this.saturationPanel.TabIndex = 4;
            this.saturationPanel.TabStop = false;
            this.saturationPanel.Text = "Saturation";
            // 
            // saturationBar
            // 
            this.saturationBar.Location = new System.Drawing.Point(6, 22);
            this.saturationBar.Maximum = 255;
            this.saturationBar.Name = "saturationBar";
            this.saturationBar.Size = new System.Drawing.Size(130, 56);
            this.saturationBar.TabIndex = 1;
            this.saturationBar.Scroll += new System.EventHandler(this.ColorChange_Scroll);
            // 
            // valuePanel
            // 
            this.valuePanel.Controls.Add(this.valueBar);
            this.valuePanel.Location = new System.Drawing.Point(312, 92);
            this.valuePanel.Name = "valuePanel";
            this.valuePanel.Size = new System.Drawing.Size(147, 81);
            this.valuePanel.TabIndex = 5;
            this.valuePanel.TabStop = false;
            this.valuePanel.Text = "Value";
            // 
            // valueBar
            // 
            this.valueBar.Location = new System.Drawing.Point(6, 22);
            this.valueBar.Maximum = 255;
            this.valueBar.Name = "valueBar";
            this.valueBar.Size = new System.Drawing.Size(130, 56);
            this.valueBar.TabIndex = 1;
            this.valueBar.Scroll += new System.EventHandler(this.ColorChange_Scroll);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(252, 182);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(111, 30);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(95, 182);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(111, 30);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 224);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.valuePanel);
            this.Controls.Add(this.saturationPanel);
            this.Controls.Add(this.colorPanel);
            this.Controls.Add(this.huePanel);
            this.Name = "ColorPicker";
            this.Text = "Hue";
            ((System.ComponentModel.ISupportInitialize)(this.hueBar)).EndInit();
            this.huePanel.ResumeLayout(false);
            this.huePanel.PerformLayout();
            this.saturationPanel.ResumeLayout(false);
            this.saturationPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.saturationBar)).EndInit();
            this.valuePanel.ResumeLayout(false);
            this.valuePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valueBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TrackBar hueBar;
        private System.Windows.Forms.Panel colorPanel;
        private System.Windows.Forms.GroupBox huePanel;
        private System.Windows.Forms.GroupBox saturationPanel;
        private System.Windows.Forms.TrackBar saturationBar;
        private System.Windows.Forms.GroupBox valuePanel;
        private System.Windows.Forms.TrackBar valueBar;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}