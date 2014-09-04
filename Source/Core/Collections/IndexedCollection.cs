using System;
using System.Collections.Generic;

namespace Accord.Extensions
{
    /// <summary>
    /// Indexed collection class.
    /// Represents the wrapper for the collection and the provided indices.
    /// <para>Can be used if the large collections must be accessed through indices to avoid data copy.</para>
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    public class IndexedCollection<T>: IReadOnlyList<T>
    {
        IList<T> collection;
        IList<int> indices;

        /// <summary>
        /// Creates new indexed collection.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <param name="indices">Indices.</param>
        public IndexedCollection(IList<T> collection, IList<int> indices)
        {
            if (collection.Count < indices.Count)
                throw new Exception("The number of elements within the collection must be greater that the number of elements within indices.");

            this.collection = collection;
            this.indices = indices;
        }

        /// <summary>
        /// Gets the element of the provided collection where the provided index is mapped using provided indices.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>Collection element.</returns>
        public T this[int index]
        {
            get 
            {
                var realIdx = indices[index];
                return collection[realIdx];
            }
        }

        /// <summary>
        /// Gets the number of elements within collection.
        /// </summary>
        public int Count
        {
            get { return indices.Count; }
        }

        /// <summary>
        /// Gets the enumerator for the indexed collection.
        /// </summary>
        /// <returns>Enumerator for the indexed collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new IndexedCollectionEnumerator<T>(this.collection, this.indices);
        }

        /// <summary>
        /// Gets the enumerator for the indexed collection.
        /// </summary>
        /// <returns>Enumerator for the indexed collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Indexed collection enumerator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class IndexedCollectionEnumerator<T> : IEnumerator<T>
        {
            IList<T> collection;
            IList<int> indices;
            int index = -1;

            /// <summary>
            /// Creates new indexed collection enumerator.
            /// </summary>
            /// <param name="collection">Collection.</param>
            /// <param name="indices">Indices.</param>
            public IndexedCollectionEnumerator(IList<T> collection, IList<int> indices)
            {
                if (collection.Count < indices.Count)
                    throw new Exception("The number of elements within the collection must be greater that the number of elements within indices.");

                this.collection = collection;
                this.indices = indices;
            }

            /// <summary>
            /// Gets the current element.
            /// </summary>
            public T Current
            {
                get
                {
                    var realIdx = indices[index];
                    return collection[realIdx];
                }
            }

            /// <summary>
            /// Disposes the indexed collection enumerator instance.
            /// </summary>
            public void Dispose()
            {}

            /// <summary>
            /// Gets the current element.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Moves the index to the next element.
            /// </summary>
            /// <returns>True if the move operation is valid, false otherwise.</returns>
            public bool MoveNext()
            {
                index++;
                return index < indices.Count;
            }

            /// <summary>
            /// Resets the enumerator.
            /// </summary>
            public void Reset()
            {
                index = -1;
            }
        }
    }
}
