using Accord.Core;
using System;

namespace Accord.Imaging
{
    public static partial class MathOperations
    {
        private static void initializeArithemticOperations() 
        {
            //Add
            mathOperatorFuncs[(int)MathOps.Add].Add(typeof(byte), add_Byte);
            mathOperatorFuncs[(int)MathOps.Add].Add(typeof(short), add_Short);
            mathOperatorFuncs[(int)MathOps.Add].Add(typeof(int), add_Int);
            mathOperatorFuncs[(int)MathOps.Add].Add(typeof(float), add_Float);
            mathOperatorFuncs[(int)MathOps.Add].Add(typeof(double), add_Double);

            //Sub
            mathOperatorFuncs[(int)MathOps.Sub].Add(typeof(byte), sub_Byte);
            mathOperatorFuncs[(int)MathOps.Sub].Add(typeof(short), sub_Short);
            mathOperatorFuncs[(int)MathOps.Sub].Add(typeof(int), sub_Int);
            mathOperatorFuncs[(int)MathOps.Sub].Add(typeof(float), sub_Float);
            mathOperatorFuncs[(int)MathOps.Sub].Add(typeof(double), sub_Double);

            //Mul
            mathOperatorFuncs[(int)MathOps.Mul].Add(typeof(byte), mul_Byte);
            mathOperatorFuncs[(int)MathOps.Mul].Add(typeof(short), mul_Short);
            mathOperatorFuncs[(int)MathOps.Mul].Add(typeof(int), mul_Int);
            mathOperatorFuncs[(int)MathOps.Mul].Add(typeof(float), mul_Float);
            mathOperatorFuncs[(int)MathOps.Mul].Add(typeof(double), mul_Double);

            //Div
            mathOperatorFuncs[(int)MathOps.Div].Add(typeof(byte), div_Byte);
            mathOperatorFuncs[(int)MathOps.Div].Add(typeof(short), div_Short);
            mathOperatorFuncs[(int)MathOps.Div].Add(typeof(int), div_Int);
            mathOperatorFuncs[(int)MathOps.Div].Add(typeof(float), div_Float);
            mathOperatorFuncs[(int)MathOps.Div].Add(typeof(double), div_Double);
        }

        #region Byte

