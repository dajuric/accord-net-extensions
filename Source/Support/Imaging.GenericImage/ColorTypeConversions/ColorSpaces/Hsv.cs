using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents Hsv color type of type <typeparam name="T">color depth</typeparam>.
    /// </summary>
    //[ColorInfo(ConversionCodename = "HSV")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv<T>: IColor3<T>
        where T: struct
    {
        /// <summary>
        /// Creates new Hsv color.
        /// </summary>
        /// <param name="hue">Hue</param>
        /// <param name="saturation">Saturation</param>
        /// <param name="value">Value.</param>
        public Hsv(T hue, T saturation, T value)
        {
            this.H = hue;
            this.S = saturation;
            this.V = value;
        }

        /// <summary>
        /// Gets or sets hue.
        /// </summary>
        public T H;
        /// <summary>
        /// Gets or sets saturation.
        /// </summary>
        public T S;
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public T V;

        /// <summary>
        /// Gets the string color representation.
        /// </summary>
        /// <returns>String color representation.</returns>
        public override string ToString()
        {
            return string.Format("H: {0}, S: {1}, V: {2}", H, S, V);
        }

        /// <summary>
        /// Gets the index of the hue component.
        /// </summary>
        public const int IdxH = 0;
        /// <summary>
        /// Gets the index of the saturation component.
        /// </summary>
        public const int IdxS = 1;
        /// <summary>
        /// Gets the index of the value component.
        /// </summary>
        public const int IdxV = 2;

        /// <summary>
        /// Converts 8-bit Hsv color to the 8-bit Bgr color.
        /// </summary>
        /// <param name="hsv">Source color.</param>
        /// <param name="bgr">Destination color.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Convert(ref Hsv<byte> hsv, ref Bgr<byte> bgr)
        {
            if (hsv.S == 0)
            {
                bgr.R = hsv.V;
                bgr.G = hsv.V;
                bgr.B = hsv.V;
                return;
            }

            int hue = hsv.H * 2; //move to [0-360 range] (only needed for byte!)

            int hQuadrant = hue / 60; // Hue quadrant 0 - 5 (60deg)
            int hOffset = hue % 60; // Hue position in quadrant
            int vs = hsv.V * hsv.S;

            byte p = (byte)(hsv.V - (vs / 255));
            byte q = (byte)(hsv.V - (vs / 255 * hOffset) / 60);
            byte t = (byte)(hsv.V - (vs / 255 * (60 - hOffset)) / 60);

            switch (hQuadrant)
            {
                case 0:
                    bgr.R = hsv.V; bgr.G = t; bgr.B = p;
                    break;
                case 1:
                    bgr.R = q; bgr.G = hsv.V; bgr.B = p;
                    break;
                case 2:
                    bgr.R = p; bgr.G = hsv.V; bgr.B = t;
                    break;
                case 3:
                    bgr.R = p; bgr.G = q; bgr.B = hsv.V;
                    break;
                case 4:
                    bgr.R = t; bgr.G = p; bgr.B = hsv.V;
                    break;
                default:
                    bgr.R = hsv.V; bgr.G = p; bgr.B = q;
                    break;
            }
        }
    }

    /// <summary>
    /// Represents 8-bit Hsv color type.
    /// <para>Its usage should be restricted only for unsafe pixel manipulation.</para>
    /// </summary>
    public struct Hsv8
    {
        /// <summary>
        /// Gets or sets hue.
        /// </summary>
        public byte H;
        /// <summary>
        /// Gets or sets saturation.
        /// </summary>
        public byte S;
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public byte V;
    }
}
