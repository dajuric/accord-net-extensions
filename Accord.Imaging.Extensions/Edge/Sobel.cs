using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using System.Threading.Tasks;
using Accord.Core;

namespace Accord.Imaging.Filters
{
    public static class SobelEdgeDetector
    {
        public static readonly Image<Gray, float> Sobel_3x3_X = new float[,] 
        {
            {+1, 0, -1},
            {+2, 0, -2},
            {+1, 0, -1}
        }
        .ToImage();

        public static readonly Image<Gray, float> Sobel_3x3_Y = new float[,] 
        {
            {+1, +2, +1},
            {+0, +0, +0},
            {-1, -2, -1}
        }
        .ToImage();

        static SobelEdgeDetector()
        {
            normalizeKernel(Sobel_3x3_X);
            normalizeKernel(Sobel_3x3_Y);
        }

        private static void normalizeKernel(Image<Gray, float> kernel)
        {
            var sum = kernel.Convert<double>().ToArray().Abs().Sum().Sum();
            kernel.Div(sum, inPlace: true);
        }

        /*static readonly float[][,] sobel_3x3_X = new float[][,] // is actually slower using separable kernel; WTF ? TODO: DO something! 
        {
            new float[,]{ 
            {1},
            {2},
            {1}},

            new float[,]{
             {1, 0, -1}
            }
        };*/

        /// <summary>
        /// Calculates the image derivative by convolving the image with the appropriate kernel.
        /// Most often, the function is called with (xorder=1, yorder=0, aperture_size=3) or (xorder=0, yorder=1, aperture_size=3) to calculate first x- or y- image derivative.
        /// </summary>
        /// <param name="im">Input image.</param>
        /// <param name="xOrder">Horizontal derivative order. </param>
        /// <param name="yOrder">Vertical derivative order.</param>
        /// <param name="apertureSize">Kernel size.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, short> Sobel<TColor>(this Image<TColor, byte> im, int xOrder, int yOrder, int apertureSize = 3)
            where TColor: IColor
        {
            //convert to short to avoid overflow
            return im.Convert<short>().Sobel(xOrder, yOrder, apertureSize);
        }

        /// <summary>
        /// Calculates the image derivative by convolving the image with the appropriate kernel.
        /// Most often, the function is called with (xorder=1, yorder=0, aperture_size=3) or (xorder=0, yorder=1, aperture_size=3) to calculate first x- or y- image derivative.
        /// </summary>
        /// <param name="im">Input image.</param>
        /// <param name="xOrder">Horizontal derivative order. </param>
        /// <param name="yOrder">Vertical derivative order.</param>
        /// <param name="apertureSize">Kernel size.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, TDepth> Sobel<TColor, TDepth>(this Image<TColor, TDepth> im, int xOrder, int yOrder, int apertureSize = 3)
           where TColor : IColor
           where TDepth: struct
        {
            //only supported aperture is 3 (for now)
            if (apertureSize != 3)
                throw new Exception("Unsuported aperture size!");

            var kernels = new List<Image<Gray, float>>();

            while (xOrder != 0)
            {
                kernels.Add(Sobel_3x3_X);
                xOrder--;
            }

            while (yOrder != 0)
            {
                kernels.Add(Sobel_3x3_Y);
                yOrder--;
            }

            return im.Convolve(kernels.ToArray());
        }

    }
}
