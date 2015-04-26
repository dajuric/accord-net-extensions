#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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
using PointF = AForge.Point;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Joint probability data association filter (JPDAF).
    /// See: <a href="https://bib.irb.hr/datoteka/519441.MSMT_Tracking_ECMR2011.pdf"/> for details.
    /// </summary>
    public static partial class JPDAF
    {
        #region Update

        private static double[,][] calculateZXMatrix<TParticle, TMeasurement>(this IList<IList<TParticle>> particleFilters, 
                                                                              IList<TMeasurement> measurements, 
                                                                              Func<TParticle, TMeasurement, double> likelihoodFunc,
                                                                              out double[,] probsZX,
                                                                              Func<int, int, bool> considerAssociation)
            where TParticle: class, IParticle
        {
            double[,][] probsZXn = new double[measurements.Count, particleFilters.Count][];

            for (int mIdx = 0; mIdx < measurements.Count; mIdx++)
            {
                var measurement = measurements[mIdx];

                for (int pfIdx = 0; pfIdx < particleFilters.Count; pfIdx++)
                {
                    if (considerAssociation(mIdx, pfIdx) == false)
                        continue;

                    var particleFilter = particleFilters[pfIdx];
                    double[] likelihoods = new double[particleFilter.Count];

                    for (int pIdx = 0; pIdx < particleFilter.Count; pIdx++)
                    {
                        var particle = particleFilter[pIdx];
                        var likelihood = likelihoodFunc(particle, measurement);

                        likelihoods[pIdx] = likelihood;
                    }

                    probsZXn[mIdx, pfIdx] = likelihoods;
                }
            }

            probsZX = new double[measurements.Count, particleFilters.Count];

            for (int mIdx = 0; mIdx < measurements.Count; mIdx++)
            {
                for (int pfIdx = 0; pfIdx < particleFilters.Count; pfIdx++)
                {
                    var mean = probsZXn[mIdx, pfIdx].Average();
                    probsZX[mIdx, pfIdx] = mean;
                }
            }

            return probsZXn;
        }

        /// <summary>
        /// Updates particle filters according to the calculated measurement-track association probability.
        /// </summary>
        /// <typeparam name="TFilter">Filter type.</typeparam>
        /// <typeparam name="TParticle">Particle type.</typeparam>
        /// <typeparam name="TMeasurement">Measurement type.</typeparam>
        /// <param name="particleFilters">Particle filters.</param>
        /// <param name="measurements">Measurements at current time.</param>
        /// <param name="likelihoodFunc">
        /// Likelihood function to calculate measurement-particle association likelihood (does not have to be normalized). 
        /// Parameters: particle, measurement.
        /// Returns: measurement-particle association likelihood.
        /// </param>
        /// <param name="resample">
        /// Filter resample function.
        /// Parameters: filter (particle collection), particle weights.
        /// Returns: re-sampled particles.
        /// </param>
        /// <param name="detectionProbability">Detection probability.</param>
        /// <param name="falseAlarmProbability">False alarm probability.</param>
        /// <param name="considerAssociation">
        /// Consider association function. If null all associations will be considered (can decrease performance if the number of tracked objects is large).
        /// Parameters: measurement, filter.
        /// Returns: true if the association needs to be considered, false otherwise.
        /// </param>
        /// <returns>Measurement-track probability association matrix.</returns>
        public static double[,] Update<TFilter, TParticle, TMeasurement>(this IList<TFilter> particleFilters,
                                                                         List<TMeasurement> measurements,
                                                                         Func<TParticle, TMeasurement, double> likelihoodFunc,
                                                                         Func<TFilter, TFilter> resample,
                                                                         double detectionProbability = 0.9, double falseAlarmProbability = 0.01,
                                                                         Func<TMeasurement, TFilter, bool> considerAssociation = null)
            where TParticle: class, IParticle
            where TFilter : List<TParticle>
        {
            considerAssociation = considerAssociation ?? ((m, t) => true);

            //measure
            double[,] probsZX;
            var probsZXn = calculateZXMatrix(particleFilters.Cast<IList<TParticle>>().ToList(), 
                                             measurements,
                                             likelihoodFunc,
                                             out probsZX, 
                                             (mIdx, tIdx) => 
                                             {
                                                 var t = particleFilters[tIdx];
                                                 var m = measurements[mIdx];

                                                 return considerAssociation(m, t);
                                             });
    
            //update weights
            var assocProbs = calculateAssciationProbabilities(probsZX, detectionProbability, falseAlarmProbability);

            for (int trackIdx = 0; trackIdx < particleFilters.Count; trackIdx++)
            {
                var particleFilter = particleFilters[trackIdx];

                for (int mIdx = 0; mIdx < measurements.Count; mIdx++)
                {
                     var likelihoods = probsZXn[mIdx, trackIdx];
                     var assocProb = assocProbs[mIdx, trackIdx];

                     for (int pIdx = 0; pIdx < particleFilter.Count; pIdx++)
                     {
                          var particle = particleFilter[pIdx];
                          particle.Weight += assocProb * likelihoods[pIdx];
                     }
                }
            }
            
            //resample
            if(measurements.Count > 0)
            {
                for (int i = 0; i < particleFilters.Count; i++)
                {
                    var resampledParticles = resample(particleFilters[i]);

                    //preserve list reference (e.g. useful if list is a key in a dictionary)
                    particleFilters[i].Clear();
                    particleFilters[i].AddRange(resampledParticles);
                }
            }

            return assocProbs;
        }

        #endregion

        #region Add

        /// <summary>
        /// Executes a user-specified function if new filters should be added. 
        /// New filters should be added if the marginal measurement probability is smaller than <paramref name="minMarginalMeasurementlProbability"/>.
        /// </summary>
        /// <typeparam name="TFilter">Filter type.</typeparam>
        /// <param name="particleFilters">Particle filters.</param>
        /// <param name="associationProbabilites">Measurement-track associations probabilities generated by Update function.</param>
        /// <param name="particleFilterAddFunc">
        /// Particle filter creation function.
        /// Parameters: measurement index.
        /// </param>
        /// <param name="minMarginalMeasurementlProbability">Minimal marginal measurement probability that needs to exist to consider that the measurement is associated with a track.</param>
        /// <returns>True if the filter is added, false otherwise.</returns>
        public static bool AddFilters<TFilter>(this List<TFilter> particleFilters,
                                               double[,] associationProbabilites,
                                               Action<int> particleFilterAddFunc,
                                               double minMarginalMeasurementlProbability = 0.1)
            where TFilter : IEnumerable<IParticle>
        {
            return addFilters
                (
                  particleFilters,
                  associationProbabilites,
                  particleFilterAddFunc,
                  minMarginalMeasurementlProbability
                );
        }

        /// <summary>
        /// Add new filters if the marginal measurement probability is smaller than <paramref name="minMarginalMeasurementlProbability"/>.
        /// </summary>
        /// <typeparam name="TFilter">Filter type.</typeparam>
        /// <param name="particleFilters">Particle filters.</param>
        /// <param name="associationProbabilites">Measurement-track associations probabilities generated by Update function.</param>
        /// <param name="particleFilterCreatorFunc">
        /// Particle filter creation function.
        /// Parameters: measurement index.
        /// Returns: particle filter.
        /// </param>
        /// <param name="minMarginalMeasurementlProbability">Minimal marginal measurement probability that needs to exist to consider that the measurement is associated with a track.</param>
        /// <returns>True if the filter is added, false otherwise.</returns>
        public static bool AddFilters<TFilter>(this List<TFilter> particleFilters, 
                                               double[,] associationProbabilites, 
                                               Func<int, TFilter> particleFilterCreatorFunc,
                                               double minMarginalMeasurementlProbability = 0.1)
            where TFilter : IEnumerable<IParticle>
        {
            return addFilters
                (
                  particleFilters, 
                  associationProbabilites, 
                  particleFilterCreatorFunc, 
                  minMarginalMeasurementlProbability
                );
        }

        #endregion
    }
}
