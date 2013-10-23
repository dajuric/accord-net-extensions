using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Math.Geometry
{
    public static class Angle
    {
        public static double ToDegrees(double angleRad)
        {
            var deg = angleRad / System.Math.PI * 180;
            deg = NormalizeDegrees(deg);
            

            return deg;
        }

        public static double NormalizeDegrees(double angleDeg)
        {
            angleDeg %= 360;

            if (angleDeg < 0)
                angleDeg += 360;

            return angleDeg;
        }

        public static double ToRadians(double angleDeg)
        {
            return angleDeg * System.Math.PI / 180;
        }
    }
}
