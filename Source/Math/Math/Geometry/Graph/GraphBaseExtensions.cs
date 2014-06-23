using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Math.Geometry
{
    public static class GraphBaseExtensions
    {
        public static IEnumerable<TVertex> GetVertices<TVertex, TEdge>(this Dictionary<TVertex, Dictionary<TVertex, TEdge>> graph)
           where TEdge : Edge<TVertex>
        {
            return graph.GetKeys<TVertex, TEdge>();
        }

        public static IEnumerable<TVertex> GetVertices<TVertex, TEdge>(this IEnumerable<TEdge> edges)
           where TEdge : Edge<TVertex>
        {
            return edges.SelectMany(x => new TVertex[] { x.Source, x.Destination }).Distinct();
        }

        public static Dictionary<TVertex, Dictionary<TVertex, TEdge>> ToGraph<TVertex, TEdge>(this IEnumerable<TEdge> edges)
            where TEdge : Edge<TVertex>
        {
            return edges.ToMatrix(x => x.Source, y => y.Destination);
        }

        public static bool AddEdge<TVertex, TEdge>(this Dictionary<TVertex, Dictionary<TVertex, TEdge>> graph, TEdge edge)
            where TEdge : Edge<TVertex>
        {
            return graph.Add(edge.Source, edge.Destination, edge);
        }

        public static bool RemoveEdge<TVertex, TEdge, TEdgeCollection>(this Dictionary<TVertex, Dictionary<TVertex, TEdge>> graph, TEdge edge)
            where TEdge : Edge<TVertex>
            where TEdgeCollection : ICollection<TEdge>
        {
            return graph.Remove(edge.Source, edge.Destination);
        }
    }
}
