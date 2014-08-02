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

namespace Accord.Extensions.Statistics.Filters
{
    public static partial class ParticleFilter
    {
        /// <summary>
        /// Particle states are initialized randomly according to provided ranges <paramref name="ranges"/>.
        /// </summary>
        /// <param name="numberOfParticles">Number of particles to create.</param>
        /// <param name="ranges">Bound for each process state dimension.</param>
        /// <param name="creator">Function that creates a single particle from floating point array.</param>
        public static IEnumerable<TParticle> UnifromParticleSpreadInitializer<TParticle>(int numberOfParticles, DoubleRange[] ranges, Func<double[], TParticle> creator)
              where TParticle : class, IParticle
        {
            List<double[]> randomRanges = new List<double[]>(numberOfParticles);
            for (int pIdx = 0; pIdx < numberOfParticles; pIdx++)
            {
                randomRanges.Add(new double[ranges.Length]);
            }

            /*************** initialize states by random value ******************/
            int stateDimensionIdx = 0;
            foreach (var range in ranges)
            {
                var unifromDistribution = new UniformContinuousDistribution(range.Min, range.Max);

                for (int pIdx = 0; pIdx < numberOfParticles; pIdx++)
                {
                    randomRanges[pIdx][stateDimensionIdx] = unifromDistribution.Generate();
                }

                stateDimensionIdx++;
            }
            /*************** initialize states by random value ******************/


            var particles = new List<TParticle>(numberOfParticles);

            /**************** make particles *****************/
            double initialWeight = 1d / numberOfParticles;
            for (int i = 0; i < numberOfParticles; i++)
            {
                var p = creator(randomRanges[i]);
                p.Weight = initialWeight;

                particles.Add(p);
            }
            /**************** make particles *****************/

            return particles;
        }

        /// <summary>
        /// Draws particles according to particle's weight.
        /// <param name="particles">Particles from which to draw samples. If they are already sorted.</param>
        ///<param name="normalizedWeights">Normalized weights of the particles.</param>
        ///<param name="sortParticles">True to sort particles, false to leave the current order. Set to true if the particles are not sorted.</param>
        /// </summary>
        public static IEnumerable<TParticle> SimpleResampler<TParticle>(IList<TParticle> particles, IList<double> normalizedWeights, bool sortParticles = true)
              where TParticle : class, IParticle
        {
            Int32[] sortedIndices = Enumerable.Range(0, particles.Count).ToArray();
            if (sortParticles)
            {
                Array.Sort(sortedIndices, (a, b) => particles[a].Weight.CompareTo(particles[b].Weight));     
            }

            /*************** calculate cumulative weights ****************/
            double[] cumulativeWeights = new double[particles.Count];

            int cumSumIdx = 0;
            double cumSum = 0;
            foreach (var idx in sortedIndices)
            {
                cumSum +=normalizedWeights[idx];
                cumulativeWeights[cumSumIdx++] = cumSum;
            }
            /*************** calculate cumulative weights ****************/

            /*************** re-sample particles ****************/
            var resampledParticles = new List<TParticle>();
            double initialWeight = 1d / particles.Count;

            Random rand = new Random();

            for (int i = 0; i < particles.Count; i++)
            {
                var randWeight = cumulativeWeights[0] + rand.NextDouble() * (cumulativeWeights[particles.Count - 1] - cumulativeWeights[0]);
               
                int particleIdx = 0;
                while (cumulativeWeights[particleIdx] < randWeight) //find particle's index
                {
                    particleIdx++;
                }

                var idx = sortedIndices[particleIdx];
                var p = particles[idx];

                var newParticle = (TParticle)p.Clone();
                newParticle.Weight = initialWeight;

                resampledParticles.Add(newParticle);
            }
            /*************** re-sample particles ****************/

            return resampledParticles;
        }

        /// <summary>
        /// Normalizer function which divides each particle weight by the sum of all weights.
        /// </summary>
        /// <param name="particles">Particles.</param>
        /// <returns>Normalized weights.</returns>
        public static IList<double> SimpleNormalizer(IEnumerable<IParticle> particles)
        {
            List<double> normalizedWeights = new List<double>();

            /*double maxLogProb = this.Particles.Max(x => x.Weight);
           
                 this.Particles.ForEach(p => 
                 {
                     var expProb = Math.Exp(p.Weight - maxLogProb);
                     p.Weight = expProb; 
                 });*/

            var weightSum = particles.Sum(x => x.Weight) + Single.Epsilon;

            foreach (var p in particles)
            {
                var normalizedWeight = p.Weight / weightSum;
                normalizedWeights.Add(normalizedWeight);
            }

            return normalizedWeights;
        }

        /// <summary>
        /// Calculates Renyi's quadratic entropy of a particle set.
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
