using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Generic color space (2 channels).
    /// </summary>
    [ColorInfo(ConversionCodename = "Generic", IsGenericColorSpace = true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color2 : IColor, IColor2
    {
        public Color2(double val0, double val1)
        {
            this.Val0 = val0;
            this.Val1 = val1;
        }

        public double Val0;
        public double Val1;
    }

    /// <summary>
    /// Generic color space (3 channels).
    /// </summary>
    [ColorInfo(ConversionCodename = "Generic", IsGenericColorSpace = true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color3: IColor, IColor3
    {
        public Color3(double val0, double val1, double val2)
        {
            this.Val0 = val0;
            this.Val1 = val1;
            this.Val2 = val2;
        }

        public double Val0;
        public double Val1;
        public double Val2;
    }

    /// <summary>
    /// Generic color space (4 channels).
    /// </summary>
    [ColorInfo(ConversionCodename = "Generic", IsGenericColorSpace = true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color4 : IColor, IColor4
    {
        public Color4(double val0, double val1, double val2, double val3)
        {
            this.Val0 = val0;
            this.Val1 = val1;
            this.Val2 = val2;
            this.Val3 = val3;
        }

        public double Val0;
        public double Val1;
        public double Val2;
        public double Val3;
    }

}
