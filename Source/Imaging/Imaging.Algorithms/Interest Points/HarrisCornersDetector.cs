using Accord.Imaging;
using AForge;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Harris corners extensions.
    /// </summary>
    public static class HarrisCornersDetectorExtensions
    {
        /// <summary>
        /// Harris Corners Detector.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.HarrisCornersDetector"/> for details.</para>
        /// </summary>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="im">Image.</param>
        /// <param name="measure">Corners measures.</param>
        /// <param name="threshold">Harris threshold.</param>
        /// <param name="sigma">Gaussian smoothing sigma.</param>
        /// <param name="suppression">Non-maximum suppression window radius.</param>
        /// <returns>Interest point locations.</returns>
        public static List<IntPoint> HarrisCorners<TDepth>(this Image<Gray, TDepth> im, HarrisCornerMeasure measure = HarrisCornerMeasure.Harris, float threshold = 20000f, double sigma = 1.2, int suppression = 3)
            where TDepth : struct
        {
            HarrisCornersDetector harris = new HarrisCornersDetector(measure, threshold, sigma, suppression);
            var points = harris.ProcessImage(im.ToAForgeImage(copyAlways: false, failIfCannotCast: true));

            return points;
        }
    }
}
