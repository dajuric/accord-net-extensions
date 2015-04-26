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
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions;
using System.IO;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Bitmap file save extensions.
    /// </summary>
    public static class BmpSaveExtensions
    {
        /// <summary>
        /// Saves the specified image.
        /// <para>
        /// Quality parameter is only supported for JPEG, PNG file types. 
        /// For other file types this value is omitted.
        /// </para>
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="filename">File name.</param>
        /// <param name="quality">Quality parameter [0..100] where 0 means maximum compression.</param>
        public static void Save(this System.Drawing.Image image, string filename, int quality = 90)
        {
            var encoder = getEncoder(new FileInfo(filename).Extension);

            if (encoder != null)
            {
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, quality);

                image.Save(filename, encoder, myEncoderParameters);
            }
            else
            {
                image.Save(filename);
            }   
        }

        private static ImageCodecInfo getEncoder(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return getEncoderCashed(ImageFormat.Jpeg);
                case ".png":
                    return getEncoderCashed(ImageFormat.Png);
                default:
                    return null;
            }
        }

        private static ImageCodecInfo getEncoderCashed(ImageFormat imageFormat)
        {
            return MethodCache.Global.Invoke<ImageFormat, ImageCodecInfo>((format) => getEncoder(format), imageFormat);
        }

        private static ImageCodecInfo getEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
