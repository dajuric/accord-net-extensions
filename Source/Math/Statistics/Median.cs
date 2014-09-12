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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Statistics
{
    /// <summary>
    /// Contains extensions for median calculation.
    /// </summary>
    public static class MedianExtensions
    {
       /// <summary>
       /// Finds median for the provided collection.
       /// <para>The collection will be mutated.</para>
       /// </summary>
       /// <typeparam name="T">Element type.</typeparam>
       /// <param name="list">List.</param>
       /// <returns>Median.</returns>
        public static T Median<T>(this IList<T> list) where T : IComparable<T>
        {
            return list.GetNthSmallest((list.Count - 1) / 2);
        }

        /// <summary>
        /// Find median for the provided collection.
        /// </summary>
        /// <typeparam name="TDst">The key type.</typeparam>
        /// <typeparam name="TSrc">The element type.</typeparam>
        /// <param name="collection">Collection.</param>
        /// <param name="selector">Key selector.</param>
        /// <returns>Median for the provided collection for the specified key.</returns>
        public static TSrc MedianBy<TDst, TSrc>(this IList<TSrc> collection, Func<TSrc, TDst> selector)
        {
            if (collection.Any() == false)
                throw new ArgumentException("Collection is empty!");

            var sortedColelction = collection.OrderBy(selector);
            return sortedColelction.ElementAt(collection.Count / 2);
        }
    }
}
