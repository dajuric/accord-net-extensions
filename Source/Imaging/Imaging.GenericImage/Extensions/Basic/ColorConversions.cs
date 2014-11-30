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
using Accord.Extensions.Imaging.Converters;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for image color conversion.
    /// </summary>
    public static class ColorConversionsExtensions
    {
        /// <summary>
        /// Converts the image from source to destination color and depth.
        /// Data may be shared if casting is used. To prevent that set <paramref name="copyAlways"/> to true.
        /// </summary>
        /// <typeparam name="DestColor">Destination color (IColor).</typeparam>
        /// <typeparam name="DestType">Destination type (primitive type).</typeparam>
        /// <param name="image">Image.</param>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <param name="failIfCannotCast">If data copy is needed throws an exception.</param>
        /// <returns>Converted image.</returns>
        public static Image<DestColor, DestType> Convert<DestColor, DestType>(this IImage image, bool copyAlways = false, bool failIfCannotCast = false)
            where DestColor : IColor
            where DestType : struct
        {
            return Convert(image, ColorInfo.GetInfo<DestColor, DestType>(), copyAlways, failIfCannotCast) as Image<DestColor, DestType>;
        }

        /// <summary>
        /// Converts the image from source to destination color and depth.
        /// Data may be shared if casting is used. To prevent that set <paramref name="copyAlways"/> to true.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="destColor">Destination color info.</param>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <param name="failIfCannotCast">If data copy is needed throws an exception.</param>
        /// <returns>Converted image.</returns>
        public static IImage Convert(this IImage image, ColorInfo destColor, bool copyAlways = false, bool failIfCannotCast = false)
        {
            if (image == null) return null;

            var conversionPath = ColorDepthConverter.GetPath(image.ColorInfo, destColor);

            if (conversionPath == null /*it should never be null*/ || conversionPath.Count == 0)
            {
                throw new Exception(String.Format("Image does not support conversion from {0} to {1}", image.ColorInfo.ColorType, destColor.ColorType));
            }

            if (failIfCannotCast && conversionPath.CopiesData() == true)
            {
                throw new Exception("Fail if cannot cast is set to true: Image data must be copied");
            }

            var convertedIm = ColorDepthConverter.Convert(image, conversionPath.ToArray(), copyAlways);
            return convertedIm;
        }
    }
}

