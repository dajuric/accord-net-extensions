using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Math.Geometry
{
    public static class SizeExtensions
    {
        public static float Area(this SizeF size)
        {
            return size.Width * size.Height;
        }

        public static int Area(this Size size)
        {
            return size.Width * size.Height;
        }
    }
}
