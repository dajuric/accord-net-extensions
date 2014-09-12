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
using System.Linq;

namespace Accord.Extensions.BinaryTree
{
    /// <summary>
    /// Contains methods that simulate binary tree using array.
    /// <para>All methods are extension on <see cref="System.Collections.Generic.IList{T}"/></para>
    /// </summary>
    public static class BinaryTreeArrayExtensions
    {
        /// <summary>
        /// Gets leaf node indices.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="collection">Data collection observed as binary tree.</param>
        /// <returns>Node indices.</returns>
        public static IEnumerable<int> LeafIndices<T>(this IList<T> collection)
        {
            var nInternalNodes = (collection.Count - 1) / 2;
            var nLeafs = (collection.Count + 1) / 2;

            return Enumerable.Range(nInternalNodes, nLeafs);
        }

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
