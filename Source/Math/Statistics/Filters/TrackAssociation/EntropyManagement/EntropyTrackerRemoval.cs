using System;
using System.Collections.Generic;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Provides extension methods for the multi-object entropy track(er) removal management.
    /// </summary>
    public static class EntropyTrackerRemoval
    {
        /// <summary>
        /// Removes tracks if the track entropy is above the specified threshold <paramref name="maxEntropyIncrease"/>. 
        /// Also updates track entropy and tentative state. 
        /// </summary>
        /// <typeparam name="TTrack">Track type.</typeparam>
        /// <param name="tracks">Collection of tracks.</param>
        /// <param name="calculateEntropy">Function for the track entropy calculation.</param>
        /// <param name="minEntropyDecrease">Minimal entropy decrease to mark a track as non-tentative.</param>
        /// <param name="maxEntropyIncrease">Maximal entropy increase to remove a track.</param>
        /// <returns>True if at least one track is removed, false otherwise.</returns>
        public static bool Remove<TTrack>(this List<TTrack> tracks, Func<TTrack, double> calculateEntropy, double minEntropyDecrease = 0.15, double maxEntropyIncrease = 0.15)
            where TTrack: class, IEntropyTrack
        {
            var tracksToRemove = new List<TTrack>();

            foreach (var track in tracks)
            {
                var entropy = calculateEntropy(track);

                if (track.IsTentative.HasValue == false)
                {
                    track.InitialEntropy = entropy;
                    track.IsTentative = true;
                }
                   
                var initialEntropy = track.InitialEntropy;

                if (entropy <= (initialEntropy - System.Math.Abs((double)initialEntropy) * minEntropyDecrease))
                {
                    track.IsTentative = false;
                }

                if (entropy >= (initialEntropy + System.Math.Abs((double)initialEntropy) * maxEntropyIncrease))
                {
                    tracksToRemove.Add(track);
                }
            }

            tracks.Remove(tracksToRemove);
            return tracksToRemove.Count > 0;
        }

        /// <summary>
        /// Removes tracks if the track entropy is above the specified threshold <paramref name="maxEntropyIncrease"/>. 
        /// Also updates track entropy and tentative state. 
        /// </summary>
        /// <typeparam name="TTrack">Track type.</typeparam>
        /// <typeparam name="TKalman">Kalman filter type.</typeparam>
        /// <typeparam name="TState">Kalman state type.</typeparam>
        /// <typeparam name="TMeasurement">Kalman measurement type.</typeparam>
        /// <param name="tracks">Collection of tracks.</param>
        /// <param name="minEntropyDecrease">Minimal entropy decrease to mark a track as non-tentative.</param>
        /// <param name="maxEntropyIncrease">Maximal entropy increase to remove a track.</param>
        /// <returns>True if at least one track is removed, false otherwise.</returns>
        public static bool Remove<TTrack, TKalman, TState, TMeasurement>(this List<TTrack> tracks, double minEntropyDecrease = 0.15, double maxEntropyIncrease = 0.15)
            where TTrack : class, IEntropyTrack<TKalman>
            where TKalman: KalmanFilter<TState, TMeasurement>
        {
            return Remove<TTrack>(tracks, 
                                 (track) => track.Tracker.CalculateEntropy(), 
                                 minEntropyDecrease, maxEntropyIncrease);
        }

        /// <summary>
        /// Removes tracks if the track entropy is above the specified threshold <paramref name="maxEntropyIncrease"/>. 
        /// Also updates track entropy and tentative state. 
        /// </summary>
        /// <typeparam name="TTrack">Track type.</typeparam>
        /// <typeparam name="TParticleFilter">Particle filter type.</typeparam>
        /// <typeparam name="TParticle">Particle type.</typeparam>
        /// <param name="tracks">Collection of tracks.</param>
        /// <param name="stateConverter">Particle state converter.</param>
        /// <param name="minEntropyDecrease">Minimal entropy decrease to mark a track as non-tentative.</param>
        /// <param name="maxEntropyIncrease">Maximal entropy increase to remove a track.</param>
        /// <returns>True if at least one track is removed, false otherwise.</returns>
        public static bool Remove<TTrack, TParticleFilter, TParticle>(this List<TTrack> tracks, Func<TParticle, double[]> stateConverter, double minEntropyDecrease = 0.15, double maxEntropyIncrease = 0.15)
            where TTrack: class, IEntropyTrack<TParticleFilter>
            where TParticleFilter: IEnumerable<TParticle>
            where TParticle: class, IParticle
        {
            return Remove<TTrack>(tracks,
                                 (track) =>
                                 {
                                     var entropy = 0d;
                                     try
                                     {
                                         entropy = track.Tracker.CalculateEntropy(stateConverter);
                                     }
                                     catch (NonPositiveDefiniteMatrixException) { }

                                     return entropy;
                                 },
                                 minEntropyDecrease, maxEntropyIncrease);
        }
    }
}
