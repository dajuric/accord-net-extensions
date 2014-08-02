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

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for setting pixel values using other image as data source.
    /// </summary>
    public static class PixelSetter
    {
        /// <summary>
        /// Sets image pixels.
        /// </summary>
        /// <param name="img">Destination image.</param>
        /// <param name="srcDataImg">Source image.</param>
        public static void SetValue<TColor, TDepth>(this Image<TColor, TDepth> img, Image<TColor, TDepth> srcDataImg)
            where TColor : IColor
            where TDepth : struct
        {
            SetValue((IImage)img, srcDataImg);
        }

        /// <summary>
        /// Sets image pixels.
        /// </summary>
        /// <param name="img">Destination image.</param>
        /// <param name="srcDataImg">Source image.</param>
        public static void SetValue(this IImage img, IImage srcDataImg)
        {
            if (img.Size != srcDataImg.Size)
                throw new Exception("Both images must be the same size!");

            if (img.ColorInfo.Equals(srcDataImg.ColorInfo, ColorInfo.ComparableParts.Castable) == false)
                throw new Exception("Image and dest image must be cast-able (the same number of channels, the same channel type)!");

            int bytesPerRow = img.Width * img.ColorInfo.Size;
            HelperMethods.CopyImage(srcDataImg.ImageData, img.ImageData, srcDataImg.Stride, img.Stride, bytesPerRow, img.Height);
        }

    }
}
