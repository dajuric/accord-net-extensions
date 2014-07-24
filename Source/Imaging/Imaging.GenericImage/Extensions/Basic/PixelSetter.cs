using System;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for setting pixel values using other image as data source.
    /// </summary>
    public static class PixelSetter
    {
        /// <summary>
        /// Sets image pixels.
        /// </summary>
        /// <param name="img">Destination image.</param>
        /// <param name="srcDataImg">Source image.</param>
        public static void SetValue<TColor, TDepth>(this Image<TColor, TDepth> img, Image<TColor, TDepth> srcDataImg)
            where TColor : IColor
            where TDepth : struct
        {
            SetValue((IImage)img, srcDataImg);
        }

        /// <summary>
        /// Sets image pixels.
        /// </summary>
        /// <param name="img">Destination image.</param>
        /// <param name="srcDataImg">Source image.</param>
        public static void SetValue(this IImage img, IImage srcDataImg)
        {
            if (img.Size != srcDataImg.Size)
                throw new Exception("Both images must be the same size!");

            if (img.ColorInfo.Equals(srcDataImg.ColorInfo, ColorInfo.ComparableParts.Castable) == false)
                throw new Exception("Image and dest image must be cast-able (the same number of channels, the same channel type)!");

            int bytesPerRow = img.Width * img.ColorInfo.Size;
            HelperMethods.CopyImage(srcDataImg.ImageData, img.ImageData, srcDataImg.Stride, img.Stride, bytesPerRow, img.Height);
        }

    }
}
