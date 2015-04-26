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

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for binary thresholding.
    /// </summary>
    public static class BinaryThreshold
    {
        /// <summary>
        /// Applies binary threshold to a input image.
        /// <para>
        /// Pixels which are not in [min..max] range are set to zero.
        /// </para>
        /// </summary>
        /// <param name="img">Image.</param> 
        /// <param name="minValue">Minimal value in range.</param>
        /// <param name="maxValue">Maximum value in range.</param>
        /// <returns>Thresholded image where pixels which are not in [min..max] range are set to zero.</returns>
        public static TColor[,] ThresholdToZero<TColor>(this TColor[,] img, TColor minValue, TColor maxValue)
            where TColor : struct, IColor<byte>
        {
            var result = img.CopyBlank();
      
            var mask = img.InRange(minValue, maxValue);
            img.CopyTo(result, mask); 

            return result;
        }
    }

}
