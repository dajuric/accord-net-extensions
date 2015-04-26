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
    /// Contains math operations applicable on an image.
    /// </summary>
    public static class ArithmeticsByteExtensions
    {
        #region Byte

        private unsafe static void add_Byte(IImage src1, IImage src2, IImage dest)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int src1Shift = src1.Stride - width * nChannels * sizeof(byte);
            int src2Shift = src2.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * nChannels * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr + *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr = (byte*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (byte*)((byte*)src2Ptr + src2Shift);
                destPtr = (byte*)((byte*)destPtr + destShift);
            }
        }

        private unsafe static void sub_Byte(IImage src1, IImage src2, IImage dest)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int src1Shift = src1.Stride - width * nChannels * sizeof(byte);
            int src2Shift = src2.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * nChannels * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr - *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr = (byte*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (byte*)((byte*)src2Ptr + src2Shift);
                destPtr = (byte*)((byte*)destPtr + destShift);
            }
        }

        private unsafe static void mul_Byte(IImage src1, IImage src2, IImage dest)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int src1Shift = src1.Stride - width * nChannels * sizeof(byte);
            int src2Shift = src2.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * nChannels * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr * *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr = (byte*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (byte*)((byte*)src2Ptr + src2Shift);
                destPtr = (byte*)((byte*)destPtr + destShift);
            }
        }

        private unsafe static void div_Byte(IImage src1, IImage src2, IImage dest)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int src1Shift = src1.Stride - width * nChannels * sizeof(byte);
            int src2Shift = src2.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * nChannels * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr / *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr = (byte*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (byte*)((byte*)src2Ptr + src2Shift);
                destPtr = (byte*)((byte*)destPtr + destShift);
            }
        }

        private unsafe static void min_Byte(IImage imageA, IImage imageB, IImage minImage)
        {
            int width = imageA.Width;
            int height = imageA.Height;
            int nChannels = imageA.ColorInfo.ChannelCount;

            int srcAOffset = imageA.Stride - width * sizeof(byte) * nChannels;
            int srcBOffset = imageB.Stride - width * sizeof(byte) * nChannels;
            int dstOffset = minImage.Stride - width * sizeof(byte) * nChannels;

            byte* srcAPtr = (byte*)imageA.ImageData;
            byte* srcBPtr = (byte*)imageB.ImageData;
            byte* dstPtr = (byte*)minImage.ImageData;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int channel = 0; channel < nChannels; channel++)
                    {
                        *dstPtr = System.Math.Min(*srcAPtr, *srcBPtr);

                        srcAPtr++;
                        srcBPtr++;
                        dstPtr++;
                    }
                }

                srcAPtr = (byte*)((byte*)srcAPtr + srcAOffset);
                srcBPtr = (byte*)((byte*)srcBPtr + srcBOffset);
                dstPtr = (byte*)((byte*)dstPtr + dstOffset);
            }
        }

        private unsafe static void max_Byte(IImage imageA, IImage imageB, IImage maxImage)
        {
            int width = imageA.Width;
            int height = imageA.Height;
            int nChannels = imageA.ColorInfo.ChannelCount;

            int srcAOffset = imageA.Stride - width * sizeof(byte) * nChannels;
            int srcBOffset = imageB.Stride - width * sizeof(byte) * nChannels;
            int dstOffset = maxImage.Stride - width * sizeof(byte) * nChannels;

            byte* srcAPtr = (byte*)imageA.ImageData;
            byte* srcBPtr = (byte*)imageB.ImageData;
            byte* dstPtr = (byte*)maxImage.ImageData;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int channel = 0; channel < nChannels; channel++)
                    {
                        *dstPtr = System.Math.Max(*srcAPtr, *srcBPtr);

                        srcAPtr++;
                        srcBPtr++;
                        dstPtr++;
                    }
                }

                srcAPtr = (byte*)((byte*)srcAPtr + srcAOffset);
                srcBPtr = (byte*)((byte*)srcBPtr + srcBOffset);
                dstPtr = (byte*)((byte*)dstPtr + dstOffset);
            }
        }
        #endregion

        /// <summary>
        /// Adds pixels of two images. The operation is executed for each channel.
        /// <para>If using 8-bit values an overflow might happen.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">First image.</param>
        /// <param name="img2">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] AddByte<TColor>(this TColor[,] img, TColor[,] img2, bool inPlace = false)
            where TColor : struct, IColor<byte>
        {
            return img.Calculate(img2, add_Byte, inPlace);
        }

        /// <summary>
        /// Subtracts pixels of two images. The operation is executed for each channel.
        /// <para>If using 8-bit values an overflow might happen.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">First image.</param>
        /// <param name="img2">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] SubByte<TColor>(this TColor[,] img, TColor[,] img2, bool inPlace = false)
                where TColor : struct, IColor<byte>
        {
            return img.Calculate(img2, sub_Byte, inPlace);
        }

        /// <summary>
        /// Multiplies pixels of two images. The operation is executed for each channel.
        /// <para>If using 8-bit values an overflow might happen.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">First image.</param>
        /// <param name="img2">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] MulByte<TColor>(this TColor[,] img, TColor[,] img2, bool inPlace = false)
                where TColor : struct, IColor<byte>
        {
            return img.Calculate(img2, mul_Byte, inPlace);
        }

        /// <summary>
        /// Divides pixels of two images. The operation is executed for each channel. If using integers, an integer division is going to be applied.
        /// <para>If using 8-bit values an overflow might happen.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">First image.</param>
        /// <param name="img2">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] DivByte<TColor>(this TColor[,] img, TColor[,] img2, bool inPlace = false)
              where TColor : struct, IColor<byte>
        {
            return img.Calculate(img2, div_Byte, inPlace);
        }

        /// <summary>
        /// Select minimal value for each channel.
        /// </summary>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="inPlace">Process in place or not.</param>
        /// <returns>MIN(imageA, imageB) per channel</returns>
        public static TColor[,] MinByte<TColor>(this TColor[,] imageA, TColor[,] imageB, bool inPlace = false)
             where TColor : struct, IColor<byte>
        {
            return imageA.Calculate(imageB, min_Byte, inPlace);
        }

        /// <summary>
        /// Select maximal value for each channel.
        /// </summary>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="inPlace">Process in place or not.</param>
        /// <returns>MAX(imageA, imageB) per channel</returns>
        public static TColor[,] MaxByte<TColor>(this TColor[,] imageA, TColor[,] imageB, bool inPlace = false)
             where TColor : struct, IColor<byte>
        {
            return imageA.Calculate(imageB, max_Byte, inPlace);
        }
    } 
     
    /// <summary>
    /// Contains math operations applicable on an image.
    /// </summary>
    public static class ArithmeticsFloatExtensions
    {
        #region Float

        private unsafe static void add_Float(IImage src1, IImage src2, IImage dest)
        {
            float* src1Ptr = (float*)src1.ImageData;
            float* src2Ptr = (float*)src2.ImageData;
            float* destPtr = (float*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int src1Shift = src1.Stride - width * nChannels * sizeof(float);
            int src2Shift = src2.Stride - width * nChannels * sizeof(float);
            int destShift = dest.Stride - width * nChannels * sizeof(float);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (float)(*src1Ptr + *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr = (float*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (float*)((byte*)src2Ptr + src2Shift);
                destPtr = (float*)((byte*)destPtr + destShift);
            }
        }

        private unsafe static void sub_Float(IImage src1, IImage src2, IImage dest)
        {
            float* src1Ptr = (float*)src1.ImageData;
            float* src2Ptr = (float*)src2.ImageData;
            float* destPtr = (float*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int src1Shift = src1.Stride - width * nChannels * sizeof(float);
            int src2Shift = src2.Stride - width * nChannels * sizeof(float);
            int destShift = dest.Stride - width * nChannels * sizeof(float);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (float)(*src1Ptr - *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr = (float*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (float*)((byte*)src2Ptr + src2Shift);
                destPtr = (float*)((byte*)destPtr + destShift);
            }
        }

        private unsafe static void mul_Float(IImage src1, IImage src2, IImage dest)
        {
            float* src1Ptr = (float*)src1.ImageData;
            float* src2Ptr = (float*)src2.ImageData;
            float* destPtr = (float*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int src1Shift = src1.Stride - width * nChannels * sizeof(float);
            int src2Shift = src2.Stride - width * nChannels * sizeof(float);
            int destShift = dest.Stride - width * nChannels * sizeof(float);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (float)(*src1Ptr * *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr = (float*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (float*)((byte*)src2Ptr + src2Shift);
                destPtr = (float*)((byte*)destPtr + destShift);
            }
        }

        private unsafe static void div_Float(IImage src1, IImage src2, IImage dest)
        {
            float* src1Ptr = (float*)src1.ImageData;
            float* src2Ptr = (float*)src2.ImageData;
            float* destPtr = (float*)dest.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.ChannelCount;

            int src1Shift = src1.Stride - width * nChannels * sizeof(float);
            int src2Shift = src2.Stride - width * nChannels * sizeof(float);
            int destShift = dest.Stride - width * nChannels * sizeof(float);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (float)(*src1Ptr / *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }
                }

                src1Ptr = (float*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (float*)((byte*)src2Ptr + src2Shift);
                destPtr = (float*)((byte*)destPtr + destShift);
            }
        }

        private unsafe static void min_Float(IImage imageA, IImage imageB, IImage minImage)
        {
            int width = imageA.Width;
            int height = imageA.Height;
            int nChannels = imageA.ColorInfo.ChannelCount;

            int srcAOffset = imageA.Stride - width * sizeof(float) * nChannels;
            int srcBOffset = imageB.Stride - width * sizeof(float) * nChannels;
            int dstOffset = minImage.Stride - width * sizeof(float) * nChannels;

            float* srcAPtr = (float*)imageA.ImageData;
            float* srcBPtr = (float*)imageB.ImageData;
            float* dstPtr = (float*)minImage.ImageData;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int channel = 0; channel < nChannels; channel++)
                    {
                        *dstPtr = System.Math.Min(*srcAPtr, *srcBPtr);

                        srcAPtr++;
                        srcBPtr++;
                        dstPtr++;
                    }
                }

                srcAPtr = (float*)((byte*)srcAPtr + srcAOffset);
                srcBPtr = (float*)((byte*)srcBPtr + srcBOffset);
                dstPtr = (float*)((byte*)dstPtr + dstOffset);
            }
        }

        private unsafe static void max_Float(IImage imageA, IImage imageB, IImage maxImage)
        {
            int width = imageA.Width;
            int height = imageA.Height;
            int nChannels = imageA.ColorInfo.ChannelCount;

            int srcAOffset = imageA.Stride - width * sizeof(float) * nChannels;
            int srcBOffset = imageB.Stride - width * sizeof(float) * nChannels;
            int dstOffset = maxImage.Stride - width * sizeof(float) * nChannels;

            float* srcAPtr = (float*)imageA.ImageData;
            float* srcBPtr = (float*)imageB.ImageData;
            float* dstPtr = (float*)maxImage.ImageData;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int channel = 0; channel < nChannels; channel++)
                    {
                        *dstPtr = System.Math.Max(*srcAPtr, *srcBPtr);

                        srcAPtr++;
                        srcBPtr++;
                        dstPtr++;
                    }
                }

                srcAPtr = (float*)((byte*)srcAPtr + srcAOffset);
                srcBPtr = (float*)((byte*)srcBPtr + srcBOffset);
                dstPtr = (float*)((byte*)dstPtr + dstOffset);
            }
        }

        #endregion

        /// <summary>
        /// Adds pixels of two images. The operation is executed for each channel.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] AddFloat<TColor>(this TColor[,] imageA, TColor[,] imageB, bool inPlace = false)
            where TColor : struct, IColor<float>
        {
            return imageA.Calculate(imageB, add_Float, inPlace);
        }

        /// <summary>
        /// Subtracts pixels of two images. The operation is executed for each channel.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] SubFloat<TColor>(this TColor[,] imageA, TColor[,] imageB, bool inPlace = false)
                where TColor : struct, IColor<float>
        {
            return imageA.Calculate(imageB, sub_Float, inPlace);
        }

        /// <summary>
        /// Multiplies pixels of two images. The operation is executed for each channel.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] MulFloat<TColor>(this TColor[,] imageA, TColor[,] imageB, bool inPlace = false)
                where TColor : struct, IColor<float>
        {
            return imageA.Calculate(imageB, mul_Float, inPlace);
        }

        /// <summary>
        /// Divides pixels of two images. The operation is executed for each channel. If using integers, an integer division is going to be applied.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>The result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] DivFloat<TColor>(this TColor[,] imageA, TColor[,] imageB, bool inPlace = false)
              where TColor : struct, IColor<float>
        {
            return imageA.Calculate(imageB, div_Float, inPlace);
        }

        /// <summary>
        /// Select minimal value for each channel.
        /// </summary>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="inPlace">Process in place or not.</param>
        /// <returns>MIN(imageA, imageB) per channel</returns>
        public static TColor[,] MinFloat<TColor>(this TColor[,] imageA, TColor[,] imageB, bool inPlace = false)
             where TColor : struct, IColor<float>
        {
            return imageA.Calculate(imageB, min_Float, inPlace);
        }

        /// <summary>
        /// Select maximal value for each channel.
        /// </summary>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="inPlace">Process in place or not.</param>
        /// <returns>MAX(imageA, imageB) per channel</returns>
        public static TColor[,] MaxFloat<TColor>(this TColor[,] imageA, TColor[,] imageB, bool inPlace = false)
             where TColor : struct, IColor<float>
        {
            return imageA.Calculate(imageB, max_Float, inPlace);
        }
    }
}
