using Accord.Imaging;
using AForge;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Features from Accelerated Segment Test (FAST) corners detector extensions.
    /// </summary>
    public static class FastCornersDetectorExtensions
    {
        /// <summary>
        /// Features from Accelerated Segment Test (FAST) corners detector.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.FastCornersDetector"/> for details.</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="threshold">The suppression threshold. Decreasing this value increases the number of points detected by the algorithm.</param>
        /// <returns>Interest point locations.</returns>
        public static List<IntPoint> CornerFeaturesDetector(this Image<Gray, byte> im, int threshold = 20)
        {
            FastCornersDetector fast = new FastCornersDetector(threshold);
            var points = fast.ProcessImage(im.ToAForgeImage(copyAlways: false, failIfCannotCast: true));
            
            return points;
        }
    }
}
