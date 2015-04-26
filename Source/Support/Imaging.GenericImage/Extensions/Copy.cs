using Accord.Extensions;
using System;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides methods for unsafe data copying as well as extensions for generic image and array cloning.
    /// </summary>
    public static class Copy
    {
        /// <summary>
        /// Copies unmanaged data from the specified source to the specified destination patch.
        /// </summary>
        /// <param name="srcPtr">Source pointer.</param>
        /// <param name="destPtr">Destination pointer.</param>
        /// <param name="srcStride">Source stride.</param>
        /// <param name="destStride">Destination stride.</param>
        /// <param name="patchStride">Amount of bytes per row to be copied (width for byte fields). (common: image width * color size)</param>
        /// <param name="patchHeight">Field's height.</param>
        public unsafe static void UnsafeCopy(IntPtr srcPtr, IntPtr destPtr, int srcStride, int destStride, int patchStride, int patchHeight)
        {
            if (srcStride == destStride && srcStride == patchStride)
                AForge.SystemTools.CopyUnmanagedMemory(destPtr, srcPtr, srcStride * patchHeight);
            else
            {
                byte* srcImgPtr = (byte*)srcPtr;
                byte* destImgPtr = (byte*)destPtr;

                for (int i = 0; i < patchHeight; i++)
                {
                    AForge.SystemTools.CopyUnmanagedMemory(destImgPtr, srcImgPtr, patchStride);

                    srcImgPtr += srcStride;
                    destImgPtr += destStride;
                }
            }
        }

        /// <summary>
        /// Copies unmanaged data from the specified source to the specified destination.
        /// </summary>
        /// <param name="srcPtr">Source pointer.</param>
        /// <param name="destPtr">Destination pointer.</param>
        /// <param name="srcStride">Source stride.</param>
        /// <param name="destStride">Destination stride.</param>
        /// <param name="destHeight">Field's height.</param>
        public unsafe static void UnsafeCopy(IntPtr srcPtr, IntPtr destPtr, int srcStride, int destStride, int destHeight)
        {
            UnsafeCopy(srcPtr, destPtr, srcStride, destStride, destStride, destHeight);
        }

        /// <summary>
        /// Clones the portion of the provided array.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="array">Array.</param>
        /// <param name="area">Working area.</param>
        /// <returns>Cloned portion of the array.</returns>
        public static T[,] Clone<T>(this T[,] array, Rectangle area)
            where T : struct
        {
            //error handling in Lock(...)

            var destination = new T[area.Height, area.Width];

            using(var srcImg = array.Lock(area))
            using (var dstImg = destination.Lock())
            {
                UnsafeCopy(srcImg.ImageData, dstImg.ImageData, srcImg.Stride, dstImg.Stride, dstImg.Height);
            }

            return destination;
        }

        /// <summary>
        /// Clones the provided image.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Cloned image.</returns>
        public static T[,] Clone<T>(this Image<T> image)
            where T : struct
        {
            if (image == null)
                return null;

            //error handling in CopyTo(..)

            var destination = new T[image.Height, image.Width];
            CopyTo(image, destination);

            return destination;
        }

        /// <summary>
        /// Copies the specified source image to the destination image.
        /// <para>Source and destination image must have the same size.</para>
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="source">Source image.</param>
        /// <param name="destination">Destination image.</param>
        public static void CopyTo<T>(this Image<T> source, Image<T> destination)
            where T : struct
        {
            if (source.Size != destination.Size)
                throw new ArgumentException("Source dimension must be the same as destination dimension.");

            UnsafeCopy(source.ImageData, destination.ImageData, source.Stride, destination.Stride, destination.Height);
        }

        /// <summary>
        /// Copies the specified source image to the destination image.
        /// <para>Source and destination image must have the same size.</para>
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="source">Source image.</param>
        /// <param name="destination">Destination image.</param>
        public static void CopyTo<T>(this Image<T> source, T[,] destination)
            where T: struct
        {
            //error handling in CopyTo(..)

            using (var dstImg = destination.Lock())
            {
                CopyTo(source, dstImg);
            }
        }

        /// <summary>
        /// Copies the specified source image to the destination image.
        /// <para>Source and destination image must have the same size.</para>
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="source">Source image.</param>
        /// <param name="destination">Destination image.</param>
        /// <param name="destinationOffset">Destination location.</param>
        public static void CopyTo<T>(this T[,] source, T[,] destination, Point destinationOffset)
            where T: struct
        {
            var destRect = new Rectangle(destinationOffset, source.Size());

            using (var srcImg = source.Lock())
            using (var dstImg = destination.Lock(destRect))
            {
                UnsafeCopy(srcImg.ImageData, dstImg.ImageData, srcImg.Stride, dstImg.Stride, dstImg.Height);
            }
        }

        /// <summary>
        /// Copies the image source to the destination buffer. 
        /// If the buffer does not have the same size as the source image, or the buffer is null, the destination buffer will be recreated.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="source">Source image.</param>
        /// <param name="destinationBuffer">destination buffer.</param>
        public static void CopyToOrCreate<TColor>(this Image<TColor> source, ref TColor[,] destinationBuffer)
            where TColor: struct
        {
            if (source == null)
            {
                destinationBuffer = null;
                return;
            }

            if (destinationBuffer == null ||
               source.Width != destinationBuffer.Width() || source.Height != destinationBuffer.Height())
            {
                destinationBuffer = source.Clone();
            }
            else
            {
                source.CopyTo(destinationBuffer);
            }
        }

        /// <summary>
        /// Copies values from source to destination image using mask. Destination values where mask == 0 are not erased!.
        /// </summary>
        /// <param name="source">Image.</param>
        /// <param name="destination">Destination image</param>
        /// <param name="mask">Mask. Color locations that need to be copied must be set to !=0 in mask.</param>
        public static void CopyTo<TColor>(this TColor[,] source, TColor[,] destination, Gray<byte>[,] mask)
            where TColor: struct
        {
            if (source.Size() != mask.Size() || source.Size() != destination.Size())
                throw new Exception("Image, mask, destImg size must be the same!");

            ParallelLauncher.Launch((thread) =>
            {
                if (mask[thread.Y, thread.X] != 0)
                    destination[thread.Y, thread.X] = source[thread.Y, thread.X];
            },
            source.Width(), source.Height());
        }

    }
}
