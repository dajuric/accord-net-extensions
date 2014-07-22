using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Kirsch's Edge Detector filter.
    /// </summary>
    public static class KirschEdgeDetectorExtensions
    {
        /// <summary>
        /// Kirsch's Edge Detector
        /// <para>Accord.NET internal call.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <typeparam name="TDepth">Depth type.</typeparam>
        /// <param name="img">Input image.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, TDepth> Kirsch<TColor, TDepth>(this Image<TColor, TDepth> img)
            where TColor : IColor
            where TDepth : struct
        {
            KirschEdgeDetector k = new KirschEdgeDetector();
            return img.ApplyFilter(k);
        }
    }
}
