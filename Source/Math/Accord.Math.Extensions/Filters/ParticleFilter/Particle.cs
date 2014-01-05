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
    }
}
