using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Math.Geometry
{
    public static class DoublePointExtensions
    {
        public static DoublePoint UpScale(this DoublePoint p, int levels, double factor = 2)
        {
            double upscaleFactor = System.Math.Pow(factor, levels);

            return new DoublePoint
            {
                X = p.X * upscaleFactor,
                Y = p.Y * upscaleFactor
            };
        }

        public static DoublePoint DownScale(this DoublePoint p, int levels, double factor = 2)
        {
            double downscaleFactor = 1 / System.Math.Pow(factor, levels);

            return new DoublePoint
            {
                X = p.X * downscaleFactor,
                Y = p.Y * downscaleFactor
            };
        }


        public static PointF UpScale(this PointF p, int levels = 1, double factor = 2)
        {
            var upscaleFactor = (float)System.Math.Pow(factor, levels);

            return new PointF
            {
                X = p.X * upscaleFactor,
                Y = p.Y * upscaleFactor
            };
        }

        public static PointF DownScale(this PointF p, int levels = 1, double factor = 2)
        {
            var downscaleFactor = (float)(1 / System.Math.Pow(factor, levels));

            return new PointF
            {
                X = p.X * downscaleFactor,
                Y = p.Y * downscaleFactor
            };
        }


        public static Point Floor(this PointF p)
        {
            return new Point
            {
                X = (int)p.X,
                Y = (int)p.Y
            };
        }

        public static float DistanceTo(this PointF a, PointF b)
        {
            return (float)System.Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public static PointF Normalize(this PointF p)
        {
            var norm = p.EuclideanNorm();
            var pt = new PointF
            {
                X = p.X / norm,
                Y = p.Y / norm
            };

            return pt;
        }


        public static AForge.Point Rotate(this AForge.Point p, float angleRad) //TODO - critical: replce with Transformation.Transform func
        {
            var a = Rotate(new PointF(p.X, p.Y), angleRad);
            return new AForge.Point(a.X, a.Y);
        }

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
