#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
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

using AForge.Imaging.Filters;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for contrast correction.
    /// </summary>
    public static class CorrectContrastExtensions
    {
        /// <summary>
        /// Adjusts pixels' contrast value by increasing RGB values of bright pixel and decreasing
        /// pixel values of dark pixels (or vise versa if contrast needs to be decreased).
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="factor">Factor which is used to adjust contrast. Factor values greater than
        /// 0 increase contrast making light areas lighter and dark areas darker. Factor values
        /// less than 0 decrease contrast - decreasing variety of contrast.</param>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        public static Image<Gray, byte> CorrectContrast(this Image<Gray, byte> im, int factor = 10, bool inPlace = false)
        {
            return CorrectContrast<Gray, byte>(im, factor, inPlace);
        }

        /// <summary>
        /// Adjusts pixels' contrast value by increasing RGB values of bright pixel and decreasing
        /// pixel values of dark pixels (or vise versa if contrast needs to be decreased).
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="factor">Factor which is used to adjust contrast. Factor values greater than
        /// 0 increase contrast making light areas lighter and dark areas darker. Factor values
        /// less than 0 decrease contrast - decreasing variety of contrast.</param>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        public static Image<TColor, byte> CorrectContrast<TColor>(this Image<TColor, byte> im, int factor = 10, bool inPlace = false)
            where TColor : IColor3
        {
            return CorrectContrast<TColor, byte>(im, factor, inPlace);
        }

        /// <summary>
        /// Adjusts pixels' contrast value by increasing RGB values of bright pixel and decreasing
        /// pixel values of dark pixels (or vise versa if contrast needs to be decreased).
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="factor">Factor which is used to adjust contrast. Factor values greater than
        /// 0 increase contrast making light areas lighter and dark areas darker. Factor values
        /// less than 0 decrease contrast - decreasing variety of contrast.</param>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        private static Image<TColor, TDepth> CorrectContrast<TColor, TDepth>(this Image<TColor, TDepth> im, int factor = 10, bool inPlace = false)
            where TColor : IColor
            where TDepth : struct
        {
            ContrastCorrection conrastCorrection = new ContrastCorrection(factor);
            return im.ApplyFilter(conrastCorrection, inPlace);
        }
    }
}
