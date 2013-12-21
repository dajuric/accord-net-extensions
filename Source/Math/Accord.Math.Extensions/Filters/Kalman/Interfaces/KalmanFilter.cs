using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace Accord.Statistics.Filters
{  
    /// <summary>
    /// A Kalman filter is a recursive solution to the general dynamic estimation problem for the
    /// important special case of linear system models and gaussian noise.
    /// <para>The Kalman Filter uses a predictor-corrector structure, in which
    /// if a measurement of the system is available at time <italic>t</italic>,
    /// we first call the Predict function, to estimate the state of the system
    /// at time <italic>t</italic>. We then call the Correct function to
    /// correct the estimate of state, based on the noisy measurement.</para
    /// </summary>
    public abstract class KalmanFilter: IKalman
    {
        /// <summary>
        /// Creates Kalman filter.
        /// </summary>
        /// <param name="initialState">The best estimate of the initial state. [n x 1] vector. It's dimension is - n.</param>
        /// <param name="processNoiseCovariance">The covariance of the initial state estimate. [n x n] matrix.</param>
        ///<param name="measurementVectorDimension">Dimensionality of the measurement vector - p.</param>
        /// <param name="controlVectorDimension">Dimensionality of the control vector - k. If there is no external control put 0.</param>
        public KalmanFilter(double[,] initialState, int measurementVectorDimension, int controlVectorDimension)
        {
            initalize(initialState, measurementVectorDimension, controlVectorDimension);
        }

        private void initalize(double[,] initialState, int measurementVectorDimension, int controlVectorDimension)
        {
            this.StateVectorDimension = initialState.RowCount();
            this.MeasurementVectorDimension = measurementVectorDimension;
            this.ControlVectorDimension = controlVectorDimension;

            this.CorrectedState = initialState;
            this.ProcessNoiseCovariance = new double[this.StateVectorDimension, this.StateVectorDimension];
            this.PosterioriErrorCovariance = this.ProcessNoiseCovariance;
        }

        #region IKalman Members

        /// <summary>
        /// Gets predicted state (x'(k)). [n x 1] vector.
        /// </summary>
        public double[,] PredictedState
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets corrected state (x(k)). [n x 1] vector.
        /// After obtaining a measurement z(k) <see cref="PredictedState"/> will be corrected.
        /// This value is used as an ultimate result.
        /// </summary>
        public double[,] CorrectedState
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets priori error estimate covariance matrix (P'(k)). [n x n] matrix.
        /// </summary>
        public double[,] PrioriErrorCovariance
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
        /// Gets posteriori error estimate covariance matrix (P(k)). [n x n] matrix.
        /// </summary>
        public double[,] PosterioriErrorCovariance
        {
            get;
            protected set;
        }

        /// <summary>
        /// Estimates the subsequent model state <see cref="PredictedState"/>.
        /// </summary>
        public void Predict()
        {
            Predict(null);
        }

        public abstract void Predict(double[,] controlVector);

        public abstract void Correct(double[,] measurement);

        #endregion

        #region IKalmanUserDefinedData Members

        double[,] transitionMatrix;
        /// <summary>
        /// Gets or sets state transition matrix (A). [n x n] matrix.
        /// </summary>
        public double[,] TransitionMatrix
        {
            get
            {
                return transitionMatrix;
            }
            set
            {
                if (value == null)
                    throw new Exception("Transition matrix cannot be null!");

                if (value.RowCount() != this.StateVectorDimension || value.ColumnCount() != this.StateVectorDimension)
                    throw new Exception("Transition matrix dimensions are not valid!");

                transitionMatrix = value;
            }
        }

        double[,] controlMatrix;
        /// <summary>
        /// Gets or sets control matrix (B). [n x k] vector.
        /// It is not used if there is no control.
        /// </summary>
        public double[,] ControlMatrix
        {
            get
            {
                return controlMatrix;
            }
            set
            {
                if (value == null && this.ControlVectorDimension != 0)
                    throw new Exception("Control matrix can be null only if control vector dimension is set to 0!");

                if (value != null && (value.RowCount() != this.StateVectorDimension || value.ColumnCount() != this.ControlVectorDimension))
                    throw new Exception("Control matrix dimensions are not valid!");

                controlMatrix = value;
            }
        }

        double[,] measurementMatrix;
        /// <summary>
        /// Gets or sets measurement matrix (H). [p x n] matrix, where p is a dimension of measurement vector. <br/>
        /// Selects components from a state vector that are obtained by measurement.
        /// </summary>
        public double[,] MeasurementMatrix
        {
            get
            {
                return measurementMatrix;
            }
            set
            {
                if (value == null)
                    throw new Exception("Measurement matrix cannot be null!");

                if (value.RowCount() != this.MeasurementVectorDimension || value.ColumnCount() != this.StateVectorDimension)
                    throw new Exception("Measurement matrix dimesnions are not valid!");

                measurementMatrix = value;
            }
        }

        double[,] processNoiseCovariance;
        /// <summary>
        /// Gets or sets process noise covariance matrix (Q). [n x n] matrix.
        /// </summary>
        public double[,] ProcessNoiseCovariance
        {
            get
            {
                return processNoiseCovariance;
            }
            set
            {
                if (value == null)
                    throw new Exception("Process noise covariance matrix cannot be null!");

                if (value.RowCount() != this.StateVectorDimension || value.ColumnCount() != this.StateVectorDimension)
                    throw new Exception("Process noise covariance matrix dimensions are not valid!");

                processNoiseCovariance = value;
            }
        }

        double[,] measurementNoiseCovariance;
        /// <summary>
        /// Gets or sets measurement noise covariance matrix (R). [p x p] matrix.
        /// </summary>
        public double[,] MeasurementNoiseCovariance
        {
            get
            {
                return measurementNoiseCovariance;
            }
            set
            {
                if (value == null)
                    throw new Exception("Measurement noise covariance matrix cannot be null!");

                if (value.RowCount() != this.MeasurementVectorDimension || value.ColumnCount() != this.MeasurementVectorDimension)
                    throw new Exception("Measurement noise covariance matrix dimensions are not valid!");

                measurementNoiseCovariance = value;
            }
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
