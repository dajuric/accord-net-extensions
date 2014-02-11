using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions.Imaging;
using Accord.Math;
using Accord.Extensions.Imaging.Helper;
using Accord.Extensions.Math;

namespace Accord.Extensions.Imaging
{
    public static class BlendExtensions
    {
        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// <para>Homography matrix is set to identity.</para>
        /// <para>Fill color is set to black with alpha set to 0 (all zeros).</para>
        /// </summary>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        private static Image<TColorDest, TDepth> Blend<TColorSrc, TColorDest, TDepth>(this Image<TColorSrc, TDepth> im, Image<TColorSrc, TDepth> overlayIm, bool gradient = true, bool alphaOnly = false)
            where TColorSrc : IColor //shuld be: Gray, IColor3, IColor4
            where TColorDest : IColor4
            where TDepth: struct
        { 
            var fillColor = HelperMethods.ArrayToColor<TColorDest, byte>(new byte[] { 0, 0, 0, 0 });
            return Blend(im, overlayIm, new MatrixH(Matrix.Identity(3)), fillColor, gradient, alphaOnly);
        }

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// </summary>
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
        private static Image<TColorDest, TDepth> Blend<TColorSrc, TColorDest, TDepth>(this Image<TColorSrc, TDepth> im, Image<TColorSrc, TDepth> overlayIm, MatrixH homography, TColorDest fillColor, bool gradient = true, bool alphaOnly = false)
            where TColorSrc: IColor
            where TColorDest: IColor4
            where TDepth: struct
        {
            var overlay = overlayIm.ToBitmap(copyAlways: false, failIfCannotCast: true);
           
            Blend blend = new Blend(homography, overlay);
            blend.AlphaOnly = alphaOnly;
            blend.Gradient = gradient;
            blend.FillColor = fillColor.ToColor(); 

            return im.ApplyFilter<TColorSrc, TDepth, Blend, TColorDest>(blend);
        }

        #region Gray

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// <para>Homography matrix is set to identity.</para>
        /// <para>Fill color is set to black with alpha set to 0 (all zeros).</para>
        /// </summary>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        public static Image<TColorDest, byte> Blend<TColorDest>(this Image<Gray, byte> im, Image<Gray, byte> overlayIm, bool gradient = true, bool alphaOnly = false)
            where TColorDest : IColor4
        {
            return im.Blend<Gray, TColorDest, byte>(overlayIm, gradient, alphaOnly);
        }

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// </summary>
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
        public static Image<TColorDest, byte> Blend<TColorDest>(this Image<Gray, byte> im, Image<Gray, byte> overlayIm, MatrixH homography, TColorDest fillColor, bool gradient = true, bool alphaOnly = false)
            where TColorDest : IColor4
        {
            return im.Blend<Gray, TColorDest, byte>(overlayIm, homography, fillColor, gradient, alphaOnly);
        }

        #endregion

        #region IColor3

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// <para>Homography matrix is set to identity.</para>
        /// <para>Fill color is set to black with alpha set to 0 (all zeros).</para>
        /// </summary>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        public static Image<TColorDest, byte> Blend<TColorSrc, TColorDest>(this Image<TColorSrc, byte> im, Image<TColorSrc, byte> overlayIm, bool gradient = true, bool alphaOnly = false)
            where TColorSrc: IColor3
            where TColorDest : IColor4
        {
            return im.Blend<TColorSrc, TColorDest, byte>(overlayIm, gradient, alphaOnly);
        }

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// </summary>
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
        public static Image<TColorDest, byte> Blend<TColorSrc, TColorDest>(this Image<TColorSrc, byte> im, Image<TColorSrc, byte> overlayIm, MatrixH homography, TColorDest fillColor, bool gradient = true, bool alphaOnly = false)
            where TColorSrc: IColor3
            where TColorDest : IColor4
        {
            return im.Blend<TColorSrc, TColorDest, byte>(overlayIm, homography, fillColor, gradient, alphaOnly);
        }

        #endregion

        /* TODO: critical: check why it dows not compile when uncommented
        #region IColor4

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// <para>Homography matrix is set to identity.</para>
        /// <para>Fill color is set to black with alpha set to 0 (all zeros).</para>
        /// </summary>
        /// <param name="overlayIm">The overlay image (also called the anchor).</param>
        /// <param name="gradient">A value indicating whether to blend using a linear
        ///  gradient or just superimpose the two images with equal weights.</param>
        /// <param name="alphaOnly">A value indicating whether only the alpha channel
        /// should be blended. This can be used together with a transparency
        /// mask to selectively blend only portions of the image.</param>
        /// <returns>Blended image.</returns>
        public static Image<TColorDest, byte> Blend<TColorSrc, TColorDest>(this Image<TColorSrc, byte> im, Image<TColorSrc, byte> overlayIm, bool gradient = true, bool alphaOnly = false)
            where TColorSrc : IColor4
            where TColorDest : IColor4
        {
            return im.Blend<TColorSrc, TColorDest, byte>(overlayIm, gradient, alphaOnly);
        }

        /// <summary>
        /// The blending filter is able to blend two images using a homography matrix.
        /// A linear alpha gradient is used to smooth out differences between the two
        /// images, effectively blending them in two images. The gradient is computed
        /// considering the distance between the centers of the two images.
        /// </summary>
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
        public static Image<TColorDest, byte> Blend<TColorSrc, TColorDest>(this Image<TColorSrc, byte> im, Image<TColorSrc, byte> overlayIm, MatrixH homography, TColorDest fillColor, bool gradient = true, bool alphaOnly = false)
            where TColorSrc : IColor4
            where TColorDest : IColor4
        {
            return im.Blend<TColorSrc, TColorDest, byte>(overlayIm, homography, fillColor, gradient, alphaOnly);
        }

        #endregion
        */
    }
}
