namespace ObjectAnnotator
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
            this.chkRecursive = new System.Windows.Forms.CheckBox();
            this.gpBoxImageSequence.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImageSeq
            // 
            this.btnImageSeq.Location = new System.Drawing.Point(69, 18);
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
            "*.jpg;*.bmp;*.png",
            "*.jpg",
            "*.bmp",
            "*.png"});
            this.boxImageFormatSelection.Location = new System.Drawing.Point(3, 67);
            this.boxImageFormatSelection.Margin = new System.Windows.Forms.Padding(2);
            this.boxImageFormatSelection.Name = "boxImageFormatSelection";
            this.boxImageFormatSelection.Size = new System.Drawing.Size(111, 21);
            this.boxImageFormatSelection.TabIndex = 2;
            // 
            // lblAnnFile
            // 
            this.lblAnnFile.Location = new System.Drawing.Point(36, 111);
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
            this.btnSaveAnnotations.Location = new System.Drawing.Point(91, 151);
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
            this.gpBoxImageSequence.Location = new System.Drawing.Point(1, 7);
            this.gpBoxImageSequence.Name = "gpBoxImageSequence";
            this.gpBoxImageSequence.Size = new System.Drawing.Size(195, 100);
            this.gpBoxImageSequence.TabIndex = 6;
            this.gpBoxImageSequence.TabStop = false;
            this.gpBoxImageSequence.Text = "Images";
            // 
            // chkRecursive
            // 
            this.chkRecursive.AutoSize = true;
            this.chkRecursive.Location = new System.Drawing.Point(126, 69);
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
            this.ClientSize = new System.Drawing.Size(197, 173);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnImageSeq;
        private System.Windows.Forms.ComboBox boxImageFormatSelection;
        private System.Windows.Forms.Label lblAnnFile;
        private System.Windows.Forms.Button btnSaveAnnotations;
        private System.Windows.Forms.GroupBox gpBoxImageSequence;
        private System.Windows.Forms.CheckBox chkRecursive;
    }
}