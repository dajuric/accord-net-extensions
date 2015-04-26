#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System.Collections.Generic;
using AForge;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides point extension methods.
    /// </summary>
    public static class Point32iExtensions
    {
        /// <summary>
        /// Selects points which satisfy minimal specified distance.
        /// </summary>
        /// <param name="candidates">Points sorted by importance. Points are tested by sequentially.</param>
        /// <param name="minimalDistance">Minimal enforced distance.</param>
        /// <returns>Filtered points which are spread by minimal <paramref name="minimalDistance"/>.</returns>
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
        /// Clamps point coordinate according to the specified size (0,0, size.Width, size.Height).
        /// </summary>
        /// <param name="point">The point to clamp.</param>
        /// <param name="size">The valid region.</param>
        /// <returns>Clamped point.</returns>
        public static Point Clamp(this Point point, Size size)
        {
            return new Point
            {
                X = System.Math.Min(System.Math.Max(0, point.X), size.Width),
                Y = System.Math.Min(System.Math.Max(0, point.Y), size.Height)
            };
        }

        /// <summary>
        /// Clamps point coordinate according to the specified size (rect.X, rect.Y, rect.Right, rect.Bottom).
        /// </summary>
        /// <param name="point">The point to clamp.</param>
        /// <param name="rect">The valid region.</param>
        /// <returns>Clamped point.</returns>
        public static Point Clamp(this Point point, Rectangle rect)
        {
            return new Point
            {
                X = System.Math.Min(System.Math.Max(rect.X, point.X), rect.Right),
                Y = System.Math.Min(System.Math.Max(rect.Y, point.Y), rect.Bottom)
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
    public static class Point32fExtensions
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
        public static PointF Rotate(this PointF p, float angleRad)
        {
            return p.Transform(Transforms2D.Rotation(angleRad));
        }

        /// <summary>
        /// Clamps point coordinate according to the specified size (0,0, size.Width, size.Height).
        /// </summary>
        /// <param name="point">The point to clamp.</param>
        /// <param name="size">The valid region.</param>
        /// <returns>Clamped point.</returns>
        public static PointF Clamp(this PointF point, SizeF size)
        {
            return new PointF
            {
                X = System.Math.Min(System.Math.Max(0, point.X), size.Width),
                Y = System.Math.Min(System.Math.Max(0, point.Y), size.Height)
            };
        }

        /// <summary>
        /// Clamps point coordinate according to the specified size (rect.X, rect.Y, rect.Right, rect.Bottom).
        /// </summary>
        /// <param name="point">The point to clamp.</param>
        /// <param name="rect">The valid region.</param>
        /// <returns>Clamped point.</returns>
        public static PointF Clamp(this PointF point, RectangleF rect)
        {
            return new PointF
            {
                X = System.Math.Min(System.Math.Max(rect.X, point.X), rect.Right),
                Y = System.Math.Min(System.Math.Max(rect.Y, point.Y), rect.Bottom)
            };
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

        /// <summary>
        /// Translates the point by the specified offset.
        /// </summary>
        /// <param name="point">The point to offset.</param>
        /// <param name="offset">Offset to be added.</param>
        /// <returns>Translated point.</returns>
        public static PointF Offset(this PointF point, PointF offset)
        {
            return new PointF
            {
                X = point.X + offset.X,
                Y = point.Y + offset.Y
            };
        }

        /// <summary>
        /// Subtracts the point by the specified offset.
        /// </summary>
        /// <param name="point">The point to subtract.</param>
        /// <param name="offset">Subtract factor.</param>
        /// <returns>Translated point.</returns>
        public static PointF Subtract(this PointF point, PointF offset)
        {
            return new PointF
            {
                X = point.X - offset.X,
                Y = point.Y - offset.Y
            };
        }
    }

    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides point extension methods.
    /// </summary>
    public static class Point64fExtensions
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
