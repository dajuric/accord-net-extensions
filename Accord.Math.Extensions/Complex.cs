using System;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using Accord.Core;

namespace Accord.Math
{
    [ColorInfo(ConversionCodename = "Complex")]
    [StructLayout(LayoutKind.Sequential)]
    public struct Complex : IColor
    {
        /// <summary>
        /// Real part of the complex number.
        /// </summary>
        public double Re;

        /// <summary>
        /// Imaginary part of the complex number.
        /// </summary>
        public double Im;

        public Complex(double re, double im)
        {
            this.Re = re;
            this.Im = im;
        }
    }

    public struct ComplexF
    {
        /// <summary>
        /// Real part of the complex number.
        /// </summary>
        public float Re;

        /// <summary>
        /// Imaginary part of the complex number.
        /// </summary>
        public float Im;
    }

}
