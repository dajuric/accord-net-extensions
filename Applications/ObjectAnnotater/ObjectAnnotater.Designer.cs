namespace ObjectAnnotater
{
    partial class ObjectAnnotater
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectAnnotater));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.cmdUndo = new System.Windows.Forms.ToolStripButton();
            this.cmdRedo = new System.Windows.Forms.ToolStripButton();
            this.btnNextFrame = new System.Windows.Forms.ToolStripButton();
            this.lblFrameIdx = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(12, 42);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(640, 493);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdUndo,
            this.cmdRedo,
            this.btnNextFrame,
            this.lblFrameIdx});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(656, 39);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip";
            // 
            // cmdUndo
            // 
            this.cmdUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdUndo.Image = ((System.Drawing.Image)(resources.GetObject("cmdUndo.Image")));
            this.cmdUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdUndo.Name = "cmdUndo";
            this.cmdUndo.Size = new System.Drawing.Size(36, 36);
            this.cmdUndo.Text = "Undo (u)";
            // 
            // cmdRedo
            // 
            this.cmdRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdRedo.Image = ((System.Drawing.Image)(resources.GetObject("cmdRedo.Image")));
            this.cmdRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdRedo.Name = "cmdRedo";
            this.cmdRedo.Size = new System.Drawing.Size(36, 36);
            this.cmdRedo.Text = "Redo (r)";
            // 
            // btnNextFrame
            // 
            this.btnNextFrame.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnNextFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNextFrame.Image = ((System.Drawing.Image)(resources.GetObject("btnNextFrame.Image")));
            this.btnNextFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextFrame.Name = "btnNextFrame";
            this.btnNextFrame.Size = new System.Drawing.Size(36, 36);
            this.btnNextFrame.Text = "Get next frame (n)";
            // 
            // lblFrameIdx
            // 
            this.lblFrameIdx.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblFrameIdx.Name = "lblFrameIdx";
            this.lblFrameIdx.Size = new System.Drawing.Size(90, 36);
            this.lblFrameIdx.Text = "Frame index";
            // 
            // ObjectAnnotater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 547);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.pictureBox);
            this.Name = "ObjectAnnotater";
            this.Text = "Object Annotater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObjectAnnotater_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ObjectAnnotater_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton cmdUndo;
        private System.Windows.Forms.ToolStripButton cmdRedo;
        private System.Windows.Forms.ToolStripButton btnNextFrame;
        private System.Windows.Forms.ToolStripLabel lblFrameIdx;
    }
}

