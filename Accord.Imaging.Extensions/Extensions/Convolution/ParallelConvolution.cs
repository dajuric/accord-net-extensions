using Accord.Core;
using System;
using System.Drawing;
using System.Linq;
using ColorConverter = Accord.Imaging.Converters.ColorConverter;

namespace Accord.Imaging
{
    /// <summary>
    /// Image border handling.
    /// </summary>
    public enum ConvolutionBorder
    {
        /// <summary>
        /// Border is zeroed (omitted).
        /// </summary>
        BorderNone = 0,
        /// <summary>
        /// Image border is mirrored.
        /// </summary>
        BorderMirror = 1
    }

    public static class ParallelConvolution
    {
        public const int MIN_KERNEL_SIZE_FOR_FFT = 13 * 13; //for [7x7] kernel FFT should pay off but this area is increased (empirical)

        #region Public Members

        /// <summary>
        /// Convolves an image with kernels. 
        /// Convolution type (FFT or spatial) is automatically selected based on kernels size.
        /// </summary>
        /// <param name="src">Input image.</param>
        /// <param name="kernels">Kernels</param>
        /// <returns>Convolved image</returns>
        public static Image<TColor, TDepth> Convolve<TColor, TDepth>(this Image<TColor, TDepth> src, params float[][,] kernels)
            where TColor : IColor
            where TDepth : struct
        {
            return Convolve(src, kernels, ConvolutionBorder.BorderMirror);
        }

        /// <summary>
        /// Convolves an image with kernels. 
        /// Convolution type (FFT or spatial) is automatically selected based on kernels size.
        /// </summary>
        /// <param name="src">Input image.</param>
        /// <param name="kernels">Kernels</param>
        /// <param name="options">Border options</param>
        /// <param name="forceSpatialConvolution">Use spatial convolution even if FFT should be used.</param>
        /// <returns>Convolved image</returns>
        public static Image<TColor, TDepth> Convolve<TColor, TDepth>(this Image<TColor, TDepth> src, float[][,] kernels, ConvolutionBorder options, bool forceSpatialConvolution = false)
            where TColor : IColor
            where TDepth : struct
        {
            return Convolve<TColor, TDepth, float>(src, kernels, options, forceSpatialConvolution);
        }

        private static Image<TColor, TDepth> Convolve<TColor, TDepth, TKernel>(Image<TColor, TDepth> src, TKernel[][,] kernelArrs, ConvolutionBorder options, bool forceSpatialConvolution = false)
            where TColor : IColor
            where TDepth : struct
            where TKernel : struct
        {
            var kernels = kernelArrs.Select(x => x.AsImage()).ToArray();
            return Convolve<TColor, TDepth, TKernel>(src, kernels, options, forceSpatialConvolution);
        }

        /// <summary>
        /// Convolves an image with kernels. 
        /// Convolution type (FFT or spatial) is automatically selected based on kernels size.
        /// </summary>
        /// <param name="src">Input image.</param>
        /// <param name="kernels">Kernels</param>
        /// <returns>Convolved image</returns>
        public static Image<TColor, TDepth> Convolve<TColor, TDepth>(this Image<TColor, TDepth> src, params Image<Gray, float>[] kernels)
            where TColor : IColor
            where TDepth : struct
        {
            return Convolve(src, kernels, ConvolutionBorder.BorderMirror);
        }

        /// <summary>
        /// Convolves an image with kernels. 
        /// Convolution type (FFT or spatial) is automatically selected based on kernels size.
        /// </summary>
        /// <param name="src">Input image.</param>
        /// <param name="kernels">Kernels</param>
        /// <param name="options">Border options</param>
        /// <param name="forceSpatialConvolution">Use spatial convolution even if FFT should be used.</param>
        /// <returns>Convolved image</returns>
        public static Image<TColor, TDepth> Convolve<TColor, TDepth>(this Image<TColor, TDepth> src, Image<Gray, float>[] kernels, ConvolutionBorder options, bool forceSpatialConvolution = false)
            where TColor : IColor
            where TDepth : struct
        {
            return Convolve<TColor, TDepth, float>(src, kernels, options, forceSpatialConvolution);
        }

        #endregion

