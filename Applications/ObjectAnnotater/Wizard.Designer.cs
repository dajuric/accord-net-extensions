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
            this.btnFileSeq = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.boxImageFormatSelection = new System.Windows.Forms.ComboBox();
            this.gpBoxImSeq = new System.Windows.Forms.GroupBox();
            this.lblAnnFile = new System.Windows.Forms.Label();
            this.btnSaveAnnotations = new System.Windows.Forms.Button();
            this.gpBoxImSeq.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImageSeq
            // 
            this.btnImageSeq.Location = new System.Drawing.Point(11, 21);
            this.btnImageSeq.Name = "btnImageSeq";
            this.btnImageSeq.Size = new System.Drawing.Size(98, 45);
            this.btnImageSeq.TabIndex = 0;
            this.btnImageSeq.Text = "Image sequence";
            this.btnImageSeq.UseVisualStyleBackColor = true;
            this.btnImageSeq.Click += new System.EventHandler(this.btnImageSeq_Click);
            // 
            // btnFileSeq
            // 
            this.btnFileSeq.Location = new System.Drawing.Point(148, 33);
            this.btnFileSeq.Name = "btnFileSeq";
            this.btnFileSeq.Size = new System.Drawing.Size(93, 45);
            this.btnFileSeq.TabIndex = 1;
            this.btnFileSeq.Text = "Video File Sequence";
            this.btnFileSeq.UseVisualStyleBackColor = true;
            this.btnFileSeq.Click += new System.EventHandler(this.btnFileSeq_Click);
            // 
            // boxImageFormatSelection
            // 
            this.boxImageFormatSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boxImageFormatSelection.FormattingEnabled = true;
            this.boxImageFormatSelection.ItemHeight = 16;
            this.boxImageFormatSelection.Items.AddRange(new object[] {
            "*.jpg",
            "*.bmp",
            "*.png"});
            this.boxImageFormatSelection.Location = new System.Drawing.Point(19, 72);
            this.boxImageFormatSelection.Name = "boxImageFormatSelection";
            this.boxImageFormatSelection.Size = new System.Drawing.Size(73, 24);
            this.boxImageFormatSelection.TabIndex = 2;
            // 
            // gpBoxImSeq
            // 
            this.gpBoxImSeq.Controls.Add(this.btnImageSeq);
            this.gpBoxImSeq.Controls.Add(this.boxImageFormatSelection);
            this.gpBoxImSeq.Location = new System.Drawing.Point(12, 12);
            this.gpBoxImSeq.Name = "gpBoxImSeq";
            this.gpBoxImSeq.Size = new System.Drawing.Size(130, 116);
            this.gpBoxImSeq.TabIndex = 3;
            this.gpBoxImSeq.TabStop = false;
            this.gpBoxImSeq.Text = "Image Sequence";
            // 
            // lblAnnFile
            // 
            this.lblAnnFile.AutoSize = true;
            this.lblAnnFile.Location = new System.Drawing.Point(19, 151);
            this.lblAnnFile.Name = "lblAnnFile";
            this.lblAnnFile.Size = new System.Drawing.Size(102, 17);
            this.lblAnnFile.TabIndex = 4;
            this.lblAnnFile.Text = "Annotation file:";
            // 
            // btnSaveAnnotations
            // 
            this.btnSaveAnnotations.Location = new System.Drawing.Point(21, 171);
            this.btnSaveAnnotations.Name = "btnSaveAnnotations";
            this.btnSaveAnnotations.Size = new System.Drawing.Size(42, 23);
            this.btnSaveAnnotations.TabIndex = 5;
            this.btnSaveAnnotations.Text = "...";
            this.btnSaveAnnotations.UseVisualStyleBackColor = true;
            this.btnSaveAnnotations.Click += new System.EventHandler(this.btnSaveAnnotations_Click);
            // 
            // Wizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 197);
            this.Controls.Add(this.btnSaveAnnotations);
            this.Controls.Add(this.lblAnnFile);
            this.Controls.Add(this.gpBoxImSeq);
            this.Controls.Add(this.btnFileSeq);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wizard";
            this.Text = "Select video source...";
            this.gpBoxImSeq.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImageSeq;
        private System.Windows.Forms.Button btnFileSeq;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ComboBox boxImageFormatSelection;
        private System.Windows.Forms.GroupBox gpBoxImSeq;
        private System.Windows.Forms.Label lblAnnFile;
        private System.Windows.Forms.Button btnSaveAnnotations;
    }
}