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
        public static IEnumerable<TVertex> GetVertices<TVertex, TEdge>(this Dictionary<TVertex, Dictionary<TVertex, TEdge>> graph)
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
        public static Dictionary<TVertex, Dictionary<TVertex, TEdge>> ToGraph<TVertex, TEdge>(this IEnumerable<TEdge> edges)
            where TEdge : Edge<TVertex>
        {
            return edges.ToMatrix(x => x.Source, y => y.Destination);
        }

        /// <summary>
        /// Adds edge to the graph.
        /// </summary>
        /// <typeparam name="TVertex">Vertex type.</typeparam>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <param name="graph">Graph.</param>
        /// <param name="edge">Edge to add.</param>
        /// <returns>True if the edge is added, false otherwise.</returns>
        public static bool AddEdge<TVertex, TEdge>(this Dictionary<TVertex, Dictionary<TVertex, TEdge>> graph, TEdge edge)
            where TEdge : Edge<TVertex>
        {
            return graph.Add(edge.Source, edge.Destination, edge);
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
        public static bool RemoveEdge<TVertex, TEdge, TEdgeCollection>(this Dictionary<TVertex, Dictionary<TVertex, TEdge>> graph, TEdge edge)
            where TEdge : Edge<TVertex>
            where TEdgeCollection : ICollection<TEdge>
        {
            return graph.Remove(edge.Source, edge.Destination);
        }
    }
}
