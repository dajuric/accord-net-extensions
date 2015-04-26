using Accord.Extensions;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides color conversion extension methods.
    /// </summary>
    public static class ColorConversionExtensions
    {
        #region Gray channel depth conversion

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Gray<TDepth>[,] Cast<TDepth>(this Gray<byte>[,] image)
           where TDepth : struct
        {
            return image.ConvertChannelDepth<Gray<byte>, Gray<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Gray<TDepth>[,] Cast<TDepth>(this Gray<short>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Gray<short>, Gray<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Gray<TDepth>[,] Cast<TDepth>(this Gray<int>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Gray<int>, Gray<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Gray<TDepth>[,] Cast<TDepth>(this Gray<float>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Gray<float>, Gray<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Gray<TDepth>[,] Cast<TDepth>(this Gray<double>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Gray<double>, Gray<TDepth>>();
        }

        #endregion

        #region Gray color conversion

        /// <summary>
        /// Converts the source color to the destination color.
        /// </summary>
        /// <param name="grayIm">Source image.</param>
        /// <returns>image with converted color.</returns>
        public static Bgr<byte>[,] ToBgr(this Gray<byte>[,] grayIm)
        {
            return grayIm.Convert<Gray<byte>, Bgr<byte>>(Gray<byte>.Convert);
        }

        /// <summary>
        /// Converts the source color to the destination color.
        /// </summary>
        /// <param name="grayIm">Source image.</param>
        /// <param name="area">Working area.</param>
        /// <returns>image with converted color.</returns>
        public static Bgr<byte>[,] ToBgr(this Gray<byte>[,] grayIm, Rectangle area)
        {
            return grayIm.Convert<Gray<byte>, Bgr<byte>>(Gray<byte>.Convert, area);
        }

        #endregion


        #region Bgr channel depth conversion

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Bgr<TDepth>[,] Cast<TDepth>(this Bgr<byte>[,] image)
           where TDepth : struct
        {
            return image.ConvertChannelDepth<Bgr<byte>, Bgr<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Bgr<TDepth>[,] Cast<TDepth>(this Bgr<short>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Bgr<short>, Bgr<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Bgr<TDepth>[,] Cast<TDepth>(this Bgr<int>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Bgr<int>, Bgr<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Bgr<TDepth>[,] Cast<TDepth>(this Bgr<float>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Bgr<float>, Bgr<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Bgr<TDepth>[,] Cast<TDepth>(this Bgr<double>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Bgr<double>, Bgr<TDepth>>();
        }

        #endregion

        #region Bgr color conversion

        /// <summary>
        /// Converts the source color to the destination color.
        /// </summary>
        /// <param name="image">Source image.</param>
        /// <returns>image with converted color.</returns>
        public static Gray<byte>[,] ToGray(this Bgr<byte>[,] image)
        {
            return image.Convert<Bgr<byte>, Gray<byte>>(Bgr<byte>.Convert);
        }

        /// <summary>
        /// Converts the source color to the destination color.
        /// </summary>
        /// <param name="image">Source image.</param>
        /// <param name="area">Working area.</param>
        /// <returns>image with converted color.</returns>
        public static Gray<byte>[,] ToGray(this Bgr<byte>[,] image, Rectangle area)
        {
            return image.Convert<Bgr<byte>, Gray<byte>>(Bgr<byte>.Convert, area);
        }

        /// <summary>
        /// Converts the source color to the destination color.
        /// </summary>
        /// <param name="image">Source image.</param>
        /// <returns>image with converted color.</returns>
        public static Hsv<byte>[,] ToHsv(this Bgr<byte>[,] image)
        {
            return image.Convert<Bgr<byte>, Hsv<byte>>(Bgr<byte>.Convert);
        }

        /// <summary>
        /// Converts the source color to the destination color.
        /// </summary>
        /// <param name="image">Source image.</param>
        /// <param name="area">Working area.</param>
        /// <returns>image with converted color.</returns>
        public static Hsv<byte>[,] ToHsv(this Bgr<byte>[,] image, Rectangle area)
        {
            return image.Convert<Bgr<byte>, Hsv<byte>>(Bgr<byte>.Convert, area);
        }

        #endregion


        #region Hsv channel depth conversion

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Hsv<TDepth>[,] Cast<TDepth>(this Hsv<byte>[,] image)
           where TDepth : struct
        {
            return image.ConvertChannelDepth<Hsv<byte>, Hsv<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Hsv<TDepth>[,] Cast<TDepth>(this Hsv<short>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Hsv<short>, Hsv<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Hsv<TDepth>[,] Cast<TDepth>(this Hsv<int>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Hsv<int>, Hsv<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Hsv<TDepth>[,] Cast<TDepth>(this Hsv<float>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Hsv<float>, Hsv<TDepth>>();
        }

        /// <summary>
        /// Converts the source channel depth to the destination channel depth.
        /// </summary>
        /// <typeparam name="TDepth">Destination channel depth.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>Image with converted element depth.</returns>
        public static Hsv<TDepth>[,] Cast<TDepth>(this Hsv<double>[,] image)
          where TDepth : struct
        {
            return image.ConvertChannelDepth<Hsv<double>, Hsv<TDepth>>();
        }

        #endregion

        #region Hsv color conversion

        /// <summary>
        /// Converts the source color to the destination color.
        /// </summary>
        /// <param name="image">Source image.</param>
        /// <returns>image with converted color.</returns>
        public static Bgr<byte>[,] ToBgr(this Hsv<byte>[,] image)
        {
            return image.Convert<Hsv<byte>, Bgr<byte>>(Hsv<byte>.Convert);
        }

        /// <summary>
        /// Converts the source color to the destination color.
        /// </summary>
        /// <param name="image">Source image.</param>
        /// <param name="area">Working area.</param>
        /// <returns>image with converted color.</returns>
        public static Bgr<byte>[,] ToGray(this Hsv<byte>[,] image, Rectangle area)
        {
            return image.Convert<Hsv<byte>, Bgr<byte>>(Hsv<byte>.Convert, area);
        }

        #endregion
    }
}
