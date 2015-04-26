using Accord.Extensions;
using System;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides channel depth conversion extensions.
    /// </summary>
    public static class PrimitiveChannelTypeConversionExtensions
    {
        /// <summary>
        /// Converts each channel of the source color to the channel of the specified destination color.
        /// </summary>
        /// <typeparam name="TColorSrc">Source color type.</typeparam>
        /// <typeparam name="TColorDst">Destination color type.</typeparam>
        /// <param name="source">Source array.</param>
        /// <returns>Array which elements have changed depth.</returns>
        public static TColorDst[,] ConvertChannelDepth<TColorSrc, TColorDst>(this TColorSrc[,] source)
            where TColorSrc : struct
            where TColorDst : struct
        {
            var area = new Rectangle(0, 0, source.Width(), source.Height());
            return source.ConvertChannelDepth<TColorSrc, TColorDst>(area); 
        }

        /// <summary>
        /// Converts each channel of the source color to the channel of the specified destination color.
        /// </summary>
        /// <typeparam name="TColorSrc">Source color type.</typeparam>
        /// <typeparam name="TColorDst">Destination color type.</typeparam>
        /// <param name="source">Source array.</param>
        /// <param name="area">Working area.</param>
        /// <returns>Array which elements have changed depth.</returns>
        public static TColorDst[,] ConvertChannelDepth<TColorSrc, TColorDst>(this TColorSrc[,] source, Rectangle area)
            where TColorSrc : struct
            where TColorDst : struct
        {
            TColorDst[,] destination = null;
            using (var srcImg = source.Lock(area))
            {
                destination = srcImg.ConvertChannelDepth<TColorSrc, TColorDst>();
            }

            return destination;
        }

        /// <summary>
        /// Converts each channel of the source color to the channel of the specified destination color.
        /// </summary>
        /// <typeparam name="TColorSrc">Source color type.</typeparam>
        /// <typeparam name="TColorDst">Destination color type.</typeparam>
        /// <param name="source">Source image.</param>
        /// <returns>Array which elements have changed depth.</returns>
        public static TColorDst[,] ConvertChannelDepth<TColorSrc, TColorDst>(this Image<TColorSrc> source)
            where TColorSrc : struct
            where TColorDst : struct
        {
            if (typeof(TColorSrc) == typeof(TColorDst))
                return null;
                //return source.CopyTo(...);

            var destination = new TColorDst[source.Height, source.Width];
            using (var dstImg = destination.Lock())
            {
                ConvertChannelDepth(source, dstImg);
            }
            return destination;
        }

        private static void ConvertChannelDepth<TColorSrc, TColorDst>(Image<TColorSrc> source, Image<TColorDst> destination)
            where TColorSrc: struct
            where TColorDst: struct
        {
            if (source.ColorInfo.ChannelCount != destination.ColorInfo.ChannelCount)
            {
                throw new NotSupportedException("Source and destination color must be binary compatible!");
            }
               
            Type sourceChannelType = source.ColorInfo.ChannelType;
            Type destChannelType = destination.ColorInfo.ChannelType;

            if (sourceChannelType == typeof(byte))
            {
                if (destChannelType == typeof(byte))
                    return;
                else if (destChannelType == typeof(short))
                    ConvertByteToShort(source, destination);
                else if (destChannelType == typeof(int))
                    ConvertByteToInt(source, destination);
                else if (destChannelType == typeof(float))
                    ConvertByteToFloat(source, destination);
                else if (destChannelType == typeof(double))
                    ConvertByteToDouble(source, destination);
                else
                    throw new NotSupportedException();
            }
            else if (sourceChannelType == typeof(short))
            {
                if (destChannelType == typeof(byte))
                    ConvertShortToByte(source, destination);
                else if (destChannelType == typeof(short))
                    return;
                else if (destChannelType == typeof(int))
                    ConvertShortToInt(source, destination);
                else if (destChannelType == typeof(float))
                    ConvertShortToFloat(source, destination);
                else if (destChannelType == typeof(double))
                    ConvertShortToDouble(source, destination);
                else
                    throw new NotSupportedException();
            }
            else if (sourceChannelType == typeof(int))
            {
                if (destChannelType == typeof(byte))
                    ConvertIntToByte(source, destination);
                else if (destChannelType == typeof(short))
                    ConvertIntToShort(source, destination);
                else if (destChannelType == typeof(int))
                    return;
                else if (destChannelType == typeof(float))
                    ConvertIntToFloat(source, destination);
                else if (destChannelType == typeof(double))
                    ConvertIntToDouble(source, destination);
                else
                    throw new NotSupportedException();
            }
            else if (sourceChannelType == typeof(float))
            {
                if (destChannelType == typeof(byte))
                    ConvertFloatToByte(source, destination);
                else if (destChannelType == typeof(short))
                    ConvertFloatToShort(source, destination);
                else if (destChannelType == typeof(int))
                    ConvertFloatToInt(source, destination);
                else if (destChannelType == typeof(float))
                    return;
                else if (destChannelType == typeof(double))
                    ConvertFloatToDouble(source, destination);
                else
                    throw new NotSupportedException();
            }
            else if (sourceChannelType == typeof(float))
            {
                if (destChannelType == typeof(byte))
                    ConvertDoubleToByte(source, destination);
                else if (destChannelType == typeof(short))
                    ConvertDoubleToShort(source, destination);
                else if (destChannelType == typeof(int))
                    ConvertDoubleToInt(source, destination);
                else if (destChannelType == typeof(float))
                    ConvertDoubleToFloat(source, destination);
                else if (destChannelType == typeof(double))
                    return;
                else
                    throw new NotSupportedException();
            }
            else
                throw new NotSupportedException();
        }

        #region ByteToX converters

        private unsafe static void ConvertByteToShort(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        private unsafe static void ConvertByteToInt(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        private unsafe static void ConvertByteToDouble(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        private unsafe static void ConvertByteToFloat(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        #endregion

        #region ShortToX converters

        private unsafe static void ConvertShortToByte(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
            {
                short* srcPtr = (short*)src.ImageData + channel;
                byte* destPtr = (byte*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (byte)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(short);
                    destPtr += destStride / sizeof(byte);
                }
            }
        }

        private unsafe static void ConvertShortToInt(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
            {
                short* srcPtr = (short*)src.ImageData + channel;
                int* destPtr = (int*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(short);
                    destPtr += destStride / sizeof(int);
                }
            }
        }

        private unsafe static void ConvertShortToFloat(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
            {
                short* srcPtr = (short*)src.ImageData + channel;
                float* destPtr = (float*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(short);
                    destPtr += destStride / sizeof(float);
                }
            }
        }

        private unsafe static void ConvertShortToDouble(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
            {
                short* srcPtr = (short*)src.ImageData + channel;
                double* destPtr = (double*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(short);
                    destPtr += destStride / sizeof(double);
                }
            }
        }

        #endregion

        #region Int32ToX converters

        private unsafe static void ConvertIntToByte(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        private unsafe static void ConvertIntToShort(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        private unsafe static void ConvertIntToFloat(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        private unsafe static void ConvertIntToDouble(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        #endregion

        #region SingleToX converters

        private unsafe static void ConvertFloatToByte(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        private unsafe static void ConvertFloatToShort(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (short channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        private unsafe static void ConvertFloatToInt(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        private unsafe static void ConvertFloatToDouble(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
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

        #endregion

        #region DoubleToX converters

        private unsafe static void ConvertDoubleToByte(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
            {
                double* srcPtr = (double*)src.ImageData + channel;
                byte* destPtr = (byte*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (byte)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(double);
                    destPtr += destStride / sizeof(byte);
                }
            }
        }

        private unsafe static void ConvertDoubleToShort(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
            {
                double* srcPtr = (double*)src.ImageData + channel;
                short* destPtr = (short*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (short)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(double);
                    destPtr += destStride / sizeof(short);
                }
            }
        }

        private unsafe static void ConvertDoubleToInt(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
            {
                double* srcPtr = (double*)src.ImageData + channel;
                int* destPtr = (int*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (int)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(double);
                    destPtr += destStride / sizeof(int);
                }
            }
        }

        private unsafe static void ConvertDoubleToFloat(IImage src, IImage dest)
        {
            int nChannels = src.ColorInfo.ChannelCount;
            int elemWidth = src.Width * nChannels;
            int height = src.Height;

            int srcStride = src.Stride;
            int destStride = dest.Stride;

            for (int channel = 0; channel < src.ColorInfo.ChannelCount; channel++)
            {
                double* srcPtr = (double*)src.ImageData + channel;
                float* destPtr = (float*)dest.ImageData + channel;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < elemWidth; c += nChannels)
                    {
                        destPtr[c] = (float)srcPtr[c];
                    }

                    srcPtr += srcStride / sizeof(double);
                    destPtr += destStride / sizeof(float);
                }
            }
        }

        #endregion
    }
}
