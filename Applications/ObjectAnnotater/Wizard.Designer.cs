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
            this.SuspendLayout();
            // 
            // btnImageSeq
            // 
            this.btnImageSeq.Location = new System.Drawing.Point(50, 12);
            this.btnImageSeq.Name = "btnImageSeq";
            this.btnImageSeq.Size = new System.Drawing.Size(98, 45);
            this.btnImageSeq.TabIndex = 0;
            this.btnImageSeq.Text = "Image sequence";
            this.btnImageSeq.UseVisualStyleBackColor = true;
            this.btnImageSeq.Click += new System.EventHandler(this.btnImageSeq_Click);
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
            this.boxImageFormatSelection.Location = new System.Drawing.Point(58, 63);
            this.boxImageFormatSelection.Name = "boxImageFormatSelection";
            this.boxImageFormatSelection.Size = new System.Drawing.Size(73, 24);
            this.boxImageFormatSelection.TabIndex = 2;
            // 
            // lblAnnFile
            // 
            this.lblAnnFile.Location = new System.Drawing.Point(12, 107);
            this.lblAnnFile.Name = "lblAnnFile";
            this.lblAnnFile.Size = new System.Drawing.Size(189, 36);
            this.lblAnnFile.TabIndex = 4;
            this.lblAnnFile.Text = "Annotation file:";
            this.lblAnnFile.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // btnSaveAnnotations
            // 
            this.btnSaveAnnotations.Enabled = false;
            this.btnSaveAnnotations.Location = new System.Drawing.Point(76, 146);
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
            this.ClientSize = new System.Drawing.Size(213, 175);
            this.Controls.Add(this.btnImageSeq);
            this.Controls.Add(this.boxImageFormatSelection);
            this.Controls.Add(this.btnSaveAnnotations);
            this.Controls.Add(this.lblAnnFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wizard";
            this.Text = "Annotater";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnImageSeq;
        private System.Windows.Forms.ComboBox boxImageFormatSelection;
        private System.Windows.Forms.Label lblAnnFile;
        private System.Windows.Forms.Button btnSaveAnnotations;
    }
}