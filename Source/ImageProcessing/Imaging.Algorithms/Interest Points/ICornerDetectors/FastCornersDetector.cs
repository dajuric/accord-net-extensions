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
using AForge;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Features from Accelerated Segment Test (FAST) corners detector extensions.
    /// </summary>
    public static class FastCornersDetectorExtensions
    {
        /// <summary>
        /// Features from Accelerated Segment Test (FAST) corners detector.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.FastCornersDetector"/> for details.</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="threshold">The suppression threshold. Decreasing this value increases the number of points detected by the algorithm.</param>
        /// <returns>Interest point locations.</returns>
        public static List<IntPoint> CornerFeaturesDetector(this Gray<byte>[,] im, int threshold = 20)
        {
            FastCornersDetector fast = new FastCornersDetector(threshold);

            List<IntPoint> points;
            using (var uImg = im.Lock())
            {
                points = fast.ProcessImage(uImg.AsAForgeImage());
            }
            
            return points;
        }
    }
}
