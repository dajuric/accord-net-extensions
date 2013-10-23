using Accord.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
   public static partial class MathOperations
    {
        private static void initializeLogicOperations()
        {
            //AND
            mathOperatorFuncs[(int)MathOps.And].Add(typeof(byte), bitwiseAnd_Byte);
            mathOperatorFuncs[(int)MathOps.And].Add(typeof(short), bitwiseAnd_Short);
            mathOperatorFuncs[(int)MathOps.And].Add(typeof(int), bitwiseAnd_Int);

            //OR
            mathOperatorFuncs[(int)MathOps.Or].Add(typeof(byte), bitwiseOr_Byte);
            mathOperatorFuncs[(int)MathOps.Or].Add(typeof(short), bitwiseOr_Short);
            mathOperatorFuncs[(int)MathOps.Or].Add(typeof(int), bitwiseOr_Int);

            //XOR
            mathOperatorFuncs[(int)MathOps.Xor].Add(typeof(byte), bitwiseXor_Byte);
            mathOperatorFuncs[(int)MathOps.Xor].Add(typeof(short), bitwiseXor_Short);
            mathOperatorFuncs[(int)MathOps.Xor].Add(typeof(int), bitwiseXor_Int);

            //for floating point numbers its segments converted to char could be compared (single bits not)
        }

        public static Image<TColor, TDepth> And<TColor, TDepth>(this Image<TColor, TDepth> img, Image<TColor, TDepth> img2, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor: IColor
            where TDepth: struct
        {
            IImage dest = img;
            if (!inPlace)
                dest = img.CopyBlank();

            calculate(MathOps.And,  img, img2, dest, mask);
            return dest as Image<TColor, TDepth>;
        }

        #region Byte

        private unsafe static void bitwiseAnd_Byte(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int shift = dest.Stride - width;
            int maskShift = mask.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr & *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
                maskPtr += maskShift;
            }
        }

        private unsafe static void bitwiseOr_Byte(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int shift = dest.Stride - width;
            int maskShift = mask.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr | *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
                maskPtr += maskShift;
            }
        }

        private unsafe static void bitwiseXor_Byte(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int shift = dest.Stride - width;
            int maskShift = mask.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr ^ *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
                maskPtr += maskShift;
            }
        }

        #endregion

        #region Short

        private unsafe static void bitwiseAnd_Short(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            short* src1Ptr = (short*)src1.ImageData;
            short* src2Ptr = (short*)src2.ImageData;
            short* destPtr = (short*)dest.ImageData;
            short* maskPtr = (short*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int shift = dest.Stride - width;
            int maskShift = mask.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (short)(*src1Ptr & *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
                maskPtr += maskShift;
            }
        }

        private unsafe static void bitwiseOr_Short(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            short* src1Ptr = (short*)src1.ImageData;
            short* src2Ptr = (short*)src2.ImageData;
            short* destPtr = (short*)dest.ImageData;
            short* maskPtr = (short*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int shift = dest.Stride - width;
            int maskShift = mask.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (short)(*src1Ptr | *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
                maskPtr += maskShift;
            }
        }


        private unsafe static void bitwiseXor_Short(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            short* src1Ptr = (short*)src1.ImageData;
            short* src2Ptr = (short*)src2.ImageData;
            short* destPtr = (short*)dest.ImageData;
            short* maskPtr = (short*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int shift = dest.Stride - width;
            int maskShift = mask.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (short)(*src1Ptr ^ *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
                maskPtr += maskShift;
            }
        }

        #endregion

        #region Int

        private unsafe static void bitwiseAnd_Int(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            int* src1Ptr = (int*)src1.ImageData;
            int* src2Ptr = (int*)src2.ImageData;
            int* destPtr = (int*)dest.ImageData;
            int* maskPtr = (int*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int shift = dest.Stride - width;
            int maskShift = mask.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (int)(*src1Ptr & *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
                maskPtr += maskShift;
            }
        }

        private unsafe static void bitwiseOr_Int(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            int* src1Ptr = (int*)src1.ImageData;
            int* src2Ptr = (int*)src2.ImageData;
            int* destPtr = (int*)dest.ImageData;
            int* maskPtr = (int*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int shift = dest.Stride - width;
            int maskShift = mask.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (int)(*src1Ptr | *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
                maskPtr += maskShift;
            }
        }

        private unsafe static void bitwiseXor_Int(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            int* src1Ptr = (int*)src1.ImageData;
            int* src2Ptr = (int*)src2.ImageData;
            int* destPtr = (int*)dest.ImageData;
            int* maskPtr = (int*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int shift = dest.Stride - width;
            int maskShift = mask.Stride - width;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (int)(*src1Ptr ^ *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr += shift;
                src2Ptr += shift;
                destPtr += shift;
                maskPtr += maskShift;
            }
        }

        #endregion

    }
}
