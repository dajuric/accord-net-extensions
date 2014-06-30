using System;
using Accord.Math;

namespace Accord.Extensions.Imaging.Filters
{
    public static class LaplaceExtensions
    {
        public static readonly Image<Gray, float> Laplace_3x3 = new float[,] 
        {
            {+0, -1, +0},
            {-1, +4, -1},
            {+0, -1, +0}
        }
        .ToImage();

        private static int kernelNormalizer = 1 + 1 + 1 + 1 + 4;

        /// <summary>
        /// Calculates the Laplacian of the image with the appropriate kernel.
        /// </summary>
        /// <param name="im">Input image.</param>
        /// <param name="apertureSize">Kernel size.</param>
        /// <param name="normalizeKernel">Normalize kernel so the sum of all elements is 1.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, short> Laplace<TColor>(this Image<TColor, byte> im, int apertureSize = 3, bool normalizeKernel = false)
            where TColor: IColor
        {
            //convert to short to avoid overflow
            return im.Convert<TColor, short>().Laplace(apertureSize);
        }

        /// <summary>
        /// Calculates the Laplacian of the image with the appropriate kernel.
        /// </summary>
        /// <param name="im">Input image.</param>
        /// <param name="apertureSize">Kernel size.</param>
        /// <param name="normalizeKernel">Normalize kernel so the sum of all elements is 1.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, TDepth> Laplace<TColor, TDepth>(this Image<TColor, TDepth> im, int apertureSize = 3, bool normalizeKernel = false)
           where TColor : IColor
           where TDepth: struct
        {
            //only supported aperture is 3 (for now)
            if (apertureSize != 3)
                throw new Exception("Unsuported aperture size!");

            var laplace_3x3 = normalizeKernel ? Laplace_3x3.Div(kernelNormalizer) : Laplace_3x3;

            return im.Convolve(laplace_3x3);
        }

    }
}
