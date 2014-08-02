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


namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Provides methods for manipulating angle calculation.
    /// </summary>
    public static class Angle
    {
        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="angleRad">Angle in radians.</param>
        /// <param name="normalizeDegrees">Returns angle in degrees in rangle [0..359].</param>
        /// <returns>Angle in degrees.</returns>
        public static double ToDegrees(double angleRad, bool normalizeDegrees = true)
        {
            var deg = angleRad / System.Math.PI * 180;

            if(normalizeDegrees)
                deg = NormalizeDegrees(deg);
            
            return deg;
        }

        /// <summary>
        /// Calculates angle in degrees in range [0..360].
        /// </summary>
        /// <param name="angleDeg">Angle in degrees.</param>
        /// <returns>Angle in degrees in range [0..360].</returns>
        public static double NormalizeDegrees(double angleDeg)
        {
            angleDeg %= 360;

            if (angleDeg < 0)
                angleDeg += 360;

            return angleDeg;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="angleDeg">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        public static double ToRadians(double angleDeg)
        {
            return angleDeg * System.Math.PI / 180;
        }

        /// <summary>
        /// Calculates absolute distance in degrees for two angles in degrees.
        /// </summary>
        /// <param name="angleDegA">First angle in degrees.</param>
        /// <param name="angleDegB">Second angle in degrees.</param>
        /// <returns>Returns absolute distance in degrees for two angles in degrees.</returns>
        public static double DistanceDeg(double angleDegA, double angleDegB)
        {
            var dist = angleDegB - angleDegA;
            dist = (dist + 180) % 360 - 180;
            return dist;
        }
    }
}
