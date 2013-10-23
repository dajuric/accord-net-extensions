using System;
using System.Runtime.InteropServices;
using Accord.Core;

namespace Accord.Imaging
{


    /// <summary>
    /// Generic color space (3 channels).
    /// </summary>
    [ColorInfo(ConversionCodename = "Generic", IsGenericColorSpace = true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color3: IColor, IColor3
    {
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
        public double Val0;
        public double Val1;
        public double Val2;
        public double Val3;
    }

}
