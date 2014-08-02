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
using System.Collections.Generic;
using System.Linq;
using Accord.Extensions.Math;
using Accord.Math;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Joint probability data association filter (JPDAF).
    /// See: <a href="https://bib.irb.hr/datoteka/519441.MSMT_Tracking_ECMR2011.pdf"/> for details.
    /// </summary>
    public static partial class JPDAF
    {
        #region Update

        /// <summary>
        /// upper limit which includes valid measurements (gating) with 99% probability
        /// </summary>
        private static double gateThreshold = new ChiSquareDistribution(2).InverseDistributionFunction(0.99);

        private static double[,][] calculateZXMatrix<TFilter, TState, TMeasurement>(this IList<TFilter> kalmanFilters,
                                                                                    IList<TMeasurement> measurements,
                                                                                    out double[,] probsZX)
            where TFilter : KalmanFilter<TState, TMeasurement>
        {
            probsZX = new double[measurements.Count, kalmanFilters.Count];
            var innovZX = new double[measurements.Count, kalmanFilters.Count][];

            for (int tIdx = 0; tIdx < kalmanFilters.Count; tIdx++)
            {
                var kalman = kalmanFilters[tIdx];
                var zeroCoordinate = new double[kalman.MeasurementVectorDimension];

                var mvnPDF = new MultivariateNormalDistribution(zeroCoordinate, kalman.CovarianceMatrix);
                var mulCorrectionFactor = (double)1 / mvnPDF.ProbabilityDensityFunction(zeroCoordinate);

                for (int mIdx = 0; mIdx < measurements.Count; mIdx++)
                {
                    var measurement = measurements[mIdx];

                    var delta = kalman.CalculatePredictionError(measurement);
                    innovZX[mIdx, tIdx] = delta;

                    //delta' / S^-1 * delta; this expression has ChiSquare distribution
                    var gate = delta.Multiply(kalman.CovarianceMatrixInv).Multiply(delta.Transpose()).Sum();
                    
                    if (gate < gateThreshold)
                    {
                        probsZX[mIdx, tIdx] = mvnPDF.ProbabilityDensityFunction(delta) * mulCorrectionFactor; //modification (added mul correction factor)
                    }
                    //else probsZX[mIdx, tIdx] = 0 (by default)
                }
            }

            return innovZX;
        }

        /// <summary>
        /// Updates Kalman filters according to the calculated measurement-track association probability.
        /// </summary>
        /// <typeparam name="TFilter">Filter type.</typeparam>
        /// <typeparam name="TState">Kalman filter state type.</typeparam>
        /// <typeparam name="TMeasurement">Kalman filter measurement type.</typeparam>
        /// <param name="kalmanFilters">Kalman filters.</param>
        /// <param name="measurements">Measurements at current time.</param>
        /// <param name="detectionProbability">Detection probability.</param>
        /// <param name="falseAlarmProbability">False alarm probability.</param>
        /// <returns>Measurement-track probability association matrix.</returns>
        public static double[,] Update<TFilter, TState, TMeasurement>(this IList<TFilter> kalmanFilters,
                                                                      List<TMeasurement> measurements,
                                                                      double detectionProbability = 0.9, double falseAlarmProbability = 0.01)
            where TFilter : DiscreteKalmanFilter<TState, TMeasurement>
        {
            if (!kalmanFilters.Any() || !measurements.Any())
                return new double[measurements.Count, kalmanFilters.Count];

            //measure
            double[,] probsZX;
            var innovZX = calculateZXMatrix<TFilter, TState, TMeasurement>(kalmanFilters, measurements, out probsZX);

            //update weights
            var assocProbs = calculateAssciationProbabilities(probsZX, detectionProbability, falseAlarmProbability);

            for (int tIdx = 0; tIdx < kalmanFilters.Count; tIdx++)
            {
                var kalman = kalmanFilters[tIdx];

                var weightedInnovation = calculateWeightedInnovation(assocProbs, innovZX, tIdx);
                var innovationCov = calculateInnovationCovariance(assocProbs, innovZX, weightedInnovation, tIdx);
                var beta = 1 - assocProbs.GetColumn(tIdx).Sum(); 
              
                kalman.Correct(weightedInnovation, beta, innovationCov);
            }

            return assocProbs;
        }

        private static double[] calculateWeightedInnovation(double[,] assocProbs, double[,][] innovZX, int trackIdx)
        {
            var nMeasurements = assocProbs.RowCount();
            var measurementDimension = innovZX[0, 0].Length;

            var innovVector = new double[measurementDimension];

            for (int mIdx = 0; mIdx < nMeasurements; mIdx++)
            {
                var assocProb = assocProbs[mIdx, trackIdx];
                var innov = innovZX[mIdx, trackIdx];

                for (int mDim = 0; mDim < measurementDimension; mDim++)
                {
                    innovVector[mDim] += innov[mDim] * assocProb;
                }
            }

            return innovVector;
        }

        private static double[,] calculateInnovationCovariance(double[,] assocProbs, double[,][] innovZX, double[] weightedInnovation, int trackIdx)
        {
            var nMeasurements = assocProbs.RowCount();
            var measurementDimension = innovZX[0, 0].Length;

            var innovationCovariance = new double[measurementDimension, measurementDimension];

            for (int mIdx = 0; mIdx < nMeasurements; mIdx++)
            {
                var assocProb = assocProbs[mIdx, trackIdx];
                var innov = innovZX[mIdx, trackIdx];
              
                var innovCorr = innov.Multiply(innov);
                innovationCovariance = innovationCovariance.Add(innovCorr.Multiply(assocProb));
            }

            var weightedInnovCorr = weightedInnovation.Multiply(weightedInnovation);
            innovationCovariance = innovationCovariance.Subtract(weightedInnovCorr);
            return innovationCovariance;
        }

        #endregion

        #region Add

        /// <summary>
        /// Executes a user-specified function if new filters should be added. 
        /// New filters should be added if the marginal measurement probability is smaller than <paramref name="minMarginalMeasurementlProbability"/>.
        /// </summary>
        /// <typeparam name="TFilter">Filter type.</typeparam>
        /// <typeparam name="TState">Kalman filter state type.</typeparam>
        /// <typeparam name="TMeasurement">Kalman filter measurement type.</typeparam>
        /// <param name="kalmanFilters">Kalman filters.</param>
        /// <param name="associationProbabilites">Association probability matrix of an measurement-track association probability. Size: [nMeasurements x nTracks].</param>
        /// <param name="kalmanFilterAddFunc">
        /// Kalman filter creation function.
        /// Parameters: measurement index.
        /// </param>
        /// <param name="minMarginalMeasurementlProbability">Minimal marginal measurement probability that needs to exist to consider that the measurement is associated with a track.</param>
        /// <returns>True if the filter is added, false otherwise.</returns>
        public static bool AddFilters<TFilter, TState, TMeasurement>(this List<TFilter> kalmanFilters,
                                                                     double[,] associationProbabilites,
                                                                     Action<int> kalmanFilterAddFunc,
                                                                     double minMarginalMeasurementlProbability = 0.1)
            where TFilter : KalmanFilter<TState, TMeasurement>
        {
            return addFilters
                (
                  kalmanFilters,
                  associationProbabilites,
                  kalmanFilterAddFunc,
                  minMarginalMeasurementlProbability
                );
        }

        /// <summary>
        /// Add new filters if the marginal measurement probability is smaller than <paramref name="minMarginalMeasurementlProbability"/>.
        /// </summary>
        /// <typeparam name="TFilter">Filter type.</typeparam>
        /// <typeparam name="TState">Kalman filter state type.</typeparam>
        /// <typeparam name="TMeasurement">Kalman filter measurement type.</typeparam>
        /// <param name="kalmanFilters">Kalman filters.</param>
        /// <param name="associationProbabilites"></param>
        /// <param name="kalmanFilterCreatorFunc">
        /// Particle filter creation function.
        /// Parameters: measurement index.
        /// Returns: particle filter.
        /// </param>
        /// <param name="minMarginalMeasurementlProbability">Minimal marginal measurement probability that needs to exist to consider that the measurement is associated with a track.</param>
        /// <returns>True if the filter is added, false otherwise.</returns>
        public static bool AddFilters<TFilter, TState, TMeasurement>(this List<TFilter> kalmanFilters,
                                                                     double[,] associationProbabilites,
                                                                     Func<int, TFilter> kalmanFilterCreatorFunc,
                                                                     double minMarginalMeasurementlProbability = 0.1)
            where TFilter : KalmanFilter<TState, TMeasurement>
        {
            return addFilters
                  (
                    kalmanFilters,
                    associationProbabilites,
                    kalmanFilterCreatorFunc,
                    minMarginalMeasurementlProbability
                  );
        }

        #endregion

        #region Remove

        /// <summary>
        /// Removes filters if an filter entropy is too big. 
        /// </summary>
        /// <typeparam name="TFilter">Filter type.</typeparam>
        /// <typeparam name="TState">Kalman filter state type.</typeparam>
        /// <typeparam name="TMeasurement">Kalman filter measurement type.</typeparam>
        /// <param name="kalmanFilters">Kalman filters.</param>
        /// <param name="removeSelector">
        /// Remove function selector.
        /// Parameters: Kalman filter index to be removed, filter entropy.
        /// Returns: true if the filter needs to be removed, false otherwise.
        /// </param>
        /// <returns>True if a filter or filters are removed, false otherwise.</returns>
        public static bool RemoveFilters<TFilter, TState, TMeasurement>(this List<TFilter> kalmanFilters,
                                                                        Func<int, double, bool> removeSelector)
           where TFilter : KalmanFilter<TState, TMeasurement>
        {
            var entropies = new List<double>();

            foreach (var kf in kalmanFilters)
            {
                var entropy = kf.CalculateEntropy();
                entropies.Add(entropy);
            }

            return removeFilters(kalmanFilters, entropies, removeSelector);
        }

        /// <summary>
        /// Removes filters if an filter entropy is too big. 
        /// </summary>
        /// <typeparam name="TFilter">Filter type.</typeparam>
        /// <typeparam name="TState">Kalman filter state type.</typeparam>
        /// <typeparam name="TMeasurement">Kalman filter measurement type.</typeparam>
        /// <param name="kalmanFilters">Kalman filters.</param>
        /// <param name="filtersEntropies">List of filter entropies. 
        /// <para>The number of entropies must match the number of filters.</para>
        /// </param>
        /// <param name="removeSelector">
        /// Remove function selector.
        /// Parameters: Kalman filter index to be removed, filter entropy.
        /// Returns: true if the filter needs to be removed, false otherwise.
        /// </param>
        /// <returns>True if a filter or filters are removed, false otherwise.</returns>
        public static bool RemoveFilters<TFilter, TState, TMeasurement>(this List<TFilter> kalmanFilters,
                                                                        IList<double> filtersEntropies,
                                                                        Func<int, double, bool> removeSelector)
          where TFilter : KalmanFilter<TState, TMeasurement>
        {
            return removeFilters(kalmanFilters, filtersEntropies, removeSelector);
        }

        #endregion

        /// <summary>
        /// Selects Kalman filter indices by using user-specified selector function.
        /// </summary>
        /// <typeparam name="TFilter">Filter type.</typeparam>
        /// <typeparam name="TState">Kalman filter state type.</typeparam>
        /// <typeparam name="TMeasurement">Kalman filter measurement type.</typeparam>
        /// <param name="kalmanFilters">Kalman filters.</param>
        /// <param name="selector">
        /// Selector function.
        /// Parameters: Kalman filter index, particle filter.
        /// Returns: true if the filter needs to be selected.
        /// </param>
        /// <returns>Selected Kalman filter indicies in the collection.</returns>
        public static IEnumerable<int> Select<TFilter, TState, TMeasurement>(this IEnumerable<TFilter> kalmanFilters,
                                                                             Func<int, TFilter, bool> selector)
            where TFilter : KalmanFilter<TState, TMeasurement>
        {
            return select(kalmanFilters, selector);
        }

    }
}
