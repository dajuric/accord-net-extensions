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
    /// Harris corners extensions.
    /// </summary>
    public static class HarrisCornersDetectorExtensions
    {
        /// <summary>
        /// Harris Corners Detector.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.HarrisCornersDetector"/> for details.</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="measure">Corners measures.</param>
        /// <param name="threshold">Harris threshold.</param>
        /// <param name="sigma">Gaussian smoothing sigma.</param>
        /// <param name="suppression">Non-maximum suppression window radius.</param>
        /// <returns>Interest point locations.</returns>
        public static List<IntPoint> HarrisCorners<TDepth>(this Gray<byte>[,] im, HarrisCornerMeasure measure = HarrisCornerMeasure.Harris, float threshold = 20000f, double sigma = 1.2, int suppression = 3)
        {
            HarrisCornersDetector harris = new HarrisCornersDetector(measure, threshold, sigma, suppression);
            
            List<IntPoint> points;        
            using(var uImg = im.Lock())
            {
                points = harris.ProcessImage(uImg.AsAForgeImage());
            }
         
            return points;
        }
    }
}
