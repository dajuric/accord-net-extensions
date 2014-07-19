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
        /// <summary>
        /// Creates new 2-channel generic color.
        /// </summary>
        /// <param name="val0">First channel.</param>
        /// <param name="val1">Second channel.</param>
        public Color2(double val0, double val1)
        {
            this.Val0 = val0;
            this.Val1 = val1;
        }

        /// <summary>
        /// Gets or sets first channel value.
        /// </summary>
        public double Val0;
        /// <summary>
        /// Gets or sets second channel value.
        /// </summary>
        public double Val1;
    }

    /// <summary>
    /// Generic color space (3 channels).
    /// </summary>
    [ColorInfo(ConversionCodename = "Generic", IsGenericColorSpace = true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color3: IColor, IColor3
    {
        /// <summary>
        /// Creates new 3-channel generic color.
        /// </summary>
        /// <param name="val0">First channel.</param>
        /// <param name="val1">Second channel.</param>
        /// <param name="val2">Third channel.</param>
        public Color3(double val0, double val1, double val2)
        {
            this.Val0 = val0;
            this.Val1 = val1;
            this.Val2 = val2;
        }

        /// <summary>
        /// Gets or sets first channel value.
        /// </summary>
        public double Val0;
        /// <summary>
        /// Gets or sets second channel value.
        /// </summary>
        public double Val1;
        /// <summary>
        /// Gets or sets third channel value.
        /// </summary>
        public double Val2;
    }

    /// <summary>
    /// Generic color space (4 channels).
    /// </summary>
    [ColorInfo(ConversionCodename = "Generic", IsGenericColorSpace = true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color4 : IColor, IColor4
    {
        /// <summary>
        /// Creates new 4-channel generic color.
        /// </summary>
        /// <param name="val0">First channel.</param>
        /// <param name="val1">Second channel.</param>
        /// <param name="val2">Third channel.</param>
        /// <param name="val3">Fourth channel.</param>
        public Color4(double val0, double val1, double val2, double val3)
        {
            this.Val0 = val0;
            this.Val1 = val1;
            this.Val2 = val2;
            this.Val3 = val3;
        }

        /// <summary>
        /// Gets or sets first channel value.
        /// </summary>
        public double Val0;
        /// <summary>
        /// Gets or sets second channel value.
        /// </summary>
        public double Val1;
        /// <summary>
        /// Gets or sets third channel value.
        /// </summary>
        public double Val2;
        /// <summary>
        /// Gets or sets fourth channel value.
        /// </summary>
        public double Val3;
    }

}
