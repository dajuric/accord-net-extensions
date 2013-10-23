using Accord.Core;
using Accord.Imaging.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorConverter = Accord.Imaging.Converters.ColorConverter;

namespace Accord.Imaging
{
    public static class BitmapConversionExtensions
    {
        public class PixelFormatMapping
        {
            public ColorInfo ColorInfo;
            public PixelFormat PixelFormat;
        }

        public static readonly List<PixelFormatMapping> PixelFormatMappings;
        //public static readonly Lookup<PixelFormat, ColorInfo> PixelToColorMapping;
        //public static readonly Lookup<ColorInfo, PixelFormat> ColorToPixelMapping;

        static BitmapConversionExtensions()
        {
            PixelFormatMappings = initPixelFormatMappings();
            //var color = PixelToColorMapping[PixelFormat.Canonical].First();
        }

        private static List<PixelFormatMapping> initPixelFormatMappings()
        {
            var mappings = new List<PixelFormatMapping>
            {
                {new PixelFormatMapping{ ColorInfo = ColorInfo.GetInfo<Gray, byte>(), PixelFormat = PixelFormat.Format8bppIndexed}},

                {new PixelFormatMapping{ ColorInfo = ColorInfo.GetInfo<Gray, short>(), PixelFormat = PixelFormat.Format16bppGrayScale}},

                {new PixelFormatMapping{ ColorInfo = ColorInfo.GetInfo<Color3, byte>(), PixelFormat = PixelFormat.Format24bppRgb}},
            
                {new PixelFormatMapping{ ColorInfo = ColorInfo.GetInfo<Color4, byte>(), PixelFormat = PixelFormat.Format32bppArgb}},

                {new PixelFormatMapping{ ColorInfo = ColorInfo.GetInfo<Color3, short>(), PixelFormat = PixelFormat.Format48bppRgb}},

                {new PixelFormatMapping{ ColorInfo = ColorInfo.GetInfo<Color4, short>(), PixelFormat = PixelFormat.Format64bppArgb}}
            };

            return mappings;
        }

        #region Conversion from Bitmap

        /// <summary>
        /// Converts a bitmap to an image (copied data). 
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
            var convertedImage = ((GenericImageBase)im).Convert(ColorInfo.GetInfo<TColor, TDepth>(), false);
            return convertedImage as Image<TColor, TDepth>;
        }

         /// <summary>
        /// Converts a bitmap to an image (copied data). 
        /// If an output color is not matched with bitmap pixel format additional conversion may be applied.
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <returns>Generic image.</returns>
        public static Image<TColor, TDepth> ToImage<TColor, TDepth>(this Image bmp)
            where TColor : IColor
            where TDepth : struct
        {
            return ToImage<TColor, TDepth>((Bitmap)bmp);
        }

        /// <summary>
        /// Converts a bitmap to an image (copied data). Color type depeneds on bitmap pixel format.
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <param name="img">Destination generic image.</param>
        public static void ToImage(this Bitmap bmp, IImage img)
        {
            var imageColor = bmp.GetColorInfo();

            if (imageColor == null)
                throw new Exception(string.Format("Pixel format {0} is not supported!", bmp.PixelFormat));

            if(ColorInfo.Equals(bmp.GetColorInfo(), img.ColorInfo, ColorInfo.ComparableParts.Castable) == false)
                throw new Exception(string.Format("Image color {0} is not valid for this bitmap pixel format!", img.ColorInfo));

            if(img.Width != bmp.Width || img.Height != bmp.Height)
                throw new Exception(string.Format("Image sizes must match!"));

            BitmapData bmpData = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);
            HelperMethods.CopyImage(bmpData.Scan0, img.ImageData, bmpData.Stride, img.Stride, img.Width * img.ColorInfo.Size, img.Height);
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// Converts a bitmap to an image (copied data). Color type depeneds on bitmap pixel format.
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <returns>Generic image.</returns>
        public static IImage ToImage(this Bitmap bmp)
        {
            var imageColor = bmp.GetColorInfo();
            IImage im = GenericImageBase.Create(imageColor, bmp.Width, bmp.Height);
            bmp.ToImage(im);

            return im;
        }

