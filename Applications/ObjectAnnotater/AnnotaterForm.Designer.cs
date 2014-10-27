using ObjectAnnotator.Components;
namespace ObjectAnnotator
{
    partial class AnnotaterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotaterForm));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.lblCurrentFrame = new System.Windows.Forms.ToolStripLabel();
            this.lblSlash = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalFrames = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lblAnnottionLabel = new System.Windows.Forms.ToolStripLabel();
            this.txtAnnotationLabel = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnToggleLabels = new System.Windows.Forms.ToolStripButton();
            this.btnPrepareSamples = new System.Windows.Forms.ToolStripButton();
            this.btnExtractSamples = new System.Windows.Forms.ToolStripButton();
            this.slider = new System.Windows.Forms.TrackBar();
            this.pictureBox = new DrawingCanvas();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.lblCurrentFrame,
            this.lblSlash,
            this.lblTotalFrames,
            this.toolStripSeparator2,
            this.lblAnnottionLabel,
            this.txtAnnotationLabel,
            this.toolStripLabel2,
            this.toolStripSeparator1,
            this.btnSave,
            this.btnToggleLabels,
            this.btnPrepareSamples,
            this.btnExtractSamples});
            this.toolStrip.Location = new System.Drawing.Point(0, 501);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(687, 27);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 6;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 24);
            // 
            // lblCurrentFrame
            // 
            this.lblCurrentFrame.Name = "lblCurrentFrame";
            this.lblCurrentFrame.Size = new System.Drawing.Size(17, 24);
            this.lblCurrentFrame.Text = "0";
            // 
            // lblSlash
            // 
            this.lblSlash.Name = "lblSlash";
            this.lblSlash.Size = new System.Drawing.Size(15, 24);
            this.lblSlash.Text = "/";
            // 
            // lblTotalFrames
            // 
            this.lblTotalFrames.Name = "lblTotalFrames";
            this.lblTotalFrames.Size = new System.Drawing.Size(41, 24);
            this.lblTotalFrames.Text = "0000";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // lblAnnottionLabel
            // 
            this.lblAnnottionLabel.Name = "lblAnnottionLabel";
            this.lblAnnottionLabel.Size = new System.Drawing.Size(48, 24);
            this.lblAnnottionLabel.Text = "Label:";
            // 
            // txtAnnotationLabel
            // 
            this.txtAnnotationLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAnnotationLabel.Name = "txtAnnotationLabel";
            this.txtAnnotationLabel.Size = new System.Drawing.Size(151, 27);
            this.txtAnnotationLabel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLabel_KeyDown);
            this.txtAnnotationLabel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtLabel_KeyUp);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(0, 24);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnSave
            // 
            this.btnSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Enabled = false;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Save (Ctrl + S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnToggleLabels
            // 
            this.btnToggleLabels.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnToggleLabels.Checked = true;
            this.btnToggleLabels.CheckOnClick = true;
            this.btnToggleLabels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnToggleLabels.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnToggleLabels.Image = ((System.Drawing.Image)(resources.GetObject("btnToggleLabels.Image")));
            this.btnToggleLabels.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnToggleLabels.Margin = new System.Windows.Forms.Padding(0, 1, 15, 2);
            this.btnToggleLabels.Name = "btnToggleLabels";
            this.btnToggleLabels.Size = new System.Drawing.Size(24, 24);
            this.btnToggleLabels.Text = "toolStripButton1";
            this.btnToggleLabels.ToolTipText = "Toggle labels (Ctrl + A)";
            this.btnToggleLabels.CheckedChanged += new System.EventHandler(this.btnToggleLabels_CheckedChanged);
            // 
            // btnPrepareSamples
            // 
            this.btnPrepareSamples.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnPrepareSamples.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrepareSamples.Image = ((System.Drawing.Image)(resources.GetObject("btnPrepareSamples.Image")));
            this.btnPrepareSamples.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrepareSamples.Margin = new System.Windows.Forms.Padding(0, 1, 15, 2);
            this.btnPrepareSamples.Name = "btnPrepareSamples";
            this.btnPrepareSamples.Size = new System.Drawing.Size(24, 24);
            this.btnPrepareSamples.Text = "Prepare Samples";
            this.btnPrepareSamples.Click += new System.EventHandler(this.btnPrepareSamples_Click);
            // 
            // btnExtractSamples
            // 
            this.btnExtractSamples.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnExtractSamples.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExtractSamples.Image = ((System.Drawing.Image)(resources.GetObject("btnExtractSamples.Image")));
            this.btnExtractSamples.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExtractSamples.Margin = new System.Windows.Forms.Padding(0, 1, 15, 2);
            this.btnExtractSamples.Name = "btnExtractSamples";
            this.btnExtractSamples.Size = new System.Drawing.Size(24, 24);
            this.btnExtractSamples.Text = "Extract Samples";
            this.btnExtractSamples.Click += new System.EventHandler(this.btnExtractSamples_Click);
            // 
            // slider
            // 
            this.slider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.slider.AutoSize = false;
            this.slider.LargeChange = 1;
            this.slider.Location = new System.Drawing.Point(4, 468);
            this.slider.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.slider.Name = "slider";
            this.slider.Size = new System.Drawing.Size(683, 30);
            this.slider.TabIndex = 9;
            this.slider.TabStop = false;
            this.slider.TickFrequency = 100;
            this.slider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.slider.Scroll += new System.EventHandler(this.slider_Scroll);
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBox.Image = null;
            this.pictureBox.Location = new System.Drawing.Point(4, 2);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.ResetTransformOnLoad = false;
            this.pictureBox.Size = new System.Drawing.Size(680, 459);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TranslateImageModifierKey = System.Windows.Forms.Keys.ShiftKey;
            // 
            // ObjectAnnotater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 528);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.slider);
            this.Controls.Add(this.pictureBox);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ObjectAnnotater";
            this.Text = "Object Annotater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObjectAnnotater_FormClosing);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DrawingCanvas pictureBox;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txtAnnotationLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.TrackBar slider;
        private System.Windows.Forms.ToolStripLabel lblAnnottionLabel;
        private System.Windows.Forms.ToolStripLabel lblCurrentFrame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel lblSlash;
        private System.Windows.Forms.ToolStripLabel lblTotalFrames;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnToggleLabels;
        private System.Windows.Forms.ToolStripButton btnPrepareSamples;
        private System.Windows.Forms.ToolStripButton btnExtractSamples;
    }
}

