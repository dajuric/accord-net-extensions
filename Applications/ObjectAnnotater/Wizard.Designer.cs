namespace ObjectAnnotater
{
    partial class Wizard
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
            this.btnImageSeq = new System.Windows.Forms.Button();
            this.boxImageFormatSelection = new System.Windows.Forms.ComboBox();
            this.lblAnnFile = new System.Windows.Forms.Label();
            this.btnSaveAnnotations = new System.Windows.Forms.Button();
            this.gpBoxImageSequence = new System.Windows.Forms.GroupBox();
            this.gpBoxVideo = new System.Windows.Forms.GroupBox();
            this.btnVideo = new System.Windows.Forms.Button();
            this.btnSelectImages = new System.Windows.Forms.RadioButton();
            this.btnSelectVideo = new System.Windows.Forms.RadioButton();
            this.chkRecursive = new System.Windows.Forms.CheckBox();
            this.gpBoxImageSequence.SuspendLayout();
            this.gpBoxVideo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImageSeq
            // 
            this.btnImageSeq.Location = new System.Drawing.Point(5, 18);
            this.btnImageSeq.Margin = new System.Windows.Forms.Padding(2);
            this.btnImageSeq.Name = "btnImageSeq";
            this.btnImageSeq.Size = new System.Drawing.Size(74, 37);
            this.btnImageSeq.TabIndex = 0;
            this.btnImageSeq.Text = "Image sequence";
            this.btnImageSeq.UseVisualStyleBackColor = true;
            this.btnImageSeq.Click += new System.EventHandler(this.btnImageSeq_Click);
            // 
            // boxImageFormatSelection
            // 
            this.boxImageFormatSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boxImageFormatSelection.FormattingEnabled = true;
            this.boxImageFormatSelection.ItemHeight = 13;
            this.boxImageFormatSelection.Items.AddRange(new object[] {
            "*.jpg",
            "*.bmp",
            "*.png"});
            this.boxImageFormatSelection.Location = new System.Drawing.Point(11, 59);
            this.boxImageFormatSelection.Margin = new System.Windows.Forms.Padding(2);
            this.boxImageFormatSelection.Name = "boxImageFormatSelection";
            this.boxImageFormatSelection.Size = new System.Drawing.Size(56, 21);
            this.boxImageFormatSelection.TabIndex = 2;
            // 
            // lblAnnFile
            // 
            this.lblAnnFile.Location = new System.Drawing.Point(47, 129);
            this.lblAnnFile.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAnnFile.Name = "lblAnnFile";
            this.lblAnnFile.Size = new System.Drawing.Size(142, 29);
            this.lblAnnFile.TabIndex = 4;
            this.lblAnnFile.Text = "Annotation file:";
            this.lblAnnFile.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // btnSaveAnnotations
            // 
            this.btnSaveAnnotations.Enabled = false;
            this.btnSaveAnnotations.Location = new System.Drawing.Point(102, 169);
            this.btnSaveAnnotations.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveAnnotations.Name = "btnSaveAnnotations";
            this.btnSaveAnnotations.Size = new System.Drawing.Size(32, 19);
            this.btnSaveAnnotations.TabIndex = 5;
            this.btnSaveAnnotations.Text = "...";
            this.btnSaveAnnotations.UseVisualStyleBackColor = true;
            this.btnSaveAnnotations.Click += new System.EventHandler(this.btnSaveAnnotations_Click);
            // 
            // gpBoxImageSequence
            // 
            this.gpBoxImageSequence.Controls.Add(this.chkRecursive);
            this.gpBoxImageSequence.Controls.Add(this.btnImageSeq);
            this.gpBoxImageSequence.Controls.Add(this.boxImageFormatSelection);
            this.gpBoxImageSequence.Location = new System.Drawing.Point(12, 7);
            this.gpBoxImageSequence.Name = "gpBoxImageSequence";
            this.gpBoxImageSequence.Size = new System.Drawing.Size(96, 116);
            this.gpBoxImageSequence.TabIndex = 6;
            this.gpBoxImageSequence.TabStop = false;
            this.gpBoxImageSequence.Text = "Images";
            // 
            // gpBoxVideo
            // 
            this.gpBoxVideo.Controls.Add(this.btnVideo);
            this.gpBoxVideo.Enabled = false;
            this.gpBoxVideo.Location = new System.Drawing.Point(129, 7);
            this.gpBoxVideo.Name = "gpBoxVideo";
            this.gpBoxVideo.Size = new System.Drawing.Size(94, 116);
            this.gpBoxVideo.TabIndex = 7;
            this.gpBoxVideo.TabStop = false;
            this.gpBoxVideo.Text = "Video file";
            // 
            // btnVideo
            // 
            this.btnVideo.Location = new System.Drawing.Point(5, 18);
            this.btnVideo.Margin = new System.Windows.Forms.Padding(2);
            this.btnVideo.Name = "btnVideo";
            this.btnVideo.Size = new System.Drawing.Size(74, 37);
            this.btnVideo.TabIndex = 8;
            this.btnVideo.Text = "Video file";
            this.btnVideo.UseVisualStyleBackColor = true;
            this.btnVideo.Click += new System.EventHandler(this.btnVideo_Click);
            // 
            // btnSelectImages
            // 
            this.btnSelectImages.AutoSize = true;
            this.btnSelectImages.Checked = true;
            this.btnSelectImages.Location = new System.Drawing.Point(23, 129);
            this.btnSelectImages.Name = "btnSelectImages";
            this.btnSelectImages.Size = new System.Drawing.Size(14, 13);
            this.btnSelectImages.TabIndex = 3;
            this.btnSelectImages.TabStop = true;
            this.btnSelectImages.UseVisualStyleBackColor = true;
            this.btnSelectImages.Click += new System.EventHandler(this.btnSelectImages_Click);
            // 
            // btnSelectVideo
            // 
            this.btnSelectVideo.AutoSize = true;
            this.btnSelectVideo.Location = new System.Drawing.Point(194, 129);
            this.btnSelectVideo.Name = "btnSelectVideo";
            this.btnSelectVideo.Size = new System.Drawing.Size(14, 13);
            this.btnSelectVideo.TabIndex = 9;
            this.btnSelectVideo.TabStop = true;
            this.btnSelectVideo.UseVisualStyleBackColor = true;
            this.btnSelectVideo.Click += new System.EventHandler(this.btnSelectVideo_Click);
            // 
            // chkRecursive
            // 
            this.chkRecursive.AutoSize = true;
            this.chkRecursive.Location = new System.Drawing.Point(5, 93);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(74, 17);
            this.chkRecursive.TabIndex = 3;
            this.chkRecursive.Text = "Recursive";
            this.chkRecursive.UseVisualStyleBackColor = true;
            // 
            // Wizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 192);
            this.Controls.Add(this.btnSelectVideo);
            this.Controls.Add(this.btnSelectImages);
            this.Controls.Add(this.gpBoxVideo);
            this.Controls.Add(this.gpBoxImageSequence);
            this.Controls.Add(this.btnSaveAnnotations);
            this.Controls.Add(this.lblAnnFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wizard";
            this.Text = "Annotater";
            this.gpBoxImageSequence.ResumeLayout(false);
            this.gpBoxImageSequence.PerformLayout();
            this.gpBoxVideo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImageSeq;
        private System.Windows.Forms.ComboBox boxImageFormatSelection;
        private System.Windows.Forms.Label lblAnnFile;
        private System.Windows.Forms.Button btnSaveAnnotations;
        private System.Windows.Forms.GroupBox gpBoxImageSequence;
        private System.Windows.Forms.GroupBox gpBoxVideo;
        private System.Windows.Forms.Button btnVideo;
        private System.Windows.Forms.RadioButton btnSelectImages;
        private System.Windows.Forms.RadioButton btnSelectVideo;
        private System.Windows.Forms.CheckBox chkRecursive;
    }
}