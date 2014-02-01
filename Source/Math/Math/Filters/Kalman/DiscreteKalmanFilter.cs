using Accord.Math;
using System;

namespace Accord.Extensions.Statistics.Filters
{
    public class DiscreteKalmanFilter<TState, TMeasurement>: KalmanFilter<TState, TMeasurement>
    {
        /// <summary>
        /// Creates Discrete Kalman filter.
        /// </summary>
        /// <param name="initialState">The best estimate of the initial state. [n x 1] vector. It's dimension is - n.</param>
        /// <param name="initialStateError">Initial error for a state: (assumed values – actual values)^2 + the variance of the values.
        /// <para>e.g. if using ConstantAccelerationModel it can be specified as: Matrix.Diagonal(StateVectorDimension, [x, y, vX, vY, aX, aY]);</para> 
        /// </param>
        /// <param name="processNoiseCovariance">The covariance of the initial state estimate. [n x n] matrix.</param>
        /// <param name="measurementVectorDimension">Dimensionality of the measurement vector - p.</param>
        /// <param name="controlVectorDimension">Dimensionality of the control vector - k. If there is no external control put 0.</param>
        /// <param name="stateConvertFunc">State conversion function: TState => double[]</param>
        /// <param name="stateConvertBackFunc">State conversion function: double[] => TState</param>
        /// <param name="measurementConvertFunc">Measurement conversion function: TMeasurement => double[]</param>
        public DiscreteKalmanFilter(TState initialState, double[,] initialStateError, 
                                    int measurementVectorDimension, int controlVectorDimension,
                                    Func<TState, double[]> stateConvertFunc, Func<double[], TState> stateConvertBackFunc, Func<TMeasurement, double[]> measurementConvertFunc)
            :base(initialState, initialStateError, 
                  measurementVectorDimension, controlVectorDimension, 
                  stateConvertFunc, stateConvertBackFunc, measurementConvertFunc)
        {}

        /// <summary>
        /// Estimates the subsequent model state <see cref="PredictedState"/>.
        /// 
        /// x'(k) = A * x(k-1) + B * u(k).
        /// P'(k) = A * P(k-1) * At + Q 
        /// </summary>
        protected override void predict(double[] controlVector)
        {
            this.state = this.TransitionMatrix.Multiply(this.state);
                
            if(controlVector != null)
                this.state = this.state.Add(this.ControlMatrix.Multiply(controlVector));

           this.ErrorCovariance = this.TransitionMatrix.Multiply(this.ErrorCovariance).Multiply(this.TransitionMatrix.Transpose()).Add(this.ProcessNoise);
        }

        /// <summary>
        /// The function adjusts the stochastic model state on the basis of the given measurement of the model state <see cref="CorrectedState"/>.
        /// 
        /// K(k) = P'(k) * Ht * (H * P'(k) * Ht + R)^(-1)
        /// x(k) = x'(k) + K(k) * (z(k) - H * x'(k))
        /// P(k) =(I - K(k) * H) * P'(k)
        /// </summary>
        /// <param name="measurement">Obtained measurement vector.</param>
        protected override void correct(double[] measurement)
        {
            var measurementMatrixTransponsed = this.MeasurementMatrix.Transpose();

            var S = this.MeasurementMatrix.Multiply(this.ErrorCovariance).Multiply(measurementMatrixTransponsed).Add(this.MeasurementNoise);
            this.KalmanGain = this.ErrorCovariance.Multiply(measurementMatrixTransponsed).Multiply(S.Inverse());

            var predictedMeasurement = this.MeasurementMatrix.Multiply(this.state);
            var measurementError = measurement.Subtract(predictedMeasurement);
            this.state = this.state.Add(this.KalmanGain.Multiply(measurementError));

            var identity = Matrix.Identity(this.StateVectorDimension);
            this.ErrorCovariance = (identity.Subtract(this.KalmanGain.Multiply(this.MeasurementMatrix))).Multiply(this.ErrorCovariance.Transpose());
        }
    }
}
