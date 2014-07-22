using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    static class WhitePatchExtensionsBase
    {
        /// <summary>
        /// Applies White Patch filter for color normalization (Accord.NET function)
        /// </summary>
        /// <param name="img">image.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        internal static Image<TColor, TDepth> WhitePatch<TColor, TDepth>(this Image<TColor, TDepth> img, bool inPlace = true)
            where TColor: IColor
            where TDepth : struct
        {
            WhitePatch wp = new WhitePatch();
            return img.ApplyFilter(wp, inPlace);
        }
    }

    /// <summary>
    /// Contains extensions for White-patch algorithm.
    /// </summary>
    public static class WhitePatchExtensionsIColor3
    {
        /// <summary>
        /// Applies White Patch filter for color normalization (Accord.NET function)
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static Image<Bgr, byte> WhitePatch(this Image<Bgr, byte> img, bool inPlace = true)
        {
            return WhitePatchExtensionsBase.WhitePatch<Bgr, byte>(img, inPlace);
        }
    }

    /// <summary>
    /// Contains extensions for White-patch algorithm.
    /// </summary>
    public static class WhitePatchExtensionsIColor4
    {
        /// <summary>
        /// Applies White Patch filter for color normalization (Accord.NET function)
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static Image<Bgra, byte> WhitePatch(this Image<Bgra, byte> img, bool inPlace = true)
        {
            return WhitePatchExtensionsBase.WhitePatch<Bgra, byte>(img, inPlace);
        }
    }
}
