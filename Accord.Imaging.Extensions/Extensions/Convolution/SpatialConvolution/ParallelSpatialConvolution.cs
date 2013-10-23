using Accord.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Accord.Imaging
{
    public static class ParallelSpatialConvolution //TODO: int, double implementation (to remove additional converting e.g. <Bgr, int> -> <Bgr, int> -> Convolve -> <Bgr, int>
    {
        public static readonly Type[] SupportedTypes = new Type[] { /*typeof(int),*/ typeof(float)/*, typeof(double)*/ };

        delegate void ConvolutionFunc(IImage src, Rectangle sourceWorkingArea, IImage dest, Point destLoc, IImage kernel);
        private static Dictionary<Type, ConvolutionFunc> convolutionFuncs;

        static ParallelSpatialConvolution()
        {
            convolutionFuncs = new Dictionary<Type, ConvolutionFunc>();
            convolutionFuncs.Add(typeof(float), convolveFloat);
        }

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

        public static void ConvolvePatch(IImage src, Rectangle srcArea, IImage kernel, IImage dest, bool convolveBorders = false)
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

            convolutionFunc(src, srcWorkingArea, dest, Point.Empty, kernel);
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

                                                   srcArea.Intersect(new Rectangle(Point.Empty, _src.Size));

                                                   convolutionFunc(_src, srcArea, _dest, new Point(kernel.Width / 2, area.Y + kernel.Height / 2), kernel);
                                               }
                                               /*,new ParallelOptions { ForceSequential = true }*/);

            var dest = proc.Process(preparedSrc);

            return dest.GetSubRect(validRegion);
        }

        private static IImage prepareSourceImage(IImage src, Size kernelSize, ConvolutionBorder options, out Rectangle validRegion)
        {
            var preparedSrc = GenericImageBase.Create(src.ColorInfo, src.Width + kernelSize.Width, src.Height + kernelSize.Height);
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
