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

        object State { get; set; }
    }

    /// <summary>
    /// Particle implementation.
    /// </summary>
    /// <typeparam name="TState">Particle's state.</typeparam>
    public class Particle<TState> : IParticle
        where TState : ICloneable
    {
        public Particle()
        {
            //this.State = new TState(); //but then arrays can not be used
        }

        /// <summary>
        /// Current particle's state
        /// </summary>
        public TState State { get; set; }

        /// <summary>
        /// Clones particle and it's state (deep cloning).
        /// <para>Usually it is used during resampling stage.</para>
        /// </summary>
        /// <returns>New particle.</returns>
        public Particle<TState> Clone()
        {
            return new Particle<TState>
            {
                State = (TState)this.State.Clone(),
                Weight = this.Weight,
            };
        }

        public double Weight
        {
            get;
            set;
        }

        #region Interface Implementation

        object IParticle.State
        {
            get { return this.State; }
            set { this.State = (TState)value; }
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
