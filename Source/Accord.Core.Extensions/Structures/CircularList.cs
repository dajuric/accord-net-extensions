using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Range = AForge.IntRange;

namespace Accord.Core
{
    public static class CircularListExtensions
    {
        public static CircularList<T> ToCircularList<T>(this List<T> list)
        {
            return new CircularList<T>(list);
        }
    }

    public class CircularList<T> : List<T>
    {
        public CircularList()
            : base()
        { }

        public CircularList(IEnumerable<T> collection)
            : base(collection)
        { }

        public new T this[int index]
        {
            get
            {
                return base[getNonNegativeIndex(index)];
            }
            set
            {
                base[getNonNegativeIndex(index)] = value;
            }
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(getNonNegativeIndex(index));
        }

        public new CircularList<T> GetRange(int index, int count)
        {
            return GetRange(new Range(index, index + count));
        }

        public CircularList<T> GetRange(Range range)
        {
            int[] segmentIndeces, segmentLengths;
            translateRange((int)range.Min, (int)range.Max, out segmentIndeces, out segmentLengths);

            var slice = new CircularList<T>();
            for (int i = 0; i < segmentIndeces.Length; i++)
            {
                slice.AddRange(base.GetRange(segmentIndeces[i], segmentLengths[i]));
            }

            return slice;
        }

        public new void RemoveRange(int index, int count)
        {
            int[] segmentIndeces, segmentLengths;
            translateRange(index, count, out segmentIndeces, out segmentLengths);

            for (int i = 0; i < segmentIndeces.Length; i++)
            {
                //second segment (if exist) starts from zero therefore there is no need to move indices after erasing some elements
                base.RemoveRange(segmentIndeces[i], segmentLengths[i]);
            }
        }

        public new void Insert(int index, T item)
        {
            base.Insert(getNonNegativeIndex(index), item);
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(getNonNegativeIndex(index), collection);
        }

        private int getNonNegativeIndex(int index)
        {
            return (this.Count + index) % this.Count;
        }

        private void translateRange(int idxA, int idxB, out int[] segmentIndeces, out int[] segmentLengths)
        {
            var realIdxA = getNonNegativeIndex(idxA);
            var realIdxB = getNonNegativeIndex(idxB);

            if (realIdxB < this.Count && realIdxA <= realIdxB)
            {
                segmentIndeces = new int[1]; segmentIndeces[0] = realIdxA;
                segmentLengths = new int[1]; segmentLengths[0] = realIdxB - realIdxA;
            }
            else
            {
                segmentIndeces = new int[2];
                segmentIndeces[0] = realIdxA;
                segmentIndeces[1] = 0;

                segmentLengths = new int[2];
                segmentLengths[0] = this.Count - realIdxA;
                segmentLengths[1] = realIdxB;
            }
        }
    }
}
