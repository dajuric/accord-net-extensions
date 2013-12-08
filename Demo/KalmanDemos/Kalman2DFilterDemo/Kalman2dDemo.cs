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
using Accord.Statistics.Filters;
using Point = AForge.Point;
using AForge;

namespace Kalman2DFilterDemo
{
    public partial class Kalman2dDemo : Form
    { 
        List<double[]> states;
        DiscreteKalmanFilter kalman;

        double timeInterval = 0.3;
        PointF startPoint = new PointF(50, 1);
        PointF startVelocity = new PointF(1, 1);

        public Kalman2dDemo()
        {
            InitializeComponent();

            states = prepareData();

            timer.Enabled = true;   
        }

        private void initializeKalman()
        {
            var measurementDimension = 2; //just coordinates

            var processNoise = (double)this.numProcessNoise.Value;
            var measurementNoise = (double)this.numMeasurementNoise.Value;

            var initialState = LinearAcceleration2DModel.GetInitialState(new Point(startPoint.X, startPoint.Y), 
                                                                           new Point(startVelocity.X, startPoint.Y));

            kalman = new DiscreteKalmanFilter(initialState, measurementDimension /*(position)*/, 0 /*no acceleration*/ );

            kalman.ProcessNoiseCovariance = LinearAcceleration2DModel.GetProcessNoiseCovariance(processNoise, timeInterval);
            kalman.MeasurementNoiseCovariance = Matrix.Identity(measurementDimension).Multiply(measurementNoise * measurementNoise);

            kalman.MeasurementMatrix = new double[,] //just pick point coordinates for an observation [2 x 4]
                { 
                    {1, 0, 0, 0},
                    {0, 1, 0, 0}
                };

            kalman.TransitionMatrix = LinearAcceleration2DModel.GetTransitionMatrix(timeInterval);
        }

        private List<double[]> prepareData()
        {
            initializeKalman();
            var states = generateProcessStates(new Size(100, 100), startPoint, startVelocity).ToList();

            initializeChart();
            plotData(states, 0, (idx, state) => { return new Point((float)state[0], (float)state[1]); }, new DataPoint(), false); //plot process without noise

            return states;
        }

        private List<double[]> correctData(List<double[]> measurements)
        {
            List<double[]> resultData = new List<double[]>();

            foreach (var d in measurements)
            {
                kalman.Predict();
                kalman.Correct(d.Transpose());

                resultData.Add(kalman.CorrectedState.GetColumn(0));
            }

            return resultData;
        }

        #region Generate model

        /// <summary>
        /// Generates process states.
        /// </summary>
        /// <returns>process states</returns>
        private List<double[]> generateProcessStates(Size area, PointF initalPt, PointF initialSpeed)
        {
            Func<PointF, bool> isBorder = (point) =>
            {
                return point.X <= 0 || point.X >= area.Width ||
                       point.Y <= 0 || point.Y >= area.Height;
            };

            var speed = initialSpeed;
            var data = new List<double[]>();
            data.Add(new double[] { initalPt.X, initalPt.Y, speed.X, speed.Y });

            int numOfSides = 0;
            do
            {
                var lastData = data.Last();	

                if (isBorder(new PointF((float)lastData[0], (float)lastData[1])))
                {
                    var x = speed.X;
                    speed.X = -speed.Y;
                    speed.Y = x;

                    numOfSides++;
                }

                data.Add(new double[]
                {
                    lastData[0] + speed.X * timeInterval,
                    lastData[1] + speed.Y * timeInterval, 
                    lastData[1],
                    lastData[2]
                });
            }
            while (numOfSides < 4);

            return data;
        }
       
        private void noiseData(List<double[]> data, out List<double[]> noisyStates, out List<double[]> noisyMeasurements)
        {
            NormalDistribution normalDistribution = new NormalDistribution();

            noisyStates = new List<double[]>();
            noisyMeasurements = new List<double[]>();

            foreach (var d in data)
            {
                var processNoise = kalman.ProcessNoiseCovariance.Multiply(normalDistribution.Generate(kalman.StateVectorDimension));
                var noisyData = d.Add(processNoise);
                noisyStates.Add(noisyData);

                var measurementNoise = normalDistribution.Generate(kalman.MeasurementVectorDimension).Multiply(kalman.MeasurementNoiseCovariance);
                var noisyMeasurement = kalman.MeasurementMatrix.Multiply(noisyData).Add(measurementNoise);
                noisyMeasurements.Add(noisyMeasurement);
            }
        }

        #endregion

        #region GUI...

        #region Chart members

        private void initializeChart()
        {
            chart.ChartAreas[0].AxisX.Title = "X";
            chart.ChartAreas[0].AxisY.Title = "Y";
            chart.ChartAreas[0].AxisX.Minimum = 0;

            chart.Series.Clear();

            //process data (without noise)
            Series s = chart.Series.Add("Process data (withoout noise)");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            s.Color = Color.Gray;

            //process data
            s = chart.Series.Add("Process data (with noise)");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            s.Color = Color.Red;

            //measurement data
            s = chart.Series.Add("Measurement Data");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            s.Color = Color.Black;

            //kalman data
            s = chart.Series.Add("Kalman Corrected Data");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            s.Color = Color.Green;
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

        private void plotData(List<double[]> data, int seriesIdx, Func<int, double[], Point> converterFunc, DataPoint dataPointStyle = null, bool markLastPt = true)
        {
            Series s = chart.Series[seriesIdx];

            dataPointStyle = (dataPointStyle != null) ? dataPointStyle : new DataPoint();

            if (markLastPt)
                tryRemoveLastMarker(seriesIdx, dataPointStyle);

            int idx = 0;
            foreach (var d in data)
            {
                var pt = converterFunc(idx, d);
                var dataPt = dataPointStyle.Clone();
                dataPt.XValue = pt.X;
                dataPt.YValues = new double[] { pt.Y };

                s.Points.Add(dataPt);
                idx++;
            }

            if (markLastPt)
                addMarker(seriesIdx);
        }

        #endregion

        private void num_ValueChanged(object sender, EventArgs e)
        {
            prepareData();
        }

        int currentPt = 0;
        private void timer_Tick(object sender, EventArgs e)
        {
            if(currentPt == (states.Count - 1))
            {
                states = prepareData();
                /*chart.Series[1].Points.Clear();
                chart.Series[2].Points.Clear();*/
                currentPt = 0;
            }

            var data = new List<double[]>();
            data.Add(states[currentPt]);
            currentPt++;

            List<double[]> noisyStates, noisyMeasurements;
            noiseData(data, out noisyStates, out noisyMeasurements);
            plotData(noisyStates, 1, (idx, state) => { return new Point((float)state[0], (float)state[1]); });
            plotData(noisyMeasurements, 2, (idx, measurement) => { return new Point((float)measurement[0], (float)measurement[1]); });

            var correctedData = correctData(noisyMeasurements);

            plotData(correctedData, 3, 
                    (idx, state) => { return new Point((float)state[0], (float)state[1]); },
                    new DataPoint { BorderWidth = 5 });
        }

        private void stopResumeTicker_Click(object sender, EventArgs e)
        {
            timer.Enabled = !timer.Enabled;
        }

        #endregion
    }
}
