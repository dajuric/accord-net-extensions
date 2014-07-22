using System;
using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Compass convolution filter.
    /// </summary>
    public static class CompassConvolutionExtensions
    {
        /// <summary>
        /// Compass convolution filter.
        /// <para>Accord.NET internal call. See <see cref="Accord.Imaging.Filters.CompassConvolution"/> for details.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="masks">Convolution masks.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, TDepth> CompassConvolution<TColor, TDepth>(this Image<TColor, TDepth> img, int[][,] masks)
            where TColor : IColor
            where TDepth : struct
        {
            CompassConvolution cc = new CompassConvolution(masks);
            return img.ApplyFilter(cc);
        }
    }
}
