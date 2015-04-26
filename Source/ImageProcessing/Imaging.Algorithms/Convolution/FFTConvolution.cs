#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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

using Accord.Extensions.Math;
using System.Collections.Generic;
using System.Linq;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
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

    /// <summary>
    /// Contains extension methods for FFT convolution.
    /// </summary>
    public static class FFTConvolution
    {
        /// <summary>
        /// Convolves the image with the specified kernels.
        /// </summary>
        /// <typeparam name="TColor">Image color type.</typeparam>
        /// <param name="image">Image.</param>
        /// <param name="kernels">Kernels.</param>
        /// <param name="options">Options for resolving border pixels.</param>
        /// <returns>Convolved image.</returns>
        public static TColor[,] ConvolveFFT<TColor>(this TColor[,] image, IList<float[,]> kernels, ConvolutionBorder options)
            where TColor : struct, IColor<float>
        {
            var channels = image.SplitChannels<TColor, float>();
            var convolvedChannels = new List<Gray<float>[,]>();

            foreach (var channel in channels)
            {
                var convolvedChannel = convolve(channel, kernels, options);
                convolvedChannels.Add(convolvedChannel);
            }

            var destination = convolvedChannels.MergeChannels<TColor, float>();
            return destination;
        }

        private static void getTheBiggestSize<TColor>(IList<TColor[,]> kernels, out int biggestWidth, out int biggestHeight)
        {
            biggestWidth = kernels.Select(x => x.Width()).Max();
            biggestHeight = kernels.Select(x => x.Height()).Max();
        }

        private static void mirrorBorders<TColor>(TColor[,] image, TColor[,] destImage, int mirrorWidth, int mirrorHeight)
            where TColor: struct
        {
            //top
            Rectangle srcTop = new Rectangle(0, 0, image.Width(), mirrorHeight);
            Point dstTopOffset = new Point(srcTop.X + mirrorWidth, srcTop.Y + 0); 
            image.FlipImage(srcTop, destImage, dstTopOffset, FlipDirection.Vertical);

            //bottom
            Rectangle srcBottom = new Rectangle(0, image.Height() - mirrorHeight, image.Width(), mirrorHeight);
            Point dstBottomOffset = new Point(srcBottom.X + mirrorWidth, srcBottom.Y + 2 * mirrorHeight);
            image.FlipImage(srcBottom, destImage, dstBottomOffset, FlipDirection.Vertical);

            //left
            Rectangle srcLeft = new Rectangle(0, 0, mirrorWidth, image.Height());
            Point dstLeftOffset = new Point(srcLeft.X + 0, srcLeft.Y + mirrorHeight);
            image.FlipImage(srcLeft, destImage, dstLeftOffset, FlipDirection.Horizontal);

            //right
            Rectangle srcRight = new Rectangle(image.Width() - mirrorWidth, 0, mirrorWidth, image.Height());
            Point dstRightOffset = new Point(srcRight.X + 2 * mirrorWidth, srcRight.Y + mirrorHeight);
            image.FlipImage(srcRight, destImage, dstRightOffset, FlipDirection.Horizontal);


            //top-left
            Rectangle srcTopLeft = new Rectangle(0, 0, mirrorWidth, mirrorHeight);
            Point dstTopLeftOffset = new Point(srcTopLeft.X + 0, srcTopLeft.Y + 0);
            image.FlipImage(srcTopLeft, destImage, dstTopLeftOffset, FlipDirection.All);

            //bottom-left
            Rectangle srcBottomLeft = new Rectangle(0, image.Height() - mirrorHeight, mirrorWidth, mirrorHeight);
            Point dstBottomLeftOffset = new Point(srcBottomLeft.X + 0, srcBottomLeft.Y + 2 * mirrorHeight);
            image.FlipImage(srcBottomLeft, destImage, dstBottomLeftOffset, FlipDirection.All);

            //top-right
            Rectangle srcTopRight = new Rectangle(image.Width() - mirrorWidth, 0, mirrorWidth, mirrorHeight);
            Point dstTopRightOffset = new Point(srcTopRight.X + 2 * mirrorWidth, srcTopRight.Y + 0);
            image.FlipImage(srcTopRight, destImage, dstTopRightOffset, FlipDirection.All);

            //bottom-right
            Rectangle srcBottomRight = new Rectangle(image.Width() - mirrorWidth, image.Height() - mirrorHeight, mirrorWidth, mirrorHeight);
            Point dstBottomRightOffset = new Point(srcBottomRight.X + 2 * mirrorWidth, srcBottomRight.Y + 2 * mirrorHeight);
            image.FlipImage(srcBottomRight, destImage, dstBottomRightOffset, FlipDirection.All);
        }

        private static ComplexF[,] prepareImage(Gray<float>[,] image, int biggestKernelWidth, int biggestKernelHeight,
                                                ConvolutionBorder options,
                                                out int fillX, out int fillY)
        {
            int FFTNumOfCols = (int)System.Math.Pow(2.0, System.Math.Ceiling(System.Math.Log(biggestKernelWidth + image.Width(), 2.0)));
            int FFTNumOfRows = (int)System.Math.Pow(2.0, System.Math.Ceiling(System.Math.Log(biggestKernelHeight + image.Height(), 2.0)));

            fillX = System.Math.Min(image.Width(), biggestKernelWidth / 2);
            fillY = System.Math.Min(image.Height(), biggestKernelHeight / 2);

            var paddedImage = new Gray<float>[FFTNumOfRows, FFTNumOfCols];

            //center
            image.CopyTo(paddedImage, new Point(fillX, fillY));

            if (options == ConvolutionBorder.BorderMirror)
            {
                mirrorBorders(image, paddedImage, fillX, fillY);
            }

            var paddedImageCmplx = paddedImage.ToComplex(); 
            paddedImageCmplx.FFT(FourierTransform.Direction.Forward, true);
            return paddedImageCmplx;
        }

        private static Gray<float>[,] convolve(Gray<float>[,] image, IList<float[,]> kernels, ConvolutionBorder options)
        {
            int biggestKernelWidth, biggestKernelHeight;
            getTheBiggestSize(kernels, out biggestKernelWidth, out biggestKernelHeight);

            int fillX, fillY;
            var paddedIm = prepareImage(image, biggestKernelWidth, biggestKernelHeight, options, out fillX, out fillY);

            var convolvedIm = paddedIm;
            foreach (var kernel in kernels)
            {
                var preparedKernel = prepareKernel(kernel, convolvedIm.Size());
                convolvedIm = convolvedIm.MulComplex(preparedKernel, inPlace: false);
            }

            return getConvolutionResult(convolvedIm, fillX, fillY, image.Size());
        }

        private static Gray<float>[,] getConvolutionResult(ComplexF[,] convolvedImage, int fillX, int fillY, Size imageSize)
        {
            ComplexF[,] iFFT_image = null; //using shorter way it could be written as: iFFT_image = convolvedImage.FFT(FourierTransform.Direction.Backward, transformImageInPlace);
            iFFT_image = convolvedImage.FFT(FourierTransform.Direction.Backward, false);

            Rectangle validRegion = new Rectangle(fillX * 2, fillY * 2, imageSize.Width, imageSize.Height);
    
            var result = convolvedImage.Magnitude(validRegion); //in the most general case (when input image is indeed complex) inverse FFT image can contain Im values != 0
            return result;
        }
   
        private static ComplexF[,] prepareKernel(float[,] kernel, Size paddedImageSize)
        {
            var preparedKernel = new ComplexF[paddedImageSize.Height, paddedImageSize.Width];
            preparedKernel.ReplaceRe(kernel, new Rectangle(new Point(), kernel.Size()));

            preparedKernel.FFT(FourierTransform.Direction.Forward, true);
            return preparedKernel;
        }       
    } 
}
