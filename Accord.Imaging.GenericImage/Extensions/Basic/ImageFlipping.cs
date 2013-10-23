using Accord.Core;
using System;
using System.Collections.Generic;

namespace Accord.Imaging
{
    [Flags]
    public enum FlipDirection
    {
        None = 0x0,
        Horizontal = 0x1,
        Vertical = 0x2,
        All = 0x3
    }

    public static class ImageFlipping
    {
        delegate void ImageFlipFunc(IImage srcImg, IImage dstImg, FlipDirection flip);
        static Dictionary<Type, ImageFlipFunc> imageFlipFuncs = null;

        static ImageFlipping()
        {
            imageFlipFuncs = new Dictionary<Type, ImageFlipFunc>();
            imageFlipFuncs.Add(typeof(byte), flipImage_Byte);
            imageFlipFuncs.Add(typeof(short), flipImage_Short);
            imageFlipFuncs.Add(typeof(int), flipImage_Int);
            imageFlipFuncs.Add(typeof(float), flipImage_Float);
            imageFlipFuncs.Add(typeof(double), flipImage_Double);
        }

        /// <summary>
        /// Flips an input image horizontaly / verticaly / both directions / or none (data copy).
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="flip">Flip direction.</param>
        /// <param name="inPlace">Do it in place.</param>
        /// <returns></returns>
        public static Image<TColor, TDepth> FlipImage<TColor, TDepth>(this Image<TColor, TDepth> img, FlipDirection flip, bool inPlace)
            where TColor:IColor
            where TDepth:struct
        {
            IImage dest = img;
            if (!inPlace)
                dest = img.CopyBlank();

            FlipImage((IImage)img, dest, flip);
            return dest as Image<TColor, TDepth>;
        }

        internal static void FlipImage(IImage srcImg, IImage dstImg, FlipDirection flip)
        {
            Type channelType = srcImg.ColorInfo.ChannelType;

            ImageFlipFunc flipFunc = null;
            if (imageFlipFuncs.TryGetValue(channelType, out flipFunc) == false)
                throw new Exception(string.Format("FlipImage can no process an image of type {0}", channelType));

            flipFunc(srcImg, dstImg, flip);
        }

        private unsafe static void flipImage_Byte(IImage srcImg, IImage dstImg, FlipDirection flip)
        {
            int dstStartRow = 0, verticalShift = 0; //for vertical flipping
            int startCol = 0, direction = 1; //for horizontal flipping

            if ((flip & FlipDirection.Vertical) != 0)
            {
                dstStartRow = dstImg.Stride * (dstImg.Height - 1);
                verticalShift = -2 * dstImg.Stride;
            }
            if ((flip & FlipDirection.Horizontal) != 0)
            {
                startCol = (dstImg.Width - 1) * dstImg.ColorInfo.NumberOfChannels * sizeof(byte);
                direction = -1;
            }

            byte* srcPtr = (byte*)srcImg.ImageData;
            byte* dstPtr = (byte*)((byte*)dstImg.ImageData + dstStartRow + startCol);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;
            int dstStride = dstImg.Stride;
            int offset = srcImg.Stride - srcImg.Width * nChannels * sizeof(byte);

            int width = srcImg.Width;
            int height = srcImg.Height;

            for (int row = 0; row < height; row++)
            {
                byte* dstRowPtr = dstPtr;

                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstRowPtr[ch] = srcPtr[ch];
                    }

                    srcPtr += nChannels;
                    dstRowPtr += nChannels * direction;
                }

