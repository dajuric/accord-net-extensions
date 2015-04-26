#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Collections.Generic;
using Accord.Extensions.Math;
using Accord.Math;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Sobel extensions.
    /// </summary>
    public static class SobelExtensions
    {
        /// <summary>
        /// Sobel 3x3 for finding vertical edges.
        /// </summary>
        public static readonly float[,] Sobel_3x3_X = new float[,] 
        {
            {+1, 0, -1},
            {+2, 0, -2},
            {+1, 0, -1}
        };

        /// <summary>
        /// Sobel 3x3 for finding horizontal edges.
        /// </summary>
        public static readonly float[,] Sobel_3x3_Y = new float[,] 
        {
            {+1, +2, +1},
            {+0, +0, +0},
            {-1, -2, -1}
        };

        private static int kernelNormalizer = 1 + 2 + 1 + 1 + 2 + 1;

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
        /// <param name="normalizeKernel">Normalize kernel so the sum of all elements is 1.</param>
        /// <returns>Processed image.</returns>
        public static TColor[,] Sobel<TColor>(this TColor[,] im, int xOrder, int yOrder, int apertureSize = 3, bool normalizeKernel = false)
           where TColor : struct, IColor<float>
        {
            //only supported aperture is 3 (for now)
            if (apertureSize != 3)
                throw new Exception("Unsuported aperture size!");

            var sobel_3x3_X = normalizeKernel ? Sobel_3x3_X.Divide(kernelNormalizer) : Sobel_3x3_X;
            var sobel_3x3_Y = normalizeKernel ? Sobel_3x3_Y.Divide(kernelNormalizer) : Sobel_3x3_Y;

            TColor[,] convolvedIm = im;
            while (xOrder != 0)
            {
                convolvedIm = convolvedIm.Convolve(sobel_3x3_X);
                xOrder--;
            }

            while (yOrder != 0)
            {
                convolvedIm = convolvedIm.Convolve(sobel_3x3_Y);
                yOrder--;
            }

            return convolvedIm;
        }

    }
}
