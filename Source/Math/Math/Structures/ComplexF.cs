
namespace Accord.Extensions.Math
{
    /// <summary>
    /// Represents complex number.
    /// </summary>
    public struct ComplexF
    {
        /// <summary>
        /// Real part of the complex number.
        /// </summary>
        public float Re;

        /// <summary>
        /// Imaginary part of the complex number.
        /// </summary>
        public float Im;

        /// <summary>
        /// Creates a new instance of <see cref="ComplexF"/> strucure.
        /// </summary>
        /// <param name="re">Real part.</param>
        /// <param name="im">Imaginary part.</param>
        public ComplexF(float re, float im)
        {
            this.Re = re;
            this.Im = im;
        }
    }

}
