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

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides extension methods for <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> matrix.
    /// </summary>
    public static class DictonaryMatExtensions
    {
        /// <summary>
        /// Determines whether the dictionary matrix contains the specified key.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="mat">Sparse matrix - nested dictionaries.</param>
        /// <param name="firstKey">First key - row selector.</param>
        /// <param name="secondKey">Second key - column selector.</param>
        /// <returns>True if the specified key exist in dictionary matrix, false otherwise.</returns>
        public static bool ContainsKey<TKey, TValue>(this IDictionary<Pair<TKey>, TValue> mat, TKey firstKey, TKey secondKey)
        {
            if (!mat.ContainsKey(new Pair<TKey>(firstKey, secondKey)))
                return false;

            return true;
        }

        /// <summary>
        /// Gets the value associated with the two keys set.
        /// </summary>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="mat">Sparse matrix - nested dictionaries.</param>
        /// <param name="firstKey">First key - row selector.</param>
        /// <param name="secondKey">Second key - column selector.</param>
        /// <returns>Value.</returns>
        public static TValue Get<TKey, TValue>(this IDictionary<Pair<TKey>, TValue> mat, TKey firstKey, TKey secondKey)
        {
            return mat[new Pair<TKey>(firstKey, secondKey)];
        }

        /// <summary>
        /// Gets the value determined by the <paramref name="firstKey"/> and <paramref name="secondKey"/>.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="mat">Sparse matrix - nested dictionaries.</param>
        /// <param name="firstKey">First key - row selector.</param>
        /// <param name="secondKey">Second key - colum selector.</param>
        /// <param name="value">Value.</param>
        /// <returns>True if provided keys exist, false otherwise.</returns>
        public static bool TryGetValue<TKey, TValue>(this IDictionary<Pair<TKey>, TValue> mat, TKey firstKey, TKey secondKey, out TValue value)
        {
            value = default(TValue);

            if (!mat.ContainsKey(firstKey, secondKey))
                return false;

            value = mat.Get(firstKey, secondKey);
            return true;
        }

        /// <summary>
        /// Adds data determined by the provided keys.
        /// <para>In case the provided keys already exist, an exception is thrown.</para>
        /// </summary>
        /// <exception cref="System.ArgumentException">Key already exist.</exception>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="mat">Sparse matrix - nested dictionaries.</param>
        /// <param name="firstKey">First key - row selector.</param>
        /// <param name="secondKey">Second key - colum selector.</param>
        /// <param name="value">Value.</param>
        public static void Add<TKey, TValue>(this IDictionary<Pair<TKey>, TValue> mat, TKey firstKey, TKey secondKey, TValue value)
        {
            mat.Add(new Pair<TKey>(firstKey, secondKey), value);
        }

        /// <summary>
        /// Adds or update data determined by the provided keys.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="mat">Sparse matrix - nested dictionaries.</param>
        /// <param name="firstKey">First key - row selector.</param>
        /// <param name="secondKey">Second key - colum selector.</param>
        /// <param name="value">Value.</param>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<Pair<TKey>, TValue> mat, TKey firstKey, TKey secondKey, TValue value)
        {
            if (!mat.ContainsKey(firstKey, secondKey))
            {
                mat.Add(firstKey, secondKey, value);
            }
            else
            {
                mat[new Pair<TKey>(firstKey, secondKey)] = value;
            }
        }

        /// <summary>
        /// Removes data determined by the provided keys.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="mat">Sparse matrix - nested dictionaries.</param>
        /// <param name="firstKey">First key - row selector.</param>
        /// <param name="secondKey">Second key - colum selector.</param>
        /// <returns>True if the provided keys exist, false otherwise.</returns>
        public static bool Remove<TKey, TValue>(this IDictionary<Pair<TKey>, TValue> mat, TKey firstKey, TKey secondKey)
        {
            return mat.Remove(new Pair<TKey>(firstKey, secondKey));
        }

        /// <summary>
        /// Creates new sparse matrix from provided collection.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="values"></param>
        /// <param name="firstKeySelector">Row matrix selector.</param>
        /// <param name="secondKeySelector">Column matrix selector.</param>
        /// <returns>Sparse matrix - nested dictionaries.</returns>
        public static IDictionary<Pair<TKey>, TValue> ToSparseMatrix<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> firstKeySelector, Func<TValue, TKey> secondKeySelector)
        {
            return values.ToSparseMatrix(firstKeySelector, secondKeySelector, x => x);
        }

        /// <summary>
        /// Creates new sparse matrix from provided collection.
        /// </summary>
        /// <typeparam name="TSrcValue">Source value type.</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="values"></param>
        /// <param name="firstKeySelector">Row matrix selector.</param>
        /// <param name="secondKeySelector">Column matrix selector.</param>
        /// <param name="valueSelector">Value selector.</param>
        /// <returns>Sparse matrix - nested dictionaries.</returns>
        public static Dictionary<Pair<TKey>, TValue> ToSparseMatrix<TKey, TValue, TSrcValue>(this IEnumerable<TSrcValue> values, Func<TSrcValue, TKey> firstKeySelector, Func<TSrcValue, TKey> secondKeySelector, Func<TSrcValue, TValue> valueSelector)
        {
            var mat = new Dictionary<Pair<TKey>, TValue>();

            foreach (var srcVal in values)
            {
                var firstKey = firstKeySelector(srcVal);
                var secondKey = secondKeySelector(srcVal);
                var dstValue = valueSelector(srcVal);

                mat.Add(firstKey, secondKey, dstValue);
            }

            return mat;
        }

        /// <summary>
        /// Returns the collection of sparse matrix values.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="mat">Sparse matrix - nested dictionaries.</param>
        /// <returns>Collection of sparse matrix values.</returns>
        public static IEnumerable<TValue> AsEnumerable<TKey, TValue>(this IDictionary<Pair<TKey>, TValue> mat)
        {
            foreach (var val in mat.Values)
            {
                yield return val;
            }
        }

        /// <summary>
        /// Returns the collection of sparse matrix row and column keys.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="mat">Sparse matrix - nested dictionaries.</param>
        /// <returns>Collection of sparse matrix row and column keys.</returns>
        public static IEnumerable<TKey> GetKeys<TKey, TValue>(this IDictionary<Pair<TKey>, TValue> mat)
        {
            var keys = new List<TKey>();

            keys.AddRange(mat.Keys.Select(x => x.First)); //source key
            keys.AddRange(mat.Keys.Select(x => x.Second)); //destination key
     
            return keys.Distinct();
        }
    }
}
