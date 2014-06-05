using Accord.Math;
using Accord.Extensions.Math;
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
        /// K(k) = P'(k) * Ht * (H * P'(k) * Ht + R)^(-1)
        /// </summary>
        protected override void predictInternal(double[] controlVector)
        {
            //x'(k) = A * x(k-1)
            this.state = this.state.Multiply(this.TransitionMatrix);

            //x'(k) =  x'(k) + B * u(k)
            if(controlVector != null)
                this.state = this.state.Add(this.ControlMatrix.Multiply(controlVector));

           //P'(k) = A * P(k-1) * At + Q 
           this.ErrorCovariance = this.TransitionMatrix.Multiply(this.ErrorCovariance).Multiply(this.TransitionMatrix.Transpose()).Add(this.ProcessNoise);

           /******* calculate Kalman gain **********/
           var measurementMatrixTransponsed = this.MeasurementMatrix.Transpose();

           //S(k) = H * P'(k) * Ht + R
           this.CovarianceMatrix = this.MeasurementMatrix.Multiply(this.ErrorCovariance).Multiply(measurementMatrixTransponsed).Add(this.MeasurementNoise);
           this.CovarianceMatrixInv = this.CovarianceMatrix.Inverse();

           //K(k) = P'(k) * Ht * S(k)^(-1)
           this.KalmanGain = this.ErrorCovariance.Multiply(measurementMatrixTransponsed).Multiply(this.CovarianceMatrixInv);
           /******* calculate Kalman gain **********/
        }

        /// <summary>
        /// The function adjusts the stochastic model state on the basis of the given measurement of the model state <see cref="CorrectedState"/>.
        /// 
        /// x(k) = x'(k) + K(k) * (z(k) - H * x'(k))
        /// P(k) =(I - K(k) * H) * P'(k)
        /// </summary>
        /// <param name="measurement">Obtained measurement vector.</param>
        protected override void correctInternal(double[] measurement)
        {
            //innovation vector (measurement error)
            var delta = this.CalculatePredictionError(measurement);
            correct(delta);
        }

        private void correct(double[] innovationVector)
        {
            if (innovationVector.Length != this.MeasurementVectorDimension)
                throw new Exception("PredicitionError error vector (innovation vector) must have the same length as measurement.");

            //correct state using Kalman gain
            this.state = this.state.Add(this.KalmanGain.Multiply(innovationVector));

            var identity = Matrix.Identity(this.StateVectorDimension);
            this.ErrorCovariance = (identity.Subtract(this.KalmanGain.Multiply(this.MeasurementMatrix))).Multiply(this.ErrorCovariance.Transpose());
        }

        /// <summary>
        /// Corrects the state error covariance based on innovation vector and Kalman update.
        /// </summary>
        /// <param name="innovationVector">The difference between predicted state and measurement.</param>
        internal void Correct(double[] innovationVector)
        {
            //innovationVector error handled by correct(...)

            checkPrerequisites();
            correct(innovationVector);
        }

        /// <summary>
        /// Corrects the state error covariance based on innovation vector and Kalman update.
        /// </summary>
        /// <param name="innovationVector">The difference between predicted state and measurement.</param>
        /// <param name="covarianceMixtureFactor">Covariance mixture factor. Used in JPDAF. For Kalman filter default value is 0.</param>
        /// <param name="innovationCovariance">The innovation covariance matrix. Used primary by JPDAF.</param>
        internal void Correct(double[] innovationVector, double covarianceMixtureFactor, double[,] innovationCovariance)
        {
            //innovationVector error handled by correct(...)

            if (innovationCovariance.ColumnCount() != this.MeasurementVectorDimension ||
                innovationCovariance.RowCount()    != this.MeasurementVectorDimension)
            {
                throw new ArgumentException("Innovation matrix must have the same dimensions and the dimension must be equal to the measurement vector length.");
            }

            checkPrerequisites();

            var priorErrorCovariance = this.ErrorCovariance.Multiply(covarianceMixtureFactor);

            this.correct(innovationVector);
            var posterioriErrorCovariance = this.ErrorCovariance.Multiply(1 - covarianceMixtureFactor);

            var innovationCov = this.KalmanGain.Multiply(innovationCovariance).Multiply(this.KalmanGain.Transpose());

            this.ErrorCovariance = priorErrorCovariance.Add(posterioriErrorCovariance).Add(innovationCov);
        }
    }
}
