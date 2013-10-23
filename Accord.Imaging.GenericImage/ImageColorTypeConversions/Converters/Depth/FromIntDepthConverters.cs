using System;

namespace Accord.Imaging.Converters
{
    class FromIntDepthConverters
    {
        public unsafe static void ConvertIntToByte(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                int* srcPtr = (int*)src.ImageData + channel;
                byte* destPtr = (byte*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (byte)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(int);
                    destPtr += destStride / sizeof(byte);
                }
            }
        }

        public unsafe static void ConvertIntToShort(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                int* srcPtr = (int*)src.ImageData + channel;
                short* destPtr = (short*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (short)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(int);
                    destPtr += destStride / sizeof(short);
                }
            }
        }

        public unsafe static void ConvertIntToFloat(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                int* srcPtr = (int*)src.ImageData + channel;
                float* destPtr = (float*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(int);
                    destPtr += destStride / sizeof(float);
                }
            }
        }

        public unsafe static void ConvertIntToDouble(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.NumberOfChannels;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.NumberOfChannels; channel++)
            {
                int* srcPtr = (int*)src.ImageData + channel;
                double* destPtr = (double*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(int);
                    destPtr += destStride / sizeof(double);
                }
            }
        }

    }
}
