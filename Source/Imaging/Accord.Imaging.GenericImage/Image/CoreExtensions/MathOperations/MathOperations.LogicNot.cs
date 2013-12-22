using Accord.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Accord.Imaging
{
    public static class LogicNot
    {
        delegate void NotFunc(IImage src, IImage dest, Image<Gray, byte> mask);
        static Dictionary<Type, NotFunc> notFuncs;
        
        static LogicNot()
        {
            notFuncs = new Dictionary<Type, NotFunc>();

            notFuncs.Add(typeof(byte), not_Byte);
            notFuncs.Add(typeof(short), not_Short);
            notFuncs.Add(typeof(int), not_Int);
        }

        private static void calculate(IImage src, IImage dest, Image<Gray, byte> mask = null)
        {          
            if (mask == null)
            {
                mask = new Image<Gray, byte>(dest.Width, dest.Height);
                mask.SetValue(new Gray(255));
            }

            NotFunc mathOpFunc = null;
            if (notFuncs.TryGetValue(src.ColorInfo.ChannelType, out mathOpFunc) == false)
                throw new Exception(string.Format("Bitwise NOT can not be executed on an image of type {0}", src.ColorInfo.ChannelType));

            var proc = new ParallelProcessor<bool, bool>(dest.Size,
                                                            () =>
                                                            {
                                                                return true;
                                                            },
                                                            (bool _, bool __, Rectangle area) =>
                                                            {
                                                                var srcPatch = src.GetSubRect(area);
                                                                var destPatch = dest.GetSubRect(area);
                                                                var maskPatch = mask.GetSubRect(area);

                                                                mathOpFunc(srcPatch, destPatch, maskPatch);
                                                            }
                /*,new ParallelOptions { ForceSequential = true}*/);

            proc.Process(true);
        }


        #region Specific implementations

        private unsafe static void not_Byte(IImage src, IImage dest, Image<Gray, byte> mask)
        {
            byte* srcPtr = (byte*)src.ImageData;
            byte* destPtr = (byte*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int srcShift = src.Stride - width * nChannels * sizeof(byte);
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
                        *destPtr = (byte)(~(*srcPtr));

                        srcPtr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                srcPtr = (byte*)((byte*)srcPtr + srcShift);
                destPtr = (byte*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void not_Short(IImage src, IImage dest, Image<Gray, byte> mask)
        {
            short* srcPtr = (short*)src.ImageData;
            short* destPtr = (short*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int srcShift = src.Stride - width * nChannels * sizeof(short);
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
                        *destPtr = (short)(~(*srcPtr));

                        srcPtr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                srcPtr = (short*)((byte*)srcPtr + srcShift);
                destPtr = (short*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        private unsafe static void not_Int(IImage src, IImage dest, Image<Gray, byte> mask)
        {
            int* srcPtr = (int*)src.ImageData;
            int* destPtr = (int*)dest.ImageData;
            byte* maskPtr = (byte*)mask.ImageData;

            int width = dest.Width;
            int height = dest.Height;
            int nChannels = dest.ColorInfo.NumberOfChannels;

            int srcShift = src.Stride - width * nChannels * sizeof(int);
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
                        *destPtr = (int)(~(*srcPtr));

                        srcPtr++;
                        destPtr++;
                    }

                    maskPtr++;
                }

                srcPtr = (int*)((byte*)srcPtr + srcShift);
                destPtr = (int*)((byte*)destPtr + destShift);
                maskPtr += maskShift;
            }
        }

        #endregion

        #region Extensions

        private static Image<TColor, TDepth> Not<TColor, TDepth>(Image<TColor, TDepth> img, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
            where TDepth : struct
        {
            IImage dest = img;
            if (!inPlace)
                dest = img.CopyBlank();

            calculate(img, dest, mask);
            return dest as Image<TColor, TDepth>;
        }

        /// <summary>
        /// Performs bitwise NOT operation on image.
        /// </summary>
        /// <param name="inPlace">Perfom this operation on original image or not.</param>
        /// <param name="mask">Execuute this operation only where mask != 0.</param>
        /// <returns>Processed image. If <see cref="inPlace"/> is set to true returned value can be discarded.</returns>
        public static Image<TColor, byte> Not<TColor>(this Image<TColor, byte> img, bool inPlace = false, Image<Gray, byte> mask = null)
            where TColor : IColor
        {
            return Not<TColor, byte>(img, inPlace, mask);
        }

        /// <summary>
        /// Performs bitwise NOT operation on image.
        /// </summary>
        /// <param name="inPlace">Perfom this operation on original image or not.</param>
        /// <param name="mask">Execuute this operation only where mask != 0.</param>
        /// <returns>Processed image. If <see cref="inPlace"/> is set to true returned value can be discarded.</returns>
        public static Image<TColor, short> Not<TColor>(this Image<TColor, short> img, bool inPlace = false, Image<Gray, byte> mask = null)
          where TColor : IColor
        {
            return Not<TColor, short>(img, inPlace, mask);
        }

        /// <summary>
        /// Performs bitwise NOT operation on image.
        /// </summary>
        /// <param name="inPlace">Perfom this operation on original image or not.</param>
        /// <param name="mask">Execuute this operation only where mask != 0.</param>
        /// <returns>Processed image. If <see cref="inPlace"/> is set to true returned value can be discarded.</returns>
        public static Image<TColor, int> Not<TColor>(this Image<TColor, int> img, bool inPlace = false, Image<Gray, byte> mask = null)
           where TColor : IColor
        {
            return Not<TColor, int>(img, inPlace, mask);
        }

        #endregion
    }
}
