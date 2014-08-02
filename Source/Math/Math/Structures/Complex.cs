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

namespace Accord.Extensions.Math
{
    /// <summary>
    /// Represents complex number.
    /// </summary>
    [ColorInfo(ConversionCodename = "Complex")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Complex : IColor
    {
        /// <summary>
        /// Real part of the complex number.
        /// </summary>
        public double Re;

        /// <summary>
        /// Imaginary part of the complex number.
        /// </summary>
        public double Im;

        /// <summary>
        /// Creates a new instance of <see cref="Complex"/> structure.
        /// </summary>
        /// <param name="re">Real part.</param>
        /// <param name="im">Imaginary part.</param>
        public Complex(double re, double im)
        {
            this.Re = re;
            this.Im = im;
        }
    }
}