        /// <summary>
        /// Converts a bitmap to an image (copied data). Color type depeneds on bitmap pixel format.
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <returns>Generic image.</returns>
        public static IImage ToImage(this System.Drawing.Image bmp)
        {
            return ToImage((Bitmap)bmp);
        }

        public static ColorInfo GetColorInfo(this Image bmp)
        {
            var imageColor = (from pixelFormatMapping in PixelFormatMappings
                              where pixelFormatMapping.PixelFormat == bmp.PixelFormat
                              select pixelFormatMapping.ColorInfo)
                           .DefaultIfEmpty()
                           .FirstOrDefault();

            return imageColor;
        }

        #endregion

        #region Conversion To Bitmap

        public static readonly PixelFormat[] DefaultPreferedDestPixelFormats = new PixelFormat[] //compatibility reasons (some Bitmap functions do not work with some PixelFormats)
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
        /// <param name="failIfCannotCast">Set to true to ensure that data will not be copied. <see cref="copyAlways"/> is ommited.</param>
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
        /// <param name="failIfCannotCast">Set to true to ensure that data will not be copied. <see cref="copyAlways"/> is ommited.</param>
        /// <param name="preferedDestinationFormats">Set prefered output format.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this IImage img, bool copyAlways = false, bool failIfCannotCast = false, params PixelFormat[] preferedDestinationFormats)
        {
            bool justCasted;
            IImage convertedIm = ToBitmapCompatibilityImage(img, preferedDestinationFormats, out justCasted);
            var destPixelFormat = PixelFormatMappings.Find(x => x.ColorInfo == convertedIm.ColorInfo).PixelFormat;

            if (failIfCannotCast && justCasted == false)
                throw new Exception("An image can not be casted to bitmap as user requested (data must be copied)!");

            copyAlways = copyAlways && !failIfCannotCast; //ommit the switch if an user setfailIfCannotCast to true

            Bitmap bmp = null;

            if (!copyAlways)
                bmp = new Bitmap(convertedIm.Width, convertedIm.Height, convertedIm.Stride, destPixelFormat, convertedIm.ImageData);
            else
            {
                bmp = new Bitmap(convertedIm.Width, convertedIm.Height, destPixelFormat);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, convertedIm.Width, convertedIm.Height), ImageLockMode.WriteOnly, destPixelFormat);

                HelperMethods.CopyImage(convertedIm.ImageData, bmpData.Scan0, convertedIm.Stride, bmpData.Stride, bmpData.Stride, convertedIm.Height);

                bmp.UnlockBits(bmpData);
                convertedIm.Dispose();
            }

            if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                AForge.Imaging.Image.SetGrayscalePalette(bmp);

            return bmp;
        }

        internal static IImage ToBitmapCompatibilityImage(IImage srcImg, PixelFormat[] preferedDestinationFormats, out bool isJustCasted)
        {
            var preferedDestinationColors = from preferedFormat in preferedDestinationFormats
                                            let preferedColor = PixelFormatMappings.Where(x => x.PixelFormat == preferedFormat).Select(x => x.ColorInfo).FirstOrDefault()
                                            where preferedColor != null
                                            select preferedColor;

            var path = ColorConverter.GetMostInexepnsiveConversionPath(srcImg.ColorInfo,
                                                                       preferedDestinationColors.ToArray());

            if (path == null)
            {
                throw new Exception(String.Format("Image with color: {0} cannot be converted to System.Drawing.Bitmap", srcImg.ColorInfo.ColorType.Name));
            }

            isJustCasted = ColorConverter.ConversionPathCopiesData(path).Value == false;

            IImage convertedIm = ColorConverter.Convert(srcImg, path.ToArray(), false);
            return convertedIm;
        }

        #endregion
    }
}
