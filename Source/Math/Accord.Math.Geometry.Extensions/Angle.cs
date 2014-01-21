using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Math.Geometry
{
    public static class Angle
    {
        public static double ToDegrees(double angleRad, bool normalizeDegrees = true)
        {
            var deg = angleRad / System.Math.PI * 180;

            if(normalizeDegrees)
                deg = NormalizeDegrees(deg);
            
            return deg;
        }

        public static double NormalizeDegrees(double angleDeg, float angleMaxVal = 360)
        {
            angleDeg %= angleMaxVal;

            if (angleDeg < 0)
                angleDeg += angleMaxVal;

            return angleDeg;
        }

        public static double ToRadians(double angleDeg)
        {
            return angleDeg * System.Math.PI / 180;
        }

        public static double DistanceDeg(double angleDegA, double angleDegB)
        {
            var dist = angleDegB - angleDegA;
            dist = (dist + 180) % 360 - 180;
            return dist;
        }
    }
}
