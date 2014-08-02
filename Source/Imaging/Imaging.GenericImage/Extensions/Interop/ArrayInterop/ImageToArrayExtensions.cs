#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
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
using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for converting an image to an array representation.
    /// </summary>
    public static class ImageToArrayExtensions
    {
        private unsafe static Array toArray(IImage image)
        {
            Array arr = Array.CreateInstance(image.ColorInfo.ChannelType, image.ColorInfo.NumberOfChannels, image.Height, image.Width);

            var channelColor = ColorInfo.GetInfo(typeof(Gray), image.ColorInfo.ChannelType);

            GCHandle arrHandle = GCHandle.Alloc(arr, GCHandleType.Pinned);
            int arrDimStride = image.Width * channelColor.Size;

            IImage[] channels = (image.ColorInfo.NumberOfChannels == 1) ? new IImage[] { image } : ChannelSplitter.SplitChannels(image);

            for (int i = 0; i < channels.Length; i++)
            {
                int dimOffset = i * arrDimStride * image.Height;
                IntPtr dimPtr = (IntPtr)((byte*)arrHandle.AddrOfPinnedObject() + dimOffset);

                var channelImg = channels[i];
                HelperMethods.CopyImage(channelImg.ImageData, dimPtr, channelImg.Stride, arrDimStride, arrDimStride, image.Height);
            }

            arrHandle.Free();

            return arr;
        }

        /// <summary>
        /// Converts image to 3D array.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>3D array.</returns>
        public static TDepth[, ,] ToArray<TColor, TDepth>(this Image<TColor, TDepth> img)
            where TColor : IColor
            where TDepth : struct
        {
            return toArray(img) as TDepth[, ,];
        }

        /// <summary>
        /// Converts gray image to 2D array.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>2D array.</returns>
        public static TDepth[,] ToArray<TDepth>(this Image<Gray, TDepth> img)
            where TDepth : struct
        {
            TDepth[,] arr2D = new TDepth[img.Height, img.Width];

            GCHandle arr2DHandle = GCHandle.Alloc(arr2D, GCHandleType.Pinned);

            int dstStride = img.ColorInfo.Size * img.Width;
            HelperMethods.CopyImage(img.ImageData, arr2DHandle.AddrOfPinnedObject(), 
                                    img.Stride, dstStride, 
                                    dstStride, img.Height);

            arr2DHandle.Free();

            return arr2D;
        }

    }
}
