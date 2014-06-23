using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using Accord.Extensions.Statistics.Filters;
using PointF = AForge.Point;
using ModelState = Accord.Extensions.Statistics.Filters.ConstantVelocity2DModel;

namespace Kalman2DFilterDemo
{
    public partial class Kalman2dDemo : Form
    { 
        ConstantVelocityProcess process;
        KalmanFilter<ModelState, PointF> kalman;

        public Kalman2dDemo()
        {
            InitializeComponent();
            initializeChart();

            process = new ConstantVelocityProcess();
            initializeKalman();

            timer.Enabled = true;   
        }

        private void initializeKalman()
        {
            float accelNoise = (float)numProcessNoise.Value;
            float measurementNoise = (float)numMeasurementNoise.Value;

            var measurementDimension = 2; //just coordinates

            var initialState = process.GetNoisyState(accelNoise); //assuming we measured process params (noise)
            var initialStateError = ModelState.GetProcessNoise(accelNoise);

            kalman = new DiscreteKalmanFilter<ModelState, PointF>(initialState, initialStateError,
                                                                  measurementDimension /*(position)*/, 0 /*no control*/,
                                                                  x => ModelState.ToArray(x), x => ModelState.FromArray(x), x => new double[] { x.X, x.Y });

            kalman.ProcessNoise = ModelState.GetProcessNoise(accelNoise);
            kalman.MeasurementNoise = Matrix.Diagonal<double>(kalman.MeasurementVectorDimension, measurementNoise).ElementwisePower(2); //assuming we measured process params (noise) - ^2 => variance

            kalman.MeasurementMatrix = new double[,] //just pick point coordinates for an observation [2 x 4] (look at ConstantVelocity2DModel)
                { 
                   //X,  vX, Y,  vY (look at ConstantVelocity2DModel)
                    {1,  0,  0,  0}, //picks X
                    {0,  0,  1,  0}  //picks Y
                };

            kalman.TransitionMatrix = ModelState.GetTransitionMatrix(ConstantVelocityProcess.TimeInterval);
        }

        #region GUI...

        #region Chart members

        private void initializeChart()
        {
            const int BORDER_OFFSET = 5;

            chart.ChartAreas[0].AxisX.Title = "X";
            chart.ChartAreas[0].AxisY.Title = "Y";

            chart.ChartAreas[0].AxisX.Minimum = -BORDER_OFFSET;
            chart.ChartAreas[0].AxisY.Minimum = -BORDER_OFFSET;
            chart.ChartAreas[0].AxisX.Maximum = ConstantVelocityProcess.WorkingArea.Width + BORDER_OFFSET;
            chart.ChartAreas[0].AxisY.Maximum = ConstantVelocityProcess.WorkingArea.Height + BORDER_OFFSET;

            chart.Series.Clear();

            //process data
            var s = chart.Series.Add("Process");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            s.Color = Color.Red;

            //measurement data
            s = chart.Series.Add("Measurement");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            s.Color = Color.Black;

            //kalman data
            s = chart.Series.Add("Kalman corrected");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            s.Color = Color.Green;
        }

        private void clearChart()
        {
            foreach (var series in chart.Series)
            {
                series.Points.Clear();
            }
        }

        private void tryRemoveLastMarker(int seriesIdx, DataPoint ordinaryDataPtStyle = null)
        {
            ordinaryDataPtStyle = (ordinaryDataPtStyle != null) ? ordinaryDataPtStyle : new DataPoint();
            Series s = chart.Series[seriesIdx];

            if (s.Points.Count > 0)
            {
                var lastPt = s.Points.Last();
                s.Points.Remove(lastPt);

                ordinaryDataPtStyle.XValue = lastPt.XValue;
                ordinaryDataPtStyle.YValues = lastPt.YValues;
                s.Points.Add(ordinaryDataPtStyle);
            }
        }

        private void addMarker(int seriesIdx, DataPoint markerStyle = null)
        {
            Series s = chart.Series[seriesIdx];

            markerStyle = (markerStyle != null) ? markerStyle : new DataPoint
            {
                MarkerColor = s.Color,
                MarkerSize = 15,
                MarkerStyle = MarkerStyle.Diamond
            };

            //remove last marker
            if (s.Points.Count > 0)
            {
                var lastPt = s.Points.Last();
                s.Points.RemoveAt(s.Points.Count - 1);

                markerStyle.XValue = lastPt.XValue;
                markerStyle.YValues = lastPt.YValues;
                s.Points.Add(markerStyle);
            }
        }

        private void plotData(PointF point, int seriesIdx, DataPoint dataPointStyle = null, bool markLastPt = true)
        {
            Series s = chart.Series[seriesIdx];

            dataPointStyle = dataPointStyle ?? new DataPoint();

            if (markLastPt)
                tryRemoveLastMarker(seriesIdx, dataPointStyle);

            var dataPt = dataPointStyle.Clone();
            dataPt.XValue = point.X;
            dataPt.YValues = new double[] { point.Y };

            s.Points.Add(dataPt);

            if (markLastPt)
                addMarker(seriesIdx);
        }

        #endregion

        private void num_ValueChanged(object sender, EventArgs e)
        {
            initializeKalman();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            float accelNoise = (float)numProcessNoise.Value;
            float measurementNoise = (float)numMeasurementNoise.Value;

            /**************************************** get data *******************************************/
            var processPosition = process.GetNoisyState(accelNoise).Position;

            bool measurementExist;
            var measurement = process.TryGetNoisyMeasurement(measurementNoise, out measurementExist);

            kalman.Predict();
            if (measurementExist) kalman.Correct(measurement);
            var correctedPosition = kalman.State.Position;
            /**************************************** get data *******************************************/

            //plot process state (what we do not know)
            plotData(processPosition, 0);
            //try plot measuremnt (what we see)
            if (measurementExist) plotData(measurement, 1);
            //plot corrected state (Kalman)
            plotData(correctedPosition, 2, new DataPoint { BorderWidth = 5 });

            bool doneFullCycle;
            process.GoToNextState(out doneFullCycle);

            if (doneFullCycle) clearChart();
        }

        private void stopResumeTicker_Click(object sender, EventArgs e)
        {
            timer.Enabled = !timer.Enabled;
        }

        #endregion
    }
}
