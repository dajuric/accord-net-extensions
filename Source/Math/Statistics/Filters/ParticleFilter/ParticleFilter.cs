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
using System.Threading.Tasks;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Contains static and extensions methods that operate on collection of particles.
    /// </summary>
    /// 
    /// <remarks>
    /// See lectures: <a href="http://www.kev-smith.com/teaching/L7_ParticleFilters.pdf">Particle filer.</a>
    /// </remarks>
    public static partial class ParticleFilter
    {      
        #region Predict Methods

        /// <summary>
        /// Draws particles randomly, clones them and reinitializes their weights.
        /// </summary>
        /// <typeparam name="TParticle">Particle type.</typeparam>
        /// <param name="particles">Particle collection.</param>
        /// <param name="sampleCount">The number of particles to draw.</param>
        /// <returns>Resampled particles (particles are cloned).</returns>
        public static List<TParticle> Resample<TParticle>(this IList<TParticle> particles, int sampleCount)
              where TParticle : class, IParticle
        {
            var resampledParticles = new List<TParticle>(sampleCount);

            var drawnParticles = particles.Draw(particles.Count);
            foreach (var dP in drawnParticles)
            {
                var newP = (TParticle)dP.Clone();
                newP.Weight = 1d / particles.Count;
                resampledParticles.Add(newP);
            }

            return resampledParticles;
        }

        /// <summary>
        /// Predicts particles' state by resampling, applying drift and diffuse.
        /// <para>Resampling is carried out if particles' effective count ratio is less drops bellow the minimum.</para>
        /// </summary>
        /// <typeparam name="TParticle">Particle type.</typeparam>
        /// <param name="particles">Particles.</param>
        /// <param name="effectiveCountMinRatio">If calculated effective count ratio is lower than user specified value re-sampling will occur, otherwise not.
        /// <para>The range is [0..1]. If resampling must occur every time put 1.</para>
        /// </param>
        /// <returns>Updated particles (particles are cloned).</returns>
        public static List<TParticle> Predict<TParticle>(this IList<TParticle> particles, float effectiveCountMinRatio = 0.9f)
           where TParticle : class, IParticle
        {
            return particles.Predict(effectiveCountMinRatio, particles.Count);
        }

        /// <summary>
        /// Predicts particles' state by resampling, applying drift and diffuse.
        /// <para>Resampling is carried out if particles' effective count ratio is less drops bellow the minimum.</para>
        /// </summary>
        /// <typeparam name="TParticle">Particle type.</typeparam>
        /// <param name="particles">Particles.</param>
        /// <param name="effectiveCountMinRatio">If calculated effective count ratio is lower than user specified value re-sampling will occur, otherwise not.
        /// <para>The range is [0..1]. If resampling must occur every time put 1.</para>
        /// </param>
        /// <param name="sampleCount">The number of particles to draw during resampling.</param>
        /// <returns>Updated particles (particles are cloned).</returns>
        public static List<TParticle> Predict<TParticle>(this IList<TParticle> particles, float effectiveCountMinRatio, int sampleCount)
            where TParticle: class, IParticle
        {
            List<TParticle> newParticles = null;
            var effectiveCountRatio = (double)effectiveParticleCount(particles.GetNormalizedWeights()) / particles.Count;
            if (effectiveCountRatio > Single.Epsilon && //do not resample if all particle weights are zero
                effectiveCountRatio < effectiveCountMinRatio)
            {
                newParticles = particles.Resample(sampleCount);
            }
            else
            {
                newParticles = particles
                               .Select(x => (TParticle)x.Clone())
                               .ToList();
            }

            foreach (var p in newParticles)
            {
                p.Drift();
                p.Diffuse();
            }

            return newParticles;
        }

        private static double effectiveParticleCount(IEnumerable<double> weights)
        {
            var sumSqr = weights.Sum(x => x * x) + Single.Epsilon;
            return /*1 if weights are normalized*/ weights.Sum() / sumSqr;
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates the collection of particles by changing their weight obtained by the measurement function.
        /// </summary>
        /// <typeparam name="TParticle">Particle type.</typeparam>
        /// <param name="particles">The collection of particles.</param>
        /// <param name="measure">Measurement function which receives a particle and returns the particle's weight.</param>
        public static void Update<TParticle>(this IEnumerable<TParticle> particles, Func<TParticle, double> measure)
            where TParticle : IParticle
        {
            foreach (var p in particles)
            {
                p.Weight = measure(p);
            }
        }

        #endregion
    }
}
