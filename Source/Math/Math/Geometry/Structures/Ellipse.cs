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

using System;
using System.Runtime.InteropServices;
using PointF = AForge.Point;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Ellipse structure.
    /// See <see cref="Accord.Extensions.Math.Geometry.Box2D"/> also.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Ellipse
    {
        /// <summary>
        /// Area center.
        /// </summary>
        public PointF Center;
        /// <summary>
        /// Area size.
        /// </summary>
        public SizeF Size;
        /// <summary>
        /// Angle in degrees.
        /// </summary>
        public float Angle;

        /// <summary>
        /// Converts Box2D to Ellipse representation.
        /// </summary>
        /// <param name="box">Box to convert.</param>
        /// <returns>Ellipse.</returns>
        public static explicit operator Ellipse(Box2D box)
        {
            return new Ellipse { Center = box.Center, Size = box.Size, Angle = box.Angle };
        }

        /// <summary>
        /// Converts Ellipse to Box2D representation.
        /// </summary>
        /// <param name="ellipse">Ellipse to convert.</param>
        /// <returns>Box2D.</returns>
        public static explicit operator Box2D(Ellipse ellipse)
        {
            return new Box2D { Center = ellipse.Center, Size = ellipse.Size, Angle = ellipse.Angle };
        }

        /// <summary>
        /// Fits the covariance matrix (or 2nd moment matrix) to the ellipse by calculating eigen-vectors and values.
        /// </summary>
        /// <param name="covMatrix">Covariance matrix (or 2nd moment matrix).</param>
        /// <param name="center">Center of the ellipse.</param>
        /// <returns>Ellipse.</returns>
        public static Ellipse Fit(double[,] covMatrix, PointF center = default(PointF))
        {
            if (covMatrix.ColumnCount() != 2 || covMatrix.RowCount() != 2)
                throw new ArgumentException("Covariance matrix must have the same dimensions, and the dimension length must be 2!");

            return Fit(covMatrix[0, 0], covMatrix[0, 1], covMatrix[1, 1], center);
        }

        /// <summary>
        /// Fits the covariance matrix (or 2nd moment matrix) to the ellipse by calculating eigen-vectors and values.
        /// </summary>
        /// <param name="a">[0, 0] value of the covariance matrix.</param>
        /// <param name="b">[0, 1] or [1, 0] value of the covariance matrix.</param>
        /// <param name="c">[1, 1] value of the covariance matrix.</param>
        /// <param name="center">Center of the ellipse.</param>
        /// <param name="success">Returns true if both calculated eigen-values are positive.</param>
        /// <returns>Ellipse.</returns>
        public static Ellipse Fit(double a, double b, double c, PointF center, out bool success)
        {  
            var acDiff = a - c;
            var acSum = a + c;

            //A * X = lambda * X => solve quadratic equation => lambda1/2 = [a + c +/- sqrt((a-c)^2 + 4b^2)]  / 2
            var sqrtDiscriminant = System.Math.Sqrt(acDiff * acDiff + 4 * b * b);

            var eigVal1 = (acSum + sqrtDiscriminant) / 2;
            var eigVal2 = (acSum - sqrtDiscriminant) / 2;

            //A * X = lambda * X => y / x = b / (lambda - c); where lambda is the first eigen-value
            var angle = System.Math.Atan2(2 * b, (acDiff + sqrtDiscriminant));

            success = eigVal1 > 0 && eigVal2 > 0;
            return new Ellipse
            {
                Center = center,
                Size = new SizeF 
                {
                    Width = (float)System.Math.Sqrt(eigVal1) * 4,
                    Height =(float)System.Math.Sqrt(eigVal2) * 4
                },
                Angle =  (float)Accord.Extensions.Math.Geometry.Angle.ToDegrees(angle) 
            };
        }

        /// <summary>
        /// Fits the covariance matrix (or 2nd moment matrix) to the ellipse by calculating eigen-vectors and values.
        /// </summary>
        /// <param name="a">[0, 0] value of the covariance matrix.</param>
        /// <param name="b">[0, 1] or [1, 0] value of the covariance matrix.</param>
        /// <param name="c">[1, 1] value of the covariance matrix.</param>
        /// <param name="center">Center of the ellipse.</param>
        /// <returns>Ellipse.</returns>
        public static Ellipse Fit(double a, double b, double c, PointF center = default(PointF))
        {
            bool success;
            return Fit(a, b, c, center, out success);
        }
    }
}
