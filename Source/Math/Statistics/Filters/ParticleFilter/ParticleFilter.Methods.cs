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
using AForge;
using Accord.Statistics.Distributions;

namespace Accord.Extensions.Statistics.Filters
{
    public static partial class ParticleFilter
    {
        /// <summary>
        /// Adds newly created particles to the provided list. Particles are created by generating random arguments using provided distributions and creator function.
        /// </summary>
        /// <typeparam name="TParticle">Particle type.</typeparam>
        /// <param name="collection">The list to fill with new particles.</param>
        /// <param name="numberOfParticles">Number of particles to create.</param>
        /// <param name="creator">Function that creates a single particle from floating point array. The array length must be equal to the number of distributions.</param>
        /// <param name="distributions">Distributions which serve as random parameter generators.</param>
        public static void CreateParticles<TParticle>(this List<TParticle> collection, int numberOfParticles, Func<double[], TParticle> creator, IList<ISampleableDistribution<double>> distributions)
            where TParticle: IParticle
        {
            if (collection == null)
                throw new ArgumentNullException("The provided collection must not be null.");

            var nDim = distributions.Count;

            /**************** make particles *****************/
            for (int i = 0; i < numberOfParticles; i++)
            {
                var randomParam = new double[nDim];
                for (int dimIdx = 0; dimIdx < nDim; dimIdx++)
                {
                    randomParam[dimIdx] = distributions[dimIdx].Generate();
                }

                var p = creator(randomParam);
                p.Weight = 1d / numberOfParticles;

                collection.Add(p);
            }
        }

        /// <summary>
        /// Draws particles randomly where particles which have higher weight have greater probability to be drawn. A single particle can be chosen more than once.
        /// </summary>
        /// <typeparam name="TParticle">Particle's type.</typeparam>
        /// <param name="particles">Particles' pool.</param>
        /// <param name="sampleCount">The number of samples to draw.</param>
        /// <returns>Chosen particles (the particles are not cloned).</returns>
        public static IList<TParticle> Draw<TParticle>(this IList<TParticle> particles, int sampleCount)
            where TParticle: class, IParticle
        {
            /*************** calculate cumulative weights ****************/
            double[] cumulativeWeights = new double[particles.Count];

            int cumSumIdx = 0;
            double cumSum = 0;
            foreach (var p in particles)
            {
                cumSum += p.Weight;
                cumulativeWeights[cumSumIdx++] = cumSum;
            }
            /*************** calculate cumulative weights ****************/

            /*************** re-sample particles ****************/
            var maxCumWeight = cumulativeWeights[particles.Count - 1];
            var minCumWeight = cumulativeWeights[0];
                
            var drawnParticles = new List<TParticle>();
            double initialWeight = 1d / particles.Count;

            Random rand = new Random();

            for (int i = 0; i < sampleCount; i++)
            {
                var randWeight = minCumWeight + rand.NextDouble() * (maxCumWeight - minCumWeight);

                int particleIdx = 0;
                while (cumulativeWeights[particleIdx] < randWeight) //find particle's index
                {
                    particleIdx++;
                }

                var p = particles[particleIdx];
                drawnParticles.Add(p);
            }
            /*************** re-sample particles ****************/

            return drawnParticles;
        }


        /// <summary>
        /// Gets normalized weights by dividing each particle weight by a sum of all weights.
        /// </summary>
        /// <param name="particles">Particles.</param>
        /// <returns>Normalized weights.</returns>
        public static IList<double> GetNormalizedWeights(this IEnumerable<IParticle> particles)
        {
            List<double> normalizedWeights = new List<double>();

            var weightSum = particles.Sum(x => x.Weight) + Single.Epsilon;

            foreach (var p in particles)
            {
                var normalizedWeight = p.Weight / weightSum;
                normalizedWeights.Add(normalizedWeight);
            }

            return normalizedWeights;
        }

        /// <summary>
        /// Calculates Renyi's quadratic entropy of a particle set (also used for JPDAF).
        /// </summary>
        /// <param name="particles">particle filter.</param>
        /// <param name="stateSelector">Particle state selector function (e.g. [x, y, vX, vY]).</param>
        /// <returns>Renyi's quadratic entropy of a particle set.</returns>
        public static double CalculateEntropy<TParticle>(this IEnumerable<TParticle> particles, Func<TParticle, double[]> stateSelector)
            where TParticle: class, IParticle
        { 
            var nParticles = particles.Count();

            /***************** kernel function calculation ******************/
            var dKrnl = 4;
            var eKrnl = 1 / (4 + dKrnl);
            var hKrnl = System.Math.Pow(4 / (dKrnl + 2), eKrnl) * System.Math.Pow(nParticles, -eKrnl); //scaling param
            
            var states = particles.Select(x=> stateSelector(x)).ToList();
            var kernel = states.ToMatrix().Covariance().Multiply(System.Math.Pow(hKrnl, dKrnl)); //particle filter kernel
            /***************** kernel function calculation ******************/

            var mean = new double[kernel.ColumnCount()];
            var normalDistribution = new MultivariateNormalDistribution(mean, kernel.Multiply(2));

            var entropy = 0d;
            for (int i = 0; i < nParticles; i++)
            {
                for (int j = 0; j < nParticles; j++)
                {
                    var stateDiff = states[i].Subtract(states[j]);
                    var kernelVal = normalDistribution.ProbabilityDensityFunction(stateDiff);

                    entropy += kernelVal;
                }
            }

            entropy = -System.Math.Log10(entropy / (nParticles * nParticles));
            return entropy;
        }

    }
}
