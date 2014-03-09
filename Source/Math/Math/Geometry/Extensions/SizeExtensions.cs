using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides size extension methods.
    /// </summary>
    public static class SizeExtensions
    {
        /// <summary>
        /// Gets the size area.
        /// </summary>
        /// <param name="size">Size.</param>
        /// <returns>Area.</returns>
        public static int Area(this Int32Size size)
        {
            return size.Width * size.Height;
        }
    }

    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides size extension methods.
    /// </summary>
    public static class SizeFExtensions
    {
        /// <summary>
        /// Gets the size area.
        /// </summary>
        /// <param name="size">Size.</param>
        /// <returns>Area.</returns>
        public static float Area(this Int32Size size)
        {
            return size.Width * size.Height;
        }
    }
}
