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

namespace Accord.Extensions.Imaging.IntegralImage
{
    /// <summary>
    /// Squared integral image extensions.
    /// </summary>
    public static class SquaredIntegralImageExtensions
    {
        delegate void MakeIntegralFunc(IImage src, IImage dest);
        static Dictionary<Type, Tuple<Type, MakeIntegralFunc>> makeIntegralFuncs;

        static SquaredIntegralImageExtensions()
        {
            makeIntegralFuncs = new Dictionary<Type, Tuple<Type, MakeIntegralFunc>>();

            makeIntegralFuncs.Add(typeof(byte), new Tuple<Type, MakeIntegralFunc>(typeof(int), makeIntegral_Byte));
            makeIntegralFuncs.Add(typeof(float), new Tuple<Type, MakeIntegralFunc>(typeof(float), makeIntegral_Float));
            makeIntegralFuncs.Add(typeof(double), new Tuple<Type, MakeIntegralFunc>(typeof(double), makeIntegral_Double));
        }

        #region Extensions Make Integral

        /// <summary>
        /// Creates squared integral image.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <returns>Integral image.</returns>
        public static Image<Gray, int> MakeSquaredIntegral(this Image<Gray, byte> img)
        {
            return makeIntegral(img) as Image<Gray, int>;
        }

        /// <summary>
        /// Creates squared integral image.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <returns>Integral image.</returns>
        public static Image<Gray, float> MakeSquaredIntegral(this Image<Gray, float> img)
        {
            return makeIntegral(img) as Image<Gray, float>;
        }

        /// <summary>
        /// Creates squared integral image.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <returns>Integral image.</returns>
        public static Image<Gray, double> MakeSquaredIntegral(this Image<Gray, double> img)
        {
            return makeIntegral(img) as Image<Gray, double>;
        }

        static IImage makeIntegral(IImage img)
        {
            Tuple<Type, MakeIntegralFunc> makeIntegral = null;
            if (makeIntegralFuncs.TryGetValue(img.ColorInfo.ChannelType, out makeIntegral) == false)
                throw new Exception(string.Format("MakeIntegral function can not process image of color depth type {0}", img.ColorInfo.ChannelType));

            var destColor = ColorInfo.GetInfo(img.ColorInfo.ColorType, makeIntegral.Item1);
            IImage dest = Image.Create(destColor, img.Width + 1, img.Height + 1);

            makeIntegral.Item2(img, dest);
            return dest;
        }

        #endregion

        #region MakeIntegral specific funcs

        unsafe static void makeIntegral_Byte(IImage src, IImage dest)
        {
            byte* srcPtr = (byte*)src.ImageData;
            int* dstPtr = (int*)dest.GetData(1, 1);

            int srcStride = src.Stride;
            int dstStride = dest.Stride;

            int width = src.Width;
            int height = src.Height;

            for (int row = 0; row < height; row++)
            {
                int* dstRowPtr = dstPtr;
                for (int col = 0; col < width; col++)
                {
                    var val = srcPtr[col] * srcPtr[col];

                    var a = (dstRowPtr - 1);                          //(x-1, y)
                    var b = (int*)((byte*)dstRowPtr - dstStride);   //(x, y-1)
                    var c = (b - 1);                                  //(x-1, y-1)

                    *dstRowPtr = val + *a + *b - *c;
                    dstRowPtr++;
                }

                srcPtr = (byte*)((byte*)srcPtr + srcStride);
                dstPtr = (int*)((byte*)dstPtr + dstStride);
            }
        }

        unsafe static void makeIntegral_Float(IImage src, IImage dest)
        {
            float* srcPtr = (float*)src.ImageData;
            float* dstPtr = (float*)dest.GetData(1, 1);

            int srcStride = src.Stride;
            int dstStride = dest.Stride;

            int width = src.Width;
            int height = src.Height;

            for (int row = 0; row < height; row++)
            {
                float* dstRowPtr = dstPtr;
                for (int col = 0; col < width; col++)
                {
                    var val = srcPtr[col] * srcPtr[col];

                    var a = (dstRowPtr - 1);                          //(x-1, y)
                    var b = (float*)((byte*)dstRowPtr - dstStride);   //(x, y-1)
                    var c = (b - 1);                                  //(x-1, y-1)

                    *dstRowPtr = val + *a + *b - *c;
                    dstRowPtr++;
                }

                srcPtr = (float*)((byte*)srcPtr + srcStride);
                dstPtr = (float*)((byte*)dstPtr + dstStride);
            }
        }


        unsafe static void makeIntegral_Double(IImage src, IImage dest)
        {
            double* srcPtr = (double*)src.ImageData;
            double* dstPtr = (double*)dest.GetData(1, 1);

            int srcStride = src.Stride;
            int dstStride = dest.Stride;

            int width = src.Width;
            int height = src.Height;

            for (int row = 0; row < height; row++)
            {
                double* dstRowPtr = dstPtr;
                for (int col = 0; col < width; col++)
                {
                    var val = srcPtr[col] * srcPtr[col];

                    var a = (dstRowPtr - 1);                          //(x-1, y)
                    var b = (double*)((byte*)dstRowPtr - dstStride);   //(x, y-1)
                    var c = (b - 1);                                  //(x-1, y-1)

                    *dstRowPtr = val + *a + *b - *c;
                    dstRowPtr++;
                }

                srcPtr = (double*)((byte*)srcPtr + srcStride);
                dstPtr = (double*)((byte*)dstPtr + dstStride);
            }
        }

        #endregion
    }
}
