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

namespace Accord.Extensions.Imaging
{
    public unsafe partial class DenseHistogram
    {
        /// <summary>
        /// Back-projects (creates probability map) from histogram values.
        /// </summary>
        /// <param name="srcs">Image channels.</param>
        /// <returns>Back-projection image (probability image) </returns>
        public Gray<byte>[,] BackProject(Gray<byte>[][,] srcs)
        {
            var destImg = srcs.First().CopyBlank();

            using (var uDestImg = destImg.Lock())
            {
                var uChannels = srcs.Select(x => x.Lock()).ToArray();
                backProjectByte(this, uChannels, uDestImg);
                uChannels.ForEach(x => x.Dispose());
            }

            return destImg;
        }

        #region Backproject Byte

        private static void backProjectByte(DenseHistogram hist, IImage[] channels, IImage projImg)
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

            byte* projImgPtr = (byte*)projImg.ImageData;
            int projImgStride = projImg.Stride;
            /******************************* prepare data *****************************/

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    float* binsTemp = bins;
                    for (int d = 0; d < numDimensions; d++)
                    {
                        var idxDecimal = channelPtrs[d][col] * valueToIndexMultipliers[d][0] + valueToIndexMultipliers[d][1];
                        int idx = (int)(idxDecimal);

                        binsTemp += idx * strides[d];
                    }

                    projImgPtr[col] = (byte)binsTemp[0];
                }

                for (int d = 0; d < numDimensions; d++)
                    channelPtrs[d] += channelStride;

                projImgPtr += projImgStride;
            }
        }

        #endregion

    }
}
