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

using Accord.Imaging;
using System.Collections.Generic;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    ///  Maximum cross-correlation feature point matching algorithm extensions.
    /// </summary>
    public static class CorrelationMatchingExtensions
    {
        /// <summary>
        ///  Maximum cross-correlation feature point matching algorithm.
        /// </summary>
        /// <param name="correlationMatching"> Maximum cross-correlation feature point matching algorithm.</param>
        /// <param name="image1">First image.</param>
        /// <param name="image2">Second image.</param>
        /// <param name="points1">Points from the first image.</param>
        /// <param name="points2">Points from the second image.</param>
        /// <returns>Matched point-pairs.</returns>
        public static Point[][] Match(this CorrelationMatching correlationMatching,
                                      Image<Gray, byte> image1, Image<Gray, byte> image2, Point[] points1, Point[] points2)
        {
            var result = correlationMatching.Match
                (
                  image1.ToBitmap(copyAlways: false, failIfCannotCast: true),
                  image2.ToBitmap(copyAlways: false, failIfCannotCast: true),
                  points1,
                  points2
                );

            return result;
        }
    }
}