        internal static Image<TColor, TDepth> Convolve<TColor, TDepth, TKernel>(this Image<TColor, TDepth> src, Image<Gray, TKernel>[] kernels, ConvolutionBorder options, bool forceSpatialConvolution = false)
            where TColor : IColor
            where TDepth : struct
            where TKernel: struct
        {
            bool useFFT = ShouldUseFFT(kernels) && !forceSpatialConvolution;
            
            Type[] supportedTypes = null;
            if (useFFT)
                supportedTypes = ParallelFFTConvolution.SupportedTypes;
            else
                supportedTypes = ParallelSpatialConvolution.SupportedTypes;

            supportedTypes = supportedTypes.Where(x => x.Equals(typeof(TKernel))).ToArray();
            if (supportedTypes.Length == 0)
                throw new NotSupportedException(string.Format("Kernel of type {0} is not supported. Used convolution: {1}. Supported types for used convolution: {2}." + 
                                                              "Please use different kernel type, or force convolution method.",
                                                              typeof(TKernel).Name,
                                                              (useFFT) ? "FFT" : "Spatial",
                                                              supportedTypes.Select(x => x.Name)));

            /************************************** convert src ********************************/
            var supportedColors = supportedTypes.Select(x => ColorInfo.GetInfo(typeof(TColor), x)).ToArray();
            var conversionPath = ColorConverter.GetMostInexepnsiveConversionPath(src.ColorInfo, supportedColors);
            IImage convertedSrc = ColorConverter.Convert(src, conversionPath.ToArray(), false);

            if (convertedSrc == null)
                throw new Exception(string.Format("Convolution does not support images of type {0}", src.ColorInfo.ChannelType));
            /************************************** convert src ********************************/

            IImage dest = null;
            if (useFFT)
            {
                dest = ParallelFFTConvolution.Convolve<TColor, TKernel>(convertedSrc as Image<TColor, TKernel>, kernels, options);
            }
            else
            {
                dest = ParallelSpatialConvolution.Convolve(convertedSrc, kernels, options);
            }


            /************************************** convert back ********************************/
            var backwardConversion = ColorConverter.GetMostInexepnsiveConversionPath(dest.ColorInfo, src.ColorInfo);
            IImage convertedDest = ColorConverter.Convert(dest, backwardConversion.ToArray(), false);
            if (convertedDest == null)
                throw new Exception(string.Format("Convolution does not support images of type {0}", src.ColorInfo.ChannelType));
            /************************************** convert back ********************************/

            return convertedDest as Image<TColor, TDepth>;
        }

        /// <summary>
        /// Returns whether FFT should be used or not.
        /// </summary>
        /// <param name="kernels">Kernels</param>
        internal static bool ShouldUseFFT(IImage[] kernels)
        {
            int biggestWidth, biggestHeight;
            GetTheBiggestSize(kernels, out biggestWidth, out biggestHeight);

            bool shouldUseFFT = (biggestWidth * biggestHeight) >= MIN_KERNEL_SIZE_FOR_FFT;
            return shouldUseFFT;
        }

        internal static void GetTheBiggestSize(IImage[] kernels, out int biggestWidth, out int biggestHeight)
        {
            biggestWidth = kernels.Select(x => x.Width).Max();
            biggestHeight = kernels.Select(x => x.Height).Max();
        }

        internal static void MirrorBorders(IImage image, IImage destImage, int mirrorWidth, int mirrorHeight)
        {
            //top
            Rectangle srcTop = new Rectangle(0, 0, image.Width, mirrorHeight);
            Rectangle dstTop = srcTop; dstTop.Offset(mirrorWidth, 0);
            ImageFlipping.FlipImage(image.GetSubRect(srcTop), destImage.GetSubRect(dstTop), FlipDirection.Vertical);

            //bottom
            Rectangle srcBottom = new Rectangle(0, image.Height - mirrorHeight, image.Width, mirrorHeight);
            Rectangle dstBottom = srcBottom; dstBottom.Offset(mirrorWidth, 2 * mirrorHeight);
            ImageFlipping.FlipImage(image.GetSubRect(srcBottom), destImage.GetSubRect(dstBottom), FlipDirection.Vertical);

            //left
            Rectangle srcLeft = new Rectangle(0, 0, mirrorWidth, image.Height);
            Rectangle dstLeft = srcLeft; dstLeft.Offset(0, mirrorHeight);
            ImageFlipping.FlipImage(image.GetSubRect(srcLeft), destImage.GetSubRect(dstLeft), FlipDirection.Horizontal);

            //right
            Rectangle srcRight = new Rectangle(image.Width - mirrorWidth, 0, mirrorWidth, image.Height);
            Rectangle dstRight = srcRight; dstRight.Offset(2 * mirrorWidth, mirrorHeight);
            ImageFlipping.FlipImage(image.GetSubRect(srcRight), destImage.GetSubRect(dstRight), FlipDirection.Horizontal);


            //top-left
            Rectangle srcTopLeft = new Rectangle(0, 0, mirrorWidth, mirrorHeight);
            Rectangle dstTopLeft = srcTopLeft; dstTopLeft.Offset(0, 0);
            ImageFlipping.FlipImage(image.GetSubRect(srcTopLeft), destImage.GetSubRect(dstTopLeft), FlipDirection.All);

            //bottom-left
            Rectangle srcBottomLeft = new Rectangle(0, image.Height - mirrorHeight, mirrorWidth, mirrorHeight);
            Rectangle dstBottomLeft = srcBottomLeft; dstBottomLeft.Offset(0, 2 * mirrorHeight);
            ImageFlipping.FlipImage(image.GetSubRect(srcBottomLeft), destImage.GetSubRect(dstBottomLeft), FlipDirection.All);

            //top-right
            Rectangle srcTopRight = new Rectangle(image.Width - mirrorWidth, 0, mirrorWidth, mirrorHeight);
            Rectangle dstTopRight = srcTopRight; dstTopRight.Offset(2 * mirrorWidth, 0);
            ImageFlipping.FlipImage(image.GetSubRect(srcTopRight), destImage.GetSubRect(dstTopRight), FlipDirection.All);

            //bottom-right
            Rectangle srcBottomRight = new Rectangle(image.Width - mirrorWidth, image.Height - mirrorHeight, mirrorWidth, mirrorHeight);
            Rectangle dstBottomRight = srcBottomRight; dstBottomRight.Offset(2 * mirrorWidth, 2 * mirrorHeight);
            ImageFlipping.FlipImage(image.GetSubRect(srcBottomRight), destImage.GetSubRect(dstBottomRight), FlipDirection.All);
        }
    }
}
