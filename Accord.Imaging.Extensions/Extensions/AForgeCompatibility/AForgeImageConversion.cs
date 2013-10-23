using Accord.Core;
using Accord.Imaging.Converters;
using Accord.Imaging.Helper;
using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
    public static class UnmanagedImageConversionExtensions
    {
        /// <summary>
        /// Converts unmanaged image to generic image (IImage) but without data copy. 
        /// Format depends upon unmanaged image pixel format (e.g. 8bppGray -> [Gray, byte], 24bppColor -> [Color3, byte]...).
        /// The same result produces <seealso cref="ToImage"/> if an output format matches unmanaged pixel format.
        /// </summary>
        /// <param name="unmanagedImage"> Unmanaged image</param>
        /// <returns>Generic image (interface)</returns>
        public static IImage AsImage(this UnmanagedImage unmanagedImage)
        {
            var imageColor = (from pixelFormatMapping in BitmapConversionExtensions.PixelFormatMappings
                              where pixelFormatMapping.PixelFormat == unmanagedImage.PixelFormat
                              select pixelFormatMapping.ColorInfo)
                              .DefaultIfEmpty()
                              .FirstOrDefault();

            if (imageColor == null)
                throw new Exception(string.Format("Pixel format {0} is not supported!", unmanagedImage.PixelFormat));

            IImage im = GenericImageBase.Create(imageColor, 
                                                unmanagedImage.ImageData, unmanagedImage.Width, unmanagedImage.Height, 
                                                unmanagedImage.Stride, unmanagedImage);

            return im;
        }

        /// <summary>
        /// Converts unmanaged image to generic image. Depending on output format a simple cast may be performed instead of data copy.
        /// <seealso cref="AsImage"/>
        /// </summary>
        /// <param name="unmanagedImage">Unmanaged image.</param>
        /// <param name="copyAlways">Forces data copy even if the cast is enough.</param>
        /// <returns>Generic image.</returns>
        public static Image<TColor, TDepth> ToImage<TColor, TDepth>(this UnmanagedImage unmanagedImage, bool copyAlways = false)
            where TColor : IColor
            where TDepth : struct
        {
            IImage im = AsImage(unmanagedImage);

            var convertedImage = ((GenericImageBase)im).Convert(ColorInfo.GetInfo<TColor, TDepth>(), copyAlways);
            return convertedImage as Image<TColor, TDepth>;
        }

        /// <summary>
        /// Converts an image to AForge (UnmanagedImage). In case when only cast is needed data is not copied. (Performance: if cast => ~0.03 ms per call)
        /// </summary>
        /// <param name="copyAlways">Forces data copy even in the case when only cast is sufficent.</param>
        /// <param name="failIfCannotCast">Fails if an image data must be converted to a format that is supported by UnmanagedImage. <see cref="copyAlways"/> switch is omitted.</param>
        /// <returns>Converted unmanaged image.</returns>
        public static UnmanagedImage ToAForgeImage(this IImage img, bool copyAlways = false, bool failIfCannotCast = false)
        {
            //all formats (please note that during Image > Bitmap conversion DefaultPreferedDestPixelFormats are choosen for compatibility reasons.
            //put DefaultPreferedDestPixelFormats if an user encounters problems (e.g. during image saving - Bitmap does not support saving some PixelFormats)
            PixelFormat[] preferedDestinationFormats = BitmapConversionExtensions.PixelFormatMappings.Select(x => x.PixelFormat).ToArray();

            bool justCast;
            IImage convertedIm = BitmapConversionExtensions.ToBitmapCompatibilityImage(img, preferedDestinationFormats, out justCast);
            if (justCast == false && failIfCannotCast)
                throw new Exception("Image can not be casted to AForge image. Data must be copied!");

            var destPixelFormat = BitmapConversionExtensions.PixelFormatMappings.Find(x => x.ColorInfo == convertedIm.ColorInfo).PixelFormat;

            UnmanagedImage uImg = null;

            if (!copyAlways) 
                uImg = new UnmanagedImage(convertedIm.ImageData, convertedIm.Width, convertedIm.Height, convertedIm.Stride, destPixelFormat);
            else
            {
                uImg = UnmanagedImage.Create(convertedIm.Width, convertedIm.Height, destPixelFormat);
              
                HelperMethods.CopyImage(convertedIm.ImageData, uImg.ImageData, convertedIm.Stride, uImg.Stride, uImg.Stride, convertedIm.Height);

                convertedIm.Dispose();
            }

            return uImg;
        }

        /// <summary>
        /// Returns if an image can be casted to UnmanagedImage without data copy.
        /// </summary>
        /// <param name="image">Generic image</param>
        /// <returns>Is image castable to UnmanagedImage.</returns>
        public static bool CanCastToAForgeImage(this IImage image)
        {
            ColorInfo[] preferedColors = BitmapConversionExtensions.PixelFormatMappings.Select(x => x.ColorInfo).ToArray();
            var conversionPath = ColorConverter.GetMostInexepnsiveConversionPath(image.ColorInfo, preferedColors);

            bool isImageCopied = ColorConverter.ConversionPathCopiesData(conversionPath).Value;
            return !isImageCopied;
        }

    }
}
