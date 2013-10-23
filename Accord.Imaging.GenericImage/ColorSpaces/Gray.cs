using System.Runtime.InteropServices;
using System;
using Accord.Core;

namespace Accord.Imaging
{
    /// <summary>
    /// Gray color.
    /// </summary>
    [ColorInfo(ConversionCodename = "Gray", IsGenericColorSpace = false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Gray : IColor
    {
        public Gray(double intensity)
        {
            this.Intensity = intensity;
        }

        public double Intensity;

        public static implicit operator double(Gray gray)
        {
            return gray.Intensity;
        }

        public static implicit operator Gray(double intensity)
        {
            return new Gray(intensity);
        }
    }
}
