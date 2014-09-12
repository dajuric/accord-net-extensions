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
using MoreLinq;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Implements Floyd-Warshall algorithm as an extension function.
    /// </summary>
    public static class FloydWarshallExtensions
    {
        /// <summary>
        /// Finds all shortest paths for-each vertex pair withing the given graph by using Floyd-Warshall algorithm.
        /// See <a href="http://en.wikipedia.org/wiki/Floyd%E2%80%93Warshall_algorithm" /> for details.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph.</param>
        /// <param name="distanceFunc">Distance function between two vertices.</param>
        /// <param name="costMat">Cost matrix.</param>
        /// <returns>2D matrix where each element is path from a source to a destination.</returns>
        public static IDictionary<Pair<TVertex>, List<TEdge>> FindShortestPaths<TVertex, TEdge>(this IDictionary<Pair<TVertex>, TEdge> graph,
                                                                                           Func<TEdge, double> distanceFunc,
                                                                                           out IDictionary<Pair<TVertex>, double> costMat)
            where TEdge : Edge<TVertex>
        {
            var edges = graph.AsEnumerable();
            var vertices = edges.GetVertices<TVertex, TEdge>(); //graph.Keys; => does not pick destination vertices (some vertices may only exist as destinations)

            var costMatrix = edges.ToSparseMatrix(x => x.Source, y => y.Destination, value => distanceFunc(value));
            vertices.ForEach(x => costMatrix.AddOrUpdate(x, x, 0)); //add 0 path cost

            var nextVertexMatrix = edges.ToSparseMatrix(x => x.Source, y => y.Destination, value => value.Source);

            foreach (var kV in vertices)
            {
                foreach (var iV in vertices)
                {
                    foreach (var jV in vertices)
                    {
                        if (!costMatrix.ContainsKey(iV, kV) || !costMatrix.ContainsKey(kV, jV))
                            continue;

                        var newDist = costMatrix.Get(iV,kV) + costMatrix.Get(kV, jV);

                        if (!costMatrix.ContainsKey(iV, jV) || newDist < costMatrix.Get(iV, jV))
                        {
                            costMatrix.AddOrUpdate(iV, jV, newDist);
                            nextVertexMatrix.AddOrUpdate(iV, jV, nextVertexMatrix.Get(kV, jV));
                        }
                    }
                }
            }

            /*************************** path reconstruction *************************/
            var pathMatrix = edges.ToSparseMatrix(x => x.Source, y => y.Destination, value => new List<TEdge>());

            foreach (var iV in vertices)
            {
                foreach (var jV in vertices)
                {
                    var path = getPath(graph, nextVertexMatrix, iV, jV);
                    pathMatrix.AddOrUpdate(iV, jV, path);
                }
            }

            costMat = costMatrix;
            return pathMatrix;
        }

        private static List<TEdge> getPath<TVertex, TEdge>(IDictionary<Pair<TVertex>, TEdge> graph,
                                                           IDictionary<Pair<TVertex>, TVertex> nextVertexMatrix,
                                                           TVertex source, TVertex destination)
        {
            if (!nextVertexMatrix.ContainsKey(source, destination))
                return new List<TEdge>();

            var intermediate = nextVertexMatrix.Get(source, destination);
            if (intermediate.Equals(source))
            {
                return new List<TEdge>() { graph.Get(source, destination) };
            }
            else
            {
                var firstHalf = getPath(graph, nextVertexMatrix, source, intermediate);
                var secondHalf = getPath(graph, nextVertexMatrix, intermediate, destination);

                return firstHalf.Union(secondHalf).ToList();
            }
        }
    }
}
