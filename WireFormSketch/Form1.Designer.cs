
namespace WireFormSketch
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
            this.button1 = new System.Windows.Forms.Button();
            this.imageBox2 = new Emgu.CV.UI.ImageBox();
            this.ValueLowerBound = new System.Windows.Forms.TrackBar();
            this.ValueUpperBound = new System.Windows.Forms.TrackBar();
            this.imageBox3 = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueLowerBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueUpperBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox1
            // 
            this.imageBox1.Location = new System.Drawing.Point(140, 21);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(582, 395);
            this.imageBox1.TabIndex = 2;
            this.imageBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // imageBox2
            // 
            this.imageBox2.Location = new System.Drawing.Point(731, 21);
            this.imageBox2.Name = "imageBox2";
            this.imageBox2.Size = new System.Drawing.Size(582, 395);
            this.imageBox2.TabIndex = 4;
            this.imageBox2.TabStop = false;
            // 
            // ValueLowerBound
            // 
            this.ValueLowerBound.Location = new System.Drawing.Point(0, 150);
            this.ValueLowerBound.Maximum = 255;
            this.ValueLowerBound.Name = "ValueLowerBound";
            this.ValueLowerBound.Size = new System.Drawing.Size(130, 56);
            this.ValueLowerBound.TabIndex = 5;
            this.ValueLowerBound.Value = 130;
            // 
            // ValueUpperBound
            // 
            this.ValueUpperBound.Location = new System.Drawing.Point(0, 75);
            this.ValueUpperBound.Maximum = 255;
            this.ValueUpperBound.Name = "ValueUpperBound";
            this.ValueUpperBound.Size = new System.Drawing.Size(130, 56);
            this.ValueUpperBound.TabIndex = 5;
            this.ValueUpperBound.Value = 130;
            // 
            // imageBox3
            // 
            this.imageBox3.Location = new System.Drawing.Point(168, 422);
            this.imageBox3.Name = "imageBox3";
            this.imageBox3.Size = new System.Drawing.Size(374, 183);
            this.imageBox3.TabIndex = 2;
            this.imageBox3.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1325, 658);
            this.Controls.Add(this.imageBox3);
            this.Controls.Add(this.ValueUpperBound);
            this.Controls.Add(this.ValueLowerBound);
            this.Controls.Add(this.imageBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.imageBox1);
            this.Location = new System.Drawing.Point(0, 150);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueLowerBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueUpperBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TrackBar ValueLowerBound;
        private Emgu.CV.UI.ImageBox imageBox2;
        private System.Windows.Forms.TrackBar ValueUpperBound;
        private Emgu.CV.UI.ImageBox imageBox3;
    }
}

