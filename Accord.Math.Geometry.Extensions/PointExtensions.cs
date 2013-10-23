using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Math.Geometry
{
    public static class PointExtensions
    {
        public static PointF Rotate(this PointF p, float angleRad, bool useScreenCoordinateSystem = true)
        {
            if (useScreenCoordinateSystem)
                angleRad *= -1;

            double cos = System.Math.Cos(angleRad);
            double sin = System.Math.Sin(angleRad);

            double rotatedX = p.X * cos - p.Y * sin;
            double rotatedY = p.X * sin + p.Y * cos;

            return new PointF((float)rotatedX, (float)rotatedY);
        }

        /// <summary>
        /// Selects points which satisfy minimal specified distance.
        /// </summary>
        /// <param name="candidates">Points sorted by importance. Points are tested by sequentially.</param>
        /// <param name="minimalDistance">Minimal enforced distance.</param>
        /// <returns>Filtered points which are spread by minimal <see cref="minimalDistance"/></returns>
        public static List<Point> EnforceMinimalDistance(this IEnumerable<Point> candidates, float minimalDistance)
        { 
            var minDistSqr = minimalDistance * minimalDistance;
            List<Point> filteredPoints = new List<Point>();

            foreach (var candidate in candidates)
            {
                bool isEnoughFar = true;
                foreach (var filteredPt in filteredPoints)
                {
                    int dx = candidate.X - filteredPt.X;
                    int dy = candidate.Y - filteredPt.Y;
                    int featureDistanceSqr = dx * dx + dy * dy;

                    if (featureDistanceSqr < minDistSqr)
                    {
                        isEnoughFar = false;
                        break;
                    }
                }

                if (isEnoughFar)
                    filteredPoints.Add(candidate);
            }

            return filteredPoints;
        }
    }
}
