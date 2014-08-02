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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides methods for parallel spatial convolution.
    /// </summary>
    public static class ParallelSpatialConvolution //TODO: int, double implementation (to remove additional converting e.g. <Bgr, int> -> <Bgr, int> -> Convolve -> <Bgr, int>
    {
        /// <summary>
        /// Supported image types for spatial convolution.
        /// </summary>
        public static readonly Type[] SupportedTypes = new Type[] { /*typeof(int),*/ typeof(float)/*, typeof(double)*/ };

        delegate void ConvolutionFunc(IImage src, Rectangle sourceWorkingArea, IImage dest, Point destLoc, IImage kernel);
        private static Dictionary<Type, ConvolutionFunc> convolutionFuncs;

        static ParallelSpatialConvolution()
        {
            convolutionFuncs = new Dictionary<Type, ConvolutionFunc>();
            convolutionFuncs.Add(typeof(float), convolveFloat);
        }

        /// <summary>
        /// Convolves the image with the provided kernels.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="src">Source image.</param>
        /// <param name="kernels">Kernels to convolve with.</param>
        /// <param name="options">Border pixels resolvent options.</param>
        /// <returns>Convolved image.</returns>
        public static Image<TColor, float> Convolve<TColor>(Image<TColor, float> src, Image<Gray, float>[] kernels, ConvolutionBorder options)
            where TColor : IColor
        {
            return Convolve((IImage)src, kernels, options) as Image<TColor, float>;
        }

        internal static IImage Convolve(IImage src, IImage[] kernels, ConvolutionBorder options)
        {
            IImage dest = src;

            foreach (var kernel in kernels)
            {
                dest = Convolve(dest, kernel, options);
            }

            return dest;
        }

        /// <summary>
        /// Convolves the image patch with the provided kernel.
        /// </summary>
        /// <param name="src">Source image.</param>
        /// <param name="srcArea">Area of the image to convolve.</param>
        /// <param name="kernel">Kernel to convolve with.</param>
        /// <param name="dest">Destination image. It's size must be equal to the size of the specified area.</param>
        /// <param name="convolveBorders">True if the borders should be convolved, false otherwise.</param>
        public static void ConvolvePatch(IImage src, Rectangle srcArea, IImage kernel, IImage dest, bool convolveBorders = false) //TODO: revise whether it should be used or not.
        {
            ConvolutionFunc convolutionFunc = null;
            if (convolutionFuncs.TryGetValue(src.ColorInfo.ChannelType, out convolutionFunc) == false)
                throw new NotSupportedException(string.Format("Can not perform spatial convolution on an image of type {0}", src.ColorInfo.ChannelType.Name));

            Rectangle srcWorkingArea = srcArea;
            if (!convolveBorders)
            {
                srcWorkingArea.Width -= kernel.Width;
                srcWorkingArea.Height -= kernel.Height;
                //srcWorkingArea.Inflate(-kernel.Width / 2, -kernel.Height / 2);
            }

            convolutionFunc(src, srcWorkingArea, dest, new Point(), kernel);
        }

        internal static IImage Convolve(IImage src, IImage kernel, ConvolutionBorder options)
        {
            ConvolutionFunc convolutionFunc = null;
            if (convolutionFuncs.TryGetValue(src.ColorInfo.ChannelType, out convolutionFunc) == false)
                throw new NotSupportedException(string.Format("Can not perform spatial convolution on an image of type {0}", src.ColorInfo.ChannelType.Name));

            Rectangle validRegion;
            var preparedSrc = prepareSourceImage(src, kernel.Size, options, out validRegion);

            var proc = new ParallelProcessor<IImage, IImage>(src.Size,
                                               () => preparedSrc.CopyBlank(), //in-place convolution is not supported due to parallel processing (junction patches handling)
                                               (_src, _dest, area) => 
                                               {                                               
                                                   Rectangle srcArea = new Rectangle
                                                   {
                                                       X = 0,
                                                       Y = area.Y,
                                                       Width = _src.Width,
                                                       Height = area.Height + kernel.Height  //get area sufficient to process with the selected kernel; area.Height is processed
                                                   };
                                                   //srcArea.Inflate(-kernel.Width , -kernel.Height );
                                                   srcArea.Width -= kernel.Width;
                                                   srcArea.Height -= kernel.Height;

                                                   srcArea.Intersect(new Rectangle(new Point(), _src.Size));

                                                   convolutionFunc(_src, srcArea, _dest, new Point(kernel.Width / 2, area.Y + kernel.Height / 2), kernel);
                                               }
                                               /*,new ParallelOptions2D { ForceSequential = true }*/);

            var dest = proc.Process(preparedSrc);

            return dest.GetSubRect(validRegion);
        }

        private static IImage prepareSourceImage(IImage src, Size kernelSize, ConvolutionBorder options, out Rectangle validRegion)
        {
            var preparedSrc = Image.Create(src.ColorInfo, src.Width + kernelSize.Width, src.Height + kernelSize.Height);
            Rectangle centerRegion = new Rectangle(kernelSize.Width / 2, kernelSize.Height / 2, src.Width, src.Height);
            preparedSrc.GetSubRect(centerRegion).SetValue(src);

            if (options == ConvolutionBorder.BorderMirror)
                ParallelConvolution.MirrorBorders(src, preparedSrc, centerRegion.X, centerRegion.Y);

            validRegion = centerRegion;
            return preparedSrc;
        }

        #region Float process

        private static unsafe void convolveFloat(IImage src, Rectangle sourceWorkingArea, IImage dest, Point destLoc, IImage kernel)
        {
            int srcStride = src.Stride;
            int destStride = dest.Stride;

            float* kernelPtr = (float*)kernel.ImageData;
            int kernelWidth = kernel.Width;
            int kernelHeight = kernel.Height;
            int nChannels = src.ColorInfo.NumberOfChannels;


            for (int channel = 0; channel < nChannels; channel++)
            {
                float* srcPtr = (float*)src.GetData(sourceWorkingArea.Y, sourceWorkingArea.X) + channel;
                float* destPtr = (float*)dest.GetData(destLoc.Y, destLoc.X) + channel;

                for (int row = sourceWorkingArea.Y; row < sourceWorkingArea.Bottom; row++)
                {
                    int chCol = 0;
                    for (int col = sourceWorkingArea.X; col < sourceWorkingArea.Right; col++)
                    {
                        destPtr[chCol] = applyKernelFloat(kernelPtr, &srcPtr[chCol], kernelWidth, kernelHeight, srcStride, nChannels);
                        chCol += nChannels;
                    }

                    srcPtr += srcStride / sizeof(float);
                    destPtr += destStride / sizeof(float);
                }
            } 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static float applyKernelFloat(float* kernelPtr, float* srcPtr, int kernelWidth, int kernelHeight, int srcStride, int nChannels)
        {
            float val = 0;

            for (int kernelRow = 0; kernelRow < kernelHeight; kernelRow++)
            {
                int chCol = 0;
                for (int kernelCol = 0; kernelCol < kernelWidth; kernelCol++)
                {
                    val += srcPtr[chCol] * kernelPtr[kernelCol];
                    chCol += nChannels;
                }

                kernelPtr += kernelHeight;
                srcPtr += srcStride / sizeof(float);
            }

            return val;
        }

        #endregion

    }
}