                srcPtr = (byte*)((byte*)srcPtr + offset);
                dstPtr = (byte*)((byte*)dstPtr + dstStride + verticalShift);
            }
        }

        private unsafe static void flipImage_Short(IImage srcImg, IImage dstImg, FlipDirection flip)
        {
            int dstStartRow = 0, verticalShift = 0; //for vertical flipping
            int startCol = 0, direction = 1; //for horizontal flipping

            if ((flip & FlipDirection.Vertical) != 0)
            {
                dstStartRow = dstImg.Stride * (dstImg.Height - 1);
                verticalShift = -2 * dstImg.Stride;
            }
            if ((flip & FlipDirection.Horizontal) != 0)
            {
                startCol = (dstImg.Width - 1) * dstImg.ColorInfo.NumberOfChannels * sizeof(short);
                direction = -1;
            }

            short* srcPtr = (short*)srcImg.ImageData;
            short* dstPtr = (short*)((byte*)dstImg.ImageData + dstStartRow + startCol);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;
            int dstStride = dstImg.Stride;
            int offset = srcImg.Stride - srcImg.Width * nChannels * sizeof(short);

            int width = srcImg.Width;
            int height = srcImg.Height;

            for (int row = 0; row < height; row++)
            {
                short* dstRowPtr = dstPtr;

                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstRowPtr[ch] = srcPtr[ch];
                    }

                    srcPtr += nChannels;
                    dstRowPtr += nChannels * direction;
                }

                srcPtr = (short*)((byte*)srcPtr + offset);
                dstPtr = (short*)((byte*)dstPtr + dstStride + verticalShift);
            }
        }

        private unsafe static void flipImage_Int(IImage srcImg, IImage dstImg, FlipDirection flip)
        {
            int dstStartRow = 0, verticalShift = 0; //for vertical flipping
            int startCol = 0, direction = 1; //for horizontal flipping

            if ((flip & FlipDirection.Vertical) != 0)
            {
                dstStartRow = dstImg.Stride * (dstImg.Height - 1);
                verticalShift = -2 * dstImg.Stride;
            }
            if ((flip & FlipDirection.Horizontal) != 0)
            {
                startCol = (dstImg.Width - 1) * dstImg.ColorInfo.NumberOfChannels * sizeof(int);
                direction = -1;
            }

            int* srcPtr = (int*)srcImg.ImageData;
            int* dstPtr = (int*)((byte*)dstImg.ImageData + dstStartRow + startCol);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;
            int dstStride = dstImg.Stride;
            int offset = srcImg.Stride - srcImg.Width * nChannels * sizeof(int);

            int width = srcImg.Width;
            int height = srcImg.Height;

            for (int row = 0; row < height; row++)
            {
                int* dstRowPtr = dstPtr;

                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstRowPtr[ch] = srcPtr[ch];
                    }

                    srcPtr += nChannels;
                    dstRowPtr += nChannels * direction;
                }

                srcPtr = (int*)((byte*)srcPtr + offset);
                dstPtr = (int*)((byte*)dstPtr + dstStride + verticalShift);
            }
        }

        private unsafe static void flipImage_Float(IImage srcImg, IImage dstImg, FlipDirection flip)
        {
            int dstStartRow = 0, verticalShift = 0; //for vertical flipping
            int startCol = 0, direction = 1; //for horizontal flipping

            if ((flip & FlipDirection.Vertical) != 0)
            {
                dstStartRow = dstImg.Stride * (dstImg.Height - 1);
                verticalShift = -2 * dstImg.Stride;
            }
            if ((flip & FlipDirection.Horizontal) != 0)
            {
                startCol = (dstImg.Width-1) * dstImg.ColorInfo.NumberOfChannels * sizeof(float);
                direction = -1;
            }

            float* srcPtr = (float*)srcImg.ImageData;
            float* dstPtr = (float*)((byte*)dstImg.ImageData + dstStartRow + startCol);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;
            int dstStride = dstImg.Stride;
            int offset = srcImg.Stride - srcImg.Width * nChannels * sizeof(float);

            int width = srcImg.Width;
            int height = srcImg.Height;

            for (int row = 0; row < height; row++)
            {
                float* dstRowPtr = dstPtr;

                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstRowPtr[ch] = srcPtr[ch];
                    }

                    srcPtr += nChannels;
                    dstRowPtr += nChannels * direction;
                }

                srcPtr = (float*)((byte*)srcPtr + offset);
                dstPtr = (float*)((byte*)dstPtr + dstStride + verticalShift);
            }
        }

        private unsafe static void flipImage_Double(IImage srcImg, IImage dstImg, FlipDirection flip)
        {
            int dstStartRow = 0, verticalShift = 0; //for vertical flipping
            int startCol = 0, direction = 1; //for horizontal flipping

            if ((flip & FlipDirection.Vertical) != 0)
            {
                dstStartRow = dstImg.Stride * (dstImg.Height - 1);
                verticalShift = -2 * dstImg.Stride;
            }
            if ((flip & FlipDirection.Horizontal) != 0)
            {
                startCol = (dstImg.Width - 1) * dstImg.ColorInfo.NumberOfChannels * sizeof(double);
                direction = -1;
            }

            double* srcPtr = (double*)srcImg.ImageData;
            double* dstPtr = (double*)((byte*)dstImg.ImageData + dstStartRow + startCol);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;
            int dstStride = dstImg.Stride;
            int offset = srcImg.Stride - srcImg.Width * nChannels * sizeof(double);

            int width = srcImg.Width;
            int height = srcImg.Height;

            for (int row = 0; row < height; row++)
            {
                double* dstRowPtr = dstPtr;

                for (int col = 0; col < width; col++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstRowPtr[ch] = srcPtr[ch];
                    }

                    srcPtr += nChannels;
                    dstRowPtr += nChannels * direction;
                }

                srcPtr = (double*)((byte*)srcPtr + offset);
                dstPtr = (double*)((byte*)dstPtr + dstStride + verticalShift);
            }
        }

    }
}
