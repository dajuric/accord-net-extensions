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

namespace Accord.Extensions.Imaging
{
    public static partial class ComplexImageExtensions
    {
        /// <summary>
        /// Calculates Fast Fourier transform.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="direction">Forward or backward direction.</param>
        /// <param name="inPlace">Process in place or not.</param>
        /// <returns>Processed image. If <paramref name="inPlace"/> is used the result is the same as input image therefore may be omitted.</returns>
        public unsafe static Image<Complex, float> FFT(this Image<Complex, float> image, FourierTransform.Direction direction, bool inPlace = false)
        {
            return FFT<float>(image, direction, inPlace);
        }

        /// <summary>
        /// Calculates Fast Fourier transform.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="direction">Forward or backward direction.</param>
        /// <param name="inPlace">Process in place or not.</param>
        /// <returns>Processed image. If <paramref name="inPlace"/> is used the result is the same as input image therefore may be omitted.</returns>
        private unsafe static Image<Complex, double> FFT(this Image<Complex, double> image, FourierTransform.Direction direction, bool inPlace = false)
        {
            return FFT<double>(image, direction, inPlace);
        }

        internal unsafe static Image<Complex, TDepth> FFT<TDepth>(this Image<Complex, TDepth> image, FourierTransform.Direction direction, bool inPlace = false)
            where TDepth:struct
        {
            Image<Complex, TDepth> dest = null;
            if (inPlace)
                dest = image;
            else
                dest = image.Clone();

            if (typeof(TDepth).Equals(typeof(float)))
            {
                FourierTransform.FFT2((ComplexF*)dest.ImageData, dest.Width, dest.Height, dest.Stride, direction);
            }
            else if (typeof(TDepth).Equals(typeof(double)))
            {
                throw new NotImplementedException(); //TODO: implement for double
                //FourierTransform.FFT2((Complex*)dest.ImageData, dest.Width, dest.Height, dest.Stride, direction);
            }
            else
                throw new NotSupportedException();
            
            return dest;
        }
    }
}
