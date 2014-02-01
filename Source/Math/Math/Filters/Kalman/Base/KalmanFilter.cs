using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions.Math;

namespace Accord.Extensions.Statistics.Filters
{  
    /// <summary>
    /// A Kalman filter is a recursive solution to the general dynamic estimation problem for the
    /// important special case of linear system models and gaussian noise.
    /// <para>The Kalman Filter uses a predictor-corrector structure, in which
    /// if a measurement of the system is available at time <italic>t</italic>,
    /// we first call the Predict function, to estimate the state of the system
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
        /// <param name="processNoiseCovariance">The covariance of the initial state estimate. [n x n] matrix.</param>
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

        private void checkPrerequisites()
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

        protected double[] state;

        /// <summary>
        /// Gets state (x(k)). [1 x n] vector.
        /// After obtaining a measurement z(k) <see cref="PredictedState"/> will be corrected.
        /// This value is used as an ultimate result.
        /// </summary>
        public TState State
        {
            get { return stateConvertBackFunc(state); }
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
        /// This function is implementation dependent.
        /// </summary>
        public void Predict()
        {
            Predict(null);
        }

        /// <summary>
        /// Estimates the subsequent model state.
        /// This function is implementation dependent.
        /// </summary>
        public void Predict(double[] controlVector)
        {
            checkPrerequisites();
            predict(controlVector);
        }

        protected abstract void predict(double[] controlVector);

        #endregion

        #region Correct methods

        public void Correct(TMeasurement measurement) 
        {
            checkPrerequisites();
            correct(measurementConvertFunc(measurement));
        }

        protected abstract void correct(double[] measurement);

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

        double[,] measurementNoiseCovariance;
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
        /// Gets state dimensionionality.
        /// </summary>
        public int StateVectorDimension
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets measurement dimensionionality.
        /// </summary>
        public int MeasurementVectorDimension
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets control vector dimensionionality.
        /// </summary>
        public int ControlVectorDimension
        {
            get;
            private set;
        }

        #endregion
    }
}
