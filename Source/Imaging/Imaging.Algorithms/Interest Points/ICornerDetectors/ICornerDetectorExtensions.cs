using AForge.Imaging;
using System.Collections.Generic;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Corner detector generic extensions.
    /// </summary>
    public static class ICornerDetectorExtensions
    {
        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// <param name="cornerDetector">Corner detection algorithm instance.</param>
        /// <param name="image">Source image to process.</param>
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        public static List<Point> ProcessImage(this ICornersDetector cornerDetector, Image<Gray, byte> image)
        {
            return cornerDetector.ProcessImage(image.ToAForgeImage(copyAlways: false, failIfCannotCast: true));
        }
    }
}
