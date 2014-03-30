using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//taken from: http://blogs.msdn.com/b/pfxteam/archive/2009/08/12/9867246.aspx
namespace Accord.Extensions
{
    public static class ParallelExtensions
    {
        public static void While(Func<bool> condition, Action<ParallelLoopState> body)
        {
            While(new System.Threading.Tasks.ParallelOptions(), condition, body);
        }

        public static void While(System.Threading.Tasks.ParallelOptions parallelOptions, Func<bool> condition, Action<ParallelLoopState> body)
        {
            System.Threading.Tasks.Parallel.ForEach(new InfinitePartitioner(), parallelOptions,
                (ignored, loopState) =>
                {
                    if (condition()) body(loopState);
                    else loopState.Stop();
                });
        }
    }

    public class InfinitePartitioner : Partitioner<bool>
    {
        public override IList<IEnumerator<bool>> GetPartitions(int partitionCount)
        {
            if (partitionCount < 1)
                throw new ArgumentOutOfRangeException("partitionCount");
            return (from i in Enumerable.Range(0, partitionCount)
                    select InfiniteEnumerator()).ToArray();
        }

        public override bool SupportsDynamicPartitions { get { return true; } }

        public override IEnumerable<bool> GetDynamicPartitions()
        {
            return new InfiniteEnumerators();
        }

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
