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
    /// Generic color space (2 channels).
    /// </summary>
    [ColorInfo(ConversionCodename = "Generic", IsGenericColorSpace = true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color2 : IColor, IColor2
    {
        /// <summary>
        /// Creates new 2-channel generic color.
        /// </summary>
        /// <param name="val0">First channel.</param>
        /// <param name="val1">Second channel.</param>
        public Color2(double val0, double val1)
        {
            this.Val0 = val0;
            this.Val1 = val1;
        }

        /// <summary>
        /// Gets or sets first channel value.
        /// </summary>
        public double Val0;
        /// <summary>
        /// Gets or sets second channel value.
        /// </summary>
        public double Val1;
    }

    /// <summary>
    /// Generic color space (3 channels).
    /// </summary>
    [ColorInfo(ConversionCodename = "Generic", IsGenericColorSpace = true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color3: IColor, IColor3
    {
        /// <summary>
        /// Creates new 3-channel generic color.
        /// </summary>
        /// <param name="val0">First channel.</param>
        /// <param name="val1">Second channel.</param>
        /// <param name="val2">Third channel.</param>
        public Color3(double val0, double val1, double val2)
        {
            this.Val0 = val0;
            this.Val1 = val1;
            this.Val2 = val2;
        }

        /// <summary>
        /// Gets or sets first channel value.
        /// </summary>
        public double Val0;
        /// <summary>
        /// Gets or sets second channel value.
        /// </summary>
        public double Val1;
        /// <summary>
        /// Gets or sets third channel value.
        /// </summary>
        public double Val2;
    }

    /// <summary>
    /// Generic color space (4 channels).
    /// </summary>
    [ColorInfo(ConversionCodename = "Generic", IsGenericColorSpace = true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color4 : IColor, IColor4
    {
        /// <summary>
        /// Creates new 4-channel generic color.
        /// </summary>
        /// <param name="val0">First channel.</param>
        /// <param name="val1">Second channel.</param>
        /// <param name="val2">Third channel.</param>
        /// <param name="val3">Fourth channel.</param>
        public Color4(double val0, double val1, double val2, double val3)
        {
            this.Val0 = val0;
            this.Val1 = val1;
            this.Val2 = val2;
            this.Val3 = val3;
        }

        /// <summary>
        /// Gets or sets first channel value.
        /// </summary>
        public double Val0;
        /// <summary>
        /// Gets or sets second channel value.
        /// </summary>
        public double Val1;
        /// <summary>
        /// Gets or sets third channel value.
        /// </summary>
        public double Val2;
        /// <summary>
        /// Gets or sets fourth channel value.
        /// </summary>
        public double Val3;
    }

}
