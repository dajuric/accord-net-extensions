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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint16 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(50D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint17 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(100D, 50D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint18 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(50D, 100D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint19 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 50D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint20 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(50D, 0D);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.numMeasurementNoise = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numProcessNoise = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMeasurementNoise)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessNoise)).BeginInit();
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
            chartArea4.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chart.Legends.Add(legend4);
            this.chart.Location = new System.Drawing.Point(151, 12);
            this.chart.Name = "chart";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            dataPoint20.Color = System.Drawing.Color.Cyan;
            dataPoint20.Label = "";
            dataPoint20.LabelBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            dataPoint20.MarkerBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            dataPoint20.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            dataPoint20.MarkerSize = 15;
            dataPoint20.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Square;
            series4.Points.Add(dataPoint16);
            series4.Points.Add(dataPoint17);
            series4.Points.Add(dataPoint18);
            series4.Points.Add(dataPoint19);
            series4.Points.Add(dataPoint20);
            this.chart.Series.Add(series4);
            this.chart.Size = new System.Drawing.Size(844, 603);
            this.chart.TabIndex = 2;
            this.chart.Text = "chart1";
            // 
            // numMeasurementNoise
            // 
            this.numMeasurementNoise.DecimalPlaces = 2;
            this.numMeasurementNoise.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numMeasurementNoise.Location = new System.Drawing.Point(15, 312);
            this.numMeasurementNoise.Name = "numMeasurementNoise";
            this.numMeasurementNoise.Size = new System.Drawing.Size(120, 22);
            this.numMeasurementNoise.TabIndex = 10;
            this.numMeasurementNoise.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numMeasurementNoise.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 292);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Measurement noise:";
            // 
            // numProcessNoise
            // 
            this.numProcessNoise.DecimalPlaces = 2;
            this.numProcessNoise.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numProcessNoise.Location = new System.Drawing.Point(12, 258);
            this.numProcessNoise.Name = "numProcessNoise";
            this.numProcessNoise.Size = new System.Drawing.Size(120, 22);
            this.numProcessNoise.TabIndex = 8;
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
            this.label2.Location = new System.Drawing.Point(12, 237);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Process noise:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 27);
            this.button1.TabIndex = 11;
            this.button1.Text = "Stop / Resume";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.stopResumeTicker_Click);
            // 
            // Kalman2dDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 627);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.numMeasurementNoise);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numProcessNoise);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chart);
            this.Name = "Kalman2dDemo";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMeasurementNoise)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProcessNoise)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.NumericUpDown numMeasurementNoise;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numProcessNoise;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;

    }
}

