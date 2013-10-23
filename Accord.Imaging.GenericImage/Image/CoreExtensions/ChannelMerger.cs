using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Accord.Core;

namespace Accord.Imaging
{
    internal static class ChannelMerger
    {
        delegate void MergeImage(IImage[] channels, IImage srcImg, int[] channelIndicies);
        static Dictionary<Type, MergeImage> mergers;

        static ChannelMerger()
        {
            mergers = new Dictionary<Type, MergeImage>();

            mergers.Add(typeof(byte), mergeImageByte);
            mergers.Add(typeof(short), mergeImageShort);
            mergers.Add(typeof(int), mergeImageInt);
            mergers.Add(typeof(float), mergeImageFloat);
            mergers.Add(typeof(double), mergeImageDouble);
        }

        /// <summary>
        /// Merges channels and creates an image.
        /// Number of channels must be lower or equal to the number of channels in image color.
        /// </summary>
        /// <param name="channels">Channels.</param>
        /// <param name="srcImg">Source image.</param>
        /// <param name="destChannelIndicies">Which channels to replace in the input image.</param>
        public static void MergeChannels<TColor, TDepth>(Image<Gray, TDepth>[] channels, Image<TColor, TDepth> srcImg, params int[] destChannelIndicies)
            where TColor : IColor
            where TDepth : struct
        {
            MergeChannels((IImage[])channels, srcImg, destChannelIndicies);
        }

        internal static void MergeChannels(IImage[] channels, IImage destImg, params int[] destChannelIndicies)
        {
            if (destChannelIndicies == null || destChannelIndicies.Length == 0)
            {
                destChannelIndicies = Enumerable.Range(0, channels.Length).ToArray();
            }

            Type depthType = destImg.ColorInfo.ChannelType;

            MergeImage merger = null;
            if (mergers.TryGetValue(depthType, out merger) == false)
            {
                throw new Exception(string.Format("Merge function can not merge image of color depth type {0}", depthType));
            }

            ParallelProcessor<IImage[], IImage> proc = new ParallelProcessor<IImage[], IImage>(destImg.Size,
                                                                                            () => //called once
                                                                                            {
                                                                                                return destImg;
                                                                                            },

                                                                                            (IImage[] _channels, IImage _destImg, Rectangle area) => //called for every thread
                                                                                            {
                                                                                                IImage[] channelsPatches = new IImage[_channels.Length];

                                                                                                for (int i = 0; i < channelsPatches.Length; i++)
                                                                                                {
                                                                                                    channelsPatches[i] = _channels[i].GetSubRect(area);
                                                                                                }

                                                                                                IImage imgPatch = _destImg.GetSubRect(area);

                                                                                                merger(channelsPatches, imgPatch, destChannelIndicies);
                                                                                            }
                                                                                            /*,new ParallelOptions { ForceSequential = true}*/);

            proc.Process(channels); //result is in srcImg
        }

        #region Specific functions

        private unsafe static void mergeImageByte(IImage[] channels, IImage srcImg, int[] destChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcShift = srcImg.Stride / sizeof(byte);
            int destShift = channels[0].Stride / sizeof(byte);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var destChannelIdx in destChannelIndicies)
            {
                byte* srcPtr = (byte*)srcImg.ImageData + destChannelIdx;
                byte* channelPtr = (byte*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        srcPtr[cSrc] = channelPtr[cDest];
                    }

                    srcPtr += srcShift;
                    channelPtr += destShift;
                }

                channelIdx++;
            }
        }

        private unsafe static void mergeImageShort(IImage[] channels, IImage srcImg, int[] destChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcShift = srcImg.Stride / sizeof(short);
            int destShift = channels[0].Stride / sizeof(short);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var destChannelIdx in destChannelIndicies)
            {
                short* srcPtr = (short*)srcImg.ImageData + destChannelIdx;
                short* channelPtr = (short*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        srcPtr[cSrc] = channelPtr[cDest];
                    }

                    srcPtr += srcShift;
                    channelPtr += destShift;
                }

                channelIdx++;
            }
        }

        private unsafe static void mergeImageInt(IImage[] channels, IImage srcImg, int[] destChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcShift = srcImg.Stride / sizeof(int);
            int destShift = channels[0].Stride / sizeof(int);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var destChannelIdx in destChannelIndicies)
            {
                int* srcPtr = (int*)srcImg.ImageData + destChannelIdx;
                int* channelPtr = (int*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        srcPtr[cSrc] = channelPtr[cDest];
                    }

                    srcPtr += srcShift;
                    channelPtr += destShift;
                }

                channelIdx++;
            }
        }

        private unsafe static void mergeImageFloat(IImage[] channels, IImage srcImg, int[] destChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcShift = srcImg.Stride / sizeof(float);
            int destShift = channels[0].Stride / sizeof(float);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var destChannelIdx in destChannelIndicies)
            {
                float* srcPtr = (float*)srcImg.ImageData + destChannelIdx;
                float* channelPtr = (float*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        srcPtr[cSrc] = channelPtr[cDest];
                    }

                    srcPtr += srcShift;
                    channelPtr += destShift;
                }

                channelIdx++;
            }
        }

        private unsafe static void mergeImageDouble(IImage[] channels, IImage srcImg, int[] destChannelIndicies)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;

            int srcShift = srcImg.Stride / sizeof(double);
            int destShift = channels[0].Stride / sizeof(double);

            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int channelIdx = 0;
            foreach (var destChannelIdx in destChannelIndicies)
            {
                double* srcPtr = (double*)srcImg.ImageData + destChannelIdx;
                double* channelPtr = (double*)channels[channelIdx].ImageData;

                for (int r = 0; r < height; r++)
                {
                    for (int cSrc = 0, cDest = 0; cDest < width; cSrc += nChannels, cDest++)
                    {
                        srcPtr[cSrc] = channelPtr[cDest];
                    }

                    srcPtr += srcShift;
                    channelPtr += destShift;
                }

                channelIdx++;
            }
        }

        #endregion
    }
}
