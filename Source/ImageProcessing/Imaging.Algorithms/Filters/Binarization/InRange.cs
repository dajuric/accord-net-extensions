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
using System.Linq;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for pixel in range checking.
    /// </summary>
    public static class InRangeFilter
    {
        /// <summary>
        /// Checks and produces mask where values != 0 means that values on those indicies are in the range.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="min">Minimal value.</param>
        /// <param name="max">Maximal value.</param>
        /// <param name="valueToSet">Value to set to result mask.</param>
        /// <param name="channelIndicies">Which channel indicies to check. If not used then it is assumed that all indicies are used.</param>
        /// <returns>Mask</returns>
        public static Gray<byte>[,] InRange<TColor>(this TColor[,] img, TColor min, TColor max, byte valueToSet = 255, params int[] channelIndicies)
            where TColor : struct, IColor<byte>
        {
            var minArr = min.ColorToArray<TColor, byte>();
            var maxArr = max.ColorToArray<TColor, byte>();

            if (channelIndicies == null || channelIndicies.Length == 0)
                channelIndicies = Enumerable.Range(0, img.ColorInfo().ChannelCount).ToArray();

            if (channelIndicies.Length > img.ColorInfo().ChannelCount)
                throw new Exception("Number of processed channels must not exceed the number of available image channels!");

            var destMask = new Gray<byte>[img.Height(), img.Width()];
            destMask.SetValue<Gray<byte>>(Byte.MaxValue);

            using (var uImg = img.Lock())
            using(var uDestMask = destMask.Lock())
            {
                inRangeByte(uImg, minArr, maxArr, channelIndicies, uDestMask, valueToSet);
            }

            return destMask;
        }

        private unsafe static void inRangeByte(IImage src, Array min, Array max, int[] channelIndicies, Image<Gray<byte>> dest, byte valueToSet)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int width = src.Width;
            int height = src.Height;

            int srcShift = src.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * 1 * sizeof(byte);

            foreach (var channel in channelIndicies)
            {
                byte minVal = (byte)min.GetValue(channel);
                byte maxVal = (byte)max.GetValue(channel);

                byte* srcPtr = (byte*)src.ImageData + channel;
                byte* destPtr = (byte*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        byte srcVal = *srcPtr;
                        bool prevVal = *destPtr != 0;

                        bool val = (srcVal >= minVal) && (srcVal <= maxVal) && prevVal;
                        *destPtr = (byte)(*((byte*)(&val)) * valueToSet); //set value if true

                        srcPtr += nChannels;
                        destPtr++;
                    }

                    srcPtr = (byte*)((byte*)srcPtr + srcShift);
                    destPtr += destShift;
                }
            }
        }
    }
}
