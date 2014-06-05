using System;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Particle interface defining common members for all particle instances.
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

    /// <summary>
    /// Particle interface defining common members for all particle instances.
    /// </summary>
    /// <typeparam name="TState">State type.</typeparam>
    public interface IParticle<TState> : IParticle
    {
        TState State { get; set; }
    }
}
