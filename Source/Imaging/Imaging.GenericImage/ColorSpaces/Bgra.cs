using Accord.Extensions;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra: IColor, IColor4
    {
        public Bgra(double b, double g, double r, double a)
        {
            this.B = b;
            this.G = g;
            this.R = r;
            this.A = a;
        }

        public double B;
        public double G;
        public double R;
        public double A;
    }

    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra8 
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;
    }

    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra16
    {
        public short B;
        public short G;
        public short R;
        public short A;
    }

    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra32
    {
        public int B;
        public int G;
        public int R;
        public int A;
    }

    [ColorInfo(ConversionCodename = "BGRA")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgra32f
    {
        public float B;
        public float G;
        public float R;
        public float A;
    }


}
