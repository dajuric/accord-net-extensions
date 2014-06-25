using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    static class WhitePatchExtensionsBase
    {
        /// <summary>
        /// Applies White Patch filter for color normalization (Accord.NET function)
        /// </summary>
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

    public static class WhitePatchExtensionsIColor3
    {
        /// <summary>
        /// Applies White Patch filter for color normalization (Accord.NET function)
        /// </summary>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, byte> WhitePatch<TColor>(this Image<TColor, byte> img, bool inPlace = true)
            where TColor : IColor3
        {
            return WhitePatchExtensionsBase.WhitePatch<TColor, byte>(img, inPlace);
        }
    }

    public static class WhitePatchExtensionsIColor4
    {
        /// <summary>
        /// Applies White Patch filter for color normalization (Accord.NET function)
        /// </summary>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, byte> WhitePatch<TColor>(this Image<TColor, byte> img, bool inPlace = true)
            where TColor : IColor4
        {
            return WhitePatchExtensionsBase.WhitePatch<TColor, byte>(img, inPlace);
        }
    }
}
