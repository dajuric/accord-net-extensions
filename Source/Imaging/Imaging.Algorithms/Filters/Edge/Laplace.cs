using Accord.Math;
using System;

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

        static LaplaceExtensions()
        {
            normalizeKernel(Laplace_3x3);
        }

        private static void normalizeKernel(Image<Gray, float> kernel)
        {
            var sum = kernel.Convert<Gray, double>().ToArray().Abs().Sum().Sum();
            kernel.Div(sum / 2, inPlace: true);
        }

        /// <summary>
        /// Calculates the Laplacian of the image with the appropriate kernel.
        /// </summary>
        /// <param name="im">Input image.</param>
        /// <param name="apertureSize">Kernel size.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, short> Laplace<TColor>(this Image<TColor, byte> im, int apertureSize = 3)
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
        /// <returns>Processed image.</returns>
        public static Image<TColor, TDepth> Laplace<TColor, TDepth>(this Image<TColor, TDepth> im, int apertureSize = 3)
           where TColor : IColor
           where TDepth: struct
        {
            //only supported aperture is 3 (for now)
            if (apertureSize != 3)
                throw new Exception("Unsuported aperture size!");

            return im.Convolve(Laplace_3x3);
        }

    }
}
