#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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
            //see graph.png
            Edge<string, float>[] edges = new Edge<string, float>[] 
            {
                new Edge<string, float>("1", "2", 7),
                new Edge<string, float>("1", "3", 9),
                new Edge<string, float>("1", "6", 14),

                new Edge<string, float>("2", "3", 10),
                new Edge<string, float>("2", "4", 15),

                new Edge<string, float>("3", "6", 2),
                new Edge<string, float>("3", "4", 11),

                new Edge<string, float>("4", "5", 6),

                new Edge<string, float>("5", "6", 9)
            };

            var graph = edges.ToGraph<string, Edge<string, float>>();

            IDictionary<Pair<string>, double> costMatrix;
            var paths = graph.FindShortestPaths(x => x.Tag, out costMatrix);

            var vertices = graph.GetVertices<string, Edge<string, float>>();

            Console.WriteLine("Possible paths:");
            foreach (var v1 in vertices)
            {
                foreach (var v2 in vertices)
                {
                    var path = paths.Get(v1, v2);
                    var cost = costMatrix.ContainsKey(v1, v2) ? costMatrix.Get(v1, v2): Double.NaN;

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
