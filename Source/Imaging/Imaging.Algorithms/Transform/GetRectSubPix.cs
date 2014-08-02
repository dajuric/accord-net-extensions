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
using PointF = AForge.Point;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for calculating warped image portion.
    /// </summary>
    public static class GetRectSubPixExtensions
    {
        delegate void InterpolateRectFunc(IImage image, PointF startPt, IImage destImage);
        static Dictionary<Type, InterpolateRectFunc> interpolateRectFuncs;

        static GetRectSubPixExtensions()
        {
            interpolateRectFuncs = new Dictionary<Type, InterpolateRectFunc>();
            interpolateRectFuncs.Add(typeof(short), getRectSubPix_Short);
            interpolateRectFuncs.Add(typeof(float), getRectSubPix_Float);
        }

        /*public static void GetRectSubPix<TColor>(this Image<TColor, short> img, PointF startPt, Image<TColor, float> destImg)
            where TColor : IColor
        {
            GetRectSubPix((IImage)img, startPt, destImg);
        }*/

        /// <summary>
        /// Gets specified image portion. 
        /// If the coordinates are not the rounded, they will be interpolated.
        /// </summary>
        /// <typeparam name="TColor">Image color type.</typeparam>
        /// <typeparam name="TDepth">Channel depth type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="area">Requested area.</param>
        /// <returns>Interpolated image area.</returns>
        public static Image<TColor, float> GetRectSubPix<TColor, TDepth>(this Image<TColor, TDepth> img, RectangleF area)
            where TColor : IColor
            where TDepth : struct
        {
            var destImg = new Image<TColor, float>((Size)area.Size);
            GetRectSubPix((IImage)img, area.Location, destImg);
            return destImg;
        }

        /// <summary>
        /// Gets specified image portion. 
        /// If the coordinates are not the rounded, they will be interpolated.
        /// </summary>
        /// <typeparam name="TColor">Image color type.</typeparam>
        /// <typeparam name="TDepth">Channel depth type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="startPt">Location of the area to interpolate.</param>
        /// <param name="destImg">Destination image. The size of the destination image will be used to determine area size.</param>
        public static void GetRectSubPix<TColor, TDepth>(this Image<TColor, TDepth> img, PointF startPt, Image<TColor, float> destImg)
            where TColor : IColor
            where TDepth : struct
        {
            GetRectSubPix((IImage)img, startPt, destImg);
        }

        private static void GetRectSubPix(IImage img, PointF startPt, IImage destImg)
        {
            if(destImg.ColorInfo.ChannelType != typeof(float))
                throw new Exception("Destination image channels type are not float.");

            Type depthType = img.ColorInfo.ChannelType;

            InterpolateRectFunc interpolateFunc = null;
            if (interpolateRectFuncs.TryGetValue(depthType, out interpolateFunc) == false)
            {
                throw new Exception(string.Format("GetRectSubPix function of color depth type {0}", depthType));
            }

            interpolateFunc(img, startPt, destImg);
        }

        private unsafe static void getRectSubPix_Short(IImage img, PointF startPt, IImage destImg)
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

            int nChannels = img.ColorInfo.NumberOfChannels;

            int srcStride = img.Stride;
            int dstStride = destImg.Stride;
            int srcOffset = img.Stride - width * img.ColorInfo.Size;
            int dstOffset = destImg.Stride - width * destImg.ColorInfo.Size;

            for (int ch = 0; ch < nChannels; ch++)
            {
                short* imgPtr = (short*)img.GetData(yt, xt) + ch;
                float* dstPtr = (float*)destImg.ImageData + ch;

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        short* nextRowPtr = (short*)((byte*)imgPtr + srcStride);

                        float x0y0 = *imgPtr;
                        float x1y0 = *(imgPtr + nChannels);
                        float x0y1 = *nextRowPtr;
                        float x1y1 = *(nextRowPtr + nChannels);

                        *dstPtr = a0 * x0y0 + a1 * x1y0 + a2 * x1y1 + a3 * x0y1;

                        imgPtr += nChannels;
                        dstPtr += nChannels;
                    }

                    imgPtr = (short*)((byte*)imgPtr + srcOffset);
                    dstPtr = (float*)((byte*)dstPtr + dstOffset);
                }
            }  
        }

        private unsafe static void getRectSubPix_Float(IImage img, PointF startPt, IImage destImg)
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

            int nChannels = img.ColorInfo.NumberOfChannels;

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
