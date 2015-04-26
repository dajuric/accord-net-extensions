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
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for logic NOT operation applicable on an image.
    /// </summary>
    public static class ArithmeticLogicNotByteExtensions
    {
        #region Byte
        private unsafe static void not_Byte(IImage src, IImage dest)
        {
            byte* srcPtr = (byte*)src.ImageData;
            byte* destPtr = (byte*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int srcShift = src.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * nChannels * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(~(*srcPtr));

                        srcPtr++;
                        destPtr++;
                    }
                }

                srcPtr = (byte*)((byte*)srcPtr + srcShift);
                destPtr = (byte*)((byte*)destPtr + destShift);
            }
        }
        #endregion

        /// <summary>
        /// Performs bitwise NOT operation on image.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="inPlace">Perform this operation on original image or not.</param>
        /// <returns>Processed image. If <paramref name="inPlace"/> is set to true returned value can be discarded.</returns>
        public static TColor[,] Not<TColor>(this TColor[,] img, bool inPlace = false)
          where TColor : struct, IColor<byte>
        {
            TColor[,] dest = img;
            if (!inPlace)
                dest = img.CopyBlank();

            using (var uImg = img.Lock())
            using (var uDest = dest.Lock())
            {
                not_Byte(uImg, uDest);
            }

            return dest;
        }
    }
}
