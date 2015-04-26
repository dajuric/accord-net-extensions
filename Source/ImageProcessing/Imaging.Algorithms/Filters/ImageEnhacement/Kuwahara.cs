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

using System;
using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Kuwahara filter.
    /// </summary>
    public static class KuwaharaExtensions
    {
        /// <summary>
        /// Kuwahara filter.
        /// <para>Accord.NET internal call. See: <see cref="Accord.Imaging.Filters.Kuwahara"/> for details.</para>
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="size">the size of the kernel used in the Kuwahara filter. This should be odd and greater than or equal to five</param>
        /// <param name="blockSize">the size of each of the four inner blocks used in the Kuwahara filter. This is always half the <paramref name="size"/> minus one.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static Gray<byte>[,] Kuwahara(this Gray<byte>[,] img, int size = 5, int blockSize = 2, bool inPlace = false)
        {
            Kuwahara k = new Kuwahara();
            return img.ApplyFilter(k, inPlace);
        }
    }
}
