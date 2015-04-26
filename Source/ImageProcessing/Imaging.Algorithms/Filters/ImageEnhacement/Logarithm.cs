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

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Logarithm filter.
    /// </summary>
    public static class LogarithmExtensions
    {
        /// <summary>
        /// Simple log image filter. Applies the <see cref="System.Math.Log(double)"/>
        /// function for each pixel in the image, clipping values as needed.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.Logarithm"/> for details.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static TColor[,] Logarithm<TColor>(this TColor[,] img, bool inPlace = false)
            where TColor : struct, IColor
        {
            Logarithm l = new Logarithm();
            return img.ApplyFilter(l, inPlace);
        }
    }
}
