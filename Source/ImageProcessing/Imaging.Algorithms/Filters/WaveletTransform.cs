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
        public static Gray<byte>[,] WaveletTransform(this Gray<byte>[,] img, IWavelet wavelet, bool backward = false)
        {
            WaveletTransform wt = new WaveletTransform(wavelet, backward);
            return img.ApplyFilter((BaseFilter)wt);
        }
    }
}
