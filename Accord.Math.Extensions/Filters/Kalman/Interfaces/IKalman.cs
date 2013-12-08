using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Statistics.Filters
{
    public interface IKalmanDataInfo
    {
        /// <summary>
        /// Gets state dimensionionality.
        /// </summary>
        int StateVectorDimension { get; }

        /// <summary>
        /// Gets measurement dimensionionality.
        /// </summary>
        int MeasurementVectorDimension { get; }

        /// <summary>
        /// Gets control vector dimensionionality.
        /// </summary>
        int ControlVectorDimension { get; }
    }

    public interface IKalmanUserDefinedData
    {
        /// <summary>
        /// Gets or sets state transition matrix (A). [n x n] matrix.
        /// </summary>
        double[,] TransitionMatrix { get; set; }

        /// <summary>
        /// Gets or sets control matrix (B). [n x k] vector.
        /// It is not used if there is no control.
        /// </summary>
        double[,] ControlMatrix { get; set; }

        /// <summary>
        /// Gets or sets measurement matrix (H). [p x n] matrix, where p is a dimension of measurement vector.
        /// Selects components from a state vector that are obtained by measurement.
        /// </summary>
        double[,] MeasurementMatrix { get; set; }

        /// <summary>
        /// Gets or sets process noise covariance matrix (Q). [n x n] matrix.
        /// </summary>
        double[,] ProcessNoiseCovariance { get; set; }

        /// <summary>
        /// Gets or sets measurement noise covariance matrix (R). [p x p] matrix.
        /// </summary>
        double[,] MeasurementNoiseCovariance { get; set; }
    }

    public interface IKalman : IKalmanUserDefinedData, IKalmanDataInfo
    {
        /// <summary>
        /// Gets predicted state (x'(k)). [n x 1] vector.
        /// </summary>
        double[,] PredictedState { get; }

        /// <summary>
        /// Gets corrected state (x(k)). [n x 1] vector.
        /// After obtaining a measurement z(k) <see cref="PredictedState"/> will be corrected.
        /// This value is used an ultimate result.
        /// </summary>
        double[,] CorrectedState { get; }

        /// <summary>
        /// Gets priori error estimate covariance matrix (P'(k)). [n x n] matrix.
        /// </summary>
        double[,] PrioriErrorCovariance { get; }

        /// <summary>
        /// Gets Kalman gain matrix (K). [n x p] matrix.
        /// </summary>
        double[,] KalmanGain { get; }

        /// <summary>
        /// Gets posteriori error estimate covariance matrix (P(k)). [n x n] matrix.
        /// </summary>
        double[,] PosterioriErrorCovariance { get; }


        /// <summary>
        /// Estimates the subsequent model state <see cref="PredictedState"/>.
        /// </summary>
        void Predict();

        /// <summary>
        /// Estimates the subsequent model state <see cref="PredictedState"/>.
        /// </summary>
        /// <param name="controlVector">Control vector u(k). If there is no external control please use another function override.</param>
        void Predict(double[,] controlVector);

        /// <summary>
        /// The function adjusts the stochastic model state on the basis of the given measurement of the model state <see cref="CorrectedState"/>.
        /// </summary>
        /// <param name="measurement">Obtained measurement vector.</param>
        void Correct(double[,] measurement);
    }
}
