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
using Accord.Imaging;
using Accord.Extensions.Imaging;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Border following extensions.
    /// </summary>
    public static class BorderFollowingExtensions
    {
        /// <summary>
        /// Extracts the contour from a single object in a grayscale image. (uses Accord built-in function)
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="minGradientStrength">The pixel value threshold above which a pixel
        /// is considered black (belonging to the object). Default is zero.</param>
        public static List<Point> FindContour(this Gray<byte>[,] im, byte minGradientStrength = 0)
        {
            BorderFollowing bf = new BorderFollowing(minGradientStrength);

            List<Point> points;
            using (var uImg = im.Lock())
            {
                points = bf.FindContour(uImg.AsAForgeImage());
            }

            return points;
        }
    }
}
