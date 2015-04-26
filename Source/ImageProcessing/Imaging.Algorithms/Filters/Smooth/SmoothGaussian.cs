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

using Accord.Math;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Gaussian smooth extensions.
    /// </summary>
    public static class SmoothGaussianExtensions
    {
        /// <summary>
        /// Smooths an image with Gaussian kernel.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="kernelSize">Kernel size.</param>
        /// <param name="sigma">Sigma (standard deviation)</param>
        /// <returns>Smoothed image.</returns>
        public static TColor[,] SmoothGaussian<TColor>(this TColor[,] img, int kernelSize, double sigma)
            where TColor : struct, IColor<float>
        {
            var gaussianKernel = new AForge.Math.Gaussian(sigma); //TODO: it would be nice if I could get separated kernel
            var kernel = gaussianKernel.Kernel2D(kernelSize);
            
            //normalize kernel
            double factor = 1d / kernel.Sum().Sum();
            kernel.ApplyInPlace(x => x * factor);

            return img.Convolve(kernel.ToSingle());
        }

        /// <summary>
        /// <para>Smooths an image with Gaussian kernel.</para>
        /// 
        /// <para>
        /// Sigma calculation: 
        /// When you want to cover an area below the Gaussian (centered on the mean) that covers 99.73% of the total area, you have to at least consider an interval of size 3*sigma, spread in both directions from the mean.
        /// In other words, using the parameter n of discrete cells for representing a Gaussian, we get 3*sigma &lt;= n/2. We could reformulate this as
        /// 3*sigma &lt;= (n/2-1) + (1+eps), where eps is a non-zero constant. Rearranging gives sigma &lt;= 0.3334*(n/2-1) + (1+eps)/3.
        /// eps is set to: 1.4
        /// </para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="kernelSize">Kernel size.</param>
        /// <returns>Smoothed image.</returns>
        public static TColor[,] SmoothGaussian<TColor>(this TColor[,] img, int kernelSize)
            where TColor : struct, IColor<float>
        {
            const double eps = 1.4;
            double sigma = 0.3334 * ((double)kernelSize / 2 - 1) + (1 + eps) / 3;

            return SmoothGaussian(img, kernelSize, sigma);
        }
    }
}
