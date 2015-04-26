using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents Bgra color type of type <typeparam name="T">depth</typeparam>.
    /// </summary>
    //[ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra<T> : IColor4<T>
        where T: struct
    {
        /// <summary>
        /// Creates new Bgra color.
        /// </summary>
        /// <param name="b">Blue</param>
        /// <param name="g">Green</param>
        /// <param name="r">Red</param>
        /// <param name="a">Alpha (transparency).</param>
        public Bgra(T b, T g, T r, T a)
        {
            this.B = b;
            this.G = g;
            this.R = r;
            this.A = a;
        }

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public T B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public T G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public T R;
        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public T A;

        /// <summary>
        /// Gets the string color representation.
        /// </summary>
        /// <returns>String color representation.</returns>
        public override string ToString()
        {
            return string.Format("B: {0}, G: {1}, R: {2}, A: {3}", B, G, R, A);
        }

        /// <summary>
        /// Gets the index of the blue component.
        /// </summary>
        public const int IdxB = 0;
        /// <summary>
        /// Gets the index of the green component.
        /// </summary>
        public const int IdxG = 1;
        /// <summary>
        /// Gets the index of the red component.
        /// </summary>
        public const int IdxR = 2;
        /// <summary>
        /// Gets the index of the alpha component.
        /// </summary>
        public const int IdxA = 3;
    }
}
