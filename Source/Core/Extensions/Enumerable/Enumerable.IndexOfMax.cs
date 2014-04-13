using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides methods for finding the index of the max element in a sequence.
    /// </summary>
    public static class IndexOfMaxExtensions
    {

        /// <summary>
        /// Finds the index of the max element in a sequence.
        /// <para>Default comparer is used for a selected key.</para>
        /// </summary>
        /// <typeparam name="TSource">Collection type.</typeparam>
        /// <param name="collection">Collection.</param>
        /// <returns>
        /// The index of the maximum element.
        /// </returns>
        /// <exception cref="InvalidOperationException">in case when the collection is empty.</exception>
        public static int IndexOfMax<TSource>(this IEnumerable<TSource> collection)
        {
            return collection.IndexOfMax((x, i) => x, Comparer<TSource>.Default);
        }

        /// <summary>
        /// Finds the index of the max element in a sequence.
        /// <para>Default comparer is used for a selected key.</para>
        /// </summary>
        /// <typeparam name="TSource">Collection type.</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <param name="collection">Collection.</param>
        /// <param name="selector">Key selector. Parameters are: the current element and an index of an element in the sequence.</param>
        /// <returns>
        /// The index of the maximum element.
        /// </returns>
        /// <exception cref="InvalidOperationException">in case when the collection is empty.</exception>
        public static int IndexOfMax<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, int, TKey> selector)
        {
            return collection.IndexOfMin(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Finds the index of the max element in a sequence.
        /// </summary>
        /// <typeparam name="TSource">Collection type.</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <param name="collection">Collection.</param>
        /// <param name="selector">Key selector. Parameters are: the current element and an index of an element in the sequence.</param>
        /// <param name="comparer">Comparer for the selected key type.</param>
        /// <returns>
        /// The index of the maximum element.
        /// </returns>
        /// <exception cref="InvalidOperationException">in case when the collection is empty.</exception>
        public static int IndexOfMax<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, int, TKey> selector, IComparer<TKey> comparer)
        {
            int idx = 0;
            int idxOfMax = 0;
            using (var sourceIterator = collection.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");

                var minKey = selector(sourceIterator.Current, idx);

                while (sourceIterator.MoveNext())
                {
                    idx++;

                    var item = sourceIterator.Current;
                    var key = selector(item, idx);

                    if (comparer.Compare(key, minKey) > 0)
                    {
                        minKey = key;
                        idxOfMax = idx;
                    }
                }

                return idxOfMax;
            }
        }
    }
}
