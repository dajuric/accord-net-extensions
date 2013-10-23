using Accord.Core;
using Accord.Imaging.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
    public static class ValueSetter
    {
        delegate void SetValueFunc(IImage image, Array color);
        static Dictionary<Type, SetValueFunc> valueSetters;

        static ValueSetter()
        {
            valueSetters = new Dictionary<Type, SetValueFunc>();
            valueSetters.Add(typeof(byte), setValueByte);
            valueSetters.Add(typeof(short), setValueShort);
            valueSetters.Add(typeof(int), setValueInt);
            valueSetters.Add(typeof(float), setValueFloat);
            valueSetters.Add(typeof(double), setValueDouble);
        }

        /// <summary>
        /// Sets value for an input image.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="value">The color to set.</param>
        public static void SetValue<TColor, TDepth>(this Image<TColor, TDepth> img, TColor value)
            where TColor : IColor
            where TDepth : struct
        {
            var arr = HelperMethods.ColorToArray<TColor, TDepth>(value);
            SetValue(img, arr);
        }

        internal static void SetValue<TColor, TDepth>(this Image<TColor, TDepth> img, TDepth[] valueArr)
            where TColor : IColor
            where TDepth : struct
        {
            if (valueArr.Length > img.ColorInfo.NumberOfChannels)
                throw new Exception("Value array length must be the same as number of channels (or less)!");

            Type depthType = img.ColorInfo.ChannelType;

            SetValueFunc valueSetter = null;
            if (valueSetters.TryGetValue(typeof(TDepth), out valueSetter) == false)
            {
                throw new Exception(string.Format("Setter function can not split image of color depth type {0}", depthType));
            }

            ParallelProcessor<IImage, bool> proc = new ParallelProcessor<IImage, bool>(img.Size,
                                                                                            () => //called once
                                                                                            {
                                                                                                return true;
                                                                                            },

                                                                                            (IImage srcImg, bool dummy, Rectangle area) => //called for every thread
                                                                                            {
                                                                                                valueSetter(srcImg.GetSubRect(area), valueArr);
                                                                                            }
                                                                                            /*,new ParallelOptions { ForceSequential = true}*/);

            proc.Process(img); //result is in srcImg
        }

        #region Value Setters

        private unsafe static void setValueByte(IImage srcImg, Array color)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(byte);

            int channelIdx = 0;
            foreach (var channelVal in color.Cast<byte>())
            {
                byte* srcPtr = (byte*)srcImg.ImageData + channelIdx;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        *srcPtr = channelVal;
                        srcPtr += nChannels;
                    }

                    srcPtr = (byte*)((byte*)srcPtr + srcShift);
                }

                channelIdx++;
            }
        }

        private unsafe static void setValueShort(IImage srcImg, Array color)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(short);

            int channelIdx = 0;
            foreach (var channelVal in color.Cast<short>())
            {
                short* srcPtr = (short*)srcImg.ImageData + channelIdx;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        *srcPtr = channelVal;
                        srcPtr += nChannels;
                    }

                    srcPtr = (short*)((byte*)srcPtr + srcShift);
                }

                channelIdx++;
            }
        }

        private unsafe static void setValueInt(IImage srcImg, Array color)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(int);

            int channelIdx = 0;
            foreach (var channelVal in color.Cast<int>())
            {
                int* srcPtr = (int*)srcImg.ImageData + channelIdx;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        *srcPtr = channelVal;
                        srcPtr += nChannels;
                    }

                    srcPtr = (int*)((byte*)srcPtr + srcShift);
                }

                channelIdx++;
            }
        }

        private unsafe static void setValueFloat(IImage srcImg, Array color)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(float);

            int channelIdx = 0;
            foreach (var channelVal in color.Cast<float>())
            {
                float* srcPtr = (float*)srcImg.ImageData + channelIdx;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        *srcPtr = channelVal;
                        srcPtr += nChannels;
                    }

                    srcPtr = (float*)((byte*)srcPtr + srcShift);
                }

                channelIdx++;
            }
        }

        private unsafe static void setValueDouble(IImage srcImg, Array color)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(double);

            int channelIdx = 0;
            foreach (var channelVal in color.Cast<double>())
            {
                double* srcPtr = (double*)srcImg.ImageData + channelIdx;

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        *srcPtr = channelVal;
                        srcPtr += nChannels;
                    }

                    srcPtr = (double*)((byte*)srcPtr + srcShift);
                }

                channelIdx++;
            }
        }

        #endregion

    }
}
