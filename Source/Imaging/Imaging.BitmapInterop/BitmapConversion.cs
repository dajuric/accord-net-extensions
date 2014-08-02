#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
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
using ColorConverter = Accord.Extensions.Imaging.Converters.ColorDepthConverter;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides extension methods for converting generic image into <see cref="System.Drawing.Bitmap"/>.
    /// </summary>
    public static class BitmapConversionExtensions
    {
        /// <summary>
        /// mappings between color infos and pixel formats.
        /// </summary>
        public static readonly Map<ColorInfo, PixelFormat> PixelFormatMappings;

        static BitmapConversionExtensions()
        {
            PixelFormatMappings = initPixelFormatMappings();
        }

        private static Map<ColorInfo, PixelFormat> initPixelFormatMappings()
        {
            var map = new Map<ColorInfo, PixelFormat>();

            map.Add(ColorInfo.GetInfo<Gray, byte>(),  PixelFormat.Format8bppIndexed);
            map.Add(ColorInfo.GetInfo<Gray, short>(), PixelFormat.Format16bppGrayScale);
            map.Add(ColorInfo.GetInfo<Bgr, byte>(),   PixelFormat.Format24bppRgb);
            map.Add(ColorInfo.GetInfo<Bgra, byte>(),  PixelFormat.Format32bppArgb);
            map.Add(ColorInfo.GetInfo<Bgr, short>(),  PixelFormat.Format48bppRgb);
            map.Add(ColorInfo.GetInfo<Bgra, short>(), PixelFormat.Format64bppArgb);

            return map;
        }

        #region Conversion from Bitmap

        /// <summary>
        /// Converts a bitmap to an image (copies data). 
        /// If an output color is not matched with bitmap pixel format additional conversion may be applied.
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <returns>Generic image.</returns>
        public static Image<TColor, TDepth> ToImage<TColor, TDepth>(this Bitmap bmp)
            where TColor: IColor
            where TDepth: struct
        {
            IImage im = ToImage(bmp);

            //convert if necessary
            var convertedImage = ((Image)im).Convert(ColorInfo.GetInfo<TColor, TDepth>(), false);
            return convertedImage as Image<TColor, TDepth>;
        }

         /// <summary>
        /// Converts a bitmap to an image (copies data). 
        /// If an output color is not matched with bitmap pixel format additional conversion may be applied.
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <returns>Generic image.</returns>
        public static Image<TColor, TDepth> ToImage<TColor, TDepth>(this System.Drawing.Image bmp)
            where TColor : IColor
            where TDepth : struct
        {
            return ToImage<TColor, TDepth>((Bitmap)bmp);
        }

        /// <summary>
        /// Converts a bitmap to an image (copies data). Color type depends on bitmap pixel format.
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <param name="img">Destination generic image.</param>
        public static void ToImage(this Bitmap bmp, IImage img)
        {
            var imageColor = bmp.GetColorInfo();

            if (imageColor == null)
                throw new Exception(string.Format("Pixel format {0} is not supported!", bmp.PixelFormat));

            if(bmp.GetColorInfo().Equals(img.ColorInfo, ColorInfo.ComparableParts.Castable) == false)
                throw new Exception(string.Format("Image color {0} is not valid for this bitmap pixel format!", img.ColorInfo));

            if(img.Width != bmp.Width || img.Height != bmp.Height)
                throw new Exception(string.Format("Image sizes must match!"));

            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);
            HelperMethods.CopyImage(bmpData.Scan0, img.ImageData, bmpData.Stride, img.Stride, img.Width * img.ColorInfo.Size, img.Height);
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// Converts a bitmap to an image (copies data). Color type depends on bitmap pixel format.
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <returns>Generic image.</returns>
        public static IImage ToImage(this Bitmap bmp)
        {
            var imageColor = bmp.GetColorInfo();
            IImage im = Image.Create(imageColor, bmp.Width, bmp.Height);
            bmp.ToImage(im);

            return im;
        }

        /// <summary>
        /// Converts a bitmap to an image (copies data). Color type depends on bitmap pixel format.
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <returns>Generic image.</returns>
        public static IImage ToImage(this System.Drawing.Image bmp)
        {
            return ToImage((Bitmap)bmp);
        }

        /// <summary>
        /// Gets color info by using bitmap pixel format. 
        /// </summary>
        /// <param name="bmp">Bitmap.</param>
        /// <returns>Color info.</returns>
        public static ColorInfo GetColorInfo(this System.Drawing.Image bmp)
        {
            return GetColorInfo(bmp.PixelFormat);
        }

        /// <summary>
        /// Gets color info by using bitmap pixel format. 
        /// </summary>
        /// <param name="pixelFormat">Pixel format.</param>
        /// <returns>Color info.</returns>
        public static ColorInfo GetColorInfo(this PixelFormat pixelFormat)
        {
            ColorInfo colorInfo;
            PixelFormatMappings.Reverse.TryGetValue(pixelFormat, out colorInfo);
            return colorInfo;
        }

        #endregion

        #region Conversion from BitmapData

        /// <summary>
        /// Converts bitmap data to generic image (data is not copied).
        /// </summary>
        /// <param name="bmpData">Bitmap data.</param>
        /// <returns>Generic image.</returns>
        public static IImage AsImage(this BitmapData bmpData)
        {
            var colorInfo = GetColorInfo(bmpData.PixelFormat);
            return Image.Create(colorInfo, bmpData.Scan0, bmpData.Width, bmpData.Height, bmpData.Stride, bmpData /*can it be null ?*/);
        }

        #endregion

        #region Conversion To Bitmap

        /// <summary>
        /// Contains preferred pixel formats used when converting to <see cref="System.Drawing.Bitmap"/>.
        /// <para>Compatibility reasons (some Bitmap functions do not work with some PixelFormats)</para>
        /// </summary>
        public static readonly PixelFormat[] DefaultPreferedDestPixelFormats = new PixelFormat[] 
        {
           PixelFormat.Format8bppIndexed,
           PixelFormat.Format24bppRgb,
           PixelFormat.Format32bppArgb
        };

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="copyAlways">Set to true to force data copy even if a cast is enough.</param>
        /// <param name="failIfCannotCast">Set to true to ensure that data will not be copied. <paramref name="copyAlways"/> is omitted.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this IImage img, bool copyAlways = false, bool failIfCannotCast = false)
        {
            return ToBitmap(img, copyAlways, failIfCannotCast, DefaultPreferedDestPixelFormats);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <param name="copyAlways">Set to true to force data copy even if a cast is enough.</param>
        /// <param name="failIfCannotCast">Set to true to ensure that data will not be copied. <paramref name="copyAlways"/> is omitted.</param>
        /// <param name="preferedDestinationFormats">Set preferred output format.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this IImage img, bool copyAlways = false, bool failIfCannotCast = false, params PixelFormat[] preferedDestinationFormats)
        {
            bool justCasted;
            IImage convertedIm = ToBitmapCompatibilityImage(img, preferedDestinationFormats, out justCasted);
            var destPixelFormat = PixelFormatMappings.Forward[convertedIm.ColorInfo];

            if (failIfCannotCast && justCasted == false)
                throw new Exception("An image can not be casted to bitmap as user requested (data must be copied)!");

            copyAlways = copyAlways && !failIfCannotCast; //omit the switch if an user setfailIfCannotCast to true

            Bitmap bmp = null;

            if (!copyAlways)
                bmp = new Bitmap(convertedIm.Width, convertedIm.Height, convertedIm.Stride, destPixelFormat, convertedIm.ImageData);
            else
            {
                bmp = new Bitmap(convertedIm.Width, convertedIm.Height, destPixelFormat);
                BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, convertedIm.Width, convertedIm.Height), ImageLockMode.WriteOnly, destPixelFormat);

                HelperMethods.CopyImage(convertedIm.ImageData, bmpData.Scan0, convertedIm.Stride, bmpData.Stride, bmpData.Stride, convertedIm.Height);

                bmp.UnlockBits(bmpData);
                convertedIm.Dispose();
            }

            if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                bmp.SetGrayscalePalette();

            return bmp;
        }

        /// <summary>
        /// Converts generic image into bitmap compatibility image by using provided preferred pixel formats.
        /// </summary>
        /// <param name="srcImg">Source image.</param>
        /// <param name="preferedDestinationFormats">Preferred destination pixel formats.</param>
        /// <param name="isJustCasted">True if the image is just casted. False if data must be converted.</param>
        /// <returns>Generic image which color corresponds to one of the <paramref name="preferedDestinationFormats"/>.</returns>
        public static IImage ToBitmapCompatibilityImage(IImage srcImg, PixelFormat[] preferedDestinationFormats, out bool isJustCasted)
        {
            var preferedDestinationColors = from preferedFormat in preferedDestinationFormats
                                            let preferedColor = preferedFormat.GetColorInfo()
                                            where preferedColor != null
                                            select preferedColor;

            var path = ColorConverter.GetPath(srcImg.ColorInfo, preferedDestinationColors.ToArray());

            if (path == null)
            {
                throw new Exception(String.Format("Image with color: {0} cannot be converted to System.Drawing.Bitmap", srcImg.ColorInfo.ColorType.Name));
            }

            isJustCasted = ColorConverter.CopiesData(path) == false;

            IImage convertedIm = ColorConverter.Convert(srcImg, path.ToArray(), false);
            return convertedIm;
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
