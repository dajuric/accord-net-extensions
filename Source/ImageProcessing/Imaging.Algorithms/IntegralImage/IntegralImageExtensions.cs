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
    static class IntegralImageExtensions
    {
        #region Extensions Make Integral

        /// <summary>
        /// Creates integral image.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <returns>Integral image.</returns>
        public static Gray<int>[,] MakeIntegral(this Gray<byte>[,] img)
        {
            var dstImg = new Gray<int>[img.Height() + 1, img.Width() + 1];

            using (var uImg = img.Lock())
            using (var uDstImg = dstImg.Lock())
            {
                makeIntegral_Byte(uImg, uDstImg);
            }

            return dstImg;
        }

        /// <summary>
        /// Creates integral image.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <returns>Integral image.</returns>
        public static Gray<float>[,] MakeIntegral(this Gray<float>[,] img)
        {
            var dstImg = new Gray<float>[img.Height() + 1, img.Width() + 1];

            using (var uImg = img.Lock())
            using (var uDstImg = dstImg.Lock())
            {
                makeIntegral_Float(uImg, uDstImg);
            }

            return dstImg;
        }

        /// <summary>
        /// Creates integral image.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <returns>Integral image.</returns>
        public static Gray<double>[,] MakeIntegral(this Gray<double>[,] img)
        {
            var dstImg = new Gray<double>[img.Height() + 1, img.Width() + 1];

            using (var uImg = img.Lock())
            using (var uDstImg = dstImg.Lock())
            {
                makeIntegral_Double(uImg, uDstImg);
            }

            return dstImg;
        }

        #endregion

        #region Extensions Get Integral 

        /// <summary>
        /// Gets sum under image region (requires only 4 lookups).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="x">Location X.</param>
        /// <param name="y">Location Y.</param>
        /// <param name="width">Region width.</param>
        /// <param name="height">Region height.</param>
        /// <returns>Sum of pixels under specified region.</returns>
        public unsafe static int GetSum(this Gray<int>[,] img, int x, int y, int width, int height)
        {
            return img[y, x] + img[y + height, x + width] - img[y + height, x] - img[y, x + width];
        }

        /// <summary>
        /// Gets sum under image region (requires only 4 lookups).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="x">Location X.</param>
        /// <param name="y">Location Y.</param>
        /// <param name="width">Region width.</param>
        /// <param name="height">Region height.</param>
        /// <returns>Sum of pixels under specified region.</returns>
        public unsafe static float GetSum(this Gray<float>[,] img, int x, int y, int width, int height)
        {
            return img[y, x] + img[y + height, x + width] - img[y + height, x] - img[y, x + width];
        }

        /// <summary>
        /// Gets sum under image region (requires only 4 lookups).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="x">Location X.</param>
        /// <param name="y">Location Y.</param>
        /// <param name="width">Region width.</param>
        /// <param name="height">Region height.</param>
        /// <returns>Sum of pixels under specified region.</returns>
        public unsafe static double GetSum(this Gray<double>[,] img, int x, int y, int width, int height)
        {
            return img[y, x] + img[y + height, x + width] - img[y + height, x] - img[y, x + width];
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
                    var val = srcPtr[col];

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
                    var val = srcPtr[col];

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
                    var val = srcPtr[col];

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
