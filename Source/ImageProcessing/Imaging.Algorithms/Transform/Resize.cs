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
        internal static TColor[,] Resize<TColor>(this TColor[,] img, Size newSize, InterpolationMode mode)
            where TColor: struct, IColor
        {
            switch (mode)
            {
                case InterpolationMode.NearestNeighbor:
                    //return img.ApplyFilter(new ResizeNearestNeighbor(newSize.Width, newSize.Height));
                    return ResizeNearsetNeighbur.Resize(img, newSize); //faster
                case InterpolationMode.Bilinear:
                    return img.ApplyBaseTransformationFilter(new ResizeBilinear(newSize.Width, newSize.Height));
                case InterpolationMode.Bicubic:
                    return img.ApplyBaseTransformationFilter(new ResizeBicubic(newSize.Width, newSize.Height));
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
        internal static TColor[,] Resize<TColor>(this TColor[,] img, float scale, InterpolationMode mode)
            where TColor : struct, IColor
        {
            var newSize = new Size 
            {
                Width = (int)System.Math.Round(img.Width() * scale),
                Height = (int)System.Math.Round(img.Height() * scale)
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
        public static Gray<byte>[,] Resize(this Gray<byte>[,] img, Size newSize, InterpolationMode mode)
        {
            return ResizeExtensionsBase.Resize<Gray<byte>>(img, newSize, mode);
        }

        /// <summary>
        /// Resizes an image using specified interpolation mode.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="scale">Non-negative image size scale factor. If 1 the new size will be equal.</param>
        /// <param name="mode">Interpolation mode.</param>
        /// <returns>Resized image.</returns>
        public static Gray<byte>[,] Resize(this Gray<byte>[,] img, float scale, InterpolationMode mode)
        {
            return ResizeExtensionsBase.Resize<Gray<byte>>(img, scale, mode);
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
        public static TColor[,] Resize<TColor>(this TColor[,] img, Size newSize, InterpolationMode mode)
            where TColor : struct, IColor3<byte>
        {
            return ResizeExtensionsBase.Resize<TColor>(img, newSize, mode);
        }

        /// <summary>
        /// Resizes an image using specified interpolation mode.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="scale">Non-negative image size scale factor. If 1 the new size will be equal.</param>
        /// <param name="mode">Interpolation mode.</param>
        /// <returns>Resized image.</returns>
        public static TColor[,] Resize<TColor>(this TColor[,] img, float scale, InterpolationMode mode)
             where TColor : struct, IColor3<byte>
        {
            return ResizeExtensionsBase.Resize<TColor>(img, scale, mode);
        }
    }
}
