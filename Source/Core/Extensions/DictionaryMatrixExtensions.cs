using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides extension methods for <see cref="Dictionary"/> matrix.
    /// </summary>
    public static class DictonaryMatExtensions
    {
        /// <summary>
        /// Determines whether the dictionary matrix contains the specified key.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="mat">Dictionary matrix.</param>
        /// <param name="firstKey">First key.</param>
        /// <param name="secondKey">Second key.</param>
        /// <returns>True if the specified key exist in dictionary matrix, false otherwise.</returns>
        public static bool Contains<TKey, TValue>(this Dictionary<TKey, Dictionary<TKey, TValue>> mat, TKey firstKey, TKey secondKey)
        {
            TValue containedValue;
            return TryGetValue(mat, firstKey, secondKey, out containedValue);
        }

        public static bool TryGetValue<TKey, TValue>(this Dictionary<TKey, Dictionary<TKey, TValue>> mat, TKey firstKey, TKey secondKey, out TValue value)
        {
            value = default(TValue);

            if (!mat.ContainsKey(firstKey))
                return false;

            var innerDict = mat[firstKey];
            if (innerDict == null || !innerDict.ContainsKey(secondKey))
                return false;

            value = innerDict[secondKey];
            return true;
        }

        public static bool Add<TKey, TValue>(this Dictionary<TKey, Dictionary<TKey, TValue>> mat, TKey firstKey, TKey secondKey, TValue value)
        {
            if (!mat.ContainsKey(firstKey))
                mat.Add(firstKey, new Dictionary<TKey, TValue>());

            var innerDict = mat[firstKey];
            if (!innerDict.ContainsKey(secondKey))
            {
                innerDict.Add(secondKey, value);
                return true;
            }

            return false;
        }

        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, Dictionary<TKey, TValue>> mat, TKey firstKey, TKey secondKey, TValue value)
        {
            if (!mat.Contains(firstKey, secondKey))
            {
                mat.Add(firstKey, secondKey, value);
            }
            else
            {
                mat[firstKey][secondKey] = value;
            }
        }

        public static bool Remove<TKey, TValue>(this Dictionary<TKey, Dictionary<TKey, TValue>> mat, TKey firstKey, TKey secondKey)
        {
            if (!mat.Contains(firstKey, secondKey))
                return false;

            mat[firstKey].Remove(secondKey);
            return true;
        }

        public static Dictionary<TKey, Dictionary<TKey, TValue>> ToMatrix<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> firstKeySelector, Func<TValue, TKey> secondKeySelector)
        {
            return values.ToMatrix(firstKeySelector, secondKeySelector, x => x);
        }

        public static Dictionary<TKey, Dictionary<TKey, TValue>> ToMatrix<TKey, TValue, TSrcValue>(this IEnumerable<TSrcValue> values, Func<TSrcValue, TKey> firstKeySelector, Func<TSrcValue, TKey> secondKeySelector, Func<TSrcValue, TValue> valueSelector)
        {
            var mat = new Dictionary<TKey, Dictionary<TKey, TValue>>();

            foreach (var srcVal in values)
            {
                var firstKey = firstKeySelector(srcVal);
                var secondKey = secondKeySelector(srcVal);
                var dstValue = valueSelector(srcVal);

                mat.Add(firstKey, secondKey, dstValue);
            }

            return mat;
        }

        public static IEnumerable<TValue> AsEnumerable<TKey, TValue>(this Dictionary<TKey, Dictionary<TKey, TValue>> mat)
        {
            foreach (var innerDict in mat.Values)
            {
                foreach (var val in innerDict.Values)
                {
                    yield return val;
                }
            }
        }

        public static IEnumerable<TKey> GetKeys<TKey, TValue>(this Dictionary<TKey, Dictionary<TKey, TValue>> mat)
        {
            var keys = new List<TKey>();

            foreach (var srcKey in mat.Keys)
            {
                keys.Add(srcKey);

                var dstKeys = mat[srcKey].Keys;
                keys.AddRange(dstKeys);
            }

            return keys.Distinct();
        }
    }
}
