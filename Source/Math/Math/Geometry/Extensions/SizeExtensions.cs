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

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides size extension methods.
    /// </summary>
    public static class SizeExtensions
    {
        /// <summary>
        /// Gets the size area.
        /// </summary>
        /// <param name="size">Size.</param>
        /// <returns>Area.</returns>
        public static int Area(this Size size)
        {
            return size.Width * size.Height;
        }
    }

    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides size extension methods.
    /// </summary>
    public static class SizeFExtensions
    {
        /// <summary>
        /// Gets the size area.
        /// </summary>
        /// <param name="size">Size.</param>
        /// <returns>Area.</returns>
        public static float Area(this SizeF size)
        {
            return size.Width * size.Height;
        }
    }
}
