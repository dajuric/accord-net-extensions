using Accord.Core;
using Accord.Imaging.Helper;
using System;
using System.Runtime.InteropServices;

namespace Accord.Imaging
{
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

        // <summary>
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
