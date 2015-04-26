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
using System.Linq;
using AForge.Imaging;
using System.Drawing.Imaging;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for <see cref="AForge.Imaging.UnmanagedImage"/> interoperability.
    /// </summary>
    public static class UnmanagedImageConversionExtensions
    {
        private static IImage asImage<TColor>(UnmanagedImage unmanagedImage)
            where TColor: struct
        {
            return new Image<TColor>(unmanagedImage.ImageData, unmanagedImage.Width, unmanagedImage.Height, unmanagedImage.Stride, null, null);
        }

        /// <summary>
        /// Converts unmanaged image to generic image (IImage) but without data copy. 
        /// Format depends upon unmanaged image pixel format (e.g. 8bppGray -> [Gray, byte], 24bppColor -> [Color3, byte]...).
        /// </summary>
        /// <param name="unmanagedImage"> Unmanaged image</param>
        /// <returns>Generic image (interface)</returns>
        public static IImage AsImage(this UnmanagedImage unmanagedImage)
        {
            IImage image = null;
            switch (unmanagedImage.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    image = asImage<Gray<byte>>(unmanagedImage);
                    break;
                case PixelFormat.Format16bppGrayScale:
                    image = asImage<Gray<short>>(unmanagedImage);
                    break;
                case PixelFormat.Format24bppRgb:
                    image = asImage<Bgr<byte>>(unmanagedImage);
                    break;
                case PixelFormat.Format32bppArgb:
                    image = asImage<Bgra<byte>>(unmanagedImage);
                    break;
                case PixelFormat.Format48bppRgb:
                    image = asImage<Bgr<short>>(unmanagedImage);
                    break;
                case PixelFormat.Format64bppArgb:
                    image = asImage<Bgra<short>>(unmanagedImage);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return image;
        }

        private static UnmanagedImage asAForgeImage(IImage image, PixelFormat pixelFormat)
        {
            return new UnmanagedImage(image.ImageData, image.Width, image.Height, image.Stride, pixelFormat);
        }

        /// <summary>
        /// Converts generic image to unmanaged image but without data copy. 
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="image">Unmanaged image</param>
        /// <returns>Unmanaged image.</returns>
        public static UnmanagedImage AsAForgeImage<TColor>(this Image<TColor> image)
            where TColor: struct, IColor
        {
            if (typeof(TColor).Equals(typeof(Gray<byte>)))
                return asAForgeImage(image, PixelFormat.Format8bppIndexed);
            else if (typeof(TColor).Equals(typeof(Gray<short>)))
                return asAForgeImage(image, PixelFormat.Format16bppGrayScale);
            else if (typeof(TColor).Equals(typeof(Bgr<byte>)))
                return asAForgeImage(image, PixelFormat.Format24bppRgb);
            else if (typeof(TColor).Equals(typeof(Bgra<byte>)))
                return asAForgeImage(image, PixelFormat.Format32bppArgb);
            else if (typeof(TColor).Equals(typeof(Bgr<short>)))
                return asAForgeImage(image, PixelFormat.Format48bppRgb);
            else if (typeof(TColor).Equals(typeof(Bgra<short>)))
                return asAForgeImage(image, PixelFormat.Format64bppArgb);
            else
                throw new NotSupportedException();
        }
    }
}
