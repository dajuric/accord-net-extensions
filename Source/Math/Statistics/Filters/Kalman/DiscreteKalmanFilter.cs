#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using Accord.Extensions.Math;
using Accord.Math;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// A Kalman filter is a recursive solution to the general dynamic estimation problem for the
    /// important special case of linear system models and Gaussian noise.
    /// <para>The Kalman Filter uses a predictor-corrector structure, in which
    /// if a measurement of the system is available at time <italic>t</italic>,
    /// We first call the Predict function, to estimate the state of the system
    /// at time <italic>t</italic>. We then call the Correct function to
    /// correct the estimate of state, based on the noisy measurement.</para>
    /// 
    /// <para>
    /// The discrete Kalman filter can process linear models which have Gaussian noise. 
    /// If the model is not linear then estimate transition matrix (and other parameters if necessary) in each step and update Kalman filter.
    /// This "dynamic" version of an Discrete Kalman filter is called Extended Kalman filter and it is used for non-linear models.
    /// If the model is highly non-linear an Unscented Kalman filter or particle filtering is used.
    /// See: <a href="http://en.wikipedia.org/wiki/Kalman_filter"/> for details.
    /// </para>
    /// </summary>
    public class DiscreteKalmanFilter<TState, TMeasurement>: KalmanFilter<TState, TMeasurement>
    {
        /// <summary>
        /// Creates Discrete Kalman filter.
        /// </summary>
        /// <param name="initialState">The best estimate of the initial state. [n x 1] vector. It's dimension is - n.</param>
        /// <param name="initialStateError">Initial error for a state: (assumed values – actual values)^2 + the variance of the values.
        /// <para>e.g. if using ConstantAccelerationModel it can be specified as: Matrix.Diagonal(StateVectorDimension, [x, y, vX, vY, aX, aY]);</para> 
        /// </param>
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
        /// Estimates the subsequent model state.
        /// 
        /// x'(k) = A * x(k-1) + B * u(k).
        /// P'(k) = A * P(k-1) * At + Q 
        /// K(k) = P'(k) * Ht * (H * P'(k) * Ht + R)^(-1)
        /// </summary>
        protected override void predictInternal(double[] controlVector)
        {
            //x'(k) = A * x(k-1)
            //this.state = this.state.Multiply(this.TransitionMatrix);
            this.state = this.TransitionMatrix.Multiply(this.state);

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
        /// The function adjusts the stochastic model state on the basis of the given measurement of the model state.
        /// 
        /// x(k) = x'(k) + K(k) * (z(k) - H * x'(k))
        /// P(k) =(I - K(k) * H) * P'(k)
        /// </summary>
        /// <param name="measurement">Obtained measurement vector.</param>
        protected override void CorrectInternal(double[] measurement)
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
