
namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Interface for a track which removal wants to be handled by entropy threshold.
    /// <para>See <see cref="Accord.Extensions.Statistics.Filters.EntropyTrackerRemoval"/> class.</para>
    /// </summary>
    public interface IEntropyTrack
    {
        /// <summary>
        /// Initial track(er) entropy.
        /// <para>The value is modified by entropy management internally.</para>
        /// </summary>
        double InitialEntropy { get; set; }
        /// <summary>
        /// True if the track is tentative, false if not. 
        /// <para>The value must be null as the track values will be initialized by entropy management itself.</para>
        /// </summary>
        bool? IsTentative { get; set; }
    }

    /// <summary>
    /// Interface for a track which removal wants to be handled by entropy threshold.
    /// <para>See <see cref="Accord.Extensions.Statistics.Filters.EntropyTrackerRemoval"/> class.</para>
    /// </summary>
    public interface IEntropyTrack<TTracker> : IEntropyTrack
    {
        /// <summary>
        /// Tracker for the track.
        /// </summary>
        TTracker Tracker { get; }
    }
}
