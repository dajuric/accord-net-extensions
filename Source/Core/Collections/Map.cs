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

namespace Accord.Extensions
{
    /// <summary>
    /// Structure that sets association between two keys.
    /// <para>
    /// It is taken from: <a href="http://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c"/> and modified.
    /// </para>
    /// </summary>
    /// <typeparam name="T1">First key type.</typeparam>
    /// <typeparam name="T2">Second key type.</typeparam>
    /// <example>
    /// <code>
    /// var map = new Map &lt;int, string &gt;();
    /// map.Add(42, "Hello");
    /// 
    /// Console.WriteLine(map.Forward[42]);
    /// // Outputs "Hello"
    /// 
    /// Console.WriteLine(map.Reverse["Hello"]);
    /// //Outputs 42
    /// </code>
    /// </example>
    public class Map<T1, T2>
    {
        /// <summary>
        /// Represents all associations associated with a key.
        /// </summary>
        /// <typeparam name="T3">First key type.</typeparam>
        /// <typeparam name="T4">Second key type.</typeparam>
        public class Indexer<T3, T4>: IEnumerable<T3>
        {
            private Dictionary<T3, T4> dictionary;
            /// <summary>
            /// Creates new instance from a dictionary.
            /// </summary>
            /// <param name="dictionary">Association dictionary.</param>
            public Indexer(Dictionary<T3, T4> dictionary)
            {
                this.dictionary = dictionary;
            }

            /// <summary>
            /// Gets the associated key.
            /// </summary>
            /// <param name="index">Key.</param>
            /// <returns></returns>
            public T4 this[T3 index]
            {
                get { return dictionary[index]; }
                set { dictionary[index] = value; }
            }

            /// <summary>
            /// Gets the associated key.
            /// </summary>
            /// <param name="index">Key.</param>
            /// <param name="value">Associated value (other key).</param>
            /// <returns>Returns true if the specified key exists, otherwise returns false.</returns>
            public bool TryGetValue(T3 index, out T4 value)
            {
                return dictionary.TryGetValue(index, out value);
            }

            /// <summary>
            /// Determines whether the specified key exists.
            /// </summary>
            /// <param name="index">Key.</param>
            /// <returns>Returns true if the specified key exists otherwise returns false.</returns>
            public bool Contains(T3 index)
            {
                return dictionary.ContainsKey(index);
            }

            /// <summary>
            /// Return the enumerator that iterates through the collection.
            /// </summary>
            /// <returns>Collection enumerator.</returns>
            public IEnumerator<T3> GetEnumerator()
            {
                return dictionary.Keys.GetEnumerator();
            }

            /// <summary>
            /// Return the enumerator that iterates through the collection.
            /// </summary>
            /// <returns>Collection enumerator.</returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
        
        private Dictionary<T1, T2> forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> reverse = new Dictionary<T2, T1>();

        /// <summary>
        /// Initializes a new instance of <see cref="Accord.Extensions.Map{T1, T2}"/>.
        /// </summary>
        public Map()
        {
            this.Forward = new Indexer<T1, T2>(forward);
            this.Reverse = new Indexer<T2, T1>(reverse);
        }

        /// <summary>
        /// Adds the specified association.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public void Add(T1 t1, T2 t2)
        {
            forward.Add(t1, t2);
            reverse.Add(t2, t1);
        }

        /// <summary>
        /// Gets all associations with the first <typeparamref name="T1"/> key.
        /// </summary>
        public Indexer<T1, T2> Forward { get; private set; }
        /// <summary>
        /// Gets all associations with the second <typeparamref name="T2"/> key.
        /// </summary>
        public Indexer<T2, T1> Reverse { get; private set; }

        /// <summary>
        /// Removes all associations from the <see cref="Accord.Extensions.Map{T1, T2}"/>.
        /// </summary>
        public void Clear()
        {
            forward.Clear();
            reverse.Clear();
        }
    }
}
