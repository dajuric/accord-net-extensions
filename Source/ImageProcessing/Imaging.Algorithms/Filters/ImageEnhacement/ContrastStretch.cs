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
        public static Gray<byte>[,] StretchContrast(this Gray<byte>[,] im, bool inPlace = false)
        {
            ContrastStretch conrastStrech = new ContrastStretch();
            return im.ApplyFilter(conrastStrech, inPlace);
        }

        /// <summary>
        /// Stretches intensity values in a linear way across full pixel range.
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        public static TColor[,] StretchContrast<TColor>(this TColor[,] im, bool inPlace = false)
            where TColor: struct, IColor3<byte>
        {
            ContrastStretch conrastStrech = new ContrastStretch();
            return im.ApplyFilter(conrastStrech, inPlace);
        }
    }
}
