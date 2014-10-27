namespace ObjectAnnotator
{
    partial class SampleExtraction
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
            this.txtPattern = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtPattern
            // 
            this.txtPattern.Location = new System.Drawing.Point(129, 8);
            this.txtPattern.Name = "txtPattern";
            this.txtPattern.Size = new System.Drawing.Size(112, 20);
            this.txtPattern.TabIndex = 4;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(16, 11);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(107, 13);
            this.label.TabIndex = 5;
            this.label.Text = "Label search pattern:";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(70, 34);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(93, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // SampleExtraction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 60);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label);
            this.Controls.Add(this.txtPattern);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "SampleExtraction";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SampleExtraction";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPattern;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button btnStart;

    }
}