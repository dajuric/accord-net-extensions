using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents gray color of type <typeparam name="T">color depth</typeparam>.
    /// </summary>
    //[ColorInfo(ConversionCodename = "Gray", IsGenericColorSpace = false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray<T> : IColor<T>
        where T: struct
    {
        /// <summary>
        /// Creates new gray color.
        /// </summary>
        /// <param name="intensity">Intensity.</param>
        public Gray(T intensity)
        {
            this.Intensity = intensity;
        }

        /// <summary>
        /// Gets or sets the intensity.
        /// </summary>
        public T Intensity;

        /// <summary>
        /// Converts gray structure to <typeparamref name="T"/> value.
        /// </summary>
        /// <param name="gray">Gray color.</param>
        /// <returns>Intensity.</returns>
        public static implicit operator T(Gray<T> gray)
        {
            return gray.Intensity;
        }

        /// <summary>
        /// Converts intensity of type <see cref="System.Double"/> to Gray color.
        /// </summary>
        /// <param name="intensity">Intensity.</param>
        /// <returns>Gray color.</returns>
        public static implicit operator Gray<T>(T intensity)
        {
            return new Gray<T>(intensity);
        }

        /// <summary>
        /// Gets the string color representation.
        /// </summary>
        /// <returns>String color representation.</returns>
        public override string ToString()
        {
            return string.Format("{0}", Intensity);
        }

        /// <summary>
        /// Converts 8-bit gray intensity to the 8-bit Bgr color.
        /// </summary>
        /// <param name="gray">Source color.</param>
        /// <param name="bgr">Destination color.</param>
        public static void Convert(ref Gray<T> gray, ref Bgr<T> bgr)
        {
            bgr.B = gray.Intensity;
            bgr.G = gray.Intensity;
            bgr.R = gray.Intensity;
        }
    }
}
