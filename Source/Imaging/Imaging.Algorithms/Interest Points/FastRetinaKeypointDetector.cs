using Accord.Imaging;
using AForge;
using System;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Fast Retina Keypoint (FREAK) detector extensions.
    /// </summary>
    public static class FastRetinaKeypointDetectorExtensions
    {
        /// <summary>
        /// Fast Retina Keypoint (FREAK) detector.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.FastRetinaKeypointDetector"/> for details.</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="threshold">The detection threshold for the <see cref="Accord.Imaging.FastCornersDetector">FAST detector</see>.</param>
        /// <param name="computeDescriptors">
        /// Value indicating whether all feature points
        /// should have their descriptors computed after being detected.
        /// </param>
        /// <param name="octaves">The number of octaves to use when building the feature descriptor.</param>
        /// <param name="scale">The scale used when building the feature descriptor</param>
        /// <returns>FREAK's detector result.</returns>
        public static AlgorithmResult<FastRetinaKeypointDetector, List<FastRetinaKeypoint>> FREAK(this Image<Gray, byte> im, int threshold = 20, FastRetinaKeypointDescriptorType computeDescriptors = FastRetinaKeypointDescriptorType.Standard, int octaves = 4, float scale = 0.22f)
        {
            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector(threshold);
            freak.ComputeDescriptors = computeDescriptors;
            freak.Octaves = octaves;
            freak.Scale = scale;
            
            var points = freak.ProcessImage(im.ToAForgeImage(copyAlways: false, failIfCannotCast: true));

            return new AlgorithmResult<FastRetinaKeypointDetector, List<FastRetinaKeypoint>>(freak, points);
        }
    }
}
