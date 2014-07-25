using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accord.Extensions
{
    /// <summary>
    /// Provides extensions for working with collections in parallel way.
    /// <para>Taken from <a href="http://blogs.msdn.com/b/pfxteam/archive/2009/08/12/9867246.aspx">parallel while</a> and modified.</para>
    /// </summary>
    public static class ParallelExtensions
    {
        /// <summary>
        /// Executes a while loop in parallel.
        /// </summary>
        /// <param name="condition">Condition function. Returns true if the loop should advance, false otherwise.</param>
        /// <param name="body">Body function.</param>
        public static void While(Func<bool> condition, Action<ParallelLoopState> body)
        {
            While(new System.Threading.Tasks.ParallelOptions(), condition, body);
        }

        /// <summary>
        /// Executes a while loop in parallel.
        /// </summary>
        /// <param name="parallelOptions">Parallel options.</param>
        /// <param name="condition">Condition function. Returns true if the loop should advance, false otherwise.</param>
        /// <param name="body">Body function.</param>
        public static void While(System.Threading.Tasks.ParallelOptions parallelOptions, Func<bool> condition, Action<ParallelLoopState> body)
        {
            System.Threading.Tasks.Parallel.ForEach(new InfinitePartitioner(), parallelOptions,
                (ignored, loopState) =>
                {
                    if (condition()) 
                        body(loopState);
                    else 
                        loopState.Stop();
                });
        }
    }

    /// <summary>
    /// Represents an infinite data partitioner. Returns an infinite collection of type <see cref="System.Boolean"/>.
    /// <para>It is used in While function extension.</para>
    /// <para>Taken from <a href="http://blogs.msdn.com/b/pfxteam/archive/2009/08/12/9867246.aspx">parallel while</a>.</para>
    /// </summary>
    public class InfinitePartitioner : Partitioner<bool>
    {
        /// <summary>
        /// Gets the partitions.
        /// </summary>
        /// <param name="partitionCount">The partition count.</param>
        /// <returns>partitions.</returns>
        public override IList<IEnumerator<bool>> GetPartitions(int partitionCount)
        {
            if (partitionCount < 1)
                throw new ArgumentOutOfRangeException("partitionCount");
            return (from i in Enumerable.Range(0, partitionCount)
                    select InfiniteEnumerator()).ToArray();
        }

        /// <summary>
        /// This partitioner supports dynamic partitions (returns true).
        /// </summary>
        public override bool SupportsDynamicPartitions { get { return true; } }

        /// <summary>
        /// Creates and returns ininite enumerators of type <see cref="System.Boolean"/>.
        /// </summary>
        /// <returns>Ininite enumerators of type <see cref="System.Boolean"/></returns>
        public override IEnumerable<bool> GetDynamicPartitions()
        {
            return new InfiniteEnumerators();
        }

        /// <summary>
        /// Gets the ininite enumerator.
        /// </summary>
        /// <returns>The ininite enumerator.</returns>
        private static IEnumerator<bool> InfiniteEnumerator()
        {
            while (true) yield return true;
        }

        private class InfiniteEnumerators : IEnumerable<bool>
        {
            public IEnumerator<bool> GetEnumerator()
            {
                return InfiniteEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }
    }
}
