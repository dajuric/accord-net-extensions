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

using System;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Flip image direction. 
    /// They can be used as bit flags.
    /// </summary>
    [Flags]
    public enum FlipDirection
    {
        /// <summary>
        /// No flipping.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Horizontal flipping.
        /// </summary>
        Horizontal = 0x1,
        /// <summary>
        /// Vertical flipping
        /// </summary>
        Vertical = 0x2,
        /// <summary>
        /// All flipping (horizontal + vertical).
        /// </summary>
        All = 0x3
    }

    /// <summary>
    /// Contains extension methods for image flipping.
    /// </summary>
    public static class ImageFlipping
    {
        /// <summary>
        /// Flips an input image horizontally / vertically / both directions / or none (data copy).
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="source">Input image.</param>
        /// <param name="flipDirection">Flip direction.</param>
        /// <returns>Returns flipped image.</returns>
        public static TColor[,] FlipImage<TColor>(this TColor[,] source, FlipDirection flipDirection)
            where TColor: struct
        {
            TColor[,] dest = source.CopyBlank();
            var sourceArea = new Rectangle(0, 0, source.Width(), source.Height());
            var destinationOffset = new Point();
            source.FlipImage(sourceArea, dest, destinationOffset, flipDirection);

            return dest;
        }

        /// <summary>
        /// Flips an input image horizontally / vertically / both directions / or none (data copy).
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="source">Input image.</param>
        /// <param name="sourceArea">Source area.</param>
        /// <param name="destination">Destination image.</param>
        /// <param name="destinationOffset">Destination image offset.</param>
        /// <param name="flipDirection">Flip direction.</param>
        public static void FlipImage<TColor>(this TColor[,] source, Rectangle sourceArea, TColor[,] destination, Point destinationOffset, FlipDirection flipDirection)
        {
            int startDstRow = 0; int vDirection = 1;
            int startDstCol = 0; int hDirection = 1;

            if ((flipDirection & FlipDirection.Vertical) != 0)
            {
                startDstRow = (destinationOffset.Y + sourceArea.Height) - 1; vDirection = -1;
            }
            if ((flipDirection & FlipDirection.Horizontal) != 0)
            {
                startDstCol = (destinationOffset.X + sourceArea.Width) - 1; hDirection = -1;
            }

            for (int srcRow = 0, dstRow = startDstRow; srcRow < sourceArea.Bottom; srcRow++, dstRow += vDirection)
            {
                for (int srcCol = 0, dstCol = startDstCol; srcCol < sourceArea.Right; srcCol++, dstCol += hDirection)
                {
                    destination[dstRow, dstCol] = source[srcRow, srcCol];
                }
            }
        }
    }
}
