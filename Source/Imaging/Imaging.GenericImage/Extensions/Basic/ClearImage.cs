using System;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for image value initialization.
    /// </summary>
    public static class ClearImageExtensions
    {
        /// <summary>
        /// Sets each image byte to zero (using kernel function memset).
        /// </summary>
        public static void Clear(this IImage image)
        {
            SetByte(image.ImageData, image.Width * image.ColorInfo.Size, image.Height, image.Stride, 0);
        }

        private unsafe static void SetByte(IntPtr ptr, int rowLengthInBytes, int height, int stride, byte value = 0)
        {
            if (rowLengthInBytes == stride)
                AForge.SystemTools.SetUnmanagedMemory(ptr, value, rowLengthInBytes * height);
            else
            {
                for (int r = 0; r < height; r++)
                {
                    AForge.SystemTools.SetUnmanagedMemory(ptr, value, rowLengthInBytes);
                    ptr += stride;
                }
            }
        }

    }
}
