namespace KalmanObjectTracking
{
    partial class KalmanTrackingDemo
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
            this.pbProbabilityImage = new System.Windows.Forms.PictureBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.barVMax = new System.Windows.Forms.TrackBar();
            this.barVMin = new System.Windows.Forms.TrackBar();
            this.gbConstraints = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbProbabilityImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barVMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barVMin)).BeginInit();
            this.gbConstraints.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbProbabilityImage
            // 
            this.pbProbabilityImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbProbabilityImage.Location = new System.Drawing.Point(651, 61);
            this.pbProbabilityImage.Margin = new System.Windows.Forms.Padding(2);
            this.pbProbabilityImage.Name = "pbProbabilityImage";
            this.pbProbabilityImage.Size = new System.Drawing.Size(640, 480);
            this.pbProbabilityImage.TabIndex = 13;
            this.pbProbabilityImage.TabStop = false;
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(7, 61);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(640, 480);
            this.pictureBox.TabIndex = 11;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // barVMax
            // 
            this.barVMax.AutoSize = false;
            this.barVMax.Location = new System.Drawing.Point(361, 18);
            this.barVMax.Margin = new System.Windows.Forms.Padding(2);
            this.barVMax.Maximum = 255;
            this.barVMax.Name = "barVMax";
            this.barVMax.Size = new System.Drawing.Size(255, 24);
            this.barVMax.TabIndex = 7;
            this.barVMax.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barVMax.Value = 220;
            this.barVMax.ValueChanged += new System.EventHandler(this.bar_ValueChanged);
            // 
            // barVMin
            // 
            this.barVMin.AutoSize = false;
            this.barVMin.Location = new System.Drawing.Point(48, 18);
            this.barVMin.Margin = new System.Windows.Forms.Padding(2);
            this.barVMin.Maximum = 255;
            this.barVMin.Name = "barVMin";
            this.barVMin.Size = new System.Drawing.Size(255, 24);
            this.barVMin.TabIndex = 5;
            this.barVMin.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barVMin.Value = 20;
            this.barVMin.ValueChanged += new System.EventHandler(this.bar_ValueChanged);
            // 
            // gbConstraints
            // 
            this.gbConstraints.Controls.Add(this.label3);
            this.gbConstraints.Controls.Add(this.barVMax);
            this.gbConstraints.Controls.Add(this.label4);
            this.gbConstraints.Controls.Add(this.barVMin);
            this.gbConstraints.Location = new System.Drawing.Point(7, 6);
            this.gbConstraints.Margin = new System.Windows.Forms.Padding(2);
            this.gbConstraints.Name = "gbConstraints";
            this.gbConstraints.Padding = new System.Windows.Forms.Padding(2);
            this.gbConstraints.Size = new System.Drawing.Size(620, 43);
            this.gbConstraints.TabIndex = 12;
            this.gbConstraints.TabStop = false;
            this.gbConstraints.Text = "Constraints";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(324, 18);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "V max";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 18);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "V min";
            // 
            // KalmanTrackingDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1301, 552);
            this.Controls.Add(this.pbProbabilityImage);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.gbConstraints);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "KalmanTrackingDemo";
            this.Text = "Kalman object tracking demo";
            ((System.ComponentModel.ISupportInitialize)(this.pbProbabilityImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barVMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barVMin)).EndInit();
            this.gbConstraints.ResumeLayout(false);
            this.gbConstraints.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbProbabilityImage;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TrackBar barVMax;
        private System.Windows.Forms.TrackBar barVMin;
        private System.Windows.Forms.GroupBox gbConstraints;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;

    }
}

