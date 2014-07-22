using Accord.Imaging.Filters;
using Accord.Math.Wavelets;
using AForge.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains wavelet transform extensions.
    /// </summary>
    public static class WaveletTransformExtensions
    {
        /// <summary>
        /// Applies wavelet transform filter (Accord.NET).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="wavelet">A wavelet function.</param>
        /// <param name="backward">True to perform backward transform, false otherwise.</param>
        /// <returns>Transformed image.</returns>
        private static Image<TColor, TDepth> WaveletTransform<TColor, TDepth>(this Image<TColor, TDepth> img, IWavelet wavelet, bool backward)
            where TColor : IColor
            where TDepth : struct
        {
            WaveletTransform wt = new WaveletTransform(wavelet, backward);
            return img.ApplyFilter((BaseFilter)wt);
        }

        /// <summary>
        /// Applies wavelet transform filter (Accord.NET).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="wavelet">A wavelet function.</param>
        /// <param name="backward">True to perform backward transform, false otherwise.</param>
        /// <returns>Transformed image.</returns>
        public static Image<Gray, byte> WaveletTransform(this Image<Gray, byte> img, IWavelet wavelet, bool backward = false)
        {
            return WaveletTransform<Gray, byte>(img, wavelet, backward);
        }

        /// <summary>
        /// Applies wavelet transform filter (Accord.NET).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="wavelet">A wavelet function.</param>
        /// <param name="backward">True to perform backward transform, false otherwise.</param>
        /// <returns>Transformed image.</returns>
        public static Image<Gray, short> WaveletTransform(this Image<Gray, short> img, IWavelet wavelet, bool backward = false)
        {
            return WaveletTransform<Gray, short>(img, wavelet, backward);
        }
    }
}
