using System;
using System.Collections.Generic;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Extension methods for the Kalman filter.
    /// </summary>
    public static class KalmanFilterExtensions
    {
        /// <summary>
        /// Calculates Mahalanobis distance between each (tracker, measurement pair).
        /// </summary>
        /// <typeparam name="TTracker">Kalman filter tracker.</typeparam>
        /// <typeparam name="TState">Kalman filter state type..</typeparam>
        /// <typeparam name="TMeasurement">Measurement type.</typeparam>
        /// <param name="trackers">Kalman filter trackers.</param>
        /// <param name="measurements">Measurements.</param>
        /// <param name="gateDistances">
        /// If true the distance will be compared with gating threshold and if the distance exceeds the distance threshold <see cref="System.Double.PositiveInfinity"/> value will be written.
        /// <para>The gating is obtained as 99 percent probability interval of the Kalman residual covariance.</para>
        /// </param>
        /// <returns>Distance matrix [tracks x detections].</returns>
        public static double[,] CalculateDistanceMatrix<TTracker, TState, TMeasurement>(this IList<TTracker> trackers, IList<TMeasurement> measurements, bool gateDistances = false)
           where TTracker : KalmanFilter<TState, TMeasurement>
        {
            double[,] costMatrix = new double[trackers.Count, measurements.Count];

            for (int r = 0; r < trackers.Count; r++)
            {
                for (int c = 0; c < measurements.Count; c++)
                {
                    double[] delta; double distance;
                    bool isInsideGate = trackers[r].IsMeasurementInsideGate(measurements[c], out delta, out distance);

                    costMatrix[r, c] = distance;
                    if (gateDistances && !isInsideGate)
                        costMatrix[r, c] = Double.PositiveInfinity;
                }
            }

            return costMatrix;
        }
    }
}
