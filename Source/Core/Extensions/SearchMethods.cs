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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides methods for graph search. 
    /// </summary>
    [Obsolete]
    public static class SearchMethods
    {
        class Node<T>
        {
            public const int ROOT_IDX = -1;

            public Node(T data, int parentIdx, int nodeIdx)
            {
                this.Data = data;
                this.ParentIndex = parentIdx;
                this.NodeIndex = nodeIdx;
            }

            public T Data {get; set;}
            public int ParentIndex {get; set;}
            public int NodeIndex { get; set; }
        }

        /// <summary>
        /// Breadth first search.
        /// </summary>
        /// <param name="data">Initial set to search</param>
        /// <param name="src">Start element.</param>
        /// <param name="dest">End element.</param>
        /// <param name="areAdjacent">Function that returns true if two elements are adjacent.</param>
        /// <returns>All possible paths from source to destination node.</returns>
        public static List<List<T>> BreadthFirstSearch<T>(this List<T> data, T src, T dest,
            Func<T, T, bool> areAdjacent)
        {
            var possiblePaths = new List<List<T>>();

            var openedNodes = new List<Node<T>>(); openedNodes.Add(new Node<T>(src, Node<T>.ROOT_IDX, 0));
            var queue = new Queue<Node<T>>(); queue.Enqueue(new Node<T>(src, Node<T>.ROOT_IDX, 0));

            int nodeIdx = 0;
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                if (node.Data.Equals(dest))
                {
                    var possiblePath = backtrace(openedNodes, node.NodeIndex);
                    possiblePaths.Add(possiblePath);
                }

                foreach (var adjacentElement in getAdjacentElements(data, node.Data, areAdjacent))
                {
                    if (openedNodes.Exists(x => x.Data.Equals(adjacentElement)))
                        continue;

                    nodeIdx++;
                    var adjNode = new Node<T>(adjacentElement, node.NodeIndex, nodeIdx);

                    openedNodes.Add(adjNode);
                    queue.Enqueue(adjNode);
                }
            }

            return possiblePaths;
        }

        private static List<T> getAdjacentElements<T>(List<T> data, T elem,
            Func<T, T, bool> areAdjacent)
        {
            var adjNodes = new List<T>();

            foreach (var node in data)
            {
                if (areAdjacent(elem, node))
                {
                    adjNodes.Add(node);
                }
            }

            return adjNodes.Distinct().ToList();
        }

        private static List<T> backtrace<T>(List<Node<T>> openedNodes, int destElementIdx)
        {
            var path = new List<T>();
            var lastNode = openedNodes[destElementIdx];

            while (lastNode.ParentIndex != Node<T>.ROOT_IDX)
            {
                path.Add(lastNode.Data);
                lastNode = openedNodes[lastNode.ParentIndex];
            }

            path.Add(lastNode.Data); //add source node
 
            if (path.Count == 1) //if srcElem ==  endElem let path consist of two elements
                path.Add(openedNodes.First().Data);

            path.Reverse();
            return path;
        }
    }
}
