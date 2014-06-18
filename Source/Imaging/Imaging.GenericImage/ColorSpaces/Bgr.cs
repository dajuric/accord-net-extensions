using Accord.Extensions;
using System.Drawing;
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
