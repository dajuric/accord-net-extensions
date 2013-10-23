using System;

namespace Accord.Imaging.Converters
{
    static class FromByteDepthConverters
    {
        public unsafe static void ConvertByteToShort(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                byte* srcPtr = (byte*)src.ImageData + channel;
                short* destPtr = (short*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(byte);
                    destPtr += destStride / sizeof(short);
                }
            }
        }

        public unsafe static void ConvertByteToInt(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                byte* srcPtr = (byte*)src.ImageData + channel;
                int* destPtr = (int*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(byte);
                    destPtr += destStride / sizeof(int);
                }
            }
        }

        public unsafe static void ConvertByteToDouble(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                byte* srcPtr = (byte*)src.ImageData + channel;
                double* destPtr = (double*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(byte);
                    destPtr += destStride / sizeof(double);
                }
            }
        }

        public unsafe static void ConvertByteToFloat(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                byte* srcPtr = (byte*)src.ImageData + channel;
                float* destPtr = (float*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(byte);
                    destPtr += destStride / sizeof(float);
                }
            }
        }

        public unsafe static void ConvertFloatToByte(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                float* srcPtr = (float*)src.ImageData + channel;
                byte* destPtr = (byte*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (byte)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(float);
                    destPtr += destStride / sizeof(byte);
                }
            }
        }

    }
}
