using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions;

namespace GraphSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            testGraph();
        }

        private static void testGraph()
        {
            TaggedEdge<string, float>[] edges = new TaggedEdge<string, float>[] 
            {
                new TaggedEdge<string, float>("1", "2", 7),
                new TaggedEdge<string, float>("1", "3", 9),
                new TaggedEdge<string, float>("1", "6", 14),

                new TaggedEdge<string, float>("2", "3", 10),
                new TaggedEdge<string, float>("2", "4", 15),

                new TaggedEdge<string, float>("3", "6", 2),
                new TaggedEdge<string, float>("3", "4", 11),

                new TaggedEdge<string, float>("4", "5", 6),

                new TaggedEdge<string, float>("5", "6", 9)
            };

            var graph = edges.ToGraph<string, TaggedEdge<string, float>>();

            Dictionary<string, Dictionary<string, double>> costMatrix;
            var paths = graph.FindAllShorthestPaths(x => x.Tag, out costMatrix);

            var vertices = graph.GetVertices<string, TaggedEdge<string, float>>();

            Console.WriteLine("Possible paths:");
            foreach (var v1 in vertices)
            {
                foreach (var v2 in vertices)
                {
                    var path = paths[v1][v2];
                    var cost = costMatrix.Contains(v1, v2) ? costMatrix[v1][v2]: Double.NaN;

                    Console.Write("{0} => {1}: Cost {2}. Edges:", v1, v2, cost.ToString().PadLeft(3));
                    foreach (var edge in path)
                    {
                        Console.Write("({0} => {1}) ", edge.Source, edge.Destination);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
