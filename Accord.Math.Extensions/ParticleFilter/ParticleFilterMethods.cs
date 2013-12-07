using Accord.Statistics.Distributions.Univariate;
using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Math.Extensions.ParticleFilter
{
    public static class FilterMethods<TParticle, TState>
        where TParticle : Particle<TState>, new()
        where TState : ICloneable
    {
        /// <summary>
        /// Particle states are initialized randomly according to provied ranges <see cref="ranges"/>
        /// </summary>
        /// <param name="numberOfParticles">Number of particles to create.</param>
        /// <param name="model">Process model.</param>
        /// <param name="ranges">Bound for each process state dimension.</param>
        public static ParticleFilter<TParticle, TState>.InitializerFunc UnifromParticleSpreadInitializer(DoubleRange[] ranges, Func<double[], TState> converter)
        {
            return (int numberOfParticles) =>
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
                    particles.Add(new TParticle
                    {
                        State = converter(randomRanges[i]),
                        Weight = initialWeight
                    });
                }
                /**************** make particles *****************/

                return particles;
            };
        }

        /// <summary>
        /// Draw particles according to particle's weight.
        /// </summary>
        public static ParticleFilter<TParticle, TState>.ResampleFunc SimpleResampler()
        {
            return (IEnumerable<TParticle> _particles) =>
            {
                var particles = _particles.ToList();

                /*************** calculate cumulative weights ****************/
                double[] cumulativeWeights = new double[particles.Count];
                cumulativeWeights[0] = particles[0].Weight;

                for (int i = 1; i < particles.Count; i++)
                {
                    cumulativeWeights[i] = cumulativeWeights[i - 1] + particles[i].Weight;
                }
                /*************** calculate cumulative weights ****************/

                /*************** resample particles ****************/
                var resampledParticles = new List<TParticle>();
                double initialWeight = 1d / particles.Count;

                Random rand = new Random();

                for (int i = 0; i < particles.Count; i++)
                {
                    var randWeight = rand.NextDouble();

                    int particleIdx = 0;
                    while (cumulativeWeights[particleIdx] < randWeight)
                    {
                        particleIdx++;
                    }

                    var newParticle = (TParticle)particles[particleIdx].Clone();
                    newParticle.Weight = initialWeight;

                    resampledParticles.Add(newParticle);
                }
                /*************** resample particles ****************/

                return resampledParticles;
            };
        }

        public static ParticleFilter<TParticle, TState>.NormalizationFunc SimpleNormalizer()
        {
            return (IEnumerable<IParticle> particles) =>
            {
                /*double maxLogProb = this.Particles.Max(x => x.Weight);
           
                 this.Particles.ForEach(p => 
                 {
                     var expProb = Math.Exp(p.Weight - maxLogProb);
                     p.Weight = expProb; 
                 });*/

                var weightSum = particles.Sum(x => x.Weight);
                foreach (var p in particles)
                {
                    p.Weight = p.Weight / weightSum;
                }
            };
        }
    }
}
