using Accord.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
    public static class ChannelSplitter
    {
        delegate void SplitImage(IImage srcImg, IImage[] channels, int[] channelIndicies);
        static Dictionary<Type, SplitImage> splitters;

        static ChannelSplitter()
        {
            splitters = new Dictionary<Type, SplitImage>();

            splitters.Add(typeof(byte), splitImageByte);
            splitters.Add(typeof(short), splitImageShort);
            splitters.Add(typeof(int), splitImageInt);
            splitters.Add(typeof(float), splitImageFloat);
            splitters.Add(typeof(double), splitImageDouble);
        }

        /// <summary>
        /// Splits the image into channels.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="channelIndicies">Which channels to split. If zero length then all channels will be processed.</param>
        /// <returns>Channels.</returns>
        public static Image<Gray, TDepth>[] SplitChannels<TColor, TDepth>(this Image<TColor, TDepth> img, params int[] channelIndicies)
            where TColor : IColor
            where TDepth : struct
        {
            return SplitChannels((IImage)img, channelIndicies).Cast<Image<Gray, TDepth>>().ToArray();
        }

        internal static IImage[] SplitChannels(IImage img, params int[] channelIndicies)
        {
            if (channelIndicies == null || channelIndicies.Length == 0)
            {
                channelIndicies = Enumerable.Range(0, img.ColorInfo.NumberOfChannels).ToArray();
            }

            Type depthType = img.ColorInfo.ChannelType;
            ColorInfo channelType = ColorInfo.GetInfo(typeof(Gray), depthType);

            SplitImage splitter = null;
            if (splitters.TryGetValue(depthType, out splitter) == false)
            {
                throw new Exception(string.Format("Splitting function can not split image of color depth type {0}", depthType));
            }

            ParallelProcessor<IImage, IImage[]> proc = new ParallelProcessor<IImage, IImage[]>(img.Size,
                                                                                            () => //called once
                                                                                            {
                                                                                                IImage[] channels = new IImage[channelIndicies.Length];
                                                                                                for (int i = 0; i < channels.Length; i++)
                                                                                                {
                                                                                                    channels[i] = GenericImageBase.Create(channelType, img.Width, img.Height);
                                                                                                }

                                                                                                return channels;
                                                                                            },

                                                                                            (IImage src, IImage[] dest, Rectangle area) => //called for every thread
                                                                                            {
                                                                                                IImage srcPatch = src.GetSubRect(area);
                                                                                                IImage[] destPatch = new IImage[dest.Length];

                                                                                                for (int i = 0; i < dest.Length; i++)
                                                                                                {
                                                                                                    destPatch[i] = dest[i].GetSubRect(area);
                                                                                                }

                                                                                                splitter(srcPatch, destPatch, channelIndicies);
                                                                                            }
                                                                                            /*,new ParallelOptions { ForceSequential = true}*/);

            return proc.Process(img);
        }

        #region Specific functions

        private unsafe static void splitImageByte(IImage srcImg, IImage[] channels, int[] srcChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcStride = srcImg.Stride / sizeof(byte);
            int destStride = channels[0].Stride / sizeof(byte);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var srcChannelIdx in srcChannelIndicies)
            {
                byte* srcPtr = (byte*)srcImg.ImageData + srcChannelIdx;
                byte* channelPtr = (byte*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        channelPtr[cDest] = srcPtr[cSrc];
                    }

                    srcPtr += srcStride;
                    channelPtr += destStride;
                }

                channelIdx++;
            }
        }

        private unsafe static void splitImageShort(IImage srcImg, IImage[] channels, int[] srcChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcStride = srcImg.Stride / sizeof(short);
            int destStride = channels[0].Stride / sizeof(short);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var srcChannelIdx in srcChannelIndicies)
            {
                short* srcPtr = (short*)srcImg.ImageData + srcChannelIdx;
                short* channelPtr = (short*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        channelPtr[cDest] = srcPtr[cSrc];
                    }

                    srcPtr += srcStride;
                    channelPtr += destStride;
                }

                channelIdx++;
            }
        }

        private unsafe static void splitImageInt(IImage srcImg, IImage[] channels, int[] srcChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcStride = srcImg.Stride / sizeof(int);
            int destStride = channels[0].Stride / sizeof(int);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var srcChannelIdx in srcChannelIndicies)
            {
                int* srcPtr = (int*)srcImg.ImageData + srcChannelIdx;
                int* channelPtr = (int*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        channelPtr[cDest] = srcPtr[cSrc];
                    }

                    srcPtr += srcStride;
                    channelPtr += destStride;
                }

                channelIdx++;
            }
        }

        private unsafe static void splitImageFloat(IImage srcImg, IImage[] channels, int[] srcChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcStride = srcImg.Stride / sizeof(float);
            int destStride = channels[0].Stride / sizeof(float);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var srcChannelIdx in srcChannelIndicies)
            {
                float* srcPtr = (float*)srcImg.ImageData + srcChannelIdx;
                float* channelPtr = (float*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        channelPtr[cDest] = srcPtr[cSrc];
                    }

                    srcPtr += srcStride;
                    channelPtr += destStride;
                }

                channelIdx++;
            }
        }

        private unsafe static void splitImageDouble(IImage srcImg, IImage[] channels, int[] srcChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcStride = srcImg.Stride / sizeof(double);
            int destStride = channels[0].Stride / sizeof(double);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var srcChannelIdx in srcChannelIndicies)
            {
                double* srcPtr = (double*)srcImg.ImageData + srcChannelIdx;
                double* channelPtr = (double*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        channelPtr[cDest] = srcPtr[cSrc];
                    }

                    srcPtr += srcStride;
                    channelPtr += destStride;
                }

                channelIdx++;
            }
        }

        #endregion

    }
}
