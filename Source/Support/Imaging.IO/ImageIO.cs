using Accord.Extensions.Imaging;
using System;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides methods for image saving and loading
    /// </summary>
    public static class ImageIO 
    {
        //TODO: Load overloads for float, double... ??
        //TODO: add imDecode, imEncode (requires CvMat implementation)

        private unsafe static IImage load(string fileName, ImageLoadType imageLoadType)
        {
            var iplImagePtr = CvHighGuiInvoke.cvLoadImage(fileName, imageLoadType);
            var image = (*iplImagePtr).AsImage((_) =>
            {
                if (iplImagePtr == null) return;
                CvHighGuiInvoke.cvReleaseImage(ref iplImagePtr);
            });

            return image;
        }

        /// <summary>
        /// Loads an image with the specified path and name as it is.
        /// </summary>
        /// <param name="fileName">Image file name.</param>
        /// <returns>Image.</returns>
        public unsafe static IImage LoadUnchanged(string fileName)
        {
            return load(fileName, ImageLoadType.AnyColor | ImageLoadType.AnyDepth);
        }

        /// <summary>
        /// Loads an image with the specified path and name and performs and RGB conversion.
        /// </summary>
        /// <param name="fileName">Image filename.</param>
        /// <returns>Image.</returns>
        public unsafe static Image<Bgr<byte>> LoadColor(string fileName)
        {
            return load(fileName, ImageLoadType.Color) as Image<Bgr<byte>>;
        }

        /// <summary>
        /// Loads an image with the specified path and name and performs and gray conversion.
        /// </summary>
        /// <param name="fileName">Image filename.</param>
        /// <returns>Image.</returns>
        public unsafe static Image<Gray<byte>> LoadGray(string fileName)
        {
            return load(fileName, ImageLoadType.Grayscale) as Image<Gray<byte>>;
        }

        /// <summary>
        /// Saves the provided image. If the image has non-supported color or depth false value is returned.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Filename.</param>
        /// <returns>True if the image is saved, false otherwise.</returns>
        public unsafe static bool TrySave(IImage image, string fileName)
        {
            IplImage iplImage = default(IplImage);
            try
            {
                iplImage = image.AsOpenCvImage();
            }
            catch 
            {
                return false;
            }

            CvHighGuiInvoke.cvSaveImage(fileName, &iplImage, IntPtr.Zero);
            return true;
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <typeparam name="TColor">Image color.</typeparam>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        private unsafe static void Save<TColor>(this Image<TColor> image, string fileName)
            where TColor : struct, IColor
        {
            var iplImage = image.AsOpenCvImage();
            CvHighGuiInvoke.cvSaveImage(fileName, &iplImage, IntPtr.Zero);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <typeparam name="TColor">Image color.</typeparam>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        private unsafe static void Save<TColor>(this TColor[,] image, string fileName)
            where TColor: struct, IColor
        {
            using (var img = image.Lock())
            {
                var iplImage = img.AsOpenCvImage();
                CvHighGuiInvoke.cvSaveImage(fileName, &iplImage, IntPtr.Zero);
            }
        }

        #region Save-gray

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Gray<byte>[,] image, string fileName)
        {
            image.Save<Gray<byte>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Gray<sbyte>[,] image, string fileName)
        {
            image.Save<Gray<sbyte>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Gray<short>[,] image, string fileName)
        {
            image.Save<Gray<short>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Gray<ushort>[,] image, string fileName)
        {
            image.Save<Gray<ushort>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Gray<int>[,] image, string fileName)
        {
            image.Save<Gray<int>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Gray<float>[,] image, string fileName)
        {
            image.Save<Gray<float>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Gray<double>[,] image, string fileName)
        {
            image.Save<Gray<double>>(fileName);
        }

        #endregion

        #region Save-bgr

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgr<byte>[,] image, string fileName)
        {
            image.Save<Bgr<byte>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgr<sbyte>[,] image, string fileName)
        {
            image.Save<Bgr<sbyte>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgr<short>[,] image, string fileName)
        {
            image.Save<Bgr<short>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgr<ushort>[,] image, string fileName)
        {
            image.Save<Bgr<ushort>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgr<int>[,] image, string fileName)
        {
            image.Save<Bgr<int>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgr<float>[,] image, string fileName)
        {
            image.Save<Bgr<float>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgr<double>[,] image, string fileName)
        {
            image.Save<Bgr<double>>(fileName);
        }

        #endregion

        #region Save-bgra

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgra<byte>[,] image, string fileName)
        {
            image.Save<Bgra<byte>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgra<sbyte>[,] image, string fileName)
        {
            image.Save<Bgra<sbyte>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgra<short>[,] image, string fileName)
        {
            image.Save<Bgra<short>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgra<ushort>[,] image, string fileName)
        {
            image.Save<Bgra<ushort>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgra<int>[,] image, string fileName)
        {
            image.Save<Bgra<int>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgra<float>[,] image, string fileName)
        {
            image.Save<Bgra<float>>(fileName);
        }

        /// <summary>
        /// Saves the specified image.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="fileName">Image filename.</param>
        public static void Save(this Bgra<double>[,] image, string fileName)
        {
            image.Save<Bgra<double>>(fileName);
        }

        #endregion
    }
}
