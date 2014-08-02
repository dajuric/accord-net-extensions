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
        /// Calculates measurement-track probability association matrix.
        /// <para>Generates association matrices and sums the probabilities for each case (flattens hypothesizes).</para>
        /// </summary>
        /// <param name="probsZX">
        /// Matrix that contains how much each measurement corresponds to each filter.
        /// <para>If an element of the matrix is zero, the corresponding association will not be considered.</para>
        /// </param>
        /// <param name="detectionProbability">Detection probability.</param>
        /// <param name="falseAlarmProbability">False alarm probability.</param>
        /// <returns>Measurement-track probability association matrix.</returns>
        private static double[,] calculateAssciationProbabilities(double[,] probsZX, double detectionProbability = 0.9, double falseAlarmProbability = 0.01)
        {
            var nMeasurements = probsZX.RowCount();
            var nTracks = probsZX.ColumnCount();

            var assocProbMat = new double[nMeasurements, nTracks];
            var usedElem = new bool[nMeasurements, nTracks];
            double assocSum = 0;
            int nCombinations = 0;

            for (int t = 0; t <= nTracks; t++)
            {
                generatePossibleAssociations(t, usedElem, (_) =>
                {
                    updateProbabilityAssociationMatrix(probsZX, detectionProbability, falseAlarmProbability, usedElem,
                                                       assocProbMat, ref assocSum, ref nCombinations);
                },
                (mIdx, tIdx) => probsZX[mIdx, tIdx] > 0);
            }

            if (System.Math.Abs(assocSum) > 2 * Double.Epsilon)
                assocProbMat.Divide(assocSum, inPlace: true);

            return assocProbMat;
        }

        private static void generatePossibleAssociations(int trackIdx, bool[,] usedElem, Action<bool[,]> executionFunc, Func<int, int, bool> considerAssociation)
        {
            var nMeasurements = usedElem.RowCount();
            var nTracks = usedElem.ColumnCount();

            if (nMeasurements == 0 || nTracks == 0)
                return;

            if (trackIdx == nTracks)
            {
                executionFunc(usedElem);
                return;
            }

            for (int mIdx = 0; mIdx < nMeasurements; mIdx++)
            {
                var isMeasurementAssociated = usedElem.GetRow(mIdx).Any(x => x == true);

                if (isMeasurementAssociated == false || considerAssociation(mIdx, trackIdx))
                {
                    usedElem[mIdx, trackIdx] = true;

                    for (int i = trackIdx; i < nTracks; i++)
                    {
                        generatePossibleAssociations(i + 1, usedElem, executionFunc, considerAssociation);
                    }

                    usedElem[mIdx, trackIdx] = false;
                }
            }
        }

        private static void updateProbabilityAssociationMatrix(double[,] probsZX, double detectionProbability, double falseAlarmProbability, bool[,] usedElem,
                                                               double[,] assocProbMat, ref double assocSum, ref int numCalls)
        {
            var nMeasurements = usedElem.RowCount();
            var nTracks = usedElem.ColumnCount();
            var selectedAssociations = usedElem.Find(x => x == true);

            var selectedProbabilities = probsZX.GetAt(selectedAssociations).ToList();
            var nDetections = selectedProbabilities.Count;

            var probability = selectedProbabilities.DefaultIfEmpty(1d).Aggregate((a, b) => a * b);
            probability *= System.Math.Pow(falseAlarmProbability, nMeasurements - nDetections);
            probability *= System.Math.Pow(detectionProbability, nDetections) * System.Math.Pow(1 - detectionProbability, nTracks - nDetections);

            assocSum += probability;
            assocProbMat.SetAt(selectedAssociations, (oldVal) => oldVal + probability);

            numCalls++;
        }

        #endregion

        #region Add

        private static bool addFilters<TFilter>(List<TFilter> filters,
                                               double[,] associationProbabilites,
                                               Func<int, TFilter> filterCreatorFunc,
                                               double minMarginalMeasurementlProbability = 0.1)
        {
            return addFilters
                (
                  filters, 
                  associationProbabilites, 
                  (measurementIdx) => 
                  {
                      var filter = filterCreatorFunc(measurementIdx);
                      filters.Add(filter);
                  }, 
                  minMarginalMeasurementlProbability
                );
        }

        private static bool addFilters<TFilter>(List<TFilter> filters,
                                                double[,] associationProbabilites,
                                                Action<int> filterCreatorFunc,
                                                double minMarginalMeasurementlProbability = 0.1)
        {
            bool isAdded = false;

            //check measurement association probability
            var measurementsAssocs = associationProbabilites.Sum(dimension: 1);

            //add new filters if necessary
            int nMeasurements = associationProbabilites.RowCount();

            for (int mIdx = 0; mIdx < nMeasurements; mIdx++)
            {
                var measurementAssoc = measurementsAssocs[mIdx];

                if (measurementAssoc < minMarginalMeasurementlProbability)
                {
                    filterCreatorFunc(mIdx);
                    isAdded = true;
                }
            }

            return isAdded;
        }

        #endregion

        #region Remove

        private static bool removeFilters<TFilter>(List<TFilter> filters,
                                                   IList<double> filtersEntropies,
                                                   Func<int, double, bool> removeSelector)
            where TFilter:class
        {
            if (filtersEntropies.Count != filters.Count)
                throw new ArgumentException("Number of entropies must be the same as the number of filters!");
            
            var toRemove = select
                            (
                              filters, 
                              (filterIdx, _) => 
                              {
                                  var entropy = filtersEntropies[filterIdx];
                                  return removeSelector(filterIdx, entropy);
                              }
                            );

            filters.RemoveAt(toRemove);

            return toRemove.Any();
        }

        #endregion

        private static IEnumerable<int> select<TFilter>(IEnumerable<TFilter> filters,
                                                Func<int, TFilter, bool> selector)
        {
            var filterIter = filters.GetEnumerator();

            int idx = 0;
            while (filterIter.MoveNext())
            {
                var isSelected = selector(idx, filterIter.Current);
                if (isSelected)
                    yield return idx;
            }
        }
    }
}
