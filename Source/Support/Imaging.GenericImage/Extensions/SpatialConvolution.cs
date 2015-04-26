using Accord.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides extension methods for the image spatial convolution.
    /// </summary>
    public static class SpatialConvolution
    {
        /*private unsafe static void convolve(KernelThread thread, float[,] source, float[,] kernel, float[,] destination)
        {
            int kernelWidth = kernel.Width();
            int kernelHeight = kernel.Height();

            var sum = 0f;
            for (int r = 0; r < kernelHeight; r++)
            {
                for (int c = 0; c < kernelWidth; c++)
                {
                    sum += source[thread.Y + r, thread.X + c] * kernel[r, c];
                }
            }

            destination[thread.Y + kernelHeight / 2, thread.X + kernelWidth / 2] = sum;
        }*/

        private unsafe static void convolve(KernelThread thread, float* source, int sourceStride, int channelCount,
                                                                 float* kernel, int kernelWidth, int kernelHeight,
                                                                 float* destination, int destinationStride)
        {
            source = (float*)((byte*)source + sourceStride * thread.Y) + thread.X;
            destination = (float*)((byte*)destination + destinationStride * (thread.Y + kernelHeight / 2)) + (thread.X + kernelWidth / 2);

            for (int ch = 0; ch < channelCount; ch++)
            {
                var srcPtr = source + ch;
                var krnlPtr = kernel;

                var sum = 0f;
                for (int r = 0; r < kernelHeight; r++)
                {
                    for (int c = 0, srcCol = 0; c < kernelWidth; c++, srcCol += channelCount)
                    {
                        sum += srcPtr[srcCol] * krnlPtr[c];
                    }

                    srcPtr = (float*)((byte*)srcPtr + sourceStride);
                    krnlPtr += kernelWidth;
                }

                destination[ch] = sum;
            }
        }

        /// <summary>
        /// Convolves the source image with the specified kernel.
        /// </summary>
        /// <param name="source">Source data pointer.</param>
        /// <param name="sourceWidth">Source width.</param>
        /// <param name="sourceHeight">Source height.</param>
        /// <param name="sourceStride">Source stride.</param>
        /// <param name="channelCount">Source channel count.</param>
        /// <param name="kernel">Kernel pointer.</param>
        /// <param name="kernelWidth">Kernel width.</param>
        /// <param name="kernelHeight">Kernel height.</param>
        /// <param name="destination">Destination pointer.</param>
        /// <param name="destinationStride">Destination stride.</param>
        public unsafe static void ConvolveUnsafe(float* source, int sourceWidth, int sourceHeight, int sourceStride, int channelCount,
                                                 float* kernel, int kernelWidth, int kernelHeight,
                                                 float* destination, int destinationStride)
        {
           ParallelLauncher.Launch(thread =>
            {
                convolve(thread, source, sourceStride, channelCount,
                                 kernel, kernelWidth, kernelHeight,
                                 destination, destinationStride);
            },
            sourceWidth - kernelWidth, sourceHeight - kernelHeight);
        }

        /// <summary>
        /// Convolves the source image with the specified kernel.
        /// </summary>
        /// <param name="source">Source image.</param>
        /// <param name="kernel">Kernel.</param>
        /// <param name="area">Working area.</param>
        /// <returns>Convolved image.</returns>
        public unsafe static float[,] Convolve(this float[,] source, float[,] kernel, Rectangle area)
        {
            var destination = new float[area.Height, area.Width];

            fixed (float* srcPtr = &source[area.Y, area.X], kernelPtr = kernel, dstPtr = destination)
            {
                ConvolveUnsafe(srcPtr, area.Width, area.Height, source.Width() * sizeof(float), 1,
                         kernelPtr, kernel.Width(), kernel.Height(),
                         dstPtr, destination.Width() * sizeof(float));
            }

            return destination;
        }

        /// <summary>
        /// Convolves the source image with the specified kernel.
        /// </summary>
        /// <param name="source">Source image.</param>
        /// <param name="kernel">Kernel.</param>
        /// <returns>Convolved image.</returns>
        public unsafe static float[,] Convolve(this float[,] source, float[,] kernel)
        {
            var area = new Rectangle(0, 0, source.Width(), source.Height());
            return source.Convolve(kernel, area);
        }

        /// <summary>
        /// Convolves the source image with the specified kernel.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="source">Source image.</param>
        /// <param name="kernel">Kernel.</param>
        /// <returns>Convolved image.</returns>
        public unsafe static TColor[,] Convolve<TColor>(this TColor[,] source, float[,] kernel)
            where TColor: struct, IColor<float>
        {
            var channelCount = source.ColorInfo().ChannelCount;
            var destination = source.CopyBlank();

            using (var srcImg = source.Lock())
            using(var dstImg = destination.Lock())
            {
                fixed(float* kernelPtr = kernel)
                {
                    ConvolveUnsafe((float*)srcImg.ImageData, srcImg.Width, srcImg.Height, srcImg.Stride, channelCount,
                                   kernelPtr, kernel.Width(), kernel.Height(), 
                                   (float*)dstImg.ImageData, dstImg.Stride);
                }          
            }

            return destination;
        }

    }
}
