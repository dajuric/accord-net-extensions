using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents Bgr color type of type <see cref="System.Double"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr: IColor, IColor3
    {
        /// <summary>
        /// Creates new Bgr color.
        /// </summary>
        /// <param name="b">Blue</param>
        /// <param name="g">Green</param>
        /// <param name="r">Red</param>
        public Bgr(double b, double g, double r)
        {
            this.B = b;
            this.G = g;
            this.R = r;
        }

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public double B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public double G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public double R;

        /// <summary>
        /// Cast 8-bit channels to channels of type <see cref="System.Double"/>.
        /// </summary>
        /// <param name="color">8-bit color</param>
        /// <returns></returns>
        public static implicit operator Bgr(Bgr8 color)
        {
            return new Bgr(color.B, color.G, color.R);
        }

        /// <summary>
        /// Cast 32-bit channels to channels of type <see cref="System.Double"/>.
        /// </summary>
        /// <param name="color">8-bit color</param>
        /// <returns></returns>
        public static implicit operator Bgr(Bgr32 color)
        {
            return new Bgr(color.B, color.G, color.R);
        }

        /// <summary>
        /// Gets the string color representation.
        /// </summary>
        /// <returns>String color representation.</returns>
        public override string ToString()
        {
            return string.Format("B: {0}, G: {1}, R: {2}", B, G, R);
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
    }

    /// <summary>
    /// Represents Bgr color type of type <see cref="System.Byte"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr8 
    {
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public byte B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public byte G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public byte R;
        
        /// <summary>
        /// Gets the 8-bit red color.
        /// </summary>
        public static readonly Bgr8 Red =   new Bgr8 { B = 0,             G = 0,             R = byte.MaxValue };
        /// <summary>
        /// Gets the 8-bit blue color.
        /// </summary>
        public static readonly Bgr8 Blue = new Bgr8 { B = byte.MaxValue,  G = 0,             R = 0             };
        /// <summary>
        /// Gets the 8-bit green color.
        /// </summary>
        public static readonly Bgr8 Green = new Bgr8 { B = 0,             G = byte.MaxValue, R = 0             };
        /// <summary>
        /// Gets the 8-bit black color.
        /// </summary>
        public static readonly Bgr8 Black = new Bgr8 { B = 0, G = 0, R = 0 };
        /// <summary>
        /// Gets the 8-bit white color.
        /// </summary>
        public static readonly Bgr8 White = new Bgr8 { B = byte.MaxValue, G = byte.MaxValue, R = byte.MaxValue };

        /// <summary>
        /// Converts 8-bit Bgr to 8-bit Hsv color. Value range for 8-bit HSv color is  [0..180].
        /// </summary>
        /// <param name="bgr">Source color.</param>
        /// <param name="hsv">Destination color.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void ConvertBgrToHsv(Bgr8* bgr, Hsv8* hsv)
        {
            byte rgbMin, rgbMax;

            rgbMin = bgr->R < bgr->G ? (bgr->R < bgr->B ? bgr->R : bgr->B) : (bgr->G < bgr->B ? bgr->G : bgr->B);
            rgbMax = bgr->R > bgr->G ? (bgr->R > bgr->B ? bgr->R : bgr->B) : (bgr->G > bgr->B ? bgr->G : bgr->B);

            hsv->V = rgbMax;
            if (hsv->V == 0)
            {
                hsv->H = 0;
                hsv->S = 0;
                return;
            }

            hsv->S = (byte)(255 * (rgbMax - rgbMin) / rgbMax);
            if (hsv->S == 0)
            {
                hsv->H = 0;
                return;
            }

            int hue = 0;
            if (rgbMax == bgr->R)
            {
                hue = 0 + 60 * (bgr->G - bgr->B) / (rgbMax - rgbMin);
                if (hue < 0)
                    hue += 360;
            }
            else if (rgbMax == bgr->G)
            {
                hue = 120 + 60 * (bgr->B - bgr->R) / (rgbMax - rgbMin);
            }
            else //rgbMax == bgr->B
            {
                hue = 240 + 60 * (bgr->R - bgr->G) / (rgbMax - rgbMin);
            }

            hsv->H = (byte)(hue / 2); //scale [0-360] -> [0-180] (only needed for byte!)

            Debug.Assert(hue >= 0 && hue <= 360);
        }

        /// <summary>
        /// Converts 8-bit Bgr to 8-bit Hsv color. Value range for 8-bit HSv color is  [0..180].
        /// </summary>
        public unsafe Hsv8 ToHsv()
        {
            Bgr8 bgr = this;  Hsv8 hsv;
            ConvertBgrToHsv(&bgr, &hsv);
            return hsv;
        }

        /// <summary>
        /// Converts 8-bit Bgr to 8-bit gray color. 
        /// </summary>
        /// <param name="bgr">Source color.</param>
        /// <param name="gray">Destination color.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void ConvertBgrToGray(Bgr8* bgr, byte* gray)
        {
            int val = ((bgr->R << 1) +           //2 * red
                       (bgr->G << 2) + bgr->G +  //5 * green
                        bgr->B                   //1 * blue

                      ) >> 3;                   //divide by 8

            *gray = (byte)val;
        }

        /// <summary>
        /// Converts 8-bit Bgr to 8-bit gray color. 
        /// </summary>
        public unsafe byte ToGray()
        {
            Bgr8 bgr = this; byte gray;
            ConvertBgrToGray(&bgr, &gray);
            return gray;
        }
    }

    /// <summary>
    /// Represents Bgr color type of type <see cref="System.Int16"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr16
    {
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public short B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public short G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public short R;
    }

    /// <summary>
    /// Represents Bgr color type of type <see cref="System.Int32"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr32
    {
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public int B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public int G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public int R;
    }

    /// <summary>
    /// Represents Bgr color type of type <see cref="System.Single"/>.
    /// </summary>
    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr32f
    {
        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public float B;
        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public float G;
        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public float R;
    }
}
