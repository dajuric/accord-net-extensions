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

using Accord.Math;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Contains methods for geometric transformations.
    /// </summary>
    public static class Transforms2D
    {
        #region Rotation

        /// <summary>
        /// Gets the 3x3 rotation matrix.
        /// </summary>
        /// <param name="angleRad">Rotation angle in radians.</param>
        /// <returns>Rotation matrix.</returns>
        public static float[,] Rotation(float angleRad)
        {
            var cos = (float)System.Math.Cos(angleRad);
            var sin = (float)System.Math.Sin(angleRad);

            return new float[,] 
            {
                {cos, -sin, 0},
                {sin, cos,  0},
                {0,   0,    1}
            };
        }

        #endregion

        #region Translation

        /// <summary>
        /// Gets the 3x3 translation matrix.
        /// </summary>
        /// <param name="x">Horizontal offset.</param>
        /// <param name="y">Vertical offset.</param>
        /// <returns>Translation matrix.</returns>
        public static float[,] Translation(float x, float y)
        {
            return new float[,] 
            {
                {1, 0, x},
                {0, 1, y},
                {0, 0, 1}
            };
        }

        #endregion

        #region Scale

        /// <summary>
        /// Gets the 3x3 scale matrix.
        /// </summary>
        /// <param name="x">Horizontal scale.</param>
        /// <param name="y">Vertical scale.</param>
        /// <param name="z">Depth scale.</param>
        /// <returns>Scale matrix.</returns>
        public static float[,] Scale(float x, float y, float z = 1)
        {
            return new float[,] 
            {
                {x, 0, 0},
                {0, y, 0},
                {0, 0, z}
            };
        }

        #endregion

        /// <summary>
        /// Multiplies transforms starting from the last one to the first one (stack).
        /// Transforms are given by priority.
        /// </summary>
        /// <param name="transforms">Transfrom matrices.</param>
        /// <returns>Combined transfrom matrix.</returns>
        public static float[,] Combine(params float[][,] transforms)
        {
            float[,] res = Matrix.Identity(3).ToSingle();

            for (int i = transforms.Length - 1; i >= 0; i--)
            {
                res = res.Multiply(transforms[i]);
            }

            return res;
        }

    }

}
