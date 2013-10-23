using Accord.Core;
using Accord.Imaging.Helper;
using System;
using System.Runtime.InteropServices;

namespace Accord.Imaging
{
    public static class ArrayToImageExtensions
    {
        private static void getArrayDimenions(Array arr, out int nChannels, out int width, out int height)
        {
            switch (arr.Rank)
            { 
                case 2:
                    nChannels = 1;
                    width = arr.GetLength(1);
                    height = arr.GetLength(0);
                    break;
                case 3:
                    nChannels = arr.GetLength(0);
                    width = arr.GetLength(2);
                    height = arr.GetLength(1);
                    break;
                default:
                    throw new NotSupportedException("Array must be 2D or 3D");
            }
        }

        /// <summary>
        /// Gets images (channels) for 2D/3D array. 
        /// <remarks> Any array that is not 2D/3D and its element iis not a primitive type is not supported.  (an exception is thrown)</remarks>
        /// </summary>
        /// <param name="arr">Input array.</param>
        /// <returns>Channels. For 2D array output will consist of an single image.</returns>
        public unsafe static IImage[] GetChannels(this Array arr)
        {
            int nChannels, width, height;
            getArrayDimenions(arr, out nChannels, out width, out height);

            var elemType = arr.GetType().GetElementType();
            var color = ColorInfo.GetInfo(typeof(Gray), elemType);

           
            GCHandle arrHandle = GCHandle.Alloc(arr, GCHandleType.Pinned);
            int arrDimStride = width * color.Size;

            var imageChannels = new IImage[nChannels];
            for (int i = 0; i < nChannels; i++)
            {
                int dimOffset = i * arrDimStride * height;
                IntPtr dimPtr = (IntPtr)((byte*)arrHandle.AddrOfPinnedObject() + dimOffset);

                var channelImg = GenericImageBase.Create(color, width, height);
                HelperMethods.CopyImage(dimPtr, channelImg.ImageData, arrDimStride, channelImg.Stride, arrDimStride, height);
                imageChannels[i] = channelImg;
            }

            arrHandle.Free();

            return imageChannels;
        }

        internal unsafe static void SetValue(IImage img, Array arr)
        {
            IImage[] channels = GetChannels(arr);

            if (img.ColorInfo.NumberOfChannels != channels.Length)
                throw new Exception("Number of channels in the image must be equal to array depth (0th dimension)");

            ChannelMerger.MergeChannels(channels, img);
        }

        /// <summary>
        /// Sets pixels for an image from array. Array is converted to image chanels first (<seealso cref="GetChannels"/>)
        /// An image and an array must have the same dimensions.
        /// </summary>
        /// <param name="img">Destination image.</param>
        /// <param name="arr">Array.</param>
        public static void SetValue<TColor, TDepth>(this Image<TColor, TDepth> img, TDepth[,,] arr)
            where TColor:IColor
            where TDepth : struct
        {
            SetValue(img, arr);
        }

        /// <summary>
        /// Sets pixels for an image from array. Array is converted to image chanels first (one and only channel) (<seealso cref="GetChannels"/>)
        /// An image and an array must have the same dimensions.
        /// </summary>
        /// <param name="img">Destination image.</param>
        /// <param name="arr">Array.</param>
        public static void SetValue<TDepth>(this Image<Gray, TDepth> img, TDepth[,] arr)
            where TDepth : struct
        {
            SetValue((IImage)img, (Array)arr);
        }

        /// <summary>
        /// Converts an array to an image. Data is copied. 2D array also supports data sharing <seealso cref="AsImage"/>
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Image<TColor, TDepth> ToImage<TColor, TDepth>(this TDepth[,,] arr)
            where TColor:IColor
            where TDepth:struct
        {
            Image<TColor, TDepth> img = new Image<TColor, TDepth>(arr.GetLength(2), arr.GetLength(1));
            SetValue(img, arr);
            return img;
        }


        /// <summary>
        /// Converts array to image (data is copied). Array elements must be primitive types.
        /// </summary>
        /// <param name="arr">Input array</param>
        /// <returns>Image</returns>
        public static Image<Gray, TDepth> ToImage<TDepth>(this TDepth[,] arr)
            where TDepth : struct
        {
            Image<Gray, TDepth> img = new Image<Gray, TDepth>(arr.GetLength(1), arr.GetLength(0));
            SetValue(img, arr);
            return img;
        }

        /// <summary>
        /// Converts array to image (data is shared). Array elements must be primitive types.
        /// </summary>
        /// <param name="arr">Input array</param>
        /// <returns>Image</returns>
        public static Image<Gray, TDepth> AsImage<TDepth>(this TDepth[,] arr)
            where TDepth : struct
        {
            GCHandle arrHandle = GCHandle.Alloc(arr, GCHandleType.Pinned);

            int width = arr.GetLength(1);
            int height = arr.GetLength(0);
            int stride = ColorInfo.GetInfo<Gray, TDepth>().ChannelSize * width;

            Image<Gray, TDepth> img = new Image<Gray, TDepth>(arrHandle.AddrOfPinnedObject(),
                                                              width, height, stride,
                                                              arr, arrHandle);
           
            return img;
        }

    }
}
