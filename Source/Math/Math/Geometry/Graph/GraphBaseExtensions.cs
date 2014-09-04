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

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Graph extension (operates on sparse matrix).
    /// <para>All methods can be used as extensions.</para>
    /// </summary>
    public static class GraphBaseExtensions
    {
        /// <summary>
        /// Gets all vertices from the graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph.</param>
        /// <returns>Collection of vertices.</returns>
        public static IEnumerable<TVertex> GetVertices<TVertex, TEdge>(this IDictionary<Pair<TVertex>, TEdge> graph)
           where TEdge : Edge<TVertex>
        {
            return graph.GetKeys<TVertex, TEdge>();
        }

        /// <summary>
        /// Gets all vertices from the graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="edges">Collection of edges.</param>
        /// <returns>Collection of vertices.</returns>
        public static IEnumerable<TVertex> GetVertices<TVertex, TEdge>(this IEnumerable<TEdge> edges)
           where TEdge : Edge<TVertex>
        {
            return edges.SelectMany(x => new TVertex[] { x.Source, x.Destination }).Distinct();
        }

        /// <summary>
        /// Constructs the graph from the provided edges.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="edges">Collection of edges.</param>
        /// <returns>Graph.</returns>
        public static IDictionary<Pair<TVertex>, TEdge> ToGraph<TVertex, TEdge>(this IEnumerable<TEdge> edges)
            where TEdge : Edge<TVertex>
        {
            return edges.ToSparseMatrix(x => x.Source, y => y.Destination);
        }

        /// <summary>
        /// Adds edge to the graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph.</param>
        /// <param name="edge">Edge to add.</param>
        public static void AddOrUpdateEdge<TVertex, TEdge>(this IDictionary<Pair<TVertex>, TEdge> graph, TEdge edge)
            where TEdge : Edge<TVertex>
        {
            graph.AddOrUpdate(edge.Source, edge.Destination, edge);
        }

        /// <summary>
        /// Removes edge from the graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <typeparam name="TEdgeCollection">Edge collection type.</typeparam>
        /// <param name="graph">Graph.</param>
        /// <param name="edge">Edge to remove.</param>
        /// <returns>True if the edge is removed, false otherwise.</returns>
        public static bool RemoveEdge<TVertex, TEdge, TEdgeCollection>(this IDictionary<Pair<TVertex>, TEdge> graph, TEdge edge)
            where TEdge : Edge<TVertex>
            where TEdgeCollection : ICollection<TEdge>
        {
            return graph.Remove(edge.Source, edge.Destination);
        }
    }
}
