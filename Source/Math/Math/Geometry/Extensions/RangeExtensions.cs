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

using System.Collections.Generic;
using Range = AForge.IntRange;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides extension methods for <see cref="AForge.IntRange"/>.
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// Determines whether the specified values are inside the range.
        /// </summary>
        /// <param name="range">Range.</param>
        /// <param name="values">Values.</param>
        /// <returns>Collection where each element is set to true if the corresponding value is in the range, otherwise is set to false.</returns>
        public static IEnumerable<bool> IsInside(this Range range, IEnumerable<int> values)
        {
            foreach (var val in values)
            {
                yield return range.IsInside(val);
            }
        }
    }
}
