using System;
using AForge.Imaging.Filters;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Interpolation mode.
    /// </summary>
    public enum InterpolationMode
    { 
        /// <summary>
        /// Nearest-neighbor interpolation.
        /// </summary>
        NearestNeighbor,
        /// <summary>
        /// Bilinear interpolation.
        /// </summary>
        Bilinear,
        /// <summary>
        /// Bicubic interpolation.
        /// </summary>
        Bicubic
    }

    /// <summary>
    /// Contains image resize extensions.
    /// </summary>
    internal static class ResizeExtensionsBase
    {
        /// <summary>
        /// Resizes an image using specified interpolation mode.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="newSize">New image size.</param>
        /// <param name="mode">Interpolation mode.</param>
        /// <returns>Resized image.</returns>
        internal static Image<TColor, TDepth> Resize<TColor, TDepth>(this Image<TColor, TDepth> img, Size newSize, InterpolationMode mode)
            where TColor: IColor
            where TDepth: struct
        {
            switch (mode)
            {
                case InterpolationMode.NearestNeighbor:
                    //return img.ApplyFilter(new ResizeNearestNeighbor(newSize.Width, newSize.Height));
                    return ResizeNearsetNeighbur.ResizeNN(img, newSize); //faster
                case InterpolationMode.Bilinear:
                    return img.ApplyFilter(new ResizeBilinear(newSize.Width, newSize.Height));
                case InterpolationMode.Bicubic:
                    return img.ApplyFilter(new ResizeBicubic(newSize.Width, newSize.Height));
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Resizes an image using specified interpolation mode.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="scale">Non-negative image size scale factor. If 1 the new size will be equal.</param>
        /// <param name="mode">Interpolation mode.</param>
        /// <returns>Resized image.</returns>
        internal static Image<TColor, TDepth> Resize<TColor, TDepth>(this Image<TColor, TDepth> img, float scale, InterpolationMode mode)
            where TColor : IColor
            where TDepth : struct
        {
            var newSize = new Size 
            {
                Width = (int)System.Math.Round(img.Width * scale),
                Height = (int)System.Math.Round(img.Height * scale)
            };

            return Resize(img, newSize, mode);
        }
    }

    /// <summary>
    /// Contains image resize extensions.
    /// </summary>
    public static class ResizeExtensions_Gray
    {
        /// <summary>
        /// Resizes an image using specified interpolation mode.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="newSize">New image size.</param>
        /// <param name="mode">Interpolation mode.</param>
        /// <returns>Resized image.</returns>
        public static Image<Gray, byte> Resize(this Image<Gray, byte> img, Size newSize, InterpolationMode mode)
        {
            return ResizeExtensionsBase.Resize<Gray, byte>(img, newSize, mode);
        }

        /// <summary>
        /// Resizes an image using specified interpolation mode.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="scale">Non-negative image size scale factor. If 1 the new size will be equal.</param>
        /// <param name="mode">Interpolation mode.</param>
        /// <returns>Resized image.</returns>
        public static Image<Gray, byte> Resize(this Image<Gray, byte> img, float scale, InterpolationMode mode)
        {
            return ResizeExtensionsBase.Resize<Gray, byte>(img, scale, mode);
        }
    }

    /// <summary>
    /// Contains image resize extensions.
    /// </summary>
    public static class ResizeExtensions_Color
    {
        /// <summary>
        /// Resizes an image using specified interpolation mode.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="newSize">New image size.</param>
        /// <param name="mode">Interpolation mode.</param>
        /// <returns>Resized image.</returns>
        public static Image<TColor, byte> Resize<TColor>(this Image<TColor, byte> img, Size newSize, InterpolationMode mode)
            where TColor : IColor3
        {
            return ResizeExtensionsBase.Resize<TColor, byte>(img, newSize, mode);
        }

        /// <summary>
        /// Resizes an image using specified interpolation mode.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="scale">Non-negative image size scale factor. If 1 the new size will be equal.</param>
        /// <param name="mode">Interpolation mode.</param>
        /// <returns>Resized image.</returns>
        public static Image<TColor, byte> Resize<TColor>(this Image<TColor, byte> img, float scale, InterpolationMode mode)
             where TColor : IColor3
        {
            return ResizeExtensionsBase.Resize<TColor, byte>(img, scale, mode);
        }
    }
}
