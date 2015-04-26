#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions
{
    /// <summary>
    /// Represents s strongly typed list of objects. 
    /// The collection represents history; it can be more suitable than <see cref="System.Collections.Generic.List{T}"/> for object tracking where saving the detection history is common task.
    /// </summary>
    /// <typeparam name="T">Type object type.</typeparam>
    public class History<T> : ICloneable, IEnumerable<T>, IEnumerator<T>
    {
        /// <summary>
        /// Function delegate for adding an object.
        /// </summary>
        /// <param name="elem"></param>
        public delegate void AddElement(T elem);
        /// <summary>
        /// Represents an event that is fired when a new object is added.
        /// </summary>
        public event AddElement OnAddElement;

        private List<T> histElems;
        int maxNumOfElems;

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="maxNumOfElems">Maximum number of elements. If the maximum is reached the oldest elements are replaced.</param>
        public History(int maxNumOfElems = UInt16.MaxValue)
        { 
            this.histElems = new List<T>();
            this.maxNumOfElems = maxNumOfElems;
        }

        /// <summary>
        /// Adds element to the collection.
        /// </summary>
        /// <param name="elem">The specified element.</param>
        public void Add(T elem)
        { 
            if (histElems.Count == maxNumOfElems)
            {
                histElems.RemoveAt(0); //remove the oldest element
            }

            histElems.Add(elem);

            if (OnAddElement != null)
                OnAddElement(elem);
        }

        /// <summary>
        /// Gets or sets the element at specified history depth.
        /// </summary>
        /// <param name="histDepth">Histroy depth. Zero means current state.</param>
        /// <returns>An element at specified index.</returns>
        public T this[int histDepth]
        {
            get
            {
                T elem;

                if (histDepth > histElems.Count - 1)
                    elem = histElems.FirstOrDefault(); //the oldest element
                else
                    elem = histElems[histElems.Count - histDepth - 1];

                return elem;
            }
            set
            {
                if (histElems.Count == 0)
                    this.Add(value);

                else if (histDepth > histElems.Count - 1)
                    histElems[0] = value;

                else
                    histElems[histElems.Count - histDepth - 1] = value;
            }
        }

        /// <summary>
        /// Gets or sets the current element (depth zero).
        /// </summary>
        public T Current
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        /// <summary>
        /// Get or sets the oldest element (maximum depth).
        /// </summary>
        public T Oldest
        {
            get { return this[histElems.Count - 1]; }
            set { this[histElems.Count - 1] = value; }
        }

        /// <summary>
        /// Gets the number of elements.
        /// </summary>
        public int Count { get { return this.histElems.Count; } }
        /// <summary>
        /// Get the number of history capacity. If th emaximum is reached newly added elements will overwrite the oldest ondes.
        /// </summary>
        public int MaxCount { get { return this.maxNumOfElems; } }

        /// <summary>
        /// Removes all elements from the history.
        /// </summary>
        public void Clear()
        {
            this.histElems.Clear();
        }

        /// <summary>
        /// Returns part of the history. First element is the newest.
        /// </summary>
        public List<T> GetRange(int maxHistDepth)
        {
            int maxDepth = Math.Max(this.histElems.Count - maxHistDepth, 0);

            List<T> range = new List<T>();
            for (int i = this.histElems.Count - 1; i >= maxDepth; i--)
            {
                range.Add(this.histElems[i]);
            }

            return range;
        }

        /// <summary>
        /// Removes the specified range from the histroy.
        /// </summary>
        /// <param name="startDepth">Starting depth.</param>
        /// <param name="numOfElems">Number of elemets to remove. If specified more than maximum, maximum elements will be removed.</param>
        public void RemoveRange(int startDepth, int numOfElems)
        {
            if (startDepth >= this.histElems.Count)
                return;

            int index = Math.Max(this.histElems.Count - 1 - startDepth - numOfElems, 0);
            int count = Math.Min(numOfElems, this.histElems.Count - 1 - index);

            this.histElems.RemoveRange(index, count);
        }

        /// <summary>
        /// Gets all elements.
        /// </summary>
        /// <returns>The list of elements.</returns>
        public List<T> GetAllElements()
        {
            return GetRange(this.Count);
        }

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            string str = "";
            foreach (var item in histElems)
            {
                str += item.ToString() + ", ";
            }
       
            str = str.Remove(str.Length - 3);
            return str;
        }

        /// <summary>
        /// Clones the history. The data is shared.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            History<T> hist = new History<T>(this.maxNumOfElems);
            hist.histElems.AddRange(this.histElems);
            return hist;
        }

        #region IEnumerable
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        T IEnumerator<T>.Current
        {
            get { return this[idx]; }
        }

        void IDisposable.Dispose()
        { }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return (++idx < this.Count);
        }

        int idx = -1;
        void System.Collections.IEnumerator.Reset()
        {
            idx = -1;
        }
        #endregion
    }
}
