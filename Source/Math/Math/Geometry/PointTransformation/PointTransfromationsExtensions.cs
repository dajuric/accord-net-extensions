#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
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
using Accord.Math;
using PointF = AForge.Point;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides extension methods for point collection transfromation.
    /// </summary>
    public static class PointTransfromationsExtensions
    {
        /// <summary>
        /// Transfroms 3D point by using transformation matrix (3x3)
        /// </summary>
        /// <param name="point">Point to transform</param>
        /// <param name="transformationMat">Transformation matrix</param>
        /// <returns>Transformed point</returns>
        public static Point3 Transform(this Point3 point, float[,] transformationMat)
        {
            return new Point3 
            {
                X = transformationMat[0, 0] * point.X + transformationMat[0, 1] * point.Y + transformationMat[0, 2] * point.Z,
                Y = transformationMat[1, 0] * point.X + transformationMat[1, 1] * point.Y + transformationMat[1, 2] * point.Z,
                Z = transformationMat[2, 0] * point.X + transformationMat[2, 1] * point.Y + transformationMat[2, 2] * point.Z,
            };
        }

        /// <summary>
        /// Projects point regarding camera.
        /// </summary>
        /// <param name="point">Point to project.</param>
        /// <param name="camera">Camera position (default is (0,0,0))</param>
        /// <returns>Projected point</returns>
        public static PointF Project(this Point3 point, Point3 camera = default(Point3))
        {
            var F = point.Z - camera.Z;

            return new PointF
            {
                X = ((point.X - camera.X) * (F / point.Z)) + camera.X,
                Y = ((point.Y - camera.Y) * (F / point.Z)) + camera.Y
            };
        }

        /// <summary>
        /// Transforms 3D point to 2D point using transformation matrix and projection regarding camera position.
        /// </summary>
        /// <param name="point">Point to transform.</param>
        /// <param name="transformationMat">Transformation matrix (3x3).</param>
        /// <param name="camera">Camera position. Default is (0,0,0).</param>
        /// <returns>Transformed point.</returns>
        public static PointF Transform(this PointF point, float[,] transformationMat, Point3 camera = default(Point3))
        {
            var point3 = new Point3(point.X, point.Y, 1);
            return point3.Transform(transformationMat).Project(camera);
        }

        #region IEnumerable

        /// <summary>
        /// Transfroms each points in the specified collection according to transformation matrix (3x3).
        /// </summary>
        /// <param name="points">Contour points.</param>
        /// <param name="transformationMat">Transfromation matrix.</param>
        /// <returns>Transfromed points.</returns>
        public static IEnumerable<PointF> Transform(this IEnumerable<PointF> points, float[,] transformationMat)
        {
            foreach (var p in points)
            {
                yield return p.Transform(transformationMat);
            }
        }

        /// <summary>
        /// Normalizes point cloud to range [-1..1]. Ratios will be preserved.
        /// <para>A bounding rectangle will be determined and then </para>
        /// <para>  1) points will be translated for (rect center X, rect center Y)</para>
        /// <para>  2) and then rescaled for (1/scale, 1/scale) where scale is max(width, height).</para>
        /// </summary>
        /// <param name="points">Points to normalize.</param>
        /// <returns>Normalized points.</returns>
        public static IEnumerable<PointF> Normalize(this IEnumerable<PointF> points)
        {
            var rect = points.BoundingRect();
            var center = rect.Center();
            var scaleFactor = System.Math.Max(rect.Width, rect.Height);

            var transform = Transforms2D.Combine
                            (
                                Transforms2D.Translation(-center.X, -center.Y),
                                Transforms2D.Scale(1 / scaleFactor, 1 / scaleFactor)
                            );
                
            points = points.Transform(transform);

            return points;
        }

        #endregion
    }
}
