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
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="minValue">Minimal value in range.</param>
        /// <param name="maxValue">Maximum value in range.</param>
        /// <param name="valueToSet">Value to set after threshold is applied.</param>
        /// <returns> 
        /// Binary mask for which values !=0 are where source values are in specified range.
        /// To get values use Copy extension.
        /// </returns>
        public static Image<TColor, TDepth> ThresholdBinary<TColor, TDepth>(this Image<TColor, TDepth> img, TColor minValue, TColor maxValue, TColor valueToSet)
            where TColor : IColor
            where TDepth : struct
        {
            Image<TColor, TDepth> valueMask = new Image<TColor, TDepth>(img.Width, img.Height, valueToSet);

            var mask = img.InRange(minValue, maxValue);

            var result = img.CopyBlank(); //TODO - critical-mmedium: solve this to extend SetValue extension with mask paremeter (faster and less memory hungry)
            valueMask.CopyTo(result, mask);

            return result;
        }

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
        public static Image<TColor, TDepth> ThresholdToZero<TColor, TDepth>(this Image<TColor, TDepth> img, TColor minValue, TColor maxValue)
            where TColor : IColor
            where TDepth : struct
        {
            Image<TColor, TDepth> result = new Image<TColor, TDepth>(img.Width, img.Height);
      
            var mask = img.InRange(minValue, maxValue);
            img.CopyTo(result, mask); 

            return result;
        }
    }

}
