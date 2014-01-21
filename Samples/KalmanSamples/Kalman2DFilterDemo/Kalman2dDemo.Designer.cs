namespace Kalman2DFilterDemo
{
    partial class Kalman2dDemo
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(50D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(100D, 50D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(50D, 100D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint4 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 50D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint5 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(50D, 0D);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.numMeasurementNoise = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numPositionNoise = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numVelocityNoise = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMeasurementNoise)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPositionNoise)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVelocityNoise)).BeginInit();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 30;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
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
            this.chart.Location = new System.Drawing.Point(113, 10);
            this.chart.Margin = new System.Windows.Forms.Padding(2);
            this.chart.Name = "chart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            dataPoint5.Color = System.Drawing.Color.Cyan;
            dataPoint5.Label = "";
            dataPoint5.LabelBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            dataPoint5.MarkerBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            dataPoint5.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            dataPoint5.MarkerSize = 15;
            dataPoint5.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Square;
            series1.Points.Add(dataPoint1);
            series1.Points.Add(dataPoint2);
            series1.Points.Add(dataPoint3);
            series1.Points.Add(dataPoint4);
            series1.Points.Add(dataPoint5);
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(633, 490);
            this.chart.TabIndex = 2;
            this.chart.Text = "chart1";
            // 
            // numMeasurementNoise
            // 
            this.numMeasurementNoise.DecimalPlaces = 1;
            this.numMeasurementNoise.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMeasurementNoise.Location = new System.Drawing.Point(11, 290);
            this.numMeasurementNoise.Margin = new System.Windows.Forms.Padding(2);
            this.numMeasurementNoise.Name = "numMeasurementNoise";
            this.numMeasurementNoise.Size = new System.Drawing.Size(90, 20);
            this.numMeasurementNoise.TabIndex = 10;
            this.numMeasurementNoise.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numMeasurementNoise.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 273);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Measurement noise:";
            // 
            // numPositionNoise
            // 
            this.numPositionNoise.DecimalPlaces = 1;
            this.numPositionNoise.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numPositionNoise.Location = new System.Drawing.Point(6, 41);
            this.numPositionNoise.Margin = new System.Windows.Forms.Padding(2);
            this.numPositionNoise.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numPositionNoise.Name = "numPositionNoise";
            this.numPositionNoise.Size = new System.Drawing.Size(90, 20);
            this.numPositionNoise.TabIndex = 8;
            this.numPositionNoise.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numPositionNoise.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Position noise:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 26);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 22);
            this.button1.TabIndex = 11;
            this.button1.Text = "Stop / Resume";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.stopResumeTicker_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numVelocityNoise);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numPositionNoise);
            this.groupBox1.Location = new System.Drawing.Point(5, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(103, 120);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Process noise";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 78);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Velocity noise:";
            // 
            // numVelocityNoise
            // 
            this.numVelocityNoise.DecimalPlaces = 1;
            this.numVelocityNoise.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numVelocityNoise.Location = new System.Drawing.Point(5, 95);
            this.numVelocityNoise.Margin = new System.Windows.Forms.Padding(2);
            this.numVelocityNoise.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numVelocityNoise.Name = "numVelocityNoise";
            this.numVelocityNoise.Size = new System.Drawing.Size(90, 20);
            this.numVelocityNoise.TabIndex = 10;
            this.numVelocityNoise.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // Kalman2dDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 509);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.numMeasurementNoise);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chart);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Kalman2dDemo";
            this.Text = "Discrete Kalman 2D Filter Demo";
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMeasurementNoise)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPositionNoise)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVelocityNoise)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.NumericUpDown numMeasurementNoise;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numPositionNoise;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numVelocityNoise;

    }
}

