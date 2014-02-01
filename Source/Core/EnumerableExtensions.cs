using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Core
{
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
        public static T[] Get<T>(this IList<T> src, IEnumerable<int> indicies)
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
        public static void Set<T>(this IList<T> src, IEnumerable<int> indicies, IList<T> newValues)
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

    }
}
