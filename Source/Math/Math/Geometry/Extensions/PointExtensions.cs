using AForge;
using System.Collections.Generic;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides point extension methods.
    /// </summary>
    public static class PointExtensions
    {
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

        /// <summary>
        /// Clamps point coordinate according to the specified size (0,0, size.Width-1, size.Height-1).
        /// </summary>
        /// <param name="point">The point to clamp.</param>
        /// <param name="size">The valid region.</param>
        /// <returns>Clamped point.</returns>
        public static Point Clamp(this Point point, Size size)
        {
            return new Point
            {
                X = System.Math.Min(System.Math.Max(0, point.X), size.Width - 1),
                Y = System.Math.Min(System.Math.Max(0, point.Y), size.Height - 1)
            };
        }

        /// <summary>
        /// Clamps point coordinate according to the specified size (rect.X, rect.Y, rect.Right-1, rect.Bottom-1).
        /// </summary>
        /// <param name="point">The point to clamp.</param>
        /// <param name="size">The valid region.</param>
        /// <returns>Clamped point.</returns>
        public static Point Clamp(this Point point, Rectangle rect)
        {
            return new Point
            {
                X = System.Math.Min(System.Math.Max(rect.X, point.X), rect.Right - 1),
                Y = System.Math.Min(System.Math.Max(rect.Y, point.Y), rect.Bottom - 1)
            };
        }

        /// <summary>
        /// Negates point coordinates.
        /// </summary>
        /// <param name="point">The point to negate.</param>
        /// <returns>Point with negated coordinates.</returns>
        public static Point Negate(this Point point)
        {
            return new Point
            {
                X = -point.X,
                Y = -point.Y
            };
        }
    }

    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides point extension methods.
    /// </summary>
    public static class PointFExtensions
    {
        /// <summary>
        /// Transforms point to the lower pyramid level.
        /// </summary>
        /// <param name="p">Point.</param>
        /// <param name="levels">Specifies how many levels to take.</param>
        /// <param name="factor">Specifies the pyramid scale factor.</param>
        /// <returns>Scaled point.</returns>
        public static PointF UpScale(this PointF p, int levels = 1, double factor = 2)
        {
            var upscaleFactor = (float)System.Math.Pow(factor, levels);

            return new PointF
            {
                X = p.X * upscaleFactor,
                Y = p.Y * upscaleFactor
            };
        }

        /// <summary>
        /// Transforms point to the higher pyramid level.
        /// </summary>
        /// <param name="p">Point.</param>
        /// <param name="levels">Specifies how many levels to take.</param>
        /// <param name="factor">Specifies the pyramid scale factor.</param>
        /// <returns>Scaled point.</returns>
        public static PointF DownScale(this PointF p, int levels = 1, double factor = 2)
        {
            var downscaleFactor = (float)(1 / System.Math.Pow(factor, levels));

            return new PointF
            {
                X = p.X * downscaleFactor,
                Y = p.Y * downscaleFactor
            };
        }

        /// <summary>
        /// Gets integer point representation by applying floor operation.
        /// </summary>
        /// <param name="p">Point to truncate.</param>
        /// <returns>Truncated point.</returns>
        public static Point Floor(this PointF p)
        { 
            return new Point
            {
                X = (int)p.X,
                Y = (int)p.Y
            };
        }


        /// <summary>
        /// Rotates the point by specified angle in radians.
        /// </summary>
        /// <param name="p">Point.</param>
        /// <param name="angleRad">Angle in radians.</param>
        /// <returns>Rotated point.</returns>
        public static PointF Rotate(this PointF p, float angleRad) //TODO - critical: replce with Transformation.Transform func
        {
            var a = Rotate(new PointF(p.X, p.Y), angleRad);
            return new AForge.Point(a.X, a.Y);
        }

        /// <summary>
        /// Rotates the point by specified angle in radians.
        /// </summary>
        /// <param name="p">Point.</param>
        /// <param name="angleRad">Angle in radians.</param>
        /// <param name="useScreenCoordinateSystem">Assume screen coordinate system where vertical coordinate has opposite direction.</param>
        /// <returns>Rotated point.</returns>
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
        /// Negates point coordinates.
        /// </summary>
        /// <param name="point">The point to negate.</param>
        /// <returns>Point with negated coordinates.</returns>
        public static PointF Negate(this PointF point)
        {
            return new PointF
            {
                X = -point.X,
                Y = -point.Y
            };
        }
    }

    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides point extension methods.
    /// </summary>
    public static class DoublePointExtensions
    {
        /// <summary>
        /// Transforms point to the lower pyramid level.
        /// </summary>
        /// <param name="p">Point.</param>
        /// <param name="levels">Specifies how many levels to take.</param>
        /// <param name="factor">Specifies the pyramid scale factor.</param>
        /// <returns>Scaled point.</returns>
        public static DoublePoint UpScale(this DoublePoint p, int levels, double factor = 2)
        {
            double upscaleFactor = System.Math.Pow(factor, levels);

            return new DoublePoint
            {
                X = p.X * upscaleFactor,
                Y = p.Y * upscaleFactor
            };
        }

        /// <summary>
        /// Transforms point to the higher pyramid level.
        /// </summary>
        /// <param name="p">Point.</param>
        /// <param name="levels">Specifies how many levels to take.</param>
        /// <param name="factor">Specifies the pyramid scale factor.</param>
        /// <returns>Scaled point.</returns>
        public static DoublePoint DownScale(this DoublePoint p, int levels, double factor = 2)
        {
            double downscaleFactor = 1 / System.Math.Pow(factor, levels);

            return new DoublePoint
            {
                X = p.X * downscaleFactor,
                Y = p.Y * downscaleFactor
            };
        }
    }
}
