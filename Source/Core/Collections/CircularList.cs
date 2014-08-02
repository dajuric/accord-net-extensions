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

using System.Collections.Generic;
using Range = AForge.IntRange;

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides methods for circular list.
    /// </summary>
    public static class CircularListExtensions
    {
        /// <summary>
        /// Creates <see cref="Accord.Extensions.CircularList{T}"/> from <see cref="System.Collections.Generic.List{T}"/>.
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
    /// Represents s strongly typed circular list of objects meaning that any index is converted to absolute one before accessing the element - negative indices are supported.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    public class CircularList<T> : List<T>
    {
        /// <summary>
        /// Creates the default instance.
        /// </summary>
        public CircularList()
            : base()
        { }

        /// <summary>
        /// Creates the instance from collection.
        /// </summary>
        /// <param name="collection">The specified collection.</param>
        public CircularList(IEnumerable<T> collection)
            : base(collection)
        { }

        /// <summary>
        /// Gets or sets the element at specified index.
        /// </summary>
        /// <param name="index">The specified index.</param>
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
        /// <param name="index">The starting index.</param>
        /// <param name="count">The number of elements.</param>
        /// <returns>The circular list (data is shared).</returns>
        public new CircularList<T> GetRange(int index, int count)
        { 
            return GetRange(new Range(index, index + count));
        }

        /// <summary>
        /// Gets the range of elements from specified index range.
        /// </summary>
        /// <param name="range">The index range.</param>
        /// <returns>The circular list (data is shared).</returns>
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
