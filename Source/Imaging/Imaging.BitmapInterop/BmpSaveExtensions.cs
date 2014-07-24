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
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

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
