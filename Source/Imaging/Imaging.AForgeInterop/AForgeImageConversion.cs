using System;
using System.Linq;
using Accord.Extensions.Imaging.Converters;
using AForge.Imaging;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for <see cref="AForge.Imaging.UnmanagedImage"/> interoperability.
    /// </summary>
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
            var imageColor = BitmapConversionExtensions.PixelFormatMappings.Reverse[unmanagedImage.PixelFormat];

            if (imageColor == null)
                throw new Exception(string.Format("Pixel format {0} is not supported!", unmanagedImage.PixelFormat));

            IImage im = Image.Create(imageColor, 
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

            var convertedImage = ((Image)im).Convert(ColorInfo.GetInfo<TColor, TDepth>(), copyAlways);
            return convertedImage as Image<TColor, TDepth>;
        }

        /// <summary>
        /// Converts an image to AForge (UnmanagedImage). In case when only cast is needed data is not copied. (Performance: if cast => ~0.03 ms per call)
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="copyAlways">Forces data copy even in the case when only cast is sufficient.</param>
        /// <param name="failIfCannotCast">Fails if an image data must be converted to a format that is supported by UnmanagedImage. Switch <paramref name="copyAlways"/> is omitted.</param>
        /// <returns>Converted unmanaged image.</returns>
        public static UnmanagedImage ToAForgeImage(this IImage img, bool copyAlways = false, bool failIfCannotCast = false)
        {
            //all formats (please note that during Image > Bitmap conversion DefaultPreferedDestPixelFormats are chosen for compatibility reasons.
            //put DefaultPreferedDestPixelFormats if an user encounters problems (e.g. during image saving - Bitmap does not support saving some PixelFormats)
            var preferedDestinationFormats = BitmapConversionExtensions.PixelFormatMappings.Reverse.ToArray();

            bool justCast;
            IImage convertedIm = BitmapConversionExtensions.ToBitmapCompatibilityImage(img, preferedDestinationFormats, out justCast);
            if (justCast == false && failIfCannotCast)
                throw new Exception("Image can not be casted to AForge image. Data must be copied!");

            var destPixelFormat = BitmapConversionExtensions.PixelFormatMappings.Forward[convertedIm.ColorInfo];

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
        /// <returns>Is image cast-able to UnmanagedImage.</returns>
        public static bool CanCastToAForgeImage(this IImage image)
        {
            ColorInfo[] preferedColors = BitmapConversionExtensions.PixelFormatMappings.Forward.ToArray();
            var conversionPath = ColorDepthConverter.GetPath(image.ColorInfo, preferedColors);

            bool isImageCopied = ColorDepthConverter.CopiesData(conversionPath);
            return !isImageCopied;
        }
    }
}
