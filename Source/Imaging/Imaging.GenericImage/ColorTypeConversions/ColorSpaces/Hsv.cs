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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents Hsv color type of type <see cref="System.Double"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "HSV")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv : IColor
    {
        /// <summary>
        /// Creates new Hsv color.
        /// </summary>
        /// <param name="hue">Hue</param>
        /// <param name="saturation">Saturation</param>
        /// <param name="value">Value.</param>
        public Hsv(double hue, double saturation, double value)
        {
            this.H = hue;
            this.S = saturation;
            this.V = value;
        }

        /// <summary>
        /// Gets or sets hue.
        /// </summary>
        public double H;
        /// <summary>
        /// Gets or sets saturation.
        /// </summary>
        public double S;
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public double V;

        /// <summary>
        /// Gets the string color representation.
        /// </summary>
        /// <returns>String color representation.</returns>
        public override string ToString()
        {
            return string.Format("H: {0}, S: {1}, V: {2}", H, S, V);
        }

        /// <summary>
        /// Gets the index of the hue component.
        /// </summary>
        public const int IdxH = 0;
        /// <summary>
        /// Gets the index of the saturation component.
        /// </summary>
        public const int IdxS = 1;
        /// <summary>
        /// Gets the index of the value component.
        /// </summary>
        public const int IdxV = 2;
    }

    /// <summary>
    /// Represents Hsv color type of type <see cref="System.Byte"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv8
    {
        /// <summary>
        /// Gets or sets hue.
        /// </summary>
        public byte H;
        /// <summary>
        /// Gets or sets saturation.
        /// </summary>
        public byte S;
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public byte V;

        /// <summary>
        /// Converts 8-bit Hsv color to the 8-bit Bgr color.
        /// </summary>
        /// <param name="hsv">Source color.</param>
        /// <param name="bgr">Destination color.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void ConvertHsvToBgr(Hsv8* hsv, Bgr8* bgr)
        {
            if (hsv->S == 0)
            {
                bgr->R = hsv->V;
                bgr->G = hsv->V;
                bgr->B = hsv->V;
                return;
            }

            int hue = hsv->H * 2; //move to [0-360 range] (only needed for byte!)

            int hQuadrant = hue / 60; // Hue quadrant 0 - 5 (60deg)
            int hOffset = hue % 60; // Hue position in quadrant
            int vs = hsv->V * hsv->S;

            byte p = (byte)(hsv->V - (vs / 255));
            byte q = (byte)(hsv->V - (vs / 255 * hOffset) / 60);
            byte t = (byte)(hsv->V - (vs / 255 * (60 - hOffset)) / 60);

            switch (hQuadrant)
            {
                case 0:
                    bgr->R = hsv->V; bgr->G = t; bgr->B = p;
                    break;
                case 1:
                    bgr->R = q; bgr->G = hsv->V; bgr->B = p;
                    break;
                case 2:
                    bgr->R = p; bgr->G = hsv->V; bgr->B = t;
                    break;
                case 3:
                    bgr->R = p; bgr->G = q; bgr->B = hsv->V;
                    break;
                case 4:
                    bgr->R = t; bgr->G = p; bgr->B = hsv->V;
                    break;
                default:
                    bgr->R = hsv->V; bgr->G = p; bgr->B = q;
                    break;
            }
        }

        /// <summary>
        /// Converts 8-bit Hsv color to the 8-bit Bgr color.
        /// </summary>
        public unsafe Bgr8 ToBgr()
        {
            Hsv8 hsv = this;  Bgr8 bgr;
            ConvertHsvToBgr(&hsv, &bgr);
            return bgr;
        }
    }

    /// <summary>
    /// Represents Hsv color type of type <see cref="System.Int16"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv16
    {
        /// <summary>
        /// Gets or sets hue.
        /// </summary>
        public short H;
        /// <summary>
        /// Gets or sets saturation.
        /// </summary>
        public short S;
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public short V;
    }

    /// <summary>
    /// Represents Hsv color type of type <see cref="System.Int32"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv32
    {
        /// <summary>
        /// Gets or sets hue.
        /// </summary>
        public int H;
        /// <summary>
        /// Gets or sets saturation.
        /// </summary>
        public int S;
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public int V;
    }

    /// <summary>
    /// Represents Hsv color type of type <see cref="System.Single"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv32f
    {
        /// <summary>
        /// Gets or sets hue.
        /// </summary>
        public float H;
        /// <summary>
        /// Gets or sets saturation.
        /// </summary>
        public float S;
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public float V;
    }
}
