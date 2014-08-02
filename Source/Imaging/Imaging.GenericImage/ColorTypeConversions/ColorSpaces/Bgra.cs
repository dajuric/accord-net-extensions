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
    /// Represents Bgra color type of type <see cref="System.Double"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra: IColor, IColor4
    {
        /// <summary>
        /// Creates new Bgra color.
        /// </summary>
        /// <param name="b">Blue</param>
        /// <param name="g">Green</param>
        /// <param name="r">Red</param>
        /// <param name="a">Alpha (transparency).</param>
        public Bgra(double b, double g, double r, double a)
        {
            this.B = b;
            this.G = g;
            this.R = r;
            this.A = a;
        }

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public double B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public double G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public double R;
        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public double A;

        /// <summary>
        /// Gets the string color representation.
        /// </summary>
        /// <returns>String color representation.</returns>
        public override string ToString()
        {
            return string.Format("B: {0}, G: {1}, R: {2}, A: {3}", B, G, R, A);
        }

        /// <summary>
        /// Gets the index of the blue component.
        /// </summary>
        public const int IdxB = 0;
        /// <summary>
        /// Gets the index of the green component.
        /// </summary>
        public const int IdxG = 1;
        /// <summary>
        /// Gets the index of the red component.
        /// </summary>
        public const int IdxR = 2;
        /// <summary>
        /// Gets the index of the alpha component.
        /// </summary>
        public const int IdxA = 3;
    }

    /// <summary>
    /// Represents Bgra color type of type <see cref="System.Byte"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra8 
    {
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public byte B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public byte G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public byte R;
        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public byte A;
    }

    /// <summary>
    /// Represents Bgra color type of type <see cref="System.Int16"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra16
    {
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public short B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public short G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public short R;
        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public short A;
    }

    /// <summary>
    /// Represents Bgra color type of type <see cref="System.Int32"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra32
    {
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public int B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public int G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public int R;
        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public int A;
    }

    /// <summary>
    /// Represents Bgra color type of type <see cref="System.Single"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra32f
    {
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public float B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public float G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public float R;
        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public float A;
    }
}
