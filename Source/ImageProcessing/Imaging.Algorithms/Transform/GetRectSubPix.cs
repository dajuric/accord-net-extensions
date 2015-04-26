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
using PointF = AForge.Point;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for calculating warped image portion.
    /// </summary>
    public static class GetRectSubPixExtensions
    {
        #region Color type extensions

        /// <summary>
        /// Gets specified image portion. 
        /// If the coordinates are not the rounded, they will be interpolated.
        /// </summary>
        /// <param name="source">Image.</param>
        /// <param name="area">Requested area.</param>
        /// <returns>Interpolated image area.</returns>
        public static Gray<float>[,] GetRectSubPix(this Gray<float>[,] source, RectangleF area)
        {
            var destination = new Gray<float>[(int)area.Height, (int)area.Width];

            using (var srcImg = source.Lock())
            using (var dstImg = destination.Lock())
            {
                getRectSubPix_Float(srcImg, area.Location, dstImg);
            }

            return destination;
        }

        /// <summary>
        /// Gets specified image portion. 
        /// If the coordinates are not the rounded, they will be interpolated.
        /// </summary>
        /// <typeparam name="TColor">Image color type.</typeparam>
        /// <param name="source">Image.</param>
        /// <param name="area">Requested area.</param>
        /// <returns>Interpolated image area.</returns>
        public static TColor[,] GetRectSubPix<TColor>(this TColor[,] source, RectangleF area)
                  where TColor : struct, IColor<float>
        {
            var destination = new TColor[(int)area.Height, (int)area.Width];

            using(var srcImg = source.Lock())
            using (var dstImg = destination.Lock())
            {
                getRectSubPix_Float(srcImg, area.Location, dstImg);
            }

            return destination;
        }

        /// <summary>
        /// Gets specified image portion. 
        /// If the coordinates are not the rounded, they will be interpolated.
        /// </summary>
        /// <typeparam name="TColor">Image color type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="area">Requested area.</param>
        /// <returns>Interpolated image area.</returns>
        public static TColor[,] GetRectSubPix<TColor>(this Image<TColor> img, RectangleF area)
            where TColor : struct, IColor<float>
        {
            var destination = new TColor[(int)area.Height, (int)area.Width];

            using (var dstImg = destination.Lock())
            {
                getRectSubPix_Float(img, area.Location, dstImg);
            }
            
            return destination;
        }
        #endregion

        private unsafe static void getRectSubPix_Float<TColor>(Image<TColor> img, PointF startPt, Image<TColor> destImg)
            where TColor: struct
        {
            int xt = (int)startPt.X;
            int yt = (int)startPt.Y;
            float ax = startPt.X - xt;
            float ay = startPt.Y - yt;

            float bx = 1.0f - ax;
            float by = 1.0f - ay;

            float a0 = bx * by;
            float a1 = ax * by;
            float a2 = ax * ay;
            float a3 = bx * ay;

            int width = destImg.Width;
            int height = destImg.Height;

            if (xt + width > img.Width || yt + height > img.Height)
                throw new Exception("Requested region is out of bounds!");

            if (xt + width == img.Width)
            {
                width--;
                //TODO: handle right border
            }

            if (yt + height == img.Height)
            {
                height--;
                //TODO: handle bottom border
            }

            int nChannels = img.ColorInfo.ChannelCount;

            int srcStride = img.Stride;
            int dstStride = destImg.Stride;
            int srcOffset = img.Stride - width * img.ColorInfo.Size;
            int dstOffset = destImg.Stride - width * destImg.ColorInfo.Size;

            for (int ch = 0; ch < nChannels; ch++)
            {
                float* imgPtr = (float*)img.GetData(yt, xt) + ch;
                float* dstPtr = (float*)destImg.ImageData + ch;

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        float* nextRowPtr = (float*)((byte*)imgPtr + srcStride);

                        float x0y0 = *imgPtr;
                        float x1y0 = *(imgPtr + nChannels);
                        float x0y1 = *nextRowPtr;
                        float x1y1 = *(nextRowPtr + nChannels);

                        *dstPtr = a0 * x0y0 + a1 * x1y0 + a2 * x1y1 + a3 * x0y1;

                        imgPtr += nChannels;
                        dstPtr += nChannels;
                    }

                    imgPtr = (float*)((byte*)imgPtr + srcOffset);
                    dstPtr = (float*)((byte*)dstPtr + dstOffset);
                }
            }
        }
    }
}
