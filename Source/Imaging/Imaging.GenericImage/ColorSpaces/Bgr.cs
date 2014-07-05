using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr: IColor, IColor3
    {
        public Bgr(double b, double g, double r)
        {
            this.B = b;
            this.G = g;
            this.R = r;
        }

        public double B;
        public double G;
        public double R;

        public static implicit operator Bgr(Bgr8 color)
        {
            return new Bgr(color.B, color.G, color.R);
        }

        public static implicit operator Bgr(Bgr32 color)
        {
            return new Bgr(color.B, color.G, color.R);
        }

        public const int IdxB = 0;
        public const int IdxG = 1;
        public const int IdxR = 2;
    }

    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr8 
    {
        public byte B;
        public byte G;
        public byte R;

        public static readonly Bgr8 Red =   new Bgr8 { B = 0,             G = 0,             R = byte.MaxValue };
        public static readonly Bgr8 Blue = new Bgr8 { B = byte.MaxValue, G = 0, R = 0 };
        public static readonly Bgr8 Green = new Bgr8 { B = 0,             G = byte.MaxValue, R = 0             };

        public static readonly Bgr8 Black = new Bgr8 { B = 0, G = 0, R = 0 };
        public static readonly Bgr8 White = new Bgr8 { B = byte.MaxValue, G = byte.MaxValue, R = byte.MaxValue };

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

        public unsafe Hsv8 ToHsv()
        {
            Bgr8 bgr = this;  Hsv8 hsv;
            ConvertBgrToHsv(&bgr, &hsv);
            return hsv;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void ConvertBgrToGray(Bgr8* bgr, byte* gray)
        {
            int val = ((bgr->R << 1) +           //2 * red
                       (bgr->G << 2) + bgr->G +  //5 * green
                        bgr->B                   //1 * blue

                      ) >> 3;                   //divide by 8

            *gray = (byte)val;
        }

        public unsafe byte ToGray()
        {
            Bgr8 bgr = this; byte gray;
            ConvertBgrToGray(&bgr, &gray);
            return gray;
        }
    }

    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr16
    {
        public short B;
        public short G;
        public short R;
    }

    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr32
    {
        public int B;
        public int G;
        public int R;
    }

    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr32f
    {
        public float B;
        public float G;
        public float R;
    }


}
