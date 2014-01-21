using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Core
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<int> GetRange(int start, int end, int step = 1)
        {
            for (int i = start; i <= end; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<float> GetRange(float start, float end, float step = 1)
        {
            for (float i = start; i <= end; i += step)
            {
                yield return i;
            }
        }

        public static T[] Get<T>(this T[] src, IEnumerable<int> indicies)
        {
            T[] arr = new T[indicies.Count()];

            int i = 0;
            foreach (var idx in indicies)
            {
                arr[i++] = src[idx];
            }

            return arr;
        }

        public static void Set<T>(this T[] src, IEnumerable<int> indicies, T[] newValues)
        {
            int i = 0;
            foreach (var idx in indicies)
            {
                src[idx] = newValues[i++];
            }
        }

        public static void ApplyInPlace<T>(this T[] src, IEnumerable<int> indicies, Func<T, T> func)
        {
            foreach (var idx in indicies)
            {
                src[idx] = func(src[idx]);
            }
        }

    }
}
