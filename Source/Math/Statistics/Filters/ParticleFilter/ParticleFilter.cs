using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Contains sttaic and extensions methods that operate on collection of particles.
    /// </summary>
    /// 
    /// <remarks>
    /// See lectures:  <a href="http://www.kev-smith.com/teaching/L7_ParticleFilters.pdf"/>
    /// </remarks>
    public static partial class ParticleFilter
    {      
        #region Predict Methods

        /// <summary>
        /// Predicts particle's state.
        /// </summary>
        /// <param name="particles">Particles.</param>
        /// <param name="drift">Update state from model (no noise).</param>
        /// <param name="diffuse">Apply noise to spread particles.</param>
        public static void Predict<TParticle>(this IEnumerable<TParticle> particles, Action<TParticle> drift, Action<TParticle> diffuse)
              where TParticle : class, IParticle
        {
            foreach (var p in particles)
            {
                drift(p);
                diffuse(p);
            }
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates particle's state.
        /// </summary>
        /// <param name="particles">Particles.</param>
        /// <param name="measure">Assign weight to each particle.</param>
        /// <param name="normalize">Normalization function.</param>
        /// <param name="resample">Resample particles (creates new swarm).</param>
        /// <param name="effectiveCountMinRatio">If calculated effective count ratio is lower than user specified value re-sampling will occur, otherwise not.
        /// <para>The range is [0..1]. If resampling must occur every time put 1.</para>
        /// </param>
        /// <param name="measureInParallel">True to measure each particle state in parallel, false to measure it sequentially.</param>
        public static IEnumerable<TParticle> Update<TParticle>(this IEnumerable<TParticle> particles, 
                                                Action<TParticle> measure, 
                                                Func<IEnumerable<IParticle>, IEnumerable<double>> normalize, 
                                                Func<IEnumerable<TParticle>, IEnumerable<double>, IEnumerable<TParticle>> resample,
                                                float effectiveCountMinRatio = 0.9f, bool measureInParallel = true)
              where TParticle : class, IParticle
        {
            Action<IEnumerable<TParticle>> measureFunc = (_particles) =>
            {
                if (measureInParallel)
                {
                    Parallel.ForEach(_particles, (p) =>
                    {
                        measure(p);
                    });
                }
                else
                {
                    foreach (var p in particles)
                    {
                        measure(p);
                    }
                }
            };

            return particles.Update(measureFunc, normalize, resample, effectiveCountMinRatio);                    
        }

        /// <summary>
        /// Updates particle's state.
        /// </summary>
        /// <param name="particles">Particles.</param>
        /// <param name="measure">Assign weight to each particle.</param>
        /// <param name="normalize">Normalization function.</param>
        /// <param name="resample">Resample particles (creates new swarm).</param>
        /// <param name="effectiveCountMinRatio">If calculated effective count ratio is lower than user specified value resampling will occur, otherwise not.
        /// <para>The range is [0..1]. If resampling must occur every time put 1.</para>
        /// </param>
        public static IEnumerable<TParticle> Update<TParticle>(this IEnumerable<TParticle> particles,
                                                Action<IEnumerable<TParticle>> measure,
                                                Func<IEnumerable<IParticle>, IEnumerable<double>> normalize,
                                                Func<IEnumerable<TParticle>, IEnumerable<double>, IEnumerable<TParticle>> resample,
                                                float effectiveCountMinRatio = 0.9f)
              where TParticle : class, IParticle
        {
            measure(particles);

            var normalizedWeights = normalize(particles);

            var newParticles = particles;
            var effectiveCountRatio = (double)EffectiveParticleCount(normalizedWeights) / particles.Count();
            if (effectiveCountRatio > Single.Epsilon && //do not resample if all particle weights are zero
                effectiveCountRatio < effectiveCountMinRatio)
            {
                newParticles = resample(particles, normalizedWeights);
            }

            return newParticles;
        }

        private static double EffectiveParticleCount(IEnumerable<double> weights)
        {
            var sumSqr = weights.Sum(x => x * x) + Single.Epsilon;
            return /*1 if weights are normalized*/ weights.Sum() / sumSqr;
        }

        #endregion
    }
}
