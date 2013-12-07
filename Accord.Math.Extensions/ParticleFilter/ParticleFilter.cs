using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Math.Extensions.ParticleFilter
{
    public class ParticleFilter<TParticle, TState>
        where TParticle : Particle<TState>//, new()
        where TState : ICloneable
    {
        public IList<TParticle> Particles { get; private set; }

        public ParticleFilter()
        { }

        /// <summary>
        /// Initializes particles by using provided <see cref="Initializer"/>.
        /// </summary>
        public void Initialize()
        {
            this.Particles = Initializer(this.ParticlesCount).ToList();
            this.EffectiveCountMinRatio = 0.9;
        }

        public double EffectiveParticleCount
        {
            get
            {
                var sumSqr = this.Particles.Sum(x => x.Weight * x.Weight);
                return 1d / sumSqr;
            }
        }

        public double EffectiveCountMinRatio
        {
            get;
            set;
        }

        public int ParticlesCount
        {
            get;
            set;
        }

        #region Predict Methods

        public void Predict()
        {
            drift();
            diffuse();
        }

        /// <summary>
        /// Update state from model (no noise).
        /// </summary>
        private void drift()
        {
            foreach (var p in this.Particles)
            {
                var state = p.State;
                Drift(ref state);
                p.State = state;
            }
        }

        /// <summary>
        /// Apply noise to spread particles.
        /// </summary>
        private void diffuse()
        {
            foreach (var p in this.Particles)
            {
                var state = p.State;
                Diffuse(ref state);
                p.State = state;
            }
        }

        #endregion

        #region Update Methods

        public void Update()
        {
            measure(WeightAssigner);

            if ((double)this.EffectiveParticleCount / this.Particles.Count < this.EffectiveCountMinRatio)
            {
                this.Particles = this.Resampler(this.Particles).ToList();
            }
        }

        /// <summary>
        /// Assign weight to each particle. Weights are normalized afterwards.
        /// </summary>
        /// <param name="weightFunc"></param>
        private void measure(WeightFunc weightFunc)
        {
            foreach (var p in this.Particles)
            {
                p.Weight = weightFunc(this, p.State);
            }

            this.Normalizer(this.Particles);
        }

        #endregion

        #region Initialize Properties

        /// <summary>
        /// A function that initializes particles.
        /// </summary>
        /// <param name="numberOfParticles">Initial number of particles.</param>
        /// <returns></returns>
        public delegate IEnumerable<TParticle> InitializerFunc(int numberOfParticles);

        public InitializerFunc Initializer
        {
            get;
            set;
        }

        #endregion

        #region Predict Properties

        /// <summary>
        /// A function that applies state predicition from model.
        /// </summary>
        /// <param name="state">Particle's state.</param>
        public delegate void DriftFunc(ref TState state);

        /// <summary>
        /// A function that applies noise to a particle state.
        /// </summary>
        /// <param name="state">Particle's state.</param>
        public delegate void DiffuseFunc(ref TState state);

        public DriftFunc Drift
        {
            get;
            set;
        }

        public DiffuseFunc Diffuse
        {
            get;
            set;
        }

        #endregion

        #region Update Properties

        /// <summary>
        /// A function that assigns weight to a particle.
        /// </summary>
        /// <param name="filter">Particle filer that contains those particles.</param>
        /// <param name="pState">Particle's state to assign wight to.</param>
        /// <returns>Particle weight.</returns>
        public delegate double WeightFunc(ParticleFilter<TParticle, TState> filter, TState pState);
        /// <summary>
        /// Normalization function for particles' weight normalization. <br/>
        /// This function should be used with appropriate weight assign function.
        /// </summary>
        /// <param name="particles">Particles.</param>
        public delegate void NormalizationFunc(IEnumerable<IParticle> particles);
        /// <summary>
        /// Resample function.
        /// This function should be used with appropriate normalization function.
        /// </summary>
        /// <param name="particles">Particles.</param>
        /// <returns>Particles with renormalized weights.</returns>
        public delegate IEnumerable<TParticle> ResampleFunc(IEnumerable<TParticle> particles);

        public WeightFunc WeightAssigner
        {
            get;
            set;
        }

        public NormalizationFunc Normalizer
        {
            get;
            set;
        }

        public ResampleFunc Resampler
        {
            get;
            set;
        }

        #endregion
    }
}
