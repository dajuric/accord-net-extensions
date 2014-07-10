using System.Collections.Generic;

namespace Accord.Extensions.BinaryTree
{
    /// <summary>
    /// Contains methods that simulate binary tree using array.
    /// <para>All methods are extension on <see cref="System.Collections.Generic.IList{T}"/></para>
    /// </summary>
    public static class BinaryTreeArrayExtstensions
    {
        /// <summary>
        /// Returns parent index determined by its child <paramref name="nodeIndex"/>.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="collection">Data collection observed as binary tree.</param>
        /// <param name="nodeIndex">Left or right child index.</param>
        /// <returns>Parent index.</returns>
        public static int ParentIndex<T>(this IList<T> collection, int nodeIndex)
        {
            return (nodeIndex - 1) / 2;
        }

        /// <summary>
        /// Returns child index determined by its <paramref name="parentIndex"/>.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="collection">Data collection observed as binary tree.</param>
        /// <param name="parentIndex">Parent index.</param>
        /// <returns>Child index.</returns>
        public static int LeftChildIndex<T>(this IList<T> collection, int parentIndex)
        {
            return parentIndex * 2 + 1;
        }

        /// <summary>
        /// Returns child index determined by its <paramref name="parentIndex"/>.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="collection">Data collection observed as binary tree.</param>
        /// <param name="parentIndex">Parent index.</param>
        /// <returns>Child index.</returns>
        public static int RightChildIndex<T>(this IList<T> collection, int parentIndex)
        {
            return parentIndex * 2 + 2;
        }

        /// <summary>
        /// Gets the node determined by its <paramref name="parentIndex"/>.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="collection">Data collection observed as binary tree.</param>
        /// <param name="parentIndex">Parent index.</param>
        /// <param name="node">Child value.</param>
        /// <returns>True if the child index is in collection bounds, false otherwise.</returns>
        public static bool GetLeftChild<T>(this IList<T> collection, int parentIndex, out T node)
        {
            var idx = LeftChildIndex(collection, parentIndex);

            if (idx >= collection.Count)
            {
                node = default(T);
                return false;
            }

            node = collection[idx];
            return true;
        }

        /// <summary>
        /// Gets the node determined by its <paramref name="parentIndex"/>.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="collection">Data collection observed as binary tree.</param>
        /// <param name="parentIndex">Parent index.</param>
        /// <param name="node">Child value.</param>
        /// <returns>True if the child index is in collection bounds, false otherwise.</returns>
        public static bool GetRightChild<T>(this IList<T> collection, int parentIndex, out T node)
        {
            var idx = RightChildIndex(collection, parentIndex);

            if (idx >= collection.Count)
            {
                node = default(T);
                return false;
            }

            node = collection[idx];
            return true;
        }

        /// <summary>
        /// Replaces child element given by the <paramref name="parentIndex"/>.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="collection">Data collection observed as binary tree.</param>
        /// <param name="parentIndex">Parent index.</param>
        /// <param name="child">New child value.</param>
        /// <returns>True if the child index is in collection bounds - child can not be replaced, false otherwise.</returns>
        public static bool ReplaceLeftChild<T>(this IList<T> collection, int parentIndex, T child)
        {
            var idx = LeftChildIndex(collection, parentIndex);

            if (idx >= collection.Count)
                return false;

            collection[idx] = child;
            return true;
        }

        /// <summary>
        /// Replaces child element given by the <paramref name="parentIndex"/>.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="collection">Data collection observed as binary tree.</param>
        /// <param name="parentIndex">Parent index.</param>
        /// <param name="child">New child value.</param>
        /// <returns>True if the child index is in collection bounds - child can not be replaced, false otherwise.</returns>
        public static bool ReplaceRightChild<T>(this IList<T> collection, int parentIndex, T child)
        {
            var idx = RightChildIndex(collection, parentIndex);

            if (idx >= collection.Count)
                return false;

            collection[idx] = child;
            return true;
        }

        /// <summary>
        /// Gets depth of the binary tree.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="collection">Data collection observed as binary tree.</param>
        /// <returns>Depth of the binary tree.</returns>
        public static int GetBinaryTreeDepth<T>(this IList<T> collection)
        {
            int depth = 0;
            var nElements = collection.Count + 1; //binary tree has 2^d - 1 element => increase to 2^d

            while ((nElements >>= 1) != 0) ++depth; // (int)log2(nElements)
            return depth; 
        }
    }
}
