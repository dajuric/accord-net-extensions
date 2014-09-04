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
using AForge;

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Some of defined functions can be used as object extensions.</para>
    /// Provides extension methods for collections.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Gets linear range of type Int32.
        /// </summary>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Maximum value.</param>
        /// <param name="step">Step between values.</param>
        /// <returns>Range of values.</returns>
        public static IEnumerable<int> GetRange(int start, int end, int step = 1)
        {
            for (int i = start; i <= end; i += step)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Gets linear range of type Single.
        /// </summary>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Maximum value.</param>
        /// <param name="step">Step between values.</param>
        /// <returns>Range of values.</returns>
        public static IEnumerable<float> GetRange(float start, float end, float step = 1)
        {
            for (float i = start; i <= end; i += step)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Gets elements from source array at user defined indices.
        /// </summary>
        /// <param name="src">Data array.</param>
        /// <param name="indicies">User defined indices.</param>
        /// <returns>Array of values at specified indices.</returns>
        public static T[] GetAt<T>(this IList<T> src, IEnumerable<int> indicies)
        {
            T[] arr = new T[indicies.Count()];

            int i = 0;
            foreach (var idx in indicies)
            {
                arr[i++] = src[idx];
            }

            return arr;
        }

        /// <summary>
        /// Sets elements from source array at user defined indices.
        /// </summary>
        /// <param name="src">Data array.</param>
        /// <param name="indicies">User defined indices.</param>
        /// <param name="newValues">New values that replace old ones.</param>
        public static void SetAt<T>(this IList<T> src, IEnumerable<int> indicies, IList<T> newValues)
        {
            int i = 0;
            foreach (var idx in indicies)
            {
                src[idx] = newValues[i++];
            }
        }

        /// <summary>
        /// Applies a user specified function to a given array at specified indices.
        /// </summary>
        /// <param name="src">Source collection.</param>
        /// <param name="indicies">Selection indices.</param>
        /// <param name="func">Function to apply at elements which are selected with indices.</param>
        public static void ApplyInPlace<T>(this IList<T> src, IEnumerable<int> indicies, Func<T, T> func)
        {
            foreach (var idx in indicies)
            {
                src[idx] = func(src[idx]);
            }
        }


        /// <summary>
        /// Gets elements from source array at user defined indices.
        /// </summary>
        /// <param name="src">Data array.</param>
        /// <param name="range">User defined range.</param>
        /// <returns>Array of values in specified range.</returns>
        public static T[] GetRange<T>(this IList<T> src, IntRange range)
        {
            T[] arr = new T[range.Max - range.Min + 1];

            int i = 0;
            for (int idx = range.Min; idx <= range.Max; idx++)
            {
                arr[i] = src[idx];
                i++;
            }

            return arr;
        }

        /// <summary>
        /// Gets elements from source array at user defined indices.
        /// </summary>
        /// <param name="src">Data array.</param>
        /// <param name="startIndex">Start index.</param>
        /// <param name="length">Range length.</param>
        /// <returns>Array of values in specified range.</returns>
        public static T[] GetRange<T>(this IList<T> src, int startIndex, int length)
        {
            return src.GetRange(new IntRange(startIndex, startIndex + length - 1));
        }

        /// <summary>
        /// Gets elements from source array at user defined indices.
        /// </summary>
        /// <param name="src">Data array.</param>
        /// <param name="startIndex">Start index.</param>
        /// <returns>Array of values in specified range.</returns>
        public static T[] GetRange<T>(this IList<T> src, int startIndex)
        {
            var length = src.Count;
            return src.GetRange(new IntRange(startIndex, startIndex + length - 1));
        }

        /// <summary>
        /// Removes user-specified elements from the list.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="src">Data collection.</param>
        /// <param name="elements">Elements to remove.</param>
        public static void Remove<T>(this IList<T> src, IEnumerable<T> elements)
            where T: class
        {
            var readOnlyElements = elements.ToList(); //support if the elements is the same collection as src

            foreach (var e in readOnlyElements)
            {
                src.Remove(e);
            }
        }

        /// <summary>
        /// Removes user-specified element indices from the collection.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="src">Data collection.</param>
        /// <param name="elementIndices">Indices of the elements to remove.</param>
        public static void RemoveAt<T>(this IList<T> src, IEnumerable<int> elementIndices)
            where T: class
        {
            var elementsToRemove = src.GetAt(elementIndices); //TODO - noncritical: make a better implementation
            src.Remove(elementsToRemove);
        }

        /// <summary>
        /// Creates a collection using provided function.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="numberOfElements">Number of elements to create.</param>
        /// <param name="creator">Element creator. Receives current element index as parameter.</param>
        /// <returns>Array of created elements.</returns>
        public static T[] Create<T>(int numberOfElements, Func<int, T> creator)
        {
            T[] arr = new T[numberOfElements];

            for (int i = 0; i < numberOfElements; i++)
            {
                arr[i] = creator(i);
            }

            return arr;
        }

        /// <summary>
        /// Enumerates both collections simultaneously and executes user-specified function.
        /// <para>Both collection will be iterated until end is reached for one of them.</para>
        /// </summary>
        /// <typeparam name="TFirst">First collection data type.</typeparam>
        /// <typeparam name="TSecond">Second collection data type.</typeparam>
        /// <typeparam name="TOutput">Output element type.</typeparam>
        /// <param name="first">First collection.</param>
        /// <param name="second">Second collection.</param>
        /// <param name="function">User specified function.</param>
        /// <returns>The result collection which elements are generated by using user-specified function.</returns>
        public static IEnumerable<TOutput> EnumerateWith<TFirst, TSecond, TOutput>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TOutput> function)
        {
            using (var f = first.GetEnumerator())
            using(var s = second.GetEnumerator())
            {
                while (f.MoveNext() && s.MoveNext())
                {
                    yield return function(f.Current, s.Current);
                }
            }
        }
    }
}
