using System;

namespace Accord.Statistics.Filters
{
    /// <summary>
    /// Non-generic particle interface
    /// </summary>
    public interface IParticle : ICloneable
    {
        /// <summary>
        /// Particle's weight. It will be normalized after <see cref="Update"/> function.
        /// </summary>
        double Weight { get; set; }
    }
}
