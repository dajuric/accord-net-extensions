using NGenerics.DataStructures.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT
{
    public static class BinaryTreeArrayExtstensions
    {
        public static int LeftChildIndex<T>(this IList<T> collection, int parentIndex)
        {
            return parentIndex * 2 + 1;
        }

        public static int RightChildIndex<T>(this IList<T> collection, int parentIndex)
        {
            return parentIndex * 2 + 2;
        }

        public static bool GetLeftChild<T>(this IList<T> collection, int parentIdx, out T node)
        {
            var idx = LeftChildIndex(collection, parentIdx);

            if (idx >= collection.Count)
            {
                node = default(T);
                return false;
            }

            node = collection[idx];
            return true;
        }

        public static bool GetRightChild<T>(this IList<T> collection, int parentIdx, out T node)
        {
            var idx = RightChildIndex(collection, parentIdx);

            if (idx >= collection.Count)
            {
                node = default(T);
                return false;
            }

            node = collection[idx];
            return true;
        }

        public static bool AddLeftChild<T>(this IList<T> collection, int parentIdx, T child)
        {
            var idx = LeftChildIndex(collection, parentIdx);

            if (idx >= collection.Count)
                return false;

            collection[idx] = child;
            return true;
        }

        public static bool AddRightChild<T>(this IList<T> collection, int parentIdx, T child)
        {
            var idx = RightChildIndex(collection, parentIdx);

            if (idx >= collection.Count)
                return false;

            collection[idx] = child;
            return true;
        }

        public static int GetBinaryTreeDepth<T>(this IList<T> collection)
        {
            int depth = 0;
            var nElements = collection.Count + 1; //binary tree has 2^d - 1 element => increase to 2^d

            while ((nElements >>= 1) != 0) ++depth; // (int)log2(nElements)
            return depth; 
        }
    }
}
