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

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides extensions for bitwise logic operations.
    /// </summary>
    public static class ArithmeticLogicByteExtensions
    {
        #region Byte

        private unsafe static void bitwiseAnd_Byte(IImage src1, IImage src2, IImage dest)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int shift = dest.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr & *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
            }
        }

        private unsafe static void bitwiseOr_Byte(IImage src1, IImage src2, IImage dest)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int shift = dest.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr | *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
            }
        }

        private unsafe static void bitwiseXor_Byte(IImage src1, IImage src2, IImage dest)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int shift = dest.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr ^ *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
            }
        }

        #endregion

        /// <summary>
        /// Executes pixel-wise logic AND operation.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">First image.</param>
        /// <param name="img2">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] AndByte<TColor>(this TColor[,] img, TColor[,] img2, bool inPlace = false)
           where TColor : struct, IColor<byte>
        {
            return img.Calculate(img2, bitwiseAnd_Byte, inPlace);
        }

        /// <summary>
        /// Executes pixel-wise logic OR operation.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">First image.</param>
        /// <param name="img2">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] OrByte<TColor>(this TColor[,] img, TColor[,] img2, bool inPlace = false)
           where TColor : struct, IColor<byte>
        {
            return img.Calculate(img2, bitwiseOr_Byte, inPlace);
        }

        /// <summary>
        /// Executes pixel-wise logic XOR operation.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">First image.</param>
        /// <param name="img2">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] XorByte<TColor>(this TColor[,] img, TColor[,] img2, bool inPlace = false)
           where TColor : struct, IColor<byte>
        {
            return img.Calculate(img2, bitwiseXor_Byte, inPlace);
        }
    }
}
