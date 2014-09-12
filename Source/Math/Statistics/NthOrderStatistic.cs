using System;
using System.Collections.Generic;

namespace Accord.Extensions.Statistics
{
    /// <summary>
    /// Extensions for NOrderStatistics calculation.
    /// <para>
    /// Taken from: <a href="http://stackoverflow.com/questions/4140719/i-need-c-sharp-function-that-will-calculate-median">Median calculation</a> and modified.
    /// </para>
    /// </summary>
    public static class NthOrderStatisticExtensions
    {
        private static void swap<T>(this IList<T> list, int i, int j)
        {
            if (i == j)   //This check is not required but Partition function may make many calls so its for perf reason
                return;
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        /// <summary>
        /// Partitions the given list around a pivot element such that all elements on left of pivot are less or equal pivot
        /// and the ones at the right are greater or equal pivot. This method can be used for sorting, N-order statistics such as
        /// as median finding algorithms.
        /// Pivot is selected randomly if random number generator is supplied else its selected as last element in the list.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 171
        /// </summary>
        private static int partition<T>(this IList<T> list, int start, int end, Random rnd = null) where T : IComparable<T>
        {
            if (rnd != null)
                list.swap(end, rnd.Next(start, end));

            var pivot = list[end];
            var lastLow = start - 1;
            for (var i = start; i < end; i++)
            {
                if (list[i].CompareTo(pivot) <= 0)
                    list.swap(i, ++lastLow);
            }
            list.swap(end, ++lastLow);
            return lastLow;
        }

        /// <summary>
        /// Returns Nth smallest element from the list. 
        /// <para>
        /// Note: specified list would be mutated in the process.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 216
        /// </para>
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="list">List.</param>
        /// <param name="n">Index for the smallest element to return. 
        /// Here n starts from 0 so that n=0 returns minimum, n=1 returns 2nd smallest element etc.
        /// </param>
        /// <param name="start">The starting element for the provided list.</param>
        /// <param name="end">The end element for th provided list.</param>
        /// <param name="rnd">
        /// Random generator for pivot element selection.
        /// If supplied pivot element is selected randomly, otherwise it is selected as last element in the list.
        /// </param>
        /// <returns>Nth smallest element.</returns>
        public static T GetNthSmallest<T>(this IList<T> list, int n, int start, int end, Random rnd) where T : IComparable<T>
        {
            while (true)
            {
                var pivotIndex = list.partition(start, end, rnd);
                if (pivotIndex == n)
                    return list[pivotIndex];

                if (n < pivotIndex)
                    end = pivotIndex - 1;
                else
                    start = pivotIndex + 1;
            }
        }

        /// <summary>
        /// Returns Nth smallest element from the list. 
        /// <para>
        /// Note: specified list would be mutated in the process.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 216
        /// </para>
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="list">List.</param>
        /// <param name="n">Index for the smallest element to return. 
        /// Here n starts from 0 so that n=0 returns minimum, n=1 returns 2nd smallest element etc.
        /// </param>
        /// <param name="rnd">
        /// Random generator for pivot element selection.
        /// If supplied pivot element is selected randomly, otherwise it is selected as last element in the list.
        /// </param>
        /// <returns>Nth smallest element.</returns>
        public static T GetNthSmallest<T>(this IList<T> list, int n, Random rnd = null) where T : IComparable<T>
        {
            return GetNthSmallest(list, n, 0, list.Count - 1, rnd);
        }
    }
}
