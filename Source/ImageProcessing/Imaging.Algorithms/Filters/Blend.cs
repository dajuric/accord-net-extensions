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

using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains blend filter extensions.
    /// </summary>
    public static class BlendExtensions
    {
        #region Gray

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// <para>Homography matrix is set to identity.</para>
        /// <para>Fill color is set to black with alpha set to 0 (all zeros).</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        public static Bgra<byte>[,] Blend(this Gray<byte>[,] im, Gray<byte>[,] overlayIm, bool gradient = true, bool alphaOnly = false)
        {
            return Blend(im, overlayIm, new MatrixH(Matrix.Identity(3)), new Bgra<byte>(), gradient, alphaOnly);
        }

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="homography">Homography matrix used to map a image passed to
        /// the filter to the overlay image specified at filter creation.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces. The filling color will only be visible after the image is converted
        /// to 24bpp. The alpha channel will be used internally by the filter.</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        public static Bgra<byte>[,] Blend(this Gray<byte>[,] im, Gray<byte>[,] overlayIm, MatrixH homography, Bgra<byte> fillColor, bool gradient = true, bool alphaOnly = false)
        {
            Bgra<byte>[,] resultImage = null;

            using (var uOverlayIm = overlayIm.Lock())
            {
                Blend blend = new Blend(homography, uOverlayIm.AsBitmap());
                blend.AlphaOnly = alphaOnly;
                blend.Gradient = gradient;
                blend.FillColor = fillColor.ToColor();

                resultImage = im.ApplyBaseTransformationFilter<Gray<byte>, Bgra<byte>>(blend);
            }

            return resultImage;
        }

        #endregion

        #region Bgr

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// <para>Homography matrix is set to identity.</para>
        /// <para>Fill color is set to black with alpha set to 0 (all zeros).</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        public static Bgra<byte>[,] Blend(this Bgr<byte>[,] im, Bgr<byte>[,] overlayIm, bool gradient = true, bool alphaOnly = false)
        {
            return Blend(im, overlayIm, new MatrixH(Matrix.Identity(3)), new Bgra<byte>(), gradient, alphaOnly);
        }

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="homography">Homography matrix used to map a image passed to
        /// the filter to the overlay image specified at filter creation.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces. The filling color will only be visible after the image is converted
        /// to 24bpp. The alpha channel will be used internally by the filter.</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        public static Bgra<byte>[,] Blend(this Bgr<byte>[,] im, Bgr<byte>[,] overlayIm, MatrixH homography, Bgra<byte> fillColor, bool gradient = true, bool alphaOnly = false)
        {
            Bgra<byte>[,] resultImage = null;

            using (var uOverlayIm = overlayIm.Lock())
            {
                Blend blend = new Blend(homography, uOverlayIm.AsBitmap());
                blend.AlphaOnly = alphaOnly;
                blend.Gradient = gradient;
                blend.FillColor = fillColor.ToColor();

                resultImage = im.ApplyBaseTransformationFilter<Bgr<byte>, Bgra<byte>>(blend);
            }

            return resultImage;
        }

        #endregion

        #region Bgra

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// <para>Homography matrix is set to identity.</para>
        /// <para>Fill color is set to black with alpha set to 0 (all zeros).</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        public static Bgra<byte>[,] Blend(this Bgra<byte>[,] im, Bgra<byte>[,] overlayIm, bool gradient = true, bool alphaOnly = false)
        {
            return Blend(im, overlayIm, new MatrixH(Matrix.Identity(3)), new Bgra<byte>(), gradient, alphaOnly);
        }

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="homography">Homography matrix used to map a image passed to
        /// the filter to the overlay image specified at filter creation.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces. The filling color will only be visible after the image is converted
        /// to 24bpp. The alpha channel will be used internally by the filter.</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        public static Bgra<byte>[,] Blend(this Bgra<byte>[,] im, Bgra<byte>[,] overlayIm, MatrixH homography, Bgra<byte> fillColor, bool gradient = true, bool alphaOnly = false)
        {
            Bgra<byte>[,] resultImage = null;

            using (var uOverlayIm = overlayIm.Lock())
            {
                Blend blend = new Blend(homography, uOverlayIm.AsBitmap());
                blend.AlphaOnly = alphaOnly;
                blend.Gradient = gradient;
                blend.FillColor = fillColor.ToColor();

                resultImage = im.ApplyBaseTransformationFilter<Bgra<byte>, Bgra<byte>>(blend);
            }

            return resultImage;
        }

        #endregion
    }
}
