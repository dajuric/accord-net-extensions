using Accord.Imaging;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Speeded-up Robust Features (SURF) detector extensions.
    /// </summary>
    public static class SpeededUpRobustFeaturesDetectorExtensions
    {
        /// <summary>
        /// Speeded-up Robust Features (SURF) detector.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.SpeededUpRobustFeaturesDetector"/> for details.</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="threshold">The non-maximum suppression threshold.</param>
        /// <param name="octaves">
        /// The number of octaves to use when building the <see cref="Accord.Imaging.ResponseLayerCollection">
        /// response filter</see>. Each octave corresponds to a series of maps covering a
        /// doubling of scale in the image.</param>
        /// <param name="initialStep">
        /// The initial step to use when building the <see cref="Accord.Imaging.ResponseLayerCollection"> response filter
        /// </param>
        /// <param name="computeOrientation">
        /// A value indicating whether all feature points should have their orientation computed after being detected.
        /// </param>
        /// <param name="computeDescriptors">
        /// A value indicating whether all feature points should have their descriptors computed after being detected.
        /// </param>
        /// <returns></returns>
        public static AlgorithmResult<SpeededUpRobustFeaturesDetector, List<SpeededUpRobustFeaturePoint>> SURF(this Image<Gray, byte> im, 
                                                                                                               float threshold = 0.0002f, int octaves = 5, int initialStep = 2,
                                                                                                               bool computeOrientation = true,  SpeededUpRobustFeatureDescriptorType computeDescriptors = SpeededUpRobustFeatureDescriptorType.Standard)
        {
            SpeededUpRobustFeaturesDetector surf = new SpeededUpRobustFeaturesDetector(threshold, octaves, initialStep);
            surf.ComputeDescriptors = computeDescriptors;
            surf.ComputeOrientation = computeOrientation;

            var points = surf.ProcessImage(im.ToAForgeImage(copyAlways: false, failIfCannotCast: true));

            return new AlgorithmResult<SpeededUpRobustFeaturesDetector, List<SpeededUpRobustFeaturePoint>>(surf, points);
        }
    }
}
