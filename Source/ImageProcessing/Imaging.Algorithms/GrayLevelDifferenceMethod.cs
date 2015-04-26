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
    /// Gray-Level Difference Method (GLDM) extensions.
    /// </summary>
    public static class GrayLevelDifferenceMethodExtensions
    {
        /// <summary>
        ///  Gray-Level Difference Method (GLDM).
        ///  <para>Computes an gray-level histogram of difference values between adjacent pixels in an image.</para>
        ///  <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.GrayLevelDifferenceMethod">Gray-Level Difference Method</see> for details.</para>
        /// </summary>
        /// <param name="image">The source image.</param>
        /// <param name="autoGray">Whether the maximum value of gray should be automatically computed from the image. </param>
        /// <param name="degree">The direction at which the co-occurrence should be found.</param>
        /// <returns>An histogram containing co-occurrences for every gray level in <paramref name="image"/>.</returns>
        public static int[] GrayLevelDifferenceMethod(this Gray<byte>[,] image, CooccurrenceDegree degree, bool autoGray = true)
        {
            GrayLevelDifferenceMethod gldm = new GrayLevelDifferenceMethod(degree, autoGray);

            int[] hist;
            using(var uImg = image.Lock())
            {
                hist = gldm.Compute(uImg.AsAForgeImage());
            }

            return hist;
        }

    }
}
