using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Accord.Imaging.Helper;
using Accord.Math;
using Accord.Core;

namespace Accord.Imaging
{
    public static class ParallelFFTConvolution
    {
        public static readonly Type[] SupportedTypes = new Type[] { typeof(float), typeof(double) };

        public static Image<TColor, float> Convolve<TColor>(Image<TColor, float> image, float[][,] kernelArrs, ConvolutionBorder options)
            where TColor : IColor
        {
            var kernels = kernelArrs.Select(x => x.AsImage()).ToArray();
            return Convolve<TColor, float>(image, kernels, options);
        }

        public static Image<TColor, double> Convolve<TColor>(Image<TColor, double> image, double[][,] kernelArrs, ConvolutionBorder options)
            where TColor : IColor
        {
            var kernels = kernelArrs.Select(x => x.AsImage()).ToArray();
            return Convolve<TColor, double>(image, kernels, options);
        }

        public static Image<TColor, float> Convolve<TColor>(Image<TColor, float> image, Image<Gray, float>[] kernels, ConvolutionBorder options)
            where TColor : IColor
        {
            return Convolve<TColor, float>(image, kernels, options);
        }

        public static Image<TColor, double> Convolve<TColor>(Image<TColor, double> image, Image<Gray, double>[] kernels, ConvolutionBorder options)
            where TColor : IColor
        {
            return Convolve<TColor, double>(image, kernels, options);
        }


        internal static Image<TColor, TDepth> Convolve<TColor, TDepth>(Image<TColor, TDepth> image, Image<Gray, TDepth>[] kernels, ConvolutionBorder options)
           where TColor:IColor
           where TDepth : struct
        {
            if(typeof(TColor).Equals(typeof(Gray)) == true) //saving time on channel splitting and creating an image from channels (can function without this)
                return convolve<TDepth>(image as Image<Gray, TDepth>, kernels, options) as Image<TColor, TDepth>;

            var channels = image.SplitChannels();
            var convolvedChannels = new List<Image<Gray, TDepth>>();

            foreach (var ch in channels)
            {
                var convolvedCh = convolve(ch as Image<Gray, TDepth>, kernels, options);
                convolvedChannels.Add(convolvedCh);
            }

            return new Image<TColor, TDepth>(convolvedChannels.ToArray());
        }

        private static Image<Gray, TDepth> convolve<TDepth>(Image<Gray, TDepth> image, Image<Gray, TDepth>[] kernels, ConvolutionBorder options)
            where TDepth : struct
        {
            int biggestKernelWidth, biggestKernelHeight;
            ParallelConvolution.GetTheBiggestSize(kernels, out biggestKernelWidth, out biggestKernelHeight);

            int fillX, fillY;
            var paddedIm = prepareImage(image, biggestKernelWidth, biggestKernelHeight, options, out fillX, out fillY);

            Image<Complex, TDepth> convolvedIm = paddedIm;
            foreach (var kernel in kernels)
            {
                var preparedKernel = prepareKernel(kernel, convolvedIm.Size);
                convolvedIm = convolvedIm.Mul(preparedKernel, null);
            }

            return getConvolutionResult(convolvedIm, fillX, fillY, image.Size, true);
        }

        private static IEnumerable<Image<Gray, TDepth>> ConvolveSeparated<TDepth>(Image<Gray, TDepth> image, Image<Gray, TDepth>[] kernels, ConvolutionBorder options)
            where TDepth : struct /*float and double */
        {
            int biggestKernelWidth, biggestKernelHeight;
            ParallelConvolution.GetTheBiggestSize(kernels, out biggestKernelWidth, out biggestKernelHeight);

            int fillX, fillY;
            var paddedIm = prepareImage(image, biggestKernelWidth, biggestKernelHeight, options, out fillX, out fillY);

            foreach (var kernel in kernels)
            {
                var preparedKernel = prepareKernel(kernel, paddedIm.Size);
                var convolvedIm = paddedIm.Mul(preparedKernel, null);
                yield return getConvolutionResult(convolvedIm, fillX, fillY, image.Size, true);
            }
        }


        private static Image<Gray, TDepth> getConvolutionResult<TDepth>(Image<Complex, TDepth> convolvedImage, int fillX, int fillY, Size imageSize, bool transformImageInPlace = false)
           where TDepth : struct
        {
            Image<Complex, TDepth> iFFT_image = null; //using shorter way it could be written as: iFFT_image = convolvedImage.FFT(FourierTransform.Direction.Backward, transformImageInPlace);
            if(transformImageInPlace)
            {
                convolvedImage.FFT(FourierTransform.Direction.Backward, true);
                iFFT_image = convolvedImage;
            }
            else
            {
                iFFT_image =  convolvedImage.FFT(FourierTransform.Direction.Backward, false);
            }

            Rectangle validRegion = new Rectangle(fillX * 2, fillY * 2, imageSize.Width, imageSize.Height);
            //var result = convolvedImage.GetSubRect(validRegion)[0];
            var result = convolvedImage.GetSubRect(validRegion).Magnitude(); //in the most general case (when input image is indeed complex) inverse FFT image can contain Im values != 0
            return result;
        }

        private static Image<Complex, TDepth> prepareImage<TDepth>(Image<Gray, TDepth> image, int biggestKernelWidth, int biggestKernelHeight,
                                                                   ConvolutionBorder options, 
                                                                   out int fillX, out int fillY)
             where TDepth:struct
        {
            int FFTNumOfCols = (int)System.Math.Pow(2.0, System.Math.Ceiling(System.Math.Log(biggestKernelWidth + image.Width, 2.0)));
            int FFTNumOfRows = (int)System.Math.Pow(2.0, System.Math.Ceiling(System.Math.Log(biggestKernelHeight + image.Height, 2.0)));

            fillX = System.Math.Min(image.Width, biggestKernelWidth / 2);
            fillY = System.Math.Min(image.Height, biggestKernelHeight / 2);

            Rectangle centerRegion = new Rectangle(fillX, fillY, image.Width, image.Height);
            Image<Gray, TDepth> paddedImage = new Image<Gray, TDepth>(FFTNumOfCols, FFTNumOfRows);

            //center
            paddedImage.GetSubRect(centerRegion).SetValue(image);

            if (options == ConvolutionBorder.BorderMirror)
            {
               ParallelConvolution.MirrorBorders(image, paddedImage, fillX, fillY);
            }

            Image<Complex, TDepth> paddedImageCmplx = new Image<Complex, TDepth>(new Image<Gray, TDepth>[] { paddedImage, paddedImage.CopyBlank() });
            paddedImageCmplx.FFT(FourierTransform.Direction.Forward, true);
            return paddedImageCmplx;  
        }

        private static Image<Complex, TDepth> prepareKernel<TDepth>(Image<Gray, TDepth> kernel, Size paddedImageSize)
            where TDepth : struct
        {
            Image<Complex, TDepth> preparedKernel = new Image<Complex, TDepth>(paddedImageSize);
            Rectangle kernelRoi = new Rectangle(0, 0, kernel.Width, kernel.Height);

            preparedKernel.GetSubRect(kernelRoi)[0] = kernel;

            preparedKernel.FFT(FourierTransform.Direction.Forward, true);
            return preparedKernel;
        }
    }
}
