#region Licence and Terms
// MoreCollections
// https://github.com/more-dotnet/more-collections
//
// Copyright © Darko Jurić, 2015 
// darko.juric2@gmail.com
//
//The MIT License (MIT)
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
#endregion

using System.Collections.Generic;

namespace MoreCollections
{
    /// <summary>
    /// Circular list extension methods.
    /// </summary>
    public static class CircularListExtensions
    {
        /// <summary>
        /// Creates <see cref="MoreCollections.CircularList{T}"/> from <see cref="System.Collections.Generic.List{T}"/>.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="list">List of elements.</param>
        /// <returns>New instance of <see cref="Accord.Extensions.CircularList{T}"/>. Elements are not deep cloned.</returns>
        public static CircularList<T> ToCircularList<T>(this List<T> list)
        {
            return new CircularList<T>(list);
        }
    }

    /// <summary>
    /// List which supports modulo indexing.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    public class CircularList<T> : List<T>
    {
        /// <summary>
        /// Creates the default instance.
        /// </summary>
        public CircularList()
            : base()
        { }

        /// <summary>
        /// Creates the instance from a specified collection.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public CircularList(IEnumerable<T> collection)
            : base(collection)
        { }

        /// <summary>
        /// Gets or sets the element at specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>The element at specified index.</returns>
        public new T this[int index]
        {
            get { return base[getNonNegativeIndex(index)]; }
            set { base[getNonNegativeIndex(index)] = value; }
        }

        /// <summary>
        /// Removes element at index.
        /// </summary>
        /// <param name="index">The circular index.</param>
        public new void RemoveAt(int index)
        {
            base.RemoveAt(getNonNegativeIndex(index));
        }

        /// <summary>
        /// Gets the range of elements from specified index range.
        /// </summary>
        /// <param name="index">Starting index.</param>
        /// <param name="count">Elements count.</param>
        /// <returns>The circular list (data is shared).</returns>
        public new CircularList<T> GetRange(int index, int count)
        {
            int maxIdx = index + count;

            int[] segmentIndeces, segmentLengths;
            translateRange((int)index, (int)maxIdx, out segmentIndeces, out segmentLengths);

            var slice = new CircularList<T>();
            for (int i = 0; i < segmentIndeces.Length; i++)
            {
                slice.AddRange(base.GetRange(segmentIndeces[i], segmentLengths[i]));
            }

            return slice;
        }

        /// <summary>
        /// Removes range from the collection.
        /// </summary>
        /// <param name="index">The starting index (circular).</param>
        /// <param name="count">The number of elements to remove.</param>
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

        /// <summary>
        /// Inserts an element to the specified index.
        /// </summary>
        /// <param name="index">The specified index where to insert an element.</param>
        /// <param name="item">The element to insert.</param>
        public new void Insert(int index, T item)
        {
            base.Insert(getNonNegativeIndex(index), item);
        }

        /// <summary>
        /// Inserts a range to the collection.
        /// </summary>
        /// <param name="index">The starting position (circular).</param>
        /// <param name="collection">The collection to insert.</param>
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
                segmentLengths = new int[1]; segmentLengths[0] = realIdxB - realIdxA + 1;
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
