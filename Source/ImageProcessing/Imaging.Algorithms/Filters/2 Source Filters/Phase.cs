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

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains image extensions methods for calculating phase image from two images (real and imaginary).
    /// </summary>
    public static class PhaseExtensions
    {
        /// <summary>
        /// Calculates phase using Atan2 (secondImage / firstImage). 
        /// </summary>
        /// <param name="imageX">First image.</param>
        /// <param name="imageY">Second image.</param>
        /// <returns>Phase.</returns>
        public static Gray<float>[,] Phase(this  Gray<float>[,] imageX, Gray<float>[,] imageY)
        {
            return imageX.Calculate(imageY, phase_Float, inPlace: false);
        }

        /// <summary>
        /// Calculates phase using Atan2 (secondImage / firstImage). 
        /// </summary>
        /// <param name="imageX">First image.</param>
        /// <param name="imageY">Second image.</param>
        /// <returns>Phase.</returns>
        public static Gray<double>[,] Phase(this  Gray<double>[,] imageX, Gray<double>[,] imageY)
        {
            return imageX.Calculate(imageY, phase_Double, inPlace: false);
        }

        private unsafe static void phase_Float(IImage imageX, IImage imageY, IImage magnitudeImage)
        {
            int width = imageX.Width;
            int height = imageX.Height;
            int srcAOffset = imageX.Stride - width * sizeof(float);
            int srcBOffset = imageY.Stride - width * sizeof(float);
            int dstOffset = magnitudeImage.Stride - width * sizeof(float);

            float* srcXPtr = (float*)imageX.ImageData;
            float* srcYPtr = (float*)imageY.ImageData;
            float* dstPtr = (float*)magnitudeImage.ImageData;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    *dstPtr = (float)System.Math.Atan2(*srcYPtr, *srcXPtr);

                    srcXPtr++;
                    srcYPtr++;
                    dstPtr++;
                }

                srcXPtr = (float*)((byte*)srcXPtr + srcAOffset);
                srcYPtr = (float*)((byte*)srcYPtr + srcBOffset);
                dstPtr = (float*)((byte*)dstPtr + dstOffset);
            }
        }

        private unsafe static void phase_Double(IImage imageX, IImage imageY, IImage magnitudeImage)
        {
            int width = imageX.Width;
            int height = imageX.Height;
            int srcAOffset = imageX.Stride - width * sizeof(double);
            int srcBOffset = imageY.Stride - width * sizeof(double);
            int dstOffset = magnitudeImage.Stride - width * sizeof(double);

            double* srcXPtr = (double*)imageX.ImageData;
            double* srcYPtr = (double*)imageY.ImageData;
            double* dstPtr = (double*)magnitudeImage.ImageData;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    *dstPtr = (double)System.Math.Atan2(*srcYPtr, *srcXPtr);

                    srcXPtr++;
                    srcYPtr++;
                    dstPtr++;
                }

                srcXPtr = (double*)((byte*)srcXPtr + srcAOffset);
                srcYPtr = (double*)((byte*)srcYPtr + srcBOffset);
                dstPtr = (double*)((byte*)dstPtr + dstOffset);
            }
        }
    }
}
