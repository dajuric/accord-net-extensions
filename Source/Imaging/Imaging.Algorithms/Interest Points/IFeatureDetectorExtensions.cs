using Accord.Imaging;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Feature detector generic extensions.
    /// </summary>
    public static class IFeatureDetectorExtensions
    {
        /// <summary>
        /// Process image looking for interest points.
        /// </summary>
        /// <typeparam name="TPoint">The type of returned feature points.</typeparam>
        /// <typeparam name="TFeature">The type of extracted features.</typeparam>
        /// <param name="featureDetector">Feature detector.</param>
        /// <param name="image">Source image data to process.</param>
        /// <returns>Returns list of found interest points.</returns>
        public static List<TPoint> ProcessImage<TPoint, TFeature>(this IFeatureDetector<TPoint, TFeature> featureDetector, Image<Gray, byte> image)
             where TPoint : IFeatureDescriptor<TFeature>
        {
            return featureDetector.ProcessImage(image.ToAForgeImage(copyAlways: false, failIfCannotCast: true));
        }
    }
}
