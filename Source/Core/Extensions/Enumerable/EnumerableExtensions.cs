using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
