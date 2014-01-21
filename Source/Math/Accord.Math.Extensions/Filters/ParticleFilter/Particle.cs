using System;

namespace Accord.Statistics.Filters
{
    /// <summary>
    /// Non-generic particle interface
    /// </summary>
    public interface IParticle : ICloneable
    {
        /// <summary>
        /// Particle's weight.
        /// </summary>
        double Weight { get; set; }

        /// <summary>
        /// Applies model transition without noise to a particle's state.
        /// </summary>
        void Drift();

        /// <summary>
        /// Applies noise to a particle's state.
        /// </summary>
        void Difuse();
    }
}
