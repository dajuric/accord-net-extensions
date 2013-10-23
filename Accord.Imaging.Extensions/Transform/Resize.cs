using Accord.Core;
using AForge.Imaging.Filters;
using System;
using System.Drawing;

namespace Accord.Imaging
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

    public static class ResizeExtensions
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
            return Resize<Gray, byte>(img, newSize, mode);
        }

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
            return Resize<TColor, byte>(img, newSize, mode);
        }

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
                    return img.ApplyFilter(new ResizeNearestNeighbor(newSize.Width, newSize.Height), (filter) => new Size(filter.NewWidth, filter.NewHeight));
                case InterpolationMode.Bilinear:
                    return img.ApplyFilter(new ResizeBilinear(newSize.Width, newSize.Height), (filter) => new Size(filter.NewWidth, filter.NewHeight));
                case InterpolationMode.Bicubic:
                    return img.ApplyFilter(new ResizeBicubic(newSize.Width, newSize.Height), (filter) => new Size(filter.NewWidth, filter.NewHeight));
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
