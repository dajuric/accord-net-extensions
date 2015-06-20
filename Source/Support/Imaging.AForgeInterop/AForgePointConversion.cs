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

using DotImaging.Primitives2D;
using System;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extensions for conversion between Imaging.NET and AForge Point structures.
    /// </summary>
    public static class AForgePointConversion
    {
        /// <summary>
        /// Converts Imaging.NET point to AForge.NET representation.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <returns>Converted point.</returns>
        public static AForge.IntPoint ToIntPoint(this Point point)
        {
            return new AForge.IntPoint(point.X, point.Y);
        }

        /// <summary>
        /// Converts Imaging.NET point to AForge.NET representation.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <returns>Converted point.</returns>
        public static AForge.Point ToPoint(this PointF point)
        {
            return new AForge.Point(point.X, point.Y);
        }

        /// <summary>
        /// Converts AForge.NET point to Imaging.NET representation.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <returns>Converted point.</returns>
        public static Point ToPoint(this AForge.IntPoint point)
        {
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// Converts AForge.NET point to Imaging.NET representation.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <returns>Converted point.</returns>
        public static PointF ToPointF(this AForge.Point point)
        {
            return new PointF(point.X, point.Y);
        }


        /// <summary>
        /// Converts AForge.NET points to Imaging.NET representation.
        /// </summary>
        /// <param name="point">Points.</param>
        /// <returns>Converted points.</returns>
        public static Point[] ToPoints(this AForge.IntPoint[] points)
        {
            var result = new Point[points.Length];
            Array.Copy(points, result, points.Length);

            return result;
        }

        /// <summary>
        /// Converts AForge.NET points to Imaging.NET representation.
        /// </summary>
        /// <param name="point">Points.</param>
        /// <returns>Converted points.</returns>
        public static Point[][] ToPoints(this AForge.IntPoint[][] points)
        {
            var result = new Point[points.Length][];

            for (int i = 0; i < points.Length; i++)
            {
                result[i] = new Point[points[i].Length];
                Array.Copy(points[i], result[i], points[i].Length);
            }

            return result;
        }


        /// <summary>
        /// Converts Imaging.NET points to AForge.NET representation.
        /// </summary>
        /// <param name="point">Points.</param>
        /// <returns>Converted points.</returns>
        public static AForge.IntPoint[] ToPoints(this Point[] points)
        {
            var result = new AForge.IntPoint[points.Length];
            Array.Copy(points, result, points.Length);

            return result;
        }
    }
}
