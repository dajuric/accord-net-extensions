using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace Accord.Statistics.Filters
{
    public class DiscreteKalmanFilter: KalmanFilter
    {
        /// <summary>
        /// Creates Discrete Kalman filter.
        /// </summary>
        /// <param name="initialState">The best estimate of the initial state. [n x 1] vector. It's dimension is - n.</param>
        /// <param name="processNoiseCovariance">The covariance of the initial state estimate. [n x n] matrix.</param>
        ///<param name="measurementVectorDimension">Dimensionality of the measurement vector - p.</param>
        /// <param name="controlVectorDimension">Dimensionality of the control vector - k. If there is no external control put 0.</param>
        public DiscreteKalmanFilter(double[,] initialState, int measurementVectorDimension, int controlVectorDimension)
            :base(initialState, measurementVectorDimension, controlVectorDimension)
        {}

        /// <summary>
        /// Estimates the subsequent model state <see cref="PredictedState"/>.
        /// 
        /// x'(k) = A * x(k-1) + B * u(k).
        /// P'(k) = A * P(k-1) * At + Q 
        /// </summary>
        public override void Predict(double[,] controlVector)
        {
            this.PredictedState = this.TransitionMatrix.Multiply(this.CorrectedState);
                
            if(controlVector != null)
                this.PredictedState = this.PredictedState.Add(this.ControlMatrix.Multiply(controlVector));

            this.PrioriErrorCovariance = this.TransitionMatrix.Multiply(this.PosterioriErrorCovariance).Multiply(this.TransitionMatrix.Transpose()).Add(this.ProcessNoiseCovariance);
        }

        /// <summary>
        /// The function adjusts the stochastic model state on the basis of the given measurement of the model state <see cref="CorrectedState"/>.
        /// 
        /// K(k) = P'(k) * Ht * (H * P'(k) * Ht + R)^(-1)
        /// x(k) = x'(k) + K(k) * (z(k) - H * x'(k))
        /// P(k) =(I - K(k) * H) * P'(k)
        /// </summary>
        /// <param name="measurement">Obtained measurement vector.</param>
        public override void Correct(double[,] measurement)
        {
            var measurementMatrixTransponsed = this.MeasurementMatrix.Transpose();

            var S = this.MeasurementMatrix.Multiply(this.PrioriErrorCovariance).Multiply(measurementMatrixTransponsed).Add(this.MeasurementNoiseCovariance);
            this.KalmanGain = this.PrioriErrorCovariance.Multiply(measurementMatrixTransponsed).Multiply(S.Inverse());

            var predictedMeasurement = this.MeasurementMatrix.Multiply(this.PredictedState);
            var measurementError = measurement.Subtract(predictedMeasurement);
            this.CorrectedState = this.PredictedState.Add(this.KalmanGain.Multiply(measurementError));

            var identity = Matrix.Identity(this.StateVectorDimension);
            this.PosterioriErrorCovariance = (identity.Subtract(this.KalmanGain.Multiply(this.MeasurementMatrix))).Multiply(this.PrioriErrorCovariance.Transpose());
        }
    }
}
