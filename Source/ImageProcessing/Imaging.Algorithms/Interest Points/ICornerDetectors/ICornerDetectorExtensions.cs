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

using AForge.Imaging;
using System.Collections.Generic;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Corner detector generic extensions.
    /// </summary>
    public static class ICornerDetectorExtensions
    {
        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// <param name="cornerDetector">Corner detection algorithm instance.</param>
        /// <param name="image">Source image to process.</param>
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        public static List<Point> ProcessImage(this ICornersDetector cornerDetector, Gray<byte>[,] image)
        {
            List<Point> points = null;
            using (var uImg = image.Lock())
            {
                points = cornerDetector.ProcessImage(uImg.AsAForgeImage());
            }

            return points;
        }
    }
}