        private unsafe static void add_Byte(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(byte);
            int src2Shift = src2.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * nChannels * sizeof(byte);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr + *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (byte*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (byte*)((byte*)src2Ptr + src2Shift);
                destPtr = (byte*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void sub_Byte(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(byte);
            int src2Shift = src2.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * nChannels * sizeof(byte);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr - *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (byte*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (byte*)((byte*)src2Ptr + src2Shift);
                destPtr = (byte*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void mul_Byte(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(byte);
            int src2Shift = src2.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * nChannels * sizeof(byte);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr * *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (byte*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (byte*)((byte*)src2Ptr + src2Shift);
                destPtr = (byte*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void div_Byte(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            byte* src1Ptr = (byte*)src1.ImageData;
            byte* src2Ptr = (byte*)src2.ImageData;
            byte* destPtr = (byte*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(byte);
            int src2Shift = src2.Stride - width * nChannels * sizeof(byte);
            int destShift = dest.Stride - width * nChannels * sizeof(byte);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (byte)(*src1Ptr / *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (byte*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (byte*)((byte*)src2Ptr + src2Shift);
                destPtr = (byte*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        #endregion

        #region Short

        private unsafe static void add_Short(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            short* src1Ptr = (short*)src1.ImageData;
            short* src2Ptr = (short*)src2.ImageData;
            short* destPtr = (short*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(short);
            int src2Shift = src2.Stride - width * nChannels * sizeof(short);
            int destShift = dest.Stride - width * nChannels * sizeof(short);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (short)(*src1Ptr + *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (short*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (short*)((byte*)src2Ptr + src2Shift);
                destPtr = (short*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void sub_Short(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            short* src1Ptr = (short*)src1.ImageData;
            short* src2Ptr = (short*)src2.ImageData;
            short* destPtr = (short*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(short);
            int src2Shift = src2.Stride - width * nChannels * sizeof(short);
            int destShift = dest.Stride - width * nChannels * sizeof(short);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (short)(*src1Ptr - *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (short*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (short*)((byte*)src2Ptr + src2Shift);
                destPtr = (short*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void mul_Short(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            short* src1Ptr = (short*)src1.ImageData;
            short* src2Ptr = (short*)src2.ImageData;
            short* destPtr = (short*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(short);
            int src2Shift = src2.Stride - width * nChannels * sizeof(short);
            int destShift = dest.Stride - width * nChannels * sizeof(short);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (short)(*src1Ptr * *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (short*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (short*)((byte*)src2Ptr + src2Shift);
                destPtr = (short*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void div_Short(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            short* src1Ptr = (short*)src1.ImageData;
            short* src2Ptr = (short*)src2.ImageData;
            short* destPtr = (short*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(short);
            int src2Shift = src2.Stride - width * nChannels * sizeof(short);
            int destShift = dest.Stride - width * nChannels * sizeof(short);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (short)(*src1Ptr / *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (short*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (short*)((byte*)src2Ptr + src2Shift);
                destPtr = (short*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        #endregion

        #region Int

        private unsafe static void add_Int(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            int* src1Ptr = (int*)src1.ImageData;
            int* src2Ptr = (int*)src2.ImageData;
            int* destPtr = (int*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(int);
            int src2Shift = src2.Stride - width * nChannels * sizeof(int);
            int destShift = dest.Stride - width * nChannels * sizeof(int);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (int)(*src1Ptr + *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (int*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (int*)((byte*)src2Ptr + src2Shift);
                destPtr = (int*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void sub_Int(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            int* src1Ptr = (int*)src1.ImageData;
            int* src2Ptr = (int*)src2.ImageData;
            int* destPtr = (int*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(int);
            int src2Shift = src2.Stride - width * nChannels * sizeof(int);
            int destShift = dest.Stride - width * nChannels * sizeof(int);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (int)(*src1Ptr - *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (int*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (int*)((byte*)src2Ptr + src2Shift);
                destPtr = (int*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void mul_Int(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            int* src1Ptr = (int*)src1.ImageData;
            int* src2Ptr = (int*)src2.ImageData;
            int* destPtr = (int*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(int);
            int src2Shift = src2.Stride - width * nChannels * sizeof(int);
            int destShift = dest.Stride - width * nChannels * sizeof(int);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (int)(*src1Ptr * *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (int*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (int*)((byte*)src2Ptr + src2Shift);
                destPtr = (int*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void div_Int(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            int* src1Ptr = (int*)src1.ImageData;
            int* src2Ptr = (int*)src2.ImageData;
            int* destPtr = (int*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(int);
            int src2Shift = src2.Stride - width * nChannels * sizeof(int);
            int destShift = dest.Stride - width * nChannels * sizeof(int);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (int)(*src1Ptr / *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (int*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (int*)((byte*)src2Ptr + src2Shift);
                destPtr = (int*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        #endregion

        #region Float

        private unsafe static void add_Float(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            float* src1Ptr = (float*)src1.ImageData;
            float* src2Ptr = (float*)src2.ImageData;
            float* destPtr = (float*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(float);
            int src2Shift = src2.Stride - width * nChannels * sizeof(float);
            int destShift = dest.Stride - width * nChannels * sizeof(float);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (float)(*src1Ptr + *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (float*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (float*)((byte*)src2Ptr + src2Shift);
                destPtr = (float*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void sub_Float(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            float* src1Ptr = (float*)src1.ImageData;
            float* src2Ptr = (float*)src2.ImageData;
            float* destPtr = (float*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(float);
            int src2Shift = src2.Stride - width * nChannels * sizeof(float);
            int destShift = dest.Stride - width * nChannels * sizeof(float);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (float)(*src1Ptr - *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (float*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (float*)((byte*)src2Ptr + src2Shift);
                destPtr = (float*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void mul_Float(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            float* src1Ptr = (float*)src1.ImageData;
            float* src2Ptr = (float*)src2.ImageData;
            float* destPtr = (float*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(float);
            int src2Shift = src2.Stride - width * nChannels * sizeof(float);
            int destShift = dest.Stride - width * nChannels * sizeof(float);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (float)(*src1Ptr * *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (float*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (float*)((byte*)src2Ptr + src2Shift);
                destPtr = (float*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void div_Float(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            float* src1Ptr = (float*)src1.ImageData;
            float* src2Ptr = (float*)src2.ImageData;
            float* destPtr = (float*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(float);
            int src2Shift = src2.Stride - width * nChannels * sizeof(float);
            int destShift = dest.Stride - width * nChannels * sizeof(float);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (float)(*src1Ptr / *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (float*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (float*)((byte*)src2Ptr + src2Shift);
                destPtr = (float*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        #endregion

        #region Double

        private unsafe static void add_Double(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            double* src1Ptr = (double*)src1.ImageData;
            double* src2Ptr = (double*)src2.ImageData;
            double* destPtr = (double*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(double);
            int src2Shift = src2.Stride - width * nChannels * sizeof(double);
            int destShift = dest.Stride - width * nChannels * sizeof(double);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (double)(*src1Ptr + *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (double*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (double*)((byte*)src2Ptr + src2Shift);
                destPtr = (double*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void sub_Double(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            double* src1Ptr = (double*)src1.ImageData;
            double* src2Ptr = (double*)src2.ImageData;
            double* destPtr = (double*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(double);
            int src2Shift = src2.Stride - width * nChannels * sizeof(double);
            int destShift = dest.Stride - width * nChannels * sizeof(double);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (double)(*src1Ptr - *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (double*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (double*)((byte*)src2Ptr + src2Shift);
                destPtr = (double*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void mul_Double(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            double* src1Ptr = (double*)src1.ImageData;
            double* src2Ptr = (double*)src2.ImageData;
            double* destPtr = (double*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(double);
            int src2Shift = src2.Stride - width * nChannels * sizeof(double);
            int destShift = dest.Stride - width * nChannels * sizeof(double);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (double)(*src1Ptr * *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (double*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (double*)((byte*)src2Ptr + src2Shift);
                destPtr = (double*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void div_Double(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask)
        {
            double* src1Ptr = (double*)src1.ImageData;
            double* src2Ptr = (double*)src2.ImageData;
            double* destPtr = (double*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int src1Shift = src1.Stride - width * nChannels * sizeof(double);
            int src2Shift = src2.Stride - width * nChannels * sizeof(double);
            int destShift = dest.Stride - width * nChannels * sizeof(double);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (*maskPtr == 0)
                        continue;

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        *destPtr = (double)(*src1Ptr / *src2Ptr);

                        src1Ptr++;
                        src2Ptr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                src1Ptr = (double*)((byte*)src1Ptr + src1Shift);
                src2Ptr = (double*)((byte*)src2Ptr + src2Shift);
                destPtr = (double*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        #endregion

        #region Extensions

        public static Image<TColor, TDepth> Add<TColor, TDepth>(this Image<TColor, TDepth> img, Image<TColor, TDepth> img2, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
            where TDepth : struct
        {
            IImage dest = img;
            if (!inPlace)
                dest = img.CopyBlank();

            calculate(MathOps.Add, img, img2, dest, mask);
            return dest as Image<TColor, TDepth>;
        }

        public static Image<TColor, TDepth> Add<TColor, TDepth>(this Image<TColor, TDepth> img, TColor value, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
            where TDepth : struct
        {
            var mulImg = img.CopyBlank(); mulImg.SetValue(value);
            return Add(img, mulImg, inPlace, mask);
        }

        public static Image<TColor, TDepth> Sub<TColor, TDepth>(this Image<TColor, TDepth> img, Image<TColor, TDepth> img2, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
            where TDepth : struct
        {
            IImage dest = img;
            if (!inPlace)
                dest = img.CopyBlank();

            calculate(MathOps.Sub, img, img2, dest, mask);
            return dest as Image<TColor, TDepth>;
        }

        public static Image<TColor, TDepth> Sub<TColor, TDepth>(this Image<TColor, TDepth> img, TColor value, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
            where TDepth : struct
        {
            var mulImg = img.CopyBlank(); mulImg.SetValue(value);
            return Sub(img, mulImg, inPlace, mask);
        }

        public static Image<TColor, TDepth> Mul<TColor, TDepth>(this Image<TColor, TDepth> img, Image<TColor, TDepth> img2, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
            where TDepth : struct
        {
            IImage dest = img;
            if (!inPlace)
                dest = img.CopyBlank();

            calculate(MathOps.Mul, img, img2, dest, mask);
            return dest as Image<TColor, TDepth>;
        }

        public static Image<TColor, TDepth> Mul<TColor, TDepth>(this Image<TColor, TDepth> img, TColor value, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
            where TDepth : struct
        {
            var mulImg = img.CopyBlank(); mulImg.SetValue(value);
            return Mul(img, mulImg, inPlace, mask);
        }

        public static Image<TColor, TDepth> Div<TColor, TDepth>(this Image<TColor, TDepth> img, Image<TColor, TDepth> img2, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
            where TDepth : struct
        {
            IImage dest = img;
            if (!inPlace)
                dest = img.CopyBlank();

            calculate(MathOps.Div, img, img2, dest, mask);
            return dest as Image<TColor, TDepth>;
        }

        public static Image<TColor, TDepth> Div<TColor, TDepth>(this Image<TColor, TDepth> img, TColor value, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
            where TDepth : struct
        {
            var mulImg = img.CopyBlank(); mulImg.SetValue(value);
            return Div(img, mulImg, inPlace, mask);
        }

        #endregion
    }

    public partial class Image<TColor, TDepth> : GenericImageBase
        where TColor : IColor
        where TDepth : struct
    {
        public static Image<TColor, TDepth> operator +(Image<TColor, TDepth> image, Image<TColor, TDepth> img2)
        {
            return image.Add(img2);
        }

        public static Image<TColor, TDepth> operator -(Image<TColor, TDepth> image, Image<TColor, TDepth> img2)
        {
            return image.Sub(img2);
        }

        public static Image<TColor, TDepth> operator * (Image<TColor, TDepth> image, Image<TColor, TDepth> img2)
        {
            return image.Mul(img2);
        }

        public static Image<TColor, TDepth> operator /(Image<TColor, TDepth> image, Image<TColor, TDepth> img2)
        {
            return image.Div(img2);
        }
    }
}
