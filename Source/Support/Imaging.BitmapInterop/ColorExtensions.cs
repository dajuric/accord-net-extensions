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
using System.Drawing;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains color format conversion extensions.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Gets System.Drawing.Color from Bgr8 color.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="opacity">Opacity. If color has 4 channels opacity is discarded.</param>
        /// <returns>System.Drawing.Color</returns>
        public static System.Drawing.Color ToColor(this Gray<byte> color, byte opacity = Byte.MaxValue)
        {
            return Color.FromArgb(opacity, color.Intensity, color.Intensity, color.Intensity);
        }

        /// <summary>
        /// Gets System.Drawing.Color from Bgr8 color.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="opacity">Opacity. If color has 4 channels opacity is discarded.</param>
        /// <returns>System.Drawing.Color</returns>
        public static System.Drawing.Color ToColor(this Bgr<byte> color, byte opacity = Byte.MaxValue)
        {
            return Color.FromArgb(opacity, color.R, color.G, color.B);
        }

        /// <summary>
        /// Gets System.Drawing.Color from Bgra8 color.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <returns>System.Drawing.Color</returns>
        public static System.Drawing.Color ToColor(this Bgra<byte> color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts (casts) the color into 32-bit BGR color.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <returns>Bgr representation.</returns>
        public static Bgr<byte> ToBgr(this System.Drawing.Color color)
        {
            return new Bgr<byte> { B = color.B, G = color.G, R = color.R };
        }
    }
}
