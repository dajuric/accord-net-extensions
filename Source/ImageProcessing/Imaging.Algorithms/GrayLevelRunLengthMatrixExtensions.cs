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
using Accord.Extensions.Imaging;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Gray-Level Run-Length Matrix extensions.
    /// </summary>
    public static class GrayLevelRunLengthMatrixExtensions
    {
        /// <summary>
        /// Computes the Gray-level Run-length for the given image source.
        /// </summary>
        /// <param name="grayLevelRunLengthMatrix">Gray-Level Run-Length Matrix.</param>
        /// <param name="source">The source image.</param>
        /// <returns>An array of run-length vectors containing level counts for every width pixel in <paramref name="source"/>.</returns>
        public static double[][] Compute(this GrayLevelRunLengthMatrix grayLevelRunLengthMatrix, Image<Gray<byte>> source)
        {
            return grayLevelRunLengthMatrix.Compute(source.AsAForgeImage());
        }
    }
}
