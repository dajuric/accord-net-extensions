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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Non-maxima suppression extensions.
    /// </summary>
    public static class NonMaximaSupressionExtensions
    {
        delegate void SupressNonMaximaFunc(IImage src, IImage dest, int radius, int discardVal);
        static Dictionary<Type, SupressNonMaximaFunc> supressNonMaximaFuncs;

        static NonMaximaSupressionExtensions()
        {
            supressNonMaximaFuncs = new Dictionary<Type, SupressNonMaximaFunc>();

            supressNonMaximaFuncs.Add(typeof(float), supressNonMaxima_Float);
        }

        /// <summary>
        /// Does non-maxima supression for the following gray image. Can be useful for detections filtering (e.g. post-processing output from Harris detector).
        /// </summary>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="radius">Non-maxima supression radius (kernel radius).</param>
        /// <param name="discardValue">The value will be discarded (0 - for black).</param>
        /// <returns>Processed image.</returns>
        public static Image<Gray, TDepth> SupressNonMaxima<TDepth>(this Image<Gray, TDepth> img, int radius = 3, int discardValue = 0)
           where TDepth : struct
        {
            var dest = new Image<Gray, TDepth>(img.Size);
            SupressNonMaxima(img, dest, radius);

            return dest;
        }

        /// <summary>
        /// Does non-maxima supression for the following gray image. Can be useful for detections filtering (e.g. post-processing output from Harris detector).
        /// </summary>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="dest">Destination image. Must have the same size as source image.</param>
        /// <param name="radius">Non-maxima supression radius (kernel radius).</param>
        /// <param name="discardValue">The value will be discarded (0 - for black).</param>
        public static void SupressNonMaxima<TDepth>(this Image<Gray, TDepth> img, Image<Gray, TDepth> dest, int radius = 3, int discardValue = 0)
            where TDepth:struct
        {
            SupressNonMaximaFunc supressNonMaximaFunc = null;
            if (supressNonMaximaFuncs.TryGetValue(img.ColorInfo.ChannelType, out supressNonMaximaFunc) == false)
                throw new NotSupportedException(string.Format("Can not perform non-maxima suppression on an image of type {0}", img.ColorInfo.ChannelType.Name));

            var proc = new ParallelProcessor<IImage, bool>(img.Size,
                                                          () => true,
                                                          (_src, _, area) =>
                                                          {
                                                              Rectangle srcArea = new Rectangle
                                                              {
                                                                  X = 0,
                                                                  Y = area.Y,
                                                                  Width = _src.Width,
                                                                  Height = area.Height + 2 * radius
                                                              };
                                                              srcArea.Intersect(new Rectangle(new Point(), img.Size));

                                                              supressNonMaximaFunc(img.GetSubRect(srcArea), dest.GetSubRect(srcArea), radius, discardValue);
                                                          },
                                                          new ParallelOptions2D {  /*ForceSequential = true*/ });

            proc.Process(img);
        }

        private unsafe static void supressNonMaxima_Float(IImage src, IImage dest, int radius, int discardVal)
        {
            float* srcPtr = (float*)src.ImageData;
            float* dstPtr = (float*)dest.ImageData;

            int width = src.Width;
            int height = src.Height;

            int srcStride = src.Stride;
            int dstStride = dest.Stride;

            for (int row = 0; row < (height - 2 * radius); row++)
            {
                for (int col = 0; col < (width - 2 * radius); col++)
                {
                    supressNonMaximaPatch_Float(&srcPtr[col], srcStride, 
                                                &dstPtr[col], dstStride, 
                                                radius, discardVal);
                }

                srcPtr = (float*)((byte*)srcPtr + srcStride);
                dstPtr = (float*)((byte*)dstPtr + dstStride);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static void supressNonMaximaPatch_Float(float* srcPtr, int srcStride, 
                                                               float* dstPtr, int dstStride, 
                                                               int radius, int discardVal)
        {
            float centerVal = *((float*)((byte*)srcPtr + radius * srcStride) + radius); //[x + radius, y + radius]
            if(centerVal == discardVal)
                return;
           
            for (int row = 0; row < 2 * radius; row++)
            {
                for (int col = 0; col < 2 * radius; col++)
                {
                    var srcVal = srcPtr[col];

                    if (srcVal > centerVal)
                    {
                       return;
                    }
                }

                srcPtr = (float*)((byte*)srcPtr + srcStride);
            }

            //if centerVal is max value...
            var dstCenterPtr = (float*)((byte*)dstPtr + radius * dstStride) + radius; //[x + radius, y + radius]
            *dstCenterPtr = centerVal;
        }
    }
}
