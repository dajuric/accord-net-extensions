using Accord.Imaging;
using System.Collections.Generic;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    ///  Maximum cross-correlation feature point matching algorithm extensions.
    /// </summary>
    public static class CorrelationMatchingExtensions
    {
        /// <summary>
        ///  Maximum cross-correlation feature point matching algorithm.
        /// </summary>
        /// <param name="correlationMatching"> Maximum cross-correlation feature point matching algorithm.</param>
        /// <param name="image1">First image.</param>
        /// <param name="image2">Second image.</param>
        /// <param name="points1">Points from the first image.</param>
        /// <param name="points2">Points from the second image.</param>
        /// <returns>Matched point-pairs.</returns>
        public static Point[][] Match(this CorrelationMatching correlationMatching,
                                      Image<Gray, byte> image1, Image<Gray, byte> image2, Point[] points1, Point[] points2)
        {
            var result = correlationMatching.Match
                (
                  image1.ToBitmap(copyAlways: false, failIfCannotCast: true),
                  image2.ToBitmap(copyAlways: false, failIfCannotCast: true),
                  points1,
                  points2
                );

            return result;
        }
    }
}
