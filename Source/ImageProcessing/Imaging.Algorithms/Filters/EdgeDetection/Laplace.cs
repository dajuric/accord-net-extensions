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
using Accord.Math;
using Accord.Extensions.Math;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Laplace extensions.
    /// </summary>
    public static class LaplaceExtensions
    {
        /// <summary>
        /// laplace 3x3 kernel.
        /// </summary>
        public static readonly float[,] Laplace_3x3 = new float[,] 
        {
            {+0, -1, +0},
            {-1, +4, -1},
            {+0, -1, +0}
        };

        private static int kernelNormalizer = 1 + 1 + 1 + 1 + 4;

        /// <summary>
        /// Calculates the Laplacian of the image with the appropriate kernel.
        /// </summary>
        /// <param name="im">Input image.</param>
        /// <param name="apertureSize">Kernel size.</param>
        /// <param name="normalizeKernel">Normalize kernel so the sum of all elements is 1.</param>
        /// <returns>Processed image.</returns>
        public static TColor[,] Laplace<TColor>(this TColor[,] im, int apertureSize = 3, bool normalizeKernel = false)
           where TColor : struct, IColor<float>
        {
            //only supported aperture is 3 (for now)
            if (apertureSize != 3)
                throw new Exception("Unsuported aperture size!");

            var laplace_3x3 = normalizeKernel ? Laplace_3x3.Divide(kernelNormalizer) : Laplace_3x3;

            return im.Convolve(laplace_3x3);
        }

    }
}
