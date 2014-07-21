using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    static class VarianceExtensionsBase
    {
        /// <summary>
        /// <para>(Accord .NET internal call)</para>
        /// The Variance filter replaces each pixel in an image by its
        /// neighborhood variance. The end result can be regarded as an
        /// border enhancement, making the Variance filter suitable to
        /// be used as an edge detection mechanism.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="radius">The radius neighborhood used to compute a pixel's local variance.</param>
        /// <returns>Processed image.</returns>
        internal static Image<TColor, TDepth> Variance<TColor, TDepth>(this Image<TColor, TDepth> img, int radius = 2)
            where TColor : IColor
            where TDepth : struct
        {
            Variance v = new Variance(radius);
            return img.ApplyFilter(v);
        }
    }

    /// <summary>
    /// Contains methods for variance calculation.
    /// </summary>
    public static class VarianceExtensionsGray
    {
        /// <summary>
        /// <para>(Accord .NET internal call)</para>
        /// The Variance filter replaces each pixel in an image by its
        /// neighborhood variance. The end result can be regarded as an
        /// border enhancement, making the Variance filter suitable to
        /// be used as an edge detection mechanism.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="radius">The radius neighborhood used to compute a pixel's local variance.</param>
        /// <returns>Processed image.</returns>
        internal static Image<Gray, byte> Variance(this Image<Gray, byte> img, int radius = 2)
        {
            return VarianceExtensionsBase.Variance(img, radius);
        }
    }

    /// <summary>
    /// Contains methods for variance calculation.
    /// </summary>
    public static class VarianceExtensionsColor3
    { 
        /// <summary>
        /// <para>(Accord .NET internal call)</para>
        /// The Variance filter replaces each pixel in an image by its
        /// neighborhood variance. The end result can be regarded as an
        /// border enhancement, making the Variance filter suitable to
        /// be used as an edge detection mechanism.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="radius">The radius neighborhood used to compute a pixel's local variance.</param>
        /// <returns>Processed image.</returns>
        internal static Image<TColor, byte> Variance<TColor>(this Image<TColor, byte> img, int radius = 2)
            where TColor : IColor3
        {
            return VarianceExtensionsBase.Variance(img, radius);
        }
    }

    /// <summary>
    /// Contains methods for variance calculation.
    /// </summary>
    public static class VarianceExtensionsBgra
    {
        /// <summary>
        /// <para>(Accord .NET internal call)</para>
        /// The Variance filter replaces each pixel in an image by its
        /// neighborhood variance. The end result can be regarded as an
        /// border enhancement, making the Variance filter suitable to
        /// be used as an edge detection mechanism.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="radius">The radius neighborhood used to compute a pixel's local variance.</param>
        /// <returns>Processed image.</returns>
        internal static Image<Bgra, byte> Variance(this Image<Bgra, byte> img, int radius = 2)
        {
            return VarianceExtensionsBase.Variance(img, radius);
        }
    }

}
