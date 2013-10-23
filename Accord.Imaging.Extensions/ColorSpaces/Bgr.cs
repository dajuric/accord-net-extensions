using Accord.Core;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Accord.Imaging
{
    [ColorInfo(ConversionCodename = "BGR")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr: IColor, IColor3
    {
        public Bgr(Color color)
        {
            this.B = color.B;
            this.G = color.G;
            this.R = color.R;
        }

        public Bgr(double b, double g, double r)
        {
            this.B = b;
            this.G = g;
            this.R = r;
        }

        public double B;
        public double G;
        public double R;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr8 
    {
        public byte B;
        public byte G;
        public byte R;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr16
    {
        public short B;
        public short G;
        public short R;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr32
    {
        public int B;
        public int G;
        public int R;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Bgr32f
    {
        public float B;
        public float G;
        public float R;
    }
}
