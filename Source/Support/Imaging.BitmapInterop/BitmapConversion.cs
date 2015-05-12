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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides extension methods for converting generic image into <see cref="System.Drawing.Bitmap"/>.
    /// </summary>
    public static class BitmapConversionExtensions
    {
        #region Conversion from Bitmap

        private static TColor[,] toArray<TColor>(Bitmap bmp)
            where TColor: struct, IColor
        {
            var bmpData = bmp.LockBits(ImageLockMode.ReadOnly);

            var arr = new TColor[bmp.Height, bmp.Width];
            using (var img = arr.Lock())
            {
                Copy.UnsafeCopy(bmpData.Scan0, img.ImageData, bmpData.Stride, img.Stride, bmpData.Height);
            }

            bmp.UnlockBits(bmpData);
            return arr;
        }

        /// <summary>
        /// Converts a bitmap to an image (copies data). 
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <returns>2D array.</returns>
        public static Array ToArray(this Bitmap bmp)
        {
            Array arr = null;
            switch (bmp.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    arr = toArray<Gray<byte>>(bmp);
                    break;
                case PixelFormat.Format16bppGrayScale:
                     arr = toArray<Gray<short>>(bmp);
                    break;
                case PixelFormat.Format24bppRgb:
                    arr = toArray<Bgr<byte>>(bmp);
                    break;
                case PixelFormat.Format32bppArgb:
                    arr = toArray<Bgra<byte>>(bmp);
                    break;
                case PixelFormat.Format48bppRgb:
                    arr = toArray<Bgr<short>>(bmp);
                    break;
                case PixelFormat.Format64bppArgb:
                    arr = toArray<Bgra<short>>(bmp);
                    break; 
                default:
                    throw new NotSupportedException();
            }

            return arr;
        }
   
        #endregion

        #region Conversion To Bitmap

        private static Bitmap toBitmap(IImage img, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(img.Width, img.Height, pixelFormat);
            var bmpData = bmp.LockBits(ImageLockMode.WriteOnly);
            Copy.UnsafeCopy(img.ImageData, bmpData.Scan0, img.Stride, bmpData.Stride, bmpData.Height);
            bmp.UnlockBits(bmpData);

            if (pixelFormat == PixelFormat.Format8bppIndexed)
                bmp.SetGrayscalePalette();

            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Gray<byte>> img)
        {
            return toBitmap(img, PixelFormat.Format8bppIndexed);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Gray<short>> img)
        {
            return toBitmap(img, PixelFormat.Format16bppGrayScale);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Bgr<byte>> img)
        {
            return toBitmap(img, PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Bgra<byte>> img)
        {
            return toBitmap(img, PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Bgr<short>> img)
        {
            return toBitmap(img, PixelFormat.Format48bppRgb);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Bgra<short>> img)
        {
            return toBitmap(img, PixelFormat.Format64bppArgb);
        }


        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Gray<byte>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format8bppIndexed); 
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Gray<short>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format16bppGrayScale);
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Bgr<byte>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format24bppRgb);
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Bgra<byte>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format32bppArgb);
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Bgr<short>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format48bppRgb);
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Bgra<short>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format64bppArgb);
            }
            return bmp;
        }

        #endregion

        #region Cast to Bitmap

        private static Bitmap asBitmap(IImage img, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(img.Width, img.Height, img.Stride, pixelFormat, img.ImageData);

            if (pixelFormat == PixelFormat.Format8bppIndexed)
                bmp.SetGrayscalePalette();

            return bmp;
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Gray<byte>> img)
        {
            return asBitmap(img, PixelFormat.Format8bppIndexed);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Gray<short>> img)
        {
            return asBitmap(img, PixelFormat.Format16bppGrayScale);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Bgr<byte>> img)
        {
            return asBitmap(img, PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Bgra<byte>> img)
        {
            return asBitmap(img, PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Bgr<short>> img)
        {
            return asBitmap(img, PixelFormat.Format48bppRgb);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Bgra<short>> img)
        {
            return asBitmap(img, PixelFormat.Format64bppArgb);
        }

        #endregion

        #region Misc
 
        /// <summary>
        /// Replaces color palette entries with grayscale intensities (256 entries).
        /// </summary>
        /// <param name="image">The 8-bpp grayscale image.</param>
        public static void SetGrayscalePalette(this Bitmap image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException("The provided image must have 8bpp pixel format.");

            var palette = image.Palette;
            for (int i = 0; i < (Byte.MaxValue + 1); i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            image.Palette = palette;
        }

        /// <summary>
        /// Lock a <see cref="System.Drawing.Bitmap"/> into system memory.
        /// </summary>
        /// <param name="bmp">Bitmap to lock.</param>
        /// <param name="imageLockMode">Specifies the access level.</param>
        /// <returns>Bitmap data.</returns>
        public static BitmapData LockBits(this Bitmap bmp, ImageLockMode imageLockMode = ImageLockMode.ReadWrite)
        {
            return bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), imageLockMode, bmp.PixelFormat);
        }

        #endregion
    }
}
