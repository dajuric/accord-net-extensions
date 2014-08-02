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
using Accord.Extensions.Math.Geometry;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using PointF = AForge.Point;

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
    /// </summary>
    public abstract class KalmanFilter<TState, TMeasurement>
    {
        Func<TState, double[]> stateConvertFunc;
        Func<double[], TState> stateConvertBackFunc;
        Func<TMeasurement, double[]> measurementConvertFunc;

        /// <summary>
        /// Creates Kalman filter.
        /// </summary>
        /// <param name="initialState">The best estimate of the initial state. [1 x n] vector. It's dimension is - n.</param>
        /// <param name="initialStateError">Initial error for a state: (assumed values – actual values)^2 + the variance of the values.
        /// <para>e.g. if using ConstantAccelerationModel it can be specified as: Matrix.Diagonal(StateVectorDimension, [x, y, vX, vY, aX, aY]);</para> 
        /// </param>
        ///<param name="measurementVectorDimension">Dimensionality of the measurement vector - p.</param>
        /// <param name="controlVectorDimension">Dimensionality of the control vector - k. If there is no external control put 0.</param>
        /// <param name="stateConvertFunc">State conversion function: TState => double[]</param>
        /// <param name="stateConvertBackFunc">State conversion function: double[] => TState</param>
        /// <param name="measurementConvertFunc">Measurement conversion function: TMeasurement => double[]</param>
        protected KalmanFilter(TState initialState, double[,] initialStateError, 
                               int measurementVectorDimension, int controlVectorDimension, 
                               Func<TState, double[]> stateConvertFunc, Func<double[], TState> stateConvertBackFunc, Func<TMeasurement, double[]> measurementConvertFunc)
        {
            initalize(initialState, initialStateError,
                      measurementVectorDimension, controlVectorDimension, 
                      stateConvertFunc, stateConvertBackFunc, measurementConvertFunc);
        }

        private void initalize(TState initialState, double[,] initialStateError,
                               int measurementVectorDimension, int controlVectorDimension,
                               Func<TState, double[]> stateConvertFunc, Func<double[], TState> stateConvertBackFunc, Func<TMeasurement, double[]> measurementConvertFunc)
        {
            var _state = stateConvertFunc(initialState);
            
            this.StateVectorDimension = _state.Length;
            this.MeasurementVectorDimension = measurementVectorDimension;
            this.ControlVectorDimension = controlVectorDimension;

            this.state = _state;
            this.ErrorCovariance = initialStateError;

            this.stateConvertFunc = stateConvertFunc;
            this.stateConvertBackFunc = stateConvertBackFunc;
            this.measurementConvertFunc = measurementConvertFunc;
        }

        /// <summary>
        /// Checks pre-conditions: matrix sizes.
        /// </summary>
        protected void checkPrerequisites()
        {
            /************************** TRANSITION MATRIX ***************************/
            if (this.TransitionMatrix == null)
                throw new Exception("Transition matrix cannot be null!");

            if (this.TransitionMatrix.RowCount() != this.StateVectorDimension || this.TransitionMatrix.ColumnCount() != this.StateVectorDimension)
                throw new Exception("Transition matrix dimensions are not valid!");
            /************************** TRANSITION MATRIX ***************************/

            /************************** CONTROL MATRIX ***************************/
            if (this.ControlMatrix == null && this.ControlVectorDimension != 0)
                throw new Exception("Control matrix can be null only if control vector dimension is set to 0!");

            if (this.ControlMatrix != null && (this.ControlMatrix.RowCount() != this.StateVectorDimension || this.ControlMatrix.ColumnCount() != this.ControlVectorDimension))
                throw new Exception("Control matrix dimensions are not valid!");
            /************************** CONTROL MATRIX ***************************/

            /************************** MEASUREMENT MATRIX ***************************/
            if (this.MeasurementMatrix == null)
                throw new Exception("Measurement matrix cannot be null!");

            if (this.MeasurementMatrix.RowCount() != this.MeasurementVectorDimension || this.MeasurementMatrix.ColumnCount() != this.StateVectorDimension)
                throw new Exception("Measurement matrix dimesnions are not valid!");
            /************************** MEASUREMENT MATRIX ***************************/

            /************************** PROCES NOISE COV. MATRIX ***************************/
            if (this.ProcessNoise == null)
                throw new Exception("Process noise covariance matrix cannot be null!");

            if (this.ProcessNoise.RowCount() != this.StateVectorDimension || this.ProcessNoise.ColumnCount() != this.StateVectorDimension)
                throw new Exception("Process noise covariance matrix dimensions are not valid!");
            /************************** PROCES NOISE COV. MATRIX ***************************/

            /************************** MEASUREMENT NOISE COV. MATRIX ***************************/
            if (this.MeasurementNoise == null)
                throw new Exception("Measurement noise covariance matrix cannot be null!");

            if (this.MeasurementNoise.RowCount() != this.MeasurementVectorDimension || this.MeasurementNoise.ColumnCount() != this.MeasurementVectorDimension)
                throw new Exception("Measurement noise covariance matrix dimensions are not valid!");
            /************************** MEASUREMENT NOISE COV. MATRIX ***************************/
        }

        #region IKalman Members

        /// <summary>
        /// State.
        /// </summary>
        protected double[] state;

        /// <summary>
        /// Gets state (x(k)). [1 x n] vector.
        /// After obtaining a measurement z(k) predicted state will be corrected.
        /// This value is used as an ultimate result.
        /// </summary>
        public TState State
        {
            get { return stateConvertBackFunc(state); }
        }

        /// <summary>
        /// Gets Kalman covariance matrix (S). [p x p] matrix.
        /// This matrix servers for Kalman gain calculation.
        /// <para>The matrix along with innovation vector can be used to achieve gating in JPDAF. See: <see cref="Accord.Extensions.Statistics.Filters.JPDAF"/> filter.</para>
        /// </summary>
        public double[,] CovarianceMatrix 
        {
            get; 
            protected set; 
        }

        /// <summary>
        /// Gets the inverse of covariance matrix. See: <see cref="CovarianceMatrix"/>.
        /// </summary>
        public double[,] CovarianceMatrixInv
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets Kalman gain matrix (K). [n x p] matrix.
        /// </summary>
        public double[,] KalmanGain
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets error estimate covariance matrix (P(k)). [n x n] matrix.
        /// </summary>
        public double[,] ErrorCovariance
        {
            get;
            protected set;
        }

        #region Predict methods

        /// <summary>
        /// Estimates the subsequent model state. 
        /// This function is implementation-dependent.
        /// </summary>
        public void Predict()
        {
            Predict(null);
        }

        /// <summary>
        /// Estimates the subsequent model state.
        /// This function is implementation-dependent.
        /// </summary>
        public void Predict(double[] controlVector)
        {
            checkPrerequisites();
            predictInternal(controlVector);
        }

        /// <summary>
        /// Predicts the next state using the current state and <paramref name="controlVector"/>.
        /// </summary>
        /// <param name="controlVector">Set of data for external system control.</param>
        protected abstract void predictInternal(double[] controlVector);

        #endregion

        #region Correct methods

        /// <summary>
        /// Corrects the state error covariance based on innovation vector and Kalman update.
        /// </summary>
        /// <param name="measurement">The measurement.</param>
        public void Correct(TMeasurement measurement) 
        {
            checkPrerequisites();
            CorrectInternal(measurementConvertFunc(measurement));
        }

        /// <summary>
        /// Corrects the state by using the provided measurement.
        /// </summary>
        /// <param name="measurement">Measurement.</param>
        protected abstract void CorrectInternal(double[] measurement);

        #endregion

        #endregion

        #region IKalmanUserDefinedData Members

        /// <summary>
        /// Gets or sets state transition matrix (A). [n x n] matrix.
        /// </summary>
        public double[,] TransitionMatrix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets control matrix (B). [n x k] vector.
        /// It is not used if there is no control.
        /// </summary>
        public double[,] ControlMatrix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets measurement matrix (H). [p x n] matrix, where p is a dimension of measurement vector. <br/>
        /// <para>Selects components from a state vector that are obtained by measurement.</para>
        /// </summary>
        public double[,] MeasurementMatrix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets process noise covariance matrix (Q). [n x n] matrix.
        /// <para>Deviation of selected and actual model. 
        /// e.g. for constant acceleration model it can be defined as: Matrix.Diagonal(StateVectorDimension, [x, y, vX, vY, aX, aY]); where usually (position error) &lt; (velocity error) &lt; (acceleration error).</para>
        /// </summary>
        public double[,] ProcessNoise
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets measurement noise covariance matrix (R). [p x p] matrix.
        /// <para>Variance inaccuracy of detected location. 
        /// It is usually defined as scalar, therefore it can be set as: Matrix.Diagonal(MeasurementVectorDimension, [value]);</para>
        /// </summary>
        public double[,] MeasurementNoise
        {
            get;
            set;
        }

        #endregion

        #region IKalmanDataInfo Members

        /// <summary>
        /// Gets state dimensionality.
        /// </summary>
        public int StateVectorDimension
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets measurement dimensionality.
        /// </summary>
        public int MeasurementVectorDimension
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets control vector dimensionality.
        /// </summary>
        public int ControlVectorDimension
        {
            get;
            private set;
        }

        #endregion

        #region Misc methods

        /// <summary>
        /// Calculates the residual from the measurement and predicted state.
        /// </summary>
        /// <param name="measurement">Measurement.</param>
        /// <returns>Residual, error or innovation vector.</returns>
        public double[] CalculatePredictionError(TMeasurement measurement)
        {
            checkPrerequisites();

            var m = measurementConvertFunc(measurement);
            return CalculatePredictionError(m);
        }

        internal double[] CalculatePredictionError(double[] measurement)
        {
            //innovation vector (measurement error)
            var predictedMeasurement = this.MeasurementMatrix.Multiply(this.state);
            var measurementError = measurement.Subtract(predictedMeasurement);
            return measurementError;
        }

        /// <summary>
        /// Calculates entropy from the error covariance matrix.
        /// </summary>
        public double CalculateEntropy()
        {
            return CalculateEntropy(this.ErrorCovariance);
        }

        /// <summary>
        /// Calculates entropy from the provided error covariance matrix.
        /// </summary>
        /// <param name="errorCovariance">Error covariance matrix.</param>
        /// <returns>Entropy.</returns>
        public static double CalculateEntropy(double[,] errorCovariance)
        {
            if (errorCovariance == null || errorCovariance.RowCount() != errorCovariance.ColumnCount())
                throw new ArgumentException("Error covariance matrix (P) must have the same number of rows and columns.");

            var stateVectorDim = errorCovariance.RowCount();

            var errorCovDet = errorCovariance.Determinant();
            double entropy = (float)stateVectorDim / 2 * System.Math.Log(4 * System.Math.PI) + (float)1 / 2 * System.Math.Log(errorCovDet);
            return entropy;
        }

        /// <summary>
        /// Gets the spatial representation of the error covariance. 
        /// </summary>
        /// <param name="positionSelector">Position selector function.</param>
        /// <param name="confidence">
        /// Confidence for the Chi-square distribution. 
        /// H * P * H' has the Chi-square distribution where H is measurement matrix and P is error covariance matrix.
        /// </param>
        /// <param name="positionSelectionMatrix">Measurement matrix which selects state position. If null the state measurement matrix will be used.</param>
        /// <returns>2D representation of the error covariance.</returns>
        public Ellipse GetEllipse(Func<TState, PointF> positionSelector, double confidence = 0.99, double[,] positionSelectionMatrix = null)
        {
            positionSelectionMatrix = positionSelectionMatrix ?? this.MeasurementMatrix;

            var measurementErrorCov = positionSelectionMatrix.Multiply(this.ErrorCovariance).Multiply(positionSelectionMatrix.Transpose());
            var chiSquare = new ChiSquareDistribution(2).InverseDistributionFunction(confidence);

            var cov = measurementErrorCov.Multiply(chiSquare);
            var ellipse = Ellipse.Fit(cov);
            ellipse.Center = positionSelector(this.State);
            return ellipse;
        }

        #endregion
    }
}
