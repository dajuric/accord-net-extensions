using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public partial class Test
    {
        public void TestGraph()
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
            var shorthestPaths = graph.FindAllShorthestPaths(x => x.Tag, out costMatrix);
            Console.WriteLine(shorthestPaths);
        }
    }
}
