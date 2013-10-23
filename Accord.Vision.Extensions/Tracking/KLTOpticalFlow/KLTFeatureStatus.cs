
namespace Accord.Vision
{
    /// <summary>
    /// Status of an KLT feature.
    /// </summary>
    public enum KLTFeatureStatus
    {
        /// <summary>
        /// Successfully tracked
        /// </summary>
        Success,
        /// <summary>
        /// Movement was farther than it could possibly be tracked
        /// </summary>
        Drifted,
        /// <summary>
        /// Movement was out of image bounds
        /// </summary>
        OutOfBounds,
        /// <summary>
        /// Eigen value was smaller than specified meaning that feature does no lie on corner or corner does not have strong gradients
        /// </summary>
        SmallEigenValue,
        /// <summary>
        /// Miscellaneous track failure
        /// </summary>
        Failed,
        /// <summary>
        /// Feature's error was too large
        /// </summary>
        LargeError
    }
}
