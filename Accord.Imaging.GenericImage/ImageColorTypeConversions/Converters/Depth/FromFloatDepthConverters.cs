using System;

namespace Accord.Imaging.Converters
{
    class FromFloatDepthConverters
    {
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

        public unsafe static void ConvertFloatToShort(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (short channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                float* srcPtr = (float*)src.ImageData + channel;
                short* destPtr = (short*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (short)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(float);
                    destPtr += destStride / sizeof(short);
                }
            }
        }

        public unsafe static void ConvertFloatToInt(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                float* srcPtr = (float*)src.ImageData + channel;
                int* destPtr = (int*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (int)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(float);
                    destPtr += destStride / sizeof(int);
                }
            }
        }

        public unsafe static void ConvertFloatToDouble(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                float* srcPtr = (float*)src.ImageData + channel;
                double* destPtr = (double*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(float);
                    destPtr += destStride / sizeof(double);
                }
            }
        }

    }
}
