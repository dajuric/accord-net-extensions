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
using System.Linq;
using MoreLinq;
using System.Collections.Generic;
using System.Diagnostics;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    public unsafe partial class DenseHistogram
    {
        /// <summary>
        /// Calculates histogram.
        /// </summary>
        /// <param name="channel">Image channel.</param>
        /// <param name="accumulate">Accumulate or erase histogram before.</param>
        /// <param name="mask">Mask for image color locations.</param>
        /// <param name="maskOffset">The location offset for the mask. The mask area will be [offsetX, offsetY, channelWidth, channelHeight].</param>
        public void Calculate(Gray<byte>[,] channel, bool accumulate, Gray<byte>[,] mask, Point maskOffset)
        {
            Calculate(new Gray<byte>[][,]{ channel }, accumulate, mask, maskOffset);
        }

        /// <summary>
        /// Calculates histogram.
        /// </summary>
        /// <param name="channels">Image channels.</param>
        /// <param name="accumulate">Accumulate or erase histogram before.</param>
        /// <param name="mask">Mask for image color locations.</param>
        /// <param name="maskOffset">The location offset for the mask. The mask area will be [offsetX, offsetY, channelWidth, channelHeight].</param>
        public void Calculate(Gray<byte>[][,] channels, bool accumulate, Gray<byte>[,] mask, Point maskOffset)
        {
            if (!accumulate)
                Array.Clear(histogram, 0, this.NumberOfElements);

            if (mask == null)
            {
                mask = new Gray<byte>[channels[0].Width(), channels[0].Height()];
                mask.SetValue<Gray<byte>>(Byte.MaxValue);
            }

            var maskArea = new Rectangle(maskOffset, channels.First().Size());
            using (var uMask = mask.Lock(maskArea))
            {
                var uChannels = channels.Select(x => x.Lock()).ToArray();
                calculateHistByte(this, uChannels, uMask);
                uChannels.ForEach(x => x.Dispose());
            }
        }

        #region Calculate histogram Byte

        private static unsafe void calculateHistByte(DenseHistogram hist, IImage[] channels, Image<Gray<byte>> mask)
        {
            /******************************* prepare data *****************************/
            float* bins = (float*)hist.HistogramData;
            float[][] valueToIndexMultipliers = hist.ValueToIndexMultipliers;
            int[] strides = hist.Strides;

            int width = channels[0].Width;
            int height = channels[0].Height;
            int channelStride = channels[0].Stride;
            int numDimensions = channels.Length;

            byte*[] channelPtrs = new byte*[channels.Length];
            for (int i = 0; i < channelPtrs.Length; i++)
            {
                channelPtrs[i] = (byte*)channels[i].ImageData;
            }

            byte* maskPtr = (byte*)mask.ImageData;
            int maskStride = mask.Stride;
            /******************************* prepare data *****************************/

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (maskPtr[col] == 0)
                        continue;

                    float* binsTemp = bins;
                    for (int d = 0; d < numDimensions; d++)
                    {
                        var idxDecimal = channelPtrs[d][col] * valueToIndexMultipliers[d][0] + valueToIndexMultipliers[d][1];
                        int idx = (int)(idxDecimal);

                        binsTemp += idx * strides[d];

                        //index checking
                        Debug.Assert(idx >= 0 && idx < hist.binSizes[d], string.Format("Calculated index does not match specified bin size. Dimension = {0}", d));
                    }

                    binsTemp[0]++;
                }

                for (int d = 0; d < numDimensions; d++)
                    channelPtrs[d] += channelStride;

                maskPtr += maskStride;
            }
        }

        #endregion
    }
}
