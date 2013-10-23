using Accord.Core;
using Accord.Imaging.Converters;
using System;

namespace Accord.Imaging
{
    public partial class Image<TColor, TDepth> : GenericImageBase
        where TColor : IColor
        where TDepth : struct
    {
        /// <summary>
        /// Converts the image from source to destination color and depth.
        /// Data may be shared if casting is used. To prevent that set <see cref="copyAlways"/> to true.
        /// </summary>
        /// <typeparam name="DestColor">Destination color (IColor).</typeparam>
        /// <typeparam name="DestType">Destination type (primitive type).</typeparam>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <returns>Converted image.</returns>
        public Image<DestColor, DestType> Convert<DestColor, DestType>(bool copyAlways = false)
            where DestColor : IColor
            where DestType: struct
        { 
            return Convert(ColorInfo.GetInfo<DestColor, DestType>()) as Image<DestColor, DestType>;
        }

        /// <summary>
        /// Converts the image from source to destination depth. 
        /// Data may be shared if casting is used. To prevent that set <see cref="copyAlways"/> to true.
        /// </summary>
        /// <typeparam name="DestType">Destination type (primitive type).</typeparam>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <returns>Converted image.</returns>
        public Image<TColor, DestType> Convert<DestType>(bool copyAlways = false)
            where DestType : struct
        {
            return Convert(ColorInfo.GetInfo<TColor, DestType>()) as Image<TColor, DestType>;
        }
    }

    public partial class GenericImageBase
    {
        /// <summary>
        /// Converts the image from source to destination color and depth.
        /// Data may be shared if casting is used. To prevent that set <see cref="copyAlways"/> to true.
        /// </summary>
        /// <param name="destColor">Destination color info.</param>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <returns>Converted image.</returns>
        public IImage Convert(ColorInfo destColor, bool copyAlways = false)
        {
            var conversionPath = ColorConverter.GetMostInexepnsiveConversionPath(this.ColorInfo, destColor);
            var convertedIm = ColorConverter.Convert(this, conversionPath.ToArray(), copyAlways);

            if (convertedIm == null)
            {
                throw new Exception(String.Format("Image does not support conversion from {0} to {1}", this.ColorInfo.ColorType, destColor.ColorType));
            }

            return convertedIm;
        }
    }
}
