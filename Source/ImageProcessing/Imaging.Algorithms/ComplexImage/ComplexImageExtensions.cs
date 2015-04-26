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
using Accord.Extensions.Math;
using Accord.Extensions.Imaging;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides extensions for arithmetics with a complex image type.
    /// </summary>
    public static partial class ComplexImageExtensions
    {
        /// <summary>
        /// Calculates Fast Fourier transform.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="direction">Forward or backward direction.</param>
        /// <param name="inPlace">Process in place or not.</param>
        /// <returns>Processed image. If <paramref name="inPlace"/> is used the result is the same as input image therefore may be omitted.</returns>
        public unsafe static ComplexF[,] FFT(this ComplexF[,] image, FourierTransform.Direction direction, bool inPlace = false)
        {
            ComplexF[,] dest = null;
            if (inPlace)
                dest = image;
            else
                dest = (ComplexF[,])image.Clone();

            using (var uDest = dest.Lock())
            {
                FourierTransform.FFT2((ComplexF*)uDest.ImageData, uDest.Width, uDest.Height, uDest.Stride, direction);
            }

            return dest;
        }

        /// <summary>
        /// Converts the grayscale image to a complex image where the specified image is taken as a real part.
        /// </summary>
        /// <param name="image">Real part of a complex image.</param>
        /// <returns>Complex image.</returns>
        public static ComplexF[,] ToComplex(this Gray<float>[,] image)
        {
            return image.Convert<Gray<float>, ComplexF>(convertGrayToComplex);
        }

        private static void convertGrayToComplex(ref Gray<float> source, ref ComplexF destination)
        {
            destination.Re = source.Intensity;
        }

        /// <summary>
        /// Multiplies two complex images element-wise.
        /// </summary>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="inPlace">
        /// True to save the result in the first image, false otherwise.
        /// <para>If true the returned image is the source image.</para>
        /// </param>
        /// <returns>Multiplied image.</returns>
        public static ComplexF[,] MulComplex(this ComplexF[,] imageA, ComplexF[,] imageB, bool inPlace = false)
        {
            return imageA.Calculate(imageB, mulComplex, inPlace);
        }

        private static void mulComplex(ref ComplexF sourceA, ref ComplexF sourceB, ref ComplexF destination)
        {
            destination.Re = sourceA.Re * sourceB.Re;
            destination.Im = sourceA.Im * sourceB.Im;
        }

        /// <summary>
        /// Calculates magnitude of the specified complex image.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="area">The specified working area.</param>
        /// <returns>Magnitude image.</returns>
        public static Gray<float>[,] Magnitude(this ComplexF[,] image, Rectangle area)
        {
            return image.Convert<ComplexF, Gray<float>>(convertComplexToMagnitude, area);
        }

        private static void convertComplexToMagnitude(ref ComplexF source, ref Gray<float> destination)
        {
            destination.Intensity = source.Magnitude();
        }

        /// <summary>
        /// Replaces real channel with the specified one.
        /// </summary>
        /// <param name="source">Source image.</param>
        /// <param name="value">The matrix which replaces the real channel of a source.</param>
        /// <param name="sourceArea">Source working area.</param>
        public static void ReplaceRe(this ComplexF[,] source, float[,] value, Rectangle sourceArea)
        {
            if (source.Width() > sourceArea.Right ||
               source.Height() > sourceArea.Bottom ||
               sourceArea.X < 0 ||
               sourceArea.Y < 0)
            {
                throw new ArgumentException("Source area must fit within the source image.");
            }

            if (value.Width() != sourceArea.Width ||
               value.Height() != sourceArea.Height)
            {
                throw new ArgumentException("Value size must be the same as source area size.");
            }

            ParallelLauncher.Launch(thread =>
            {
                source[thread.Y + sourceArea.Y, thread.X + sourceArea.X].Re = value[thread.Y, thread.X];
            },
            sourceArea.Width, sourceArea.Height);
        }


    }
}
