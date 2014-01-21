using Accord.Statistics.Distributions.Univariate;
using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Statistics.Filters
{
    public static partial class ParticleFilter
    {
        /// <summary>
        /// Particle states are initialized randomly according to provied ranges <see cref="ranges"/>
        /// </summary>
        /// <param name="numberOfParticles">Number of particles to create.</param>
        /// <param name="model">Process model.</param>
        /// <param name="ranges">Bound for each process state dimension.</param>
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
        /// </summary>
        public static IEnumerable<TParticle> SimpleResampler<TParticle>(IList<TParticle> particles, IList<double> normalizedWeights)
              where TParticle : class, IParticle
        {
            return SimpleResampler(particles, normalizedWeights, particles.Count);
        }

        /// <summary>
        /// Draws particles according to particle's weight.
        /// </summary>
        public static IEnumerable<TParticle> SimpleResampler<TParticle>(IList<TParticle> particles, IList<double> normalizedWeights, int nSamples)
              where TParticle : class, IParticle
        {
            /*************** calculate cumulative weights ****************/
            double[] cumulativeWeights = new double[particles.Count];
            cumulativeWeights[0] = normalizedWeights.First();

            for (int i = 1; i < particles.Count; i++)
            {
                cumulativeWeights[i] = cumulativeWeights[i - 1] + normalizedWeights[i];
            }
            /*************** calculate cumulative weights ****************/


            /*************** resample particles ****************/
            var resampledParticles = new List<TParticle>();
            double initialWeight = 1d / particles.Count;

            Random rand = new Random();

            for (int i = 0; i < nSamples; i++)
            {
                var randWeight = cumulativeWeights.First() + rand.NextDouble() * (cumulativeWeights.Last() - cumulativeWeights.First());

                int particleIdx = 0;
                while (cumulativeWeights[particleIdx] < randWeight) //find particle's index
                {
                    particleIdx++;
                }

                var newParticle = (TParticle)particles[particleIdx].Clone();
                //newParticle.Weight = initialWeight;

                resampledParticles.Add(newParticle);
            }
            /*************** resample particles ****************/

            return resampledParticles;
        }

        public static IList<double> SimpleNormalizer(IEnumerable<IParticle> particles)
        {
            List<double> normalizedWeights = new List<double>();

            /*double maxLogProb = this.Particles.Max(x => x.Weight);
           
                 this.Particles.ForEach(p => 
                 {
                     var expProb = Math.Exp(p.Weight - maxLogProb);
                     p.Weight = expProb; 
                 });*/

            var weightSum = particles.Sum(x => x.Weight);

            foreach (var p in particles)
            {
                var normalizedWeight = p.Weight / weightSum;
                normalizedWeights.Add(normalizedWeight);
            }

            return normalizedWeights;
        }
    }
}
