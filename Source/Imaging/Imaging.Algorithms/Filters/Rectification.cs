using Accord.Imaging;
using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Rectification filter.
    /// </summary>
    static class RectificationExtensionsBase
    {
        /// <summary>
        /// Rectification filter for projective transformation.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.Rectification"/> for details.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="homography">The homography matrix used to map a image passed to the filter to the overlay image.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces.</param>
        /// <returns>Rectified image.</returns>
        public static Image<TColor, TDepth> Rectification<TColor, TDepth>(this Image<TColor, TDepth> img, double[,] homography, TColor fillColor)
            where TColor : IColor
            where TDepth : struct
        {
            Rectification r = new Rectification(homography);
            r.FillColor = fillColor.ToColor();
         
            return img.ApplyFilter(r);
        }
    }

    /// <summary>
    /// Contains extensions for Rectification filter.
    /// </summary>
    public static class RectificationExtensionsGray
    {
        /// <summary>
        /// Rectification filter for projective transformation.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.Rectification"/> for details.</para>
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="homography">The homography matrix used to map a image passed to the filter to the overlay image.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces.</param>
        /// <returns>Rectified image.</returns>
        public static Image<Gray, byte> Rectification(this Image<Gray, byte> img, double[,] homography, Gray fillColor)
        {
            return RectificationExtensionsBase.Rectification(img, homography, fillColor);
        }
    }

    /// <summary>
    /// Contains extensions for Rectification filter.
    /// </summary>
    public static class RectificationExtensionsIColor3
    {
        /// <summary>
        /// Rectification filter for projective transformation.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.Rectification"/> for details.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="homography">The homography matrix used to map a image passed to the filter to the overlay image.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces.</param>
        /// <returns>Rectified image.</returns>
        public static Image<TColor, byte> Rectification<TColor>(this Image<TColor, byte> img, double[,] homography, TColor fillColor)
            where TColor: IColor3
        {
            return RectificationExtensionsBase.Rectification(img, homography, fillColor);
        }
    }

    /// <summary>
    /// Contains extensions for Rectification filter.
    /// </summary>
    public static class RectificationExtensionsIColor4
    {
        /// <summary>
        /// Rectification filter for projective transformation.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.Rectification"/> for details.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="homography">The homography matrix used to map a image passed to the filter to the overlay image.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces.</param>
        /// <returns>Rectified image.</returns>
        public static Image<TColor, byte> Rectification<TColor>(this Image<TColor, byte> img, double[,] homography, TColor fillColor)
            where TColor : IColor4
        {
            return RectificationExtensionsBase.Rectification(img, homography, fillColor);
        }
    }
}
