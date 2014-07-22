using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Robinson's Edge Detector.
    /// </summary>
    public static class RobinsonEdgeDetectorExtensions
    {
        /// <summary>
        /// Robinson's Edge Detector.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.RobinsonEdgeDetector"/> for details.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="img">Image.</param>
        /// <returns>Edge image.</returns>
        public static Image<TColor, TDepth> Rectification<TColor, TDepth>(this Image<TColor, TDepth> img)
            where TColor : IColor
            where TDepth : struct
        {
            RobinsonEdgeDetector r = new RobinsonEdgeDetector();
            return img.ApplyFilter(r);
        }
    }
}
