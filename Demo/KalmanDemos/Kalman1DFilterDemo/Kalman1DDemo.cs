using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Filters;
using Point = AForge.Point;

namespace Kalman1DFilterDemo
{
    public partial class Kalman1DDemo : Form
    {
        Func<List<double[]>> dataCreationFunction;
        DiscreteKalmanFilter kalman;

        double timeInterval = 0;
        double startSpeed = 2;

        public Kalman1DDemo()
        {
            InitializeComponent();
            dataCreationFunction = generateStates_ConstantSpeed;

            prepareData();
        }

        private void initializeKalman()
        {
            var measurementDimension = 1; //just coordinate

            var processNoise = (double)this.numProcessNoise.Value;
            var measurementNoise = (double)this.numMeasurementNoise.Value;

            var initialState = LinearAcceleration1DModel.GetInitialState(0, (float)startSpeed);

            kalman = new DiscreteKalmanFilter(initialState, measurementDimension /*(position)*/, 0 /*no acceleration*/ );

            kalman.ProcessNoiseCovariance = LinearAcceleration1DModel.GetProcessNoiseCovariance(processNoise, timeInterval);
            kalman.MeasurementNoiseCovariance = Matrix.Identity(measurementDimension).Multiply(measurementNoise * measurementNoise);

            kalman.MeasurementMatrix = new double[,] //just pick point coordinates for an observation [1 x 2]
                { 
                    {1, 0}
                };

            kalman.TransitionMatrix = LinearAcceleration1DModel.GetTransitionMatrix(timeInterval);
        }

        private void prepareData()
        { 
           timeInterval = (double)this.numTimeInterval.Value;
           initializeKalman();
           var data = dataCreationFunction();

           initializeChart();

           List<double[]> noisyStates, noisyMeasurements;
           noiseData(data, out noisyStates, out noisyMeasurements);
           plotData(noisyStates, 0, (idx, state) => { return new Point(idx, (float)state[0]); });
           plotData(noisyMeasurements, 1, (idx, measurement) => { return new Point(idx, (float)measurement[0]); });

           var correctedData = correctData(noisyMeasurements);
           plotData(correctedData, 2, (idx, state) => { return new Point(idx, (float)state[0]); }, new DataPoint{BorderWidth = 5});
        }

        private List<double[]> correctData(List<double[]> data)
        {
            List<double[]> resultData = new List<double[]>();

            foreach (var d in data)
            {
                kalman.Predict();
                kalman.Correct(d.Transpose());

                resultData.Add(kalman.CorrectedState.GetColumn(0));
            }

            return resultData;
        }

        #region Generate model

        private List<double[]> generateStates_ConstantSpeed()
        {
            double speed = startSpeed;

            int maxTime = 100;
            List<double[]> data = new List<double[]>(maxTime);

            double prevPt = 0;
            for (double i = 0; i < maxTime; i += timeInterval)
            {
                double pt = prevPt + speed * timeInterval;//0.1 * i + 0;
                prevPt = pt;
                data.Add(new double[] { pt, speed });
            }

            return data;
        }

        private List<double[]> generateStates_SuddenJump()
        {
            double speed = startSpeed;

            int maxTime = 100;
            List<double[]> data = new List<double[]>(maxTime);

            bool hasJumped = false;
            double prevPt = 0;
            for (double i = 0; i < maxTime; i += timeInterval)
            {
                if (!hasJumped && (i - maxTime / 2) > 0)
                {
                    prevPt = -prevPt / 2;
                    hasJumped = true;
                }

                double pt = prevPt + speed * timeInterval;//0.1 * i + 0;
                prevPt = pt;
                data.Add(new double[] { pt, speed });
            }

            return data;
        }

        private List<double[]> generateStates_SpeedDecrease()
        {
            double speed = startSpeed;

            int maxTime = 100;
            List<double[]> data = new List<double[]>(maxTime);

            double prevPt = 0;
            for (double i = 0; i < maxTime; i += timeInterval)
            {
                if ((i - maxTime / 2) > 0)
                    speed = -speed;

                double pt = prevPt + speed * timeInterval;//0.1 * i + 0;
                prevPt = pt;
                data.Add(new double[] { pt, speed });
            }

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

        private void initializeChart()
        {
            chart.ChartAreas[0].AxisX.Title = "Time";
            chart.ChartAreas[0].AxisY.Title = "Distance";
            chart.ChartAreas[0].AxisX.Minimum = 0;

            chart.Series.Clear();

            //process data
            Series s = chart.Series.Add("Process data (with noise)");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            s.Color = Color.Red;

            //measurement data
            s = chart.Series.Add("Measurement Data");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            s.Color = Color.Black;

            //kalman data
            s = chart.Series.Add("Kalman Data");
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            s.Color = Color.Green;
        }

        private void plotData(List<double[]> data, int seriesIdx, Func<int, double[], Point> converterFunc, DataPoint dataPointStyle = null)
        {
            Series s = chart.Series[seriesIdx];

            dataPointStyle = (dataPointStyle != null) ? dataPointStyle : new DataPoint();

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

            //chart.ChartAreas[0].RecalculateAxesScale();
        }

        private void num_ValueChanged(object sender, EventArgs e)
        {
            prepareData();
        }

        private void lstProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.lstProcess.SelectedItem.ToString())
            { 
                case "Constant Speed":
                    dataCreationFunction = generateStates_ConstantSpeed;
                    break;
                case "Sudden Jump":
                    dataCreationFunction = generateStates_SuddenJump;
                    break;
                case "Speed Decrease":
                    dataCreationFunction = generateStates_SpeedDecrease;
                    break;
            }

            prepareData();
        }

        #endregion
    }
}
