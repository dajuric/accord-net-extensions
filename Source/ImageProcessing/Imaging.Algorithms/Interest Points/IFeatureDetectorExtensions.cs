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

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Feature detector generic extensions.
    /// </summary>
    public static class IFeatureDetectorExtensions
    {
        /// <summary>
        /// Process image looking for interest points.
        /// </summary>
        /// <typeparam name="TPoint">The type of returned feature points.</typeparam>
        /// <typeparam name="TFeature">The type of extracted features.</typeparam>
        /// <param name="featureDetector">Feature detector.</param>
        /// <param name="image">Source image data to process.</param>
        /// <returns>Returns list of found interest points.</returns>
        public static List<TPoint> ProcessImage<TPoint, TFeature>(this IFeatureDetector<TPoint, TFeature> featureDetector, Gray<byte>[,] image)
             where TPoint : IFeatureDescriptor<TFeature>
        {
            List<TPoint> points;
            using (var uImg = image.Lock())
            {
                points = featureDetector.ProcessImage(uImg.AsAForgeImage());
            }

            return points;
        }
    }
}
