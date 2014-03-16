using Accord.Extensions;
using Accord.Extensions.Imaging.Converters;
using System;

namespace Accord.Extensions.Imaging
{
    public partial class Image<TColor, TDepth> : Image
        where TColor : IColor
        where TDepth : struct
    {
        /// <summary>
        /// Converts the image from source to destination depth. 
        /// Data may be shared if casting is used. To prevent that set <see cref="copyAlways"/> to true.
        /// </summary>
        /// <typeparam name="DestType">Destination type (primitive type).</typeparam>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <param name="failIfCannotCast">If data copy is needed throws an exception.</param>
        /// <returns>Converted image.</returns>
        public Image<TColor, DestType> Convert<DestType>(bool copyAlways = false, bool failIfCAnnotCast = false)
            where DestType : struct
        {
            return this.Convert(ColorInfo.GetInfo<TColor, DestType>()) as Image<TColor, DestType>;
        }
    }

    public static class ImageColorExtensions
    {
        /// <summary>
        /// Converts the image from source to destination color and depth.
        /// Data may be shared if casting is used. To prevent that set <see cref="copyAlways"/> to true.
        /// </summary>
        /// <typeparam name="DestColor">Destination color (IColor).</typeparam>
        /// <typeparam name="DestType">Destination type (primitive type).</typeparam>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <param name="failIfCannotCast">If data copy is needed throws an exception.</param>
        /// <returns>Converted image.</returns>
        public static Image<DestColor, DestType> Convert<DestColor, DestType>(this IImage image, bool copyAlways = false, bool failIfCannotCast = false)
            where DestColor : IColor
            where DestType : struct
        {
            return Convert(image, ColorInfo.GetInfo<DestColor, DestType>()) as Image<DestColor, DestType>;
        }

        /// <summary>
        /// Converts the image from source to destination color and depth.
        /// Data may be shared if casting is used. To prevent that set <see cref="copyAlways"/> to true.
        /// </summary>
        /// <param name="destColor">Destination color info.</param>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <param name="failIfCannotCast">If data copy is needed throws an exception.</param>
        /// <returns>Converted image.</returns>
        public static IImage Convert(this IImage image, ColorInfo destColor, bool copyAlways = false, bool failIfCannotCast = false)
        {
            var conversionPath = ColorDepthConverter.GetPath(image.ColorInfo, destColor);
            
            if (conversionPath == null)
            {
                throw new Exception(String.Format("Image does not support conversion from {0} to {1}", image.ColorInfo.ColorType, destColor.ColorType));
            }

            if (failIfCannotCast && conversionPath.CopiesData() == true)
            {
                throw new Exception("Fail if cannot cast is set to true: Image data must be copied");
            }

            var convertedIm = ColorDepthConverter.Convert(conversionPath.ToArray(), image, copyAlways);
            return convertedIm;
        }
    }
}
