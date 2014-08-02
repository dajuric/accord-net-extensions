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
    /// Contains extension methods for contrast stretching.
    /// </summary>
    public static class ContrastStrechExtensions
    {
        /// <summary>
        /// Stretches intensity values in a linear way across full pixel range.
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        public static Image<Gray, byte> StretchContrast(this Image<Gray, byte> im, bool inPlace = false)
        {
            return StretchContrast<Gray, byte>(im, inPlace);
        }

        /// <summary>
        /// Stretches intensity values in a linear way across full pixel range.
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        public static Image<TColor, byte> StretchContrast<TColor>(this Image<TColor, byte> im, bool inPlace = false)
            where TColor: IColor3
        {
            return StretchContrast<TColor, byte>(im, inPlace);
        }

        private static Image<TColor, TDepth> StretchContrast<TColor, TDepth>(this Image<TColor, TDepth> im, bool inPlace = false)
            where TColor : IColor
            where TDepth : struct
        {
            ContrastStretch conrastStrech = new ContrastStretch();
            return im.ApplyFilter(conrastStrech, inPlace);
        }
    }
}
