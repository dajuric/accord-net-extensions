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

using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents gray color of type <see cref="System.Double"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "Gray", IsGenericColorSpace = false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray : IColor
    {
        /// <summary>
        /// Creates new gray color.
        /// </summary>
        /// <param name="intensity">Intensity.</param>
        public Gray(double intensity)
        {
            this.Intensity = intensity;
        }

        /// <summary>
        /// Gets or sets the intensity.
        /// </summary>
        public double Intensity;

        /// <summary>
        /// Converts gray structure to <see cref="System.Double"/> value.
        /// </summary>
        /// <param name="gray">Gray color.</param>
        /// <returns>Intensity.</returns>
        public static implicit operator double(Gray gray)
        {
            return gray.Intensity;
        }

        /// <summary>
        /// Converts intensity of type <see cref="System.Double"/> to Gray color.
        /// </summary>
        /// <param name="intensity">Intensity.</param>
        /// <returns>Gray color.</returns>
        public static implicit operator Gray(double intensity)
        {
            return new Gray(intensity);
        }

        /// <summary>
        /// Gets the string color representation.
        /// </summary>
        /// <returns>String color representation.</returns>
        public override string ToString()
        {
            return string.Format("{0}", Intensity);
        }
    }
}
