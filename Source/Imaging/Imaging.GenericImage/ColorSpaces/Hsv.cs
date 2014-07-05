using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void ConvertHsvToBgr(Hsv8* hsv, Bgr8* bgr)
        {
            if (hsv->S == 0)
            {
                bgr->R = hsv->V;
                bgr->G = hsv->V;
                bgr->B = hsv->V;
                return;
            }

            int hue = hsv->H * 2; //move to [0-360 range] (only needed for byte!)

            int hQuadrant = hue / 60; // Hue quadrant 0 - 5 (60deg)
            int hOffset = hue % 60; // Hue position in quadrant
            int vs = hsv->V * hsv->S;

            byte p = (byte)(hsv->V - (vs / 255));
            byte q = (byte)(hsv->V - (vs / 255 * hOffset) / 60);
            byte t = (byte)(hsv->V - (vs / 255 * (60 - hOffset)) / 60);

            switch (hQuadrant)
            {
                case 0:
                    bgr->R = hsv->V; bgr->G = t; bgr->B = p;
                    break;
                case 1:
                    bgr->R = q; bgr->G = hsv->V; bgr->B = p;
                    break;
                case 2:
                    bgr->R = p; bgr->G = hsv->V; bgr->B = t;
                    break;
                case 3:
                    bgr->R = p; bgr->G = q; bgr->B = hsv->V;
                    break;
                case 4:
                    bgr->R = t; bgr->G = p; bgr->B = hsv->V;
                    break;
                default:
                    bgr->R = hsv->V; bgr->G = p; bgr->B = q;
                    break;
            }
        }

        public unsafe Bgr8 ToBgr()
        {
            Hsv8 hsv = this;  Bgr8 bgr;
            ConvertHsvToBgr(&hsv, &bgr);
            return bgr;
        }
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
