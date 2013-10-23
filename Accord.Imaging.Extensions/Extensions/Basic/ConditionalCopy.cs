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
    public static class ConditionalCopy
    {
        delegate void ConditionalCopyFunc(IImage image, IImage destImage, Image<Gray, byte> mask);
        static Dictionary<Type, ConditionalCopyFunc> conditionalCopyFuncs;

        static ConditionalCopy()
        {
            conditionalCopyFuncs = new Dictionary<Type, ConditionalCopyFunc>();
            conditionalCopyFuncs.Add(typeof(byte), conditionalCopyByte);
            conditionalCopyFuncs.Add(typeof(short), conditionalCopyShort);
            conditionalCopyFuncs.Add(typeof(int), conditionalCopyInt);
            conditionalCopyFuncs.Add(typeof(float), conditionalCopyFloat);
            conditionalCopyFuncs.Add(typeof(double), conditionalCopyDouble);
        }

        /// <summary>
        /// Copies values from source to destination image using mask. Destination values where mask == 0 are not erased!.
        /// </summary>
        /// <param name="destImg">Destination image</param>
        /// <param name="mask">Mask. Color locations that need to be copied must be set to !=0 in mask.</param>
        public static void Copy<TColor, TDepth>(this Image<TColor, TDepth> img, Image<TColor, TDepth> destImg, Image<Gray, byte> mask)
            where TColor:IColor
            where TDepth:struct
        {
            Copy((IImage)img, destImg, mask);
        }

        internal static void Copy(this IImage img, IImage destImg, Image<Gray, byte> mask)
        {
            if (img.Size != mask.Size || img.Size != destImg.Size)
                throw new Exception("Image, mask, destImg size must be the same!");

            if (ColorInfo.Equals(img.ColorInfo, destImg.ColorInfo, ColorInfo.ComparableParts.Castable) == false)
                throw new Exception("Image and dest image must be castable (the same number of channels, the same channel type)!");
           
            Type depthType = img.ColorInfo.ChannelType;

            ConditionalCopyFunc conditionalCopyFunc = null;
            if (conditionalCopyFuncs.TryGetValue(depthType, out conditionalCopyFunc) == false)
            {
                throw new Exception(string.Format("Conditional copy function of color depth type {0}", depthType));
            }

            ParallelProcessor<IImage, IImage> proc = new ParallelProcessor<IImage, IImage>(img.Size,
                                                                                            () => //called once
                                                                                            {
                                                                                                return destImg;
                                                                                            },

                                                                                            (IImage srcImg, IImage dstImg, Rectangle area) => //called for every thread
                                                                                            {
                                                                                                conditionalCopyFunc(srcImg.GetSubRect(area), dstImg.GetSubRect(area), mask.GetSubRect(area));
                                                                                            }
                                                                                            /*,new ParallelOptions { ForceSequential = true}*/);

            proc.Process(img); //result is in destImg
        }

        #region Conditional Copy Functions

        private unsafe static void conditionalCopyByte(IImage srcImg, IImage dstImg, Image<Gray, byte> mask)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(byte);
            int dstShift = dstImg.Stride - width * nChannels * sizeof(byte);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            byte* srcPtr = (byte*)srcImg.ImageData;
            byte* dstPtr = (byte*)dstImg.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    if (*maskPtr != 0)
                    {
                        for (int ch = 0; ch < nChannels; ch++)
                        {
                            dstPtr[ch] = srcPtr[ch];
                        }
                    }

                    srcPtr += nChannels;
                    dstPtr += nChannels;
                    maskPtr++;
                }

                srcPtr = (byte*)((byte*)srcPtr + srcShift);
                dstPtr = (byte*)((byte*)dstPtr + dstShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void conditionalCopyShort(IImage srcImg, IImage dstImg, Image<Gray, byte> mask)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(short);
            int dstShift = dstImg.Stride - width * nChannels * sizeof(short);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            short* srcPtr = (short*)srcImg.ImageData;
            short* dstPtr = (short*)dstImg.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    if (*maskPtr != 0)
                    {
                        for (int ch = 0; ch < nChannels; ch++)
                        {
                            dstPtr[ch] = srcPtr[ch];
                        }
                    }

                    srcPtr += nChannels;
                    dstPtr += nChannels;
                    maskPtr++;
                }

                srcPtr = (short*)((byte*)srcPtr + srcShift);
                dstPtr = (short*)((byte*)dstPtr + dstShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void conditionalCopyInt(IImage srcImg, IImage dstImg, Image<Gray, byte> mask)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(int);
            int dstShift = dstImg.Stride - width * nChannels * sizeof(int);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            int* srcPtr = (int*)srcImg.ImageData;
            int* dstPtr = (int*)dstImg.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    if (*maskPtr != 0)
                    {
                        for (int ch = 0; ch < nChannels; ch++)
                        {
                            dstPtr[ch] = srcPtr[ch];
                        }
                    }

                    srcPtr += nChannels;
                    dstPtr += nChannels;
                    maskPtr++;
                }

                srcPtr = (int*)((byte*)srcPtr + srcShift);
                dstPtr = (int*)((byte*)dstPtr + dstShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void conditionalCopyFloat(IImage srcImg, IImage dstImg, Image<Gray, byte> mask)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(float);
            int dstShift = dstImg.Stride - width * nChannels * sizeof(float);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            float* srcPtr = (float*)srcImg.ImageData;
            float* dstPtr = (float*)dstImg.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    if (*maskPtr != 0)
                    {
                        for (int ch = 0; ch < nChannels; ch++)
                        {
                            dstPtr[ch] = srcPtr[ch];
                        }
                    }

                    srcPtr += nChannels;
                    dstPtr += nChannels;
                    maskPtr++;
                }

                srcPtr = (float*)((byte*)srcPtr + srcShift);
                dstPtr = (float*)((byte*)dstPtr + dstShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void conditionalCopyDouble(IImage srcImg, IImage dstImg, Image<Gray, byte> mask)
        {
            int width = srcImg.Width;
            int height = srcImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int srcShift = srcImg.Stride - width * nChannels * sizeof(double);
            int dstShift = dstImg.Stride - width * nChannels * sizeof(double);
            int maskShift = mask.Stride - width * 1 * sizeof(byte);

            double* srcPtr = (double*)srcImg.ImageData;
            double* dstPtr = (double*)dstImg.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    if (*maskPtr != 0)
                    {
                        for (int ch = 0; ch < nChannels; ch++)
                        {
                            dstPtr[ch] = srcPtr[ch];
                        }
                    }

                    srcPtr += nChannels;
                    dstPtr += nChannels;
                    maskPtr++;
                }

                srcPtr = (double*)((byte*)srcPtr + srcShift);
                dstPtr = (double*)((byte*)dstPtr + dstShift);
                maskPtr += maskShift;
            }
        }

        #endregion
    }
}
