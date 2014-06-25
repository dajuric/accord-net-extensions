using System.Collections.Generic;

namespace Accord.Extensions
{
    /// <summary>
    /// Structure that sets association between two keys.
    /// It is taken from: <see cref="http://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c"/> and modified.
    /// </summary>
    /// <typeparam name="T1">First key type.</typeparam>
    /// <typeparam name="T2">Second key type.</typeparam>
    /// <example>
    /// <code>
    /// var map = new Map<int, string>();
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
            /// Creates new instance from a disctionary.
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
            /// <returns>Returns true if the specifed key exists, otherwise returns false.</returns>
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

            public IEnumerator<T3> GetEnumerator()
            {
                return dictionary.Keys.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
        
        private Dictionary<T1, T2> forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> reverse = new Dictionary<T2, T1>();

        /// <summary>
        /// Initializes a new instance of <see cref="Map"/>.
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
        /// Gets all associations with the first <see cref="T1"/> key.
        /// </summary>
        public Indexer<T1, T2> Forward { get; private set; }
        /// <summary>
        /// Gets all associations with the second <see cref="T2"/> key.
        /// </summary>
        public Indexer<T2, T1> Reverse { get; private set; }

        /// <summary>
        /// Removes all associations from the <see cref="Map"/>.
        /// </summary>
        public void Clear()
        {
            forward.Clear();
            reverse.Clear();
        }
    }
}
