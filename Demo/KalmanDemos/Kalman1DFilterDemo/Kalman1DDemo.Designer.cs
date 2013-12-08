namespace Kalman1DFilterDemo
{
    partial class Kalman1DDemo
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.numTimeInterval = new System.Windows.Forms.NumericUpDown();
            this.numProcessNoise = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numMeasurementNoise = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.lstProcess = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessNoise)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMeasurementNoise)).BeginInit();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(142, 12);
            this.chart.Name = "chart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(812, 566);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 185);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time interval:";
            // 
            // numTimeInterval
            // 
            this.numTimeInterval.DecimalPlaces = 2;
            this.numTimeInterval.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numTimeInterval.Location = new System.Drawing.Point(12, 206);
            this.numTimeInterval.Name = "numTimeInterval";
            this.numTimeInterval.Size = new System.Drawing.Size(120, 22);
            this.numTimeInterval.TabIndex = 2;
            this.numTimeInterval.Value = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            this.numTimeInterval.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // numProcessNoise
            // 
            this.numProcessNoise.DecimalPlaces = 2;
            this.numProcessNoise.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numProcessNoise.Location = new System.Drawing.Point(12, 300);
            this.numProcessNoise.Name = "numProcessNoise";
            this.numProcessNoise.Size = new System.Drawing.Size(120, 22);
            this.numProcessNoise.TabIndex = 4;
            this.numProcessNoise.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numProcessNoise.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 279);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Process noise:";
            // 
            // numMeasurementNoise
            // 
            this.numMeasurementNoise.DecimalPlaces = 2;
            this.numMeasurementNoise.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numMeasurementNoise.Location = new System.Drawing.Point(15, 354);
            this.numMeasurementNoise.Name = "numMeasurementNoise";
            this.numMeasurementNoise.Size = new System.Drawing.Size(120, 22);
            this.numMeasurementNoise.TabIndex = 6;
            this.numMeasurementNoise.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMeasurementNoise.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 334);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Measurement noise:";
            // 
            // lstProcess
            // 
            this.lstProcess.FormattingEnabled = true;
            this.lstProcess.ItemHeight = 16;
            this.lstProcess.Items.AddRange(new object[] {
            "Constant Speed",
            "Sudden Jump",
            "Speed Decrease"});
            this.lstProcess.Location = new System.Drawing.Point(12, 54);
            this.lstProcess.Name = "lstProcess";
            this.lstProcess.Size = new System.Drawing.Size(120, 84);
            this.lstProcess.TabIndex = 7;
            this.lstProcess.SelectedIndexChanged += new System.EventHandler(this.lstProcess_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Process:";
            // 
            // Kalman1DDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 587);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lstProcess);
            this.Controls.Add(this.numMeasurementNoise);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numProcessNoise);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numTimeInterval);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chart);
            this.Name = "Kalman1DDemo";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessNoise)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMeasurementNoise)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numTimeInterval;
        private System.Windows.Forms.NumericUpDown numProcessNoise;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMeasurementNoise;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstProcess;
        private System.Windows.Forms.Label label4;
    }
}

