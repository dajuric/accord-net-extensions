using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents gray color of type <see cref="System.Double"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "Gray", IsGenericColorSpace = false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray : IColor
    {
        /// <summary>
        /// Creates new gray color.
        /// </summary>
        /// <param name="intensity">Intensity.</param>
        public Gray(double intensity)
        {
            this.Intensity = intensity;
        }

        /// <summary>
        /// Gets or sets the intensity.
        /// </summary>
        public double Intensity;

        /// <summary>
        /// Converts gray structure to <see cref="System.Double"/> value.
        /// </summary>
        /// <param name="gray">Gray color.</param>
        /// <returns>Intensity.</returns>
        public static implicit operator double(Gray gray)
        {
            return gray.Intensity;
        }

        /// <summary>
        /// Converts intensity of type <see cref="System.Double"/> to Gray color.
        /// </summary>
        /// <param name="intensity">Intensity.</param>
        /// <returns>Gray color.</returns>
        public static implicit operator Gray(double intensity)
        {
            return new Gray(intensity);
        }
    }
}
