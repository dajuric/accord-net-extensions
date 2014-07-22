using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Logarithm filter.
    /// </summary>
    public static class LogarithmExtensions
    {
        /// <summary>
        /// Simple log image filter. Applies the <see cref="System.Math.Log(double)"/>
        /// function for each pixel in the image, clipping values as needed.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.Logarithm"/> for details.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, TDepth> Logarithm<TColor, TDepth>(this Image<TColor, TDepth> img, bool inPlace = false)
            where TColor : IColor
            where TDepth : struct
        {
            Logarithm l = new Logarithm();
            return img.ApplyFilter(l, inPlace);
        }
    }
}
