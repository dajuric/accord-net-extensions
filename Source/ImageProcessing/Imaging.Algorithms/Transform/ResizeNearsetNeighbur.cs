#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Nearest-neighbor interpolation.
    /// <para>The methods of this class are used internally in Resize() method extension.</para> 
    /// </summary>
    internal static class ResizeNearsetNeighbur
    {
        delegate void ResizeFunc(IImage image, IImage destImage);
        static Dictionary<Type, ResizeFunc> resizeFuncs;

        static ResizeNearsetNeighbur()
        {
            resizeFuncs = new Dictionary<Type, ResizeFunc>();

            resizeFuncs.Add(typeof(byte), resizeByte);
            resizeFuncs.Add(typeof(short), resizeShort);
            resizeFuncs.Add(typeof(int), resizeInt);
            resizeFuncs.Add(typeof(float), resizeFloat);
            resizeFuncs.Add(typeof(double), resizeDouble);
        }

        /// <summary>
        /// Resizes the input image by using nearest neighbor interpolation.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="newSize">New image size.</param>
        /// <returns>Resized image.</returns>
        internal static Image<TColor> Resize<TColor>(Image<TColor> img, Size newSize)
            where TColor : struct, IColor
        {
            var resizedIm = new TColor[newSize.Height, newSize.Width].Lock();
            Resize((IImage)img, resizedIm);
            return resizedIm;
        }

        internal static TColor[,] Resize<TColor>(TColor[,] img, Size newSize)
            where TColor : struct, IColor
        {
            var resizedIm = new TColor[newSize.Height, newSize.Width];

            using (var uImg = img.Lock())
            using (var uResizedIm = resizedIm.Lock())
            {
                Resize((IImage)uImg, uResizedIm);
            }
           
            return resizedIm;
        }

        internal static void Resize(IImage img, IImage destImg)
        {
            if (img.ColorInfo.Equals(destImg.ColorInfo, ColorInfo.ComparableParts.Castable) == false)
                throw new Exception("Image and dest image must be at least castable (the same number of channels, the same channel type)!");

            Type depthType = img.ColorInfo.ChannelType;

            ResizeFunc resizeFunc = null;
            if (resizeFuncs.TryGetValue(depthType, out resizeFunc) == false)
            {
                throw new Exception(string.Format("Resize NN function of color depth type {0}", depthType));
            }

            resizeFunc(img, destImg);
        }

        #region Data type specific functions

        private unsafe static void resizeByte(IImage srcImg, IImage dstImg)
        {
            float xFactor = (float)srcImg.Width / dstImg.Width;
            float yFactor = (float)srcImg.Height / dstImg.Height;

            int newWidth = dstImg.Width;
            int newHeight = dstImg.Height;
            int nChannels = srcImg.ColorInfo.ChannelCount;

            int dstShift = dstImg.Stride - newWidth * srcImg.ColorInfo.Size;

            byte* srcPtr = (byte*)srcImg.ImageData;
            byte* dstPtr = (byte*)dstImg.ImageData;

            for (int r = 0; r < newHeight; r++)
            {
                byte* srcRowPtr = (byte*)srcImg.GetData((int)(r * yFactor));

                for (int c = 0; c < newWidth; c++)
                {
                    byte* srcColPtr = srcRowPtr + nChannels * (int)(c * xFactor);

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstPtr[ch] = srcColPtr[ch];
                    }

                    dstPtr += nChannels;
                }

                dstPtr = (byte*)((byte*)dstPtr + dstShift);
            }
        }

        private unsafe static void resizeShort(IImage srcImg, IImage dstImg)
        {
            float xFactor = (float)srcImg.Width / dstImg.Width;
            float yFactor = (float)srcImg.Height / dstImg.Height;

            int newWidth = dstImg.Width;
            int newHeight = dstImg.Height;
            int nChannels = srcImg.ColorInfo.ChannelCount;

            int dstShift = dstImg.Stride - newWidth * srcImg.ColorInfo.Size;

            short* srcPtr = (short*)srcImg.ImageData;
            short* dstPtr = (short*)dstImg.ImageData;

            for (int r = 0; r < newHeight; r++)
            {
                short* srcRowPtr = (short*)srcImg.GetData((int)(r * yFactor));

                for (int c = 0; c < newWidth; c++)
                {
                    short* srcColPtr = srcRowPtr + nChannels * (int)(c * xFactor);

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstPtr[ch] = srcColPtr[ch];
                    }

                    dstPtr += nChannels;
                }

                dstPtr = (short*)((short*)dstPtr + dstShift);
            }
        }

        private unsafe static void resizeInt(IImage srcImg, IImage dstImg)
        {
            float xFactor = (float)srcImg.Width / dstImg.Width;
            float yFactor = (float)srcImg.Height / dstImg.Height;

            int newWidth = dstImg.Width;
            int newHeight = dstImg.Height;
            int nChannels = srcImg.ColorInfo.ChannelCount;

            int dstShift = dstImg.Stride - newWidth * srcImg.ColorInfo.Size;

            int* srcPtr = (int*)srcImg.ImageData;
            int* dstPtr = (int*)dstImg.ImageData;

            for (int r = 0; r < newHeight; r++)
            {
                int* srcRowPtr = (int*)srcImg.GetData((int)(r * yFactor));

                for (int c = 0; c < newWidth; c++)
                {
                    int* srcColPtr = srcRowPtr + nChannels * (int)(c * xFactor);

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstPtr[ch] = srcColPtr[ch];
                    }

                    dstPtr += nChannels;
                }

                dstPtr = (int*)((int*)dstPtr + dstShift);
            }
        }

        private unsafe static void resizeFloat(IImage srcImg, IImage dstImg)
        {
            float xFactor = (float) srcImg.Width / dstImg.Width;
            float yFactor = (float) srcImg.Height / dstImg.Height;

            int newWidth = dstImg.Width;
            int newHeight = dstImg.Height;
            int nChannels = srcImg.ColorInfo.ChannelCount;

            int dstShift = dstImg.Stride - newWidth * srcImg.ColorInfo.Size;

            float* srcPtr = (float*)srcImg.ImageData;
            float* dstPtr = (float*)dstImg.ImageData;

            for (int r = 0; r < newHeight; r++)
            {
                float* srcRowPtr = (float*)srcImg.GetData((int)(r * yFactor));

                for (int c = 0; c < newWidth; c++)
                {
                    float* srcColPtr = srcRowPtr + nChannels * (int)(c * xFactor);

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstPtr[ch] = srcColPtr[ch];
                    }

                    dstPtr += nChannels;
                }

                dstPtr = (float*)((byte*)dstPtr + dstShift);
            }
        }

        private unsafe static void resizeDouble(IImage srcImg, IImage dstImg)
        {
            float xFactor = (float)srcImg.Width / dstImg.Width;
            float yFactor = (float)srcImg.Height / dstImg.Height;

            int newWidth = dstImg.Width;
            int newHeight = dstImg.Height;
            int nChannels = srcImg.ColorInfo.ChannelCount;

            int dstShift = dstImg.Stride - newWidth * srcImg.ColorInfo.Size;

            double* srcPtr = (double*)srcImg.ImageData;
            double* dstPtr = (double*)dstImg.ImageData;

            for (int r = 0; r < newHeight; r++)
            {
                double* srcRowPtr = (double*)srcImg.GetData((int)(r * yFactor));

                for (int c = 0; c < newWidth; c++)
                {
                    double* srcColPtr = srcRowPtr + nChannels * (int)(c * xFactor);

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstPtr[ch] = srcColPtr[ch];
                    }

                    dstPtr += nChannels;
                }

                dstPtr = (double*)((double*)dstPtr + dstShift);
            }
        }

        #endregion
    }
}
