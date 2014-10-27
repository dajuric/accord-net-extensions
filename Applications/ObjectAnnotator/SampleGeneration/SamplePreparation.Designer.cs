namespace ObjectAnnotator
{
    partial class SamplePreparation
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
            this.gpScale = new System.Windows.Forms.GroupBox();
            this.nScaleMaxHeight = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.nScaleMinHeight = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.nScaleMaxWidth = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nScaleMinWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.gpLocation = new System.Windows.Forms.GroupBox();
            this.nLocMaxY = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nLocMinY = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nLocMaxX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nLocMinX = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nSamples = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.gpRandomization = new System.Windows.Forms.GroupBox();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.gpInflate = new System.Windows.Forms.GroupBox();
            this.nInflateFactor = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.gpUnifyScales = new System.Windows.Forms.GroupBox();
            this.nWidthHeightRatio = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.chkInflate = new System.Windows.Forms.CheckBox();
            this.chkRandomize = new System.Windows.Forms.CheckBox();
            this.chkUnifiyScales = new System.Windows.Forms.CheckBox();
            this.gpMoveAnn = new System.Windows.Forms.GroupBox();
            this.nImageHeight = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.nImageWidth = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.gpScale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nScaleMaxHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nScaleMinHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nScaleMaxWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nScaleMinWidth)).BeginInit();
            this.gpLocation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nLocMaxY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLocMinY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLocMaxX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLocMinX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nSamples)).BeginInit();
            this.gpRandomization.SuspendLayout();
            this.gpInflate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nInflateFactor)).BeginInit();
            this.gpUnifyScales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nWidthHeightRatio)).BeginInit();
            this.gpMoveAnn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nImageHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nImageWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // gpScale
            // 
            this.gpScale.Controls.Add(this.nScaleMaxHeight);
            this.gpScale.Controls.Add(this.label10);
            this.gpScale.Controls.Add(this.nScaleMinHeight);
            this.gpScale.Controls.Add(this.label11);
            this.gpScale.Controls.Add(this.nScaleMaxWidth);
            this.gpScale.Controls.Add(this.label2);
            this.gpScale.Controls.Add(this.nScaleMinWidth);
            this.gpScale.Controls.Add(this.label1);
            this.gpScale.Location = new System.Drawing.Point(19, 93);
            this.gpScale.Margin = new System.Windows.Forms.Padding(2);
            this.gpScale.Name = "gpScale";
            this.gpScale.Padding = new System.Windows.Forms.Padding(2);
            this.gpScale.Size = new System.Drawing.Size(226, 74);
            this.gpScale.TabIndex = 0;
            this.gpScale.TabStop = false;
            this.gpScale.Text = "Scale - (newScale = oldScale * factor)";
            // 
            // nScaleMaxHeight
            // 
            this.nScaleMaxHeight.DecimalPlaces = 2;
            this.nScaleMaxHeight.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nScaleMaxHeight.Location = new System.Drawing.Point(132, 45);
            this.nScaleMaxHeight.Margin = new System.Windows.Forms.Padding(2);
            this.nScaleMaxHeight.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nScaleMaxHeight.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nScaleMaxHeight.Name = "nScaleMaxHeight";
            this.nScaleMaxHeight.Size = new System.Drawing.Size(50, 20);
            this.nScaleMaxHeight.TabIndex = 7;
            this.nScaleMaxHeight.Value = new decimal(new int[] {
            11,
            0,
            0,
            65536});
            this.nScaleMaxHeight.ValueChanged += new System.EventHandler(this.nScaleHeight_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(110, 46);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(19, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "to:";
            // 
            // nScaleMinHeight
            // 
            this.nScaleMinHeight.DecimalPlaces = 2;
            this.nScaleMinHeight.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nScaleMinHeight.Location = new System.Drawing.Point(55, 45);
            this.nScaleMinHeight.Margin = new System.Windows.Forms.Padding(2);
            this.nScaleMinHeight.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nScaleMinHeight.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nScaleMinHeight.Name = "nScaleMinHeight";
            this.nScaleMinHeight.Size = new System.Drawing.Size(50, 20);
            this.nScaleMinHeight.TabIndex = 5;
            this.nScaleMinHeight.Value = new decimal(new int[] {
            9,
            0,
            0,
            65536});
            this.nScaleMinHeight.ValueChanged += new System.EventHandler(this.nScaleHeight_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 46);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "H: From:";
            // 
            // nScaleMaxWidth
            // 
            this.nScaleMaxWidth.DecimalPlaces = 2;
            this.nScaleMaxWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nScaleMaxWidth.Location = new System.Drawing.Point(130, 19);
            this.nScaleMaxWidth.Margin = new System.Windows.Forms.Padding(2);
            this.nScaleMaxWidth.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nScaleMaxWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nScaleMaxWidth.Name = "nScaleMaxWidth";
            this.nScaleMaxWidth.Size = new System.Drawing.Size(50, 20);
            this.nScaleMaxWidth.TabIndex = 3;
            this.nScaleMaxWidth.Value = new decimal(new int[] {
            11,
            0,
            0,
            65536});
            this.nScaleMaxWidth.ValueChanged += new System.EventHandler(this.nScaleWidth_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(108, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "to:";
            // 
            // nScaleMinWidth
            // 
            this.nScaleMinWidth.DecimalPlaces = 2;
            this.nScaleMinWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nScaleMinWidth.Location = new System.Drawing.Point(53, 19);
            this.nScaleMinWidth.Margin = new System.Windows.Forms.Padding(2);
            this.nScaleMinWidth.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nScaleMinWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nScaleMinWidth.Name = "nScaleMinWidth";
            this.nScaleMinWidth.Size = new System.Drawing.Size(50, 20);
            this.nScaleMinWidth.TabIndex = 1;
            this.nScaleMinWidth.Value = new decimal(new int[] {
            9,
            0,
            0,
            65536});
            this.nScaleMinWidth.ValueChanged += new System.EventHandler(this.nScaleWidth_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "W: From:";
            // 
            // gpLocation
            // 
            this.gpLocation.Controls.Add(this.nLocMaxY);
            this.gpLocation.Controls.Add(this.label5);
            this.gpLocation.Controls.Add(this.nLocMinY);
            this.gpLocation.Controls.Add(this.label6);
            this.gpLocation.Controls.Add(this.nLocMaxX);
            this.gpLocation.Controls.Add(this.label3);
            this.gpLocation.Controls.Add(this.nLocMinX);
            this.gpLocation.Controls.Add(this.label4);
            this.gpLocation.Location = new System.Drawing.Point(16, 17);
            this.gpLocation.Margin = new System.Windows.Forms.Padding(2);
            this.gpLocation.Name = "gpLocation";
            this.gpLocation.Padding = new System.Windows.Forms.Padding(2);
            this.gpLocation.Size = new System.Drawing.Size(226, 72);
            this.gpLocation.TabIndex = 4;
            this.gpLocation.TabStop = false;
            this.gpLocation.Text = "Location - (newLoc = oldLoc + size * factor) ";
            // 
            // nLocMaxY
            // 
            this.nLocMaxY.DecimalPlaces = 2;
            this.nLocMaxY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nLocMaxY.Location = new System.Drawing.Point(130, 46);
            this.nLocMaxY.Margin = new System.Windows.Forms.Padding(2);
            this.nLocMaxY.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nLocMaxY.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nLocMaxY.Name = "nLocMaxY";
            this.nLocMaxY.Size = new System.Drawing.Size(50, 20);
            this.nLocMaxY.TabIndex = 7;
            this.nLocMaxY.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(108, 46);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "to:";
            // 
            // nLocMinY
            // 
            this.nLocMinY.DecimalPlaces = 2;
            this.nLocMinY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nLocMinY.Location = new System.Drawing.Point(53, 46);
            this.nLocMinY.Margin = new System.Windows.Forms.Padding(2);
            this.nLocMinY.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nLocMinY.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nLocMinY.Name = "nLocMinY";
            this.nLocMinY.Size = new System.Drawing.Size(50, 20);
            this.nLocMinY.TabIndex = 5;
            this.nLocMinY.Value = new decimal(new int[] {
            5,
            0,
            0,
            -2147352576});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 47);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Y: From:";
            // 
            // nLocMaxX
            // 
            this.nLocMaxX.DecimalPlaces = 2;
            this.nLocMaxX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nLocMaxX.Location = new System.Drawing.Point(130, 19);
            this.nLocMaxX.Margin = new System.Windows.Forms.Padding(2);
            this.nLocMaxX.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nLocMaxX.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nLocMaxX.Name = "nLocMaxX";
            this.nLocMaxX.Size = new System.Drawing.Size(50, 20);
            this.nLocMaxX.TabIndex = 3;
            this.nLocMaxX.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nLocMaxX.ValueChanged += new System.EventHandler(this.nLocX_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(108, 20);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "to:";
            // 
            // nLocMinX
            // 
            this.nLocMinX.DecimalPlaces = 2;
            this.nLocMinX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nLocMinX.Location = new System.Drawing.Point(53, 19);
            this.nLocMinX.Margin = new System.Windows.Forms.Padding(2);
            this.nLocMinX.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nLocMinX.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nLocMinX.Name = "nLocMinX";
            this.nLocMinX.Size = new System.Drawing.Size(50, 20);
            this.nLocMinX.TabIndex = 1;
            this.nLocMinX.Value = new decimal(new int[] {
            5,
            0,
            0,
            -2147352576});
            this.nLocMinX.ValueChanged += new System.EventHandler(this.nLocX_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 20);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "X: From:";
            // 
            // nSamples
            // 
            this.nSamples.Location = new System.Drawing.Point(124, 172);
            this.nSamples.Margin = new System.Windows.Forms.Padding(2);
            this.nSamples.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nSamples.Name = "nSamples";
            this.nSamples.Size = new System.Drawing.Size(50, 20);
            this.nSamples.TabIndex = 9;
            this.nSamples.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 174);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Number Of Samples:";
            // 
            // gpRandomization
            // 
            this.gpRandomization.Controls.Add(this.gpScale);
            this.gpRandomization.Controls.Add(this.nSamples);
            this.gpRandomization.Controls.Add(this.label7);
            this.gpRandomization.Controls.Add(this.gpLocation);
            this.gpRandomization.Location = new System.Drawing.Point(9, 54);
            this.gpRandomization.Margin = new System.Windows.Forms.Padding(2);
            this.gpRandomization.Name = "gpRandomization";
            this.gpRandomization.Padding = new System.Windows.Forms.Padding(2);
            this.gpRandomization.Size = new System.Drawing.Size(247, 197);
            this.gpRandomization.TabIndex = 10;
            this.gpRandomization.TabStop = false;
            this.gpRandomization.Text = "2) Randomization";
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Location = new System.Drawing.Point(82, 356);
            this.btnSaveAs.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(80, 27);
            this.btnSaveAs.TabIndex = 11;
            this.btnSaveAs.Text = "Save As...";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // gpInflate
            // 
            this.gpInflate.Controls.Add(this.nInflateFactor);
            this.gpInflate.Controls.Add(this.label8);
            this.gpInflate.Location = new System.Drawing.Point(9, 10);
            this.gpInflate.Margin = new System.Windows.Forms.Padding(2);
            this.gpInflate.Name = "gpInflate";
            this.gpInflate.Padding = new System.Windows.Forms.Padding(2);
            this.gpInflate.Size = new System.Drawing.Size(247, 40);
            this.gpInflate.TabIndex = 12;
            this.gpInflate.TabStop = false;
            this.gpInflate.Text = "1) Inflate";
            // 
            // nInflateFactor
            // 
            this.nInflateFactor.DecimalPlaces = 1;
            this.nInflateFactor.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nInflateFactor.Location = new System.Drawing.Point(89, 17);
            this.nInflateFactor.Margin = new System.Windows.Forms.Padding(2);
            this.nInflateFactor.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nInflateFactor.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
            this.nInflateFactor.Name = "nInflateFactor";
            this.nInflateFactor.Size = new System.Drawing.Size(50, 20);
            this.nInflateFactor.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(23, 19);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Inflate factor:";
            // 
            // gpUnifyScales
            // 
            this.gpUnifyScales.Controls.Add(this.nWidthHeightRatio);
            this.gpUnifyScales.Controls.Add(this.label9);
            this.gpUnifyScales.Location = new System.Drawing.Point(9, 256);
            this.gpUnifyScales.Margin = new System.Windows.Forms.Padding(2);
            this.gpUnifyScales.Name = "gpUnifyScales";
            this.gpUnifyScales.Padding = new System.Windows.Forms.Padding(2);
            this.gpUnifyScales.Size = new System.Drawing.Size(247, 41);
            this.gpUnifyScales.TabIndex = 13;
            this.gpUnifyScales.TabStop = false;
            this.gpUnifyScales.Text = "3) Unify scales ";
            // 
            // nWidthHeightRatio
            // 
            this.nWidthHeightRatio.DecimalPlaces = 1;
            this.nWidthHeightRatio.Location = new System.Drawing.Point(124, 17);
            this.nWidthHeightRatio.Margin = new System.Windows.Forms.Padding(2);
            this.nWidthHeightRatio.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nWidthHeightRatio.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nWidthHeightRatio.Name = "nWidthHeightRatio";
            this.nWidthHeightRatio.Size = new System.Drawing.Size(50, 20);
            this.nWidthHeightRatio.TabIndex = 7;
            this.nWidthHeightRatio.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 19);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Width - height ratio:";
            // 
            // chkInflate
            // 
            this.chkInflate.AutoSize = true;
            this.chkInflate.Checked = true;
            this.chkInflate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInflate.Location = new System.Drawing.Point(238, 6);
            this.chkInflate.Margin = new System.Windows.Forms.Padding(2);
            this.chkInflate.Name = "chkInflate";
            this.chkInflate.Size = new System.Drawing.Size(15, 14);
            this.chkInflate.TabIndex = 6;
            this.chkInflate.UseVisualStyleBackColor = true;
            this.chkInflate.CheckedChanged += new System.EventHandler(this.chkInflate_CheckedChanged);
            // 
            // chkRandomize
            // 
            this.chkRandomize.AutoSize = true;
            this.chkRandomize.Checked = true;
            this.chkRandomize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRandomize.Location = new System.Drawing.Point(238, 53);
            this.chkRandomize.Margin = new System.Windows.Forms.Padding(2);
            this.chkRandomize.Name = "chkRandomize";
            this.chkRandomize.Size = new System.Drawing.Size(15, 14);
            this.chkRandomize.TabIndex = 7;
            this.chkRandomize.UseVisualStyleBackColor = true;
            this.chkRandomize.CheckedChanged += new System.EventHandler(this.chkRandomize_CheckedChanged);
            // 
            // chkUnifiyScales
            // 
            this.chkUnifiyScales.AutoSize = true;
            this.chkUnifiyScales.Checked = true;
            this.chkUnifiyScales.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnifiyScales.Location = new System.Drawing.Point(237, 254);
            this.chkUnifiyScales.Margin = new System.Windows.Forms.Padding(2);
            this.chkUnifiyScales.Name = "chkUnifiyScales";
            this.chkUnifiyScales.Size = new System.Drawing.Size(15, 14);
            this.chkUnifiyScales.TabIndex = 8;
            this.chkUnifiyScales.UseVisualStyleBackColor = true;
            this.chkUnifiyScales.CheckedChanged += new System.EventHandler(this.chkUnifiyScales_CheckedChanged);
            // 
            // gpMoveAnn
            // 
            this.gpMoveAnn.Controls.Add(this.nImageHeight);
            this.gpMoveAnn.Controls.Add(this.label13);
            this.gpMoveAnn.Controls.Add(this.nImageWidth);
            this.gpMoveAnn.Controls.Add(this.label12);
            this.gpMoveAnn.Location = new System.Drawing.Point(9, 301);
            this.gpMoveAnn.Margin = new System.Windows.Forms.Padding(2);
            this.gpMoveAnn.Name = "gpMoveAnn";
            this.gpMoveAnn.Padding = new System.Windows.Forms.Padding(2);
            this.gpMoveAnn.Size = new System.Drawing.Size(253, 41);
            this.gpMoveAnn.TabIndex = 14;
            this.gpMoveAnn.TabStop = false;
            this.gpMoveAnn.Text = "4) Move annotation to fit image";
            // 
            // nImageHeight
            // 
            this.nImageHeight.Location = new System.Drawing.Point(198, 17);
            this.nImageHeight.Margin = new System.Windows.Forms.Padding(2);
            this.nImageHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nImageHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nImageHeight.Name = "nImageHeight";
            this.nImageHeight.Size = new System.Drawing.Size(50, 20);
            this.nImageHeight.TabIndex = 9;
            this.nImageHeight.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(128, 19);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 13);
            this.label13.TabIndex = 8;
            this.label13.Text = "Image height:";
            // 
            // nImageWidth
            // 
            this.nImageWidth.Location = new System.Drawing.Point(72, 17);
            this.nImageWidth.Margin = new System.Windows.Forms.Padding(2);
            this.nImageWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nImageWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nImageWidth.Name = "nImageWidth";
            this.nImageWidth.Size = new System.Drawing.Size(50, 20);
            this.nImageWidth.TabIndex = 7;
            this.nImageWidth.Value = new decimal(new int[] {
            640,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 19);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 13);
            this.label12.TabIndex = 6;
            this.label12.Text = "Image width:";
            // 
            // SamplePreparation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 404);
            this.Controls.Add(this.gpMoveAnn);
            this.Controls.Add(this.chkUnifiyScales);
            this.Controls.Add(this.chkRandomize);
            this.Controls.Add(this.chkInflate);
            this.Controls.Add(this.gpUnifyScales);
            this.Controls.Add(this.btnSaveAs);
            this.Controls.Add(this.gpRandomization);
            this.Controls.Add(this.gpInflate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "SamplePreparation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sample Preparation";
            this.gpScale.ResumeLayout(false);
            this.gpScale.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nScaleMaxHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nScaleMinHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nScaleMaxWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nScaleMinWidth)).EndInit();
            this.gpLocation.ResumeLayout(false);
            this.gpLocation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nLocMaxY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLocMinY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLocMaxX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLocMinX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nSamples)).EndInit();
            this.gpRandomization.ResumeLayout(false);
            this.gpRandomization.PerformLayout();
            this.gpInflate.ResumeLayout(false);
            this.gpInflate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nInflateFactor)).EndInit();
            this.gpUnifyScales.ResumeLayout(false);
            this.gpUnifyScales.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nWidthHeightRatio)).EndInit();
            this.gpMoveAnn.ResumeLayout(false);
            this.gpMoveAnn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nImageHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nImageWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gpScale;
        private System.Windows.Forms.NumericUpDown nScaleMinWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nScaleMaxWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gpLocation;
        private System.Windows.Forms.NumericUpDown nLocMaxY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nLocMinY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nLocMaxX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nLocMinX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nSamples;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gpRandomization;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.GroupBox gpInflate;
        private System.Windows.Forms.GroupBox gpUnifyScales;
        private System.Windows.Forms.NumericUpDown nInflateFactor;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nWidthHeightRatio;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nScaleMaxHeight;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nScaleMinHeight;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkRandomize;
        private System.Windows.Forms.CheckBox chkInflate;
        private System.Windows.Forms.CheckBox chkUnifiyScales;
        private System.Windows.Forms.GroupBox gpMoveAnn;
        private System.Windows.Forms.NumericUpDown nImageWidth;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nImageHeight;
        private System.Windows.Forms.Label label13;
    }
}