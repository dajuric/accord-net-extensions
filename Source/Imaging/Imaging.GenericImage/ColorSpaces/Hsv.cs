using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    [ColorInfo(ConversionCodename = "HSV")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv : IColor
    {
        public Hsv(double hue, double saturation, double value)
        {
            this.H = hue;
            this.S = saturation;
            this.V = value;
        }

        public double H;
        public double S;
        public double V;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv8
    {
        public byte H;
        public byte S;
        public byte V;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv16
    {
        public short H;
        public short S;
        public short V;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv32
    {
        public int H;
        public int S;
        public int V;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Hsv32f
    {
        public float H;
        public float S;
        public float V;
    }
}
