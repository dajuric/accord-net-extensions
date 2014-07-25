using System.Runtime.InteropServices;

namespace Accord.Extensions.Math
{
    /// <summary>
    /// Represents complex number.
    /// </summary>
    [ColorInfo(ConversionCodename = "Complex")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Complex : IColor
    {
        /// <summary>
        /// Real part of the complex number.
        /// </summary>
        public double Re;

        /// <summary>
        /// Imaginary part of the complex number.
        /// </summary>
        public double Im;

        /// <summary>
        /// Creates a new instance of <see cref="Complex"/> structure.
        /// </summary>
        /// <param name="re">Real part.</param>
        /// <param name="im">Imaginary part.</param>
        public Complex(double re, double im)
        {
            this.Re = re;
            this.Im = im;
        }
    }
}
