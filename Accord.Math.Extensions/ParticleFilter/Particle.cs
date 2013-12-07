using System;

namespace Accord.Math.Extensions.ParticleFilter
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

    /// <summary>
    /// Particle implementation.
    /// </summary>
    /// <typeparam name="TModel">Generic parameter for those particles that need extra meta-data.</typeparam>
    /// <typeparam name="TState">Particle's state.</typeparam>
    public class Particle<TModel, TState> : Particle<TState>
        where TState : ICloneable
    {
        /// <summary>
        /// Process update model. <br/>
        /// Although is common that every particle has the same model, 
        /// there are implementations that each particle chooses process model according to some probability (switches between models); 
        /// therefore each this property belongs to particle.
        /// </summary>
        public TModel ProcessModel { get; internal set; }

        /// <summary>
        /// Clones particle and it's state (deep cloning).
        /// </summary>
        /// <returns>New particle.</returns>
        public new Particle<TModel, TState> Clone()
        {
            var newParticle = base.Clone() as Particle<TModel, TState>;
            newParticle.ProcessModel = this.ProcessModel;

            return newParticle;
        }
    }
}
