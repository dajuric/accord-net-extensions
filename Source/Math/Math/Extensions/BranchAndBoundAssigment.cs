using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Math
{
    /// <summary>
    /// Contain extension methods for the Branch and Bound assignment matching algorithm.
    /// <para>See: <a href="https://www.youtube.com/watch?v=WsHgMfgNsHI">Assignment Problem using Branch and Bound</a></para>
    /// <para>See: <a href="https://www.youtube.com/watch?v=F4vI_qc_u0Q">Branch and Bound - An Assignment Problem</a></para>
    /// <para>See: <a href="http://homepages.ius.edu/RWISMAN/C455/html/notes/Backtracking/BranchandBound.htm">Branch and Bound</a></para>
    /// </summary>
    public static class BranchAndBoundAssigmentExtensions
    {
        /// <summary>
        /// Branching node used internally.
        /// </summary>
        private class BranchingNode
        {
            public int Row;
            public int Column;
            
            public double Bound;

            public BranchingNode Parent;

            public bool[] GetTakenColumns(int numberOfColumns)
            {
                var isColumnTaken = new bool[numberOfColumns];

                var node = this;
                while (node.IsRoot == false)
                {
                    isColumnTaken[node.Column] = true;
                    node = node.Parent;
                }

                return isColumnTaken;
            }

            public IEnumerable<BranchingNode> Backtrace()
            {
                var node = this;
                while (node.Parent != null) //do no return the root
                {
                    yield return node;
                    node = node.Parent;
                }
            }

            public double GetPathCost(double[,] costMatrix)
            {
                double cost = 0;
                foreach (var node in this.Backtrace())
                {
                    cost += costMatrix[node.Row, node.Column];
                }

                return cost;
            }

            public bool IsRoot 
            {
                get { return this.Parent == null; }
            }
        }

        /// <summary>
        /// Gets the first optimal assignment matching where the provided cost matrix represents the cost between each (row, column) pair.
        /// <para>(Row, column) pairs represent the bipartite graph.</para>
        /// </summary>
        /// <param name="costs">Cost matrix for each (row, column) pair where infinity costs represent disconnected vertexes.</param>
        /// <returns>
        /// Assignment matching matrix where true elements represent the pairing.
        /// <para>If a row or a column does not contain true value, the corresponding vertex (row/column) is not matched with any other vertex (column/row).</para>
        /// </returns>
        public static bool[,] MatchAssigments(this double[,] costs)
        {
            var terminalNode = matchAssigments(costs);

            var associationMatrix = new bool[costs.GetLength(0), costs.GetLength(1)];

            if (terminalNode != null) //if there are associations
            {
                foreach (var node in terminalNode.Backtrace())
                {
                    associationMatrix[node.Row, node.Column] = true;
                }
            }
           
            return associationMatrix;
        }

        /// <summary>
        /// Gets the optimal assignment matching where the provided cost matrix represents the cost between each (row, column) pair.
        /// <para>(Row, column) pairs represent the bipartite graph.</para>
        /// </summary>
        /// <param name="costs">Cost matrix for each (row, column) pair where infinity costs represent disconnected vertexes.</param>
        /// <returns>The first best solution (terminal node that needs to be backtracked).</returns>
        private static BranchingNode matchAssigments(this double[,] costs)
        {         
           var nodes = new Stack<BranchingNode>();
           var terminalNodes = new List<BranchingNode>();

            var root = createRoot(costs);
            nodes.Push(root);

            while (nodes.Any())
            {
                var node = nodes.Pop();
                var candidates = branchAndSelectMinCostNodes(costs, node);

                foreach (var candidate in candidates)
                {
                    nodes.Push(candidate);
                }

                if (candidates.Count == 0) //if the node is a terminal node
                {
                    terminalNodes.Add(node);
                    continue;
                }
            }

            if (terminalNodes.Count > 1)
                throw new Exception("Multiple solutions have been found ??");

            return terminalNodes.FirstOrDefault();
        }

        /// <summary>
        /// Creates immediate child nodes for the specified node.
        /// </summary>
        /// <param name="costs">Cost matrix.</param>
        /// <param name="node">Node that needs to be branched.</param>
        /// <returns>List of feasible immediate child nodes.</returns>
        private static List<BranchingNode> branchAndSelectMinCostNodes(double[,] costs, BranchingNode node)
        {
            const double EPSILON = 1e-3;

            var immediateChildren = new List<BranchingNode>();
            var minChildCost = Double.MaxValue;

            //make children and the min cost
            for (int c = 0; c < costs.ColumnCount(); c++)
            {
                var child = makeNode(costs, node, c);
                if (child == null) continue;

                immediateChildren.Add(child);
                minChildCost = System.Math.Min(minChildCost, child.Bound);            
            }

            //add children which have the min cost
            var candidates = new List<BranchingNode>();
            foreach (var child in immediateChildren)
            {
                if (System.Math.Abs(child.Bound - minChildCost) < EPSILON)
                    candidates.Add(child);
            }

            return candidates;
        }

        /// <summary>
        /// Makes node by using the specified parent node and the child index (column in the cost matrix).
        /// </summary>
        /// <param name="costs">Cost matrix.</param>
        /// <param name="parent">Parent node.</param>
        /// <param name="column">Child index (column in cost matrix).</param>
        /// <returns>Node.</returns>
        static BranchingNode makeNode(double[,] costs, BranchingNode parent, int column)
        {
            var node = new BranchingNode { Row = parent.Row + 1, Column = column, Bound = parent.GetPathCost(costs), Parent = parent };
            var isColumnTaken = parent.GetTakenColumns(costs.ColumnCount());

            if (node.Row == costs.RowCount() || isColumnTaken[column])
                return null; //invalid node

            node.Bound += costs[node.Row, node.Column];
            isColumnTaken[column] = true;

            for (int r = node.Row + 1; r < costs.RowCount(); r++)
            {
                var min = findMinInRowOmmitingTaken(costs, r, isColumnTaken);
                node.Bound += min;
            }

            return node;
        }

        /// <summary>
        /// Finds the minimum number inside the cost matrix committing columns that are already taken.
        /// </summary>
        /// <param name="costs">Cost matrix.</param>
        /// <param name="row">The row in the cost matrix.</param>
        /// <param name="isColumnTaken">The binary mask which tells which columns are taken (used by other nodes).</param>
        /// <returns>Minimum number.</returns>
        static double findMinInRowOmmitingTaken(double[,] costs, int row, bool[] isColumnTaken)
        {
            //find minimum in the specified row
            double min = Double.MaxValue;
            for (int c = 0; c < costs.GetLength(1); c++)
            {
                var val = costs[row, c];
                if (Double.IsInfinity(val) || isColumnTaken[c]) //omit the disconnected vertexes and the fixed ones
                    continue;

                min = System.Math.Min(min, val);
            }

            return min;
        }

        /// <summary>
        /// Creates root node.
        /// </summary>
        /// <param name="costs">Cost matrix.</param>
        /// <returns>Root node.</returns>
        private static BranchingNode createRoot(double[,] costs)
        {
            var root = new BranchingNode { Row = -1, Column = -1, Bound = 0, Parent = null };

            for (int r = 0; r < costs.GetLength(0); r++)
            {
                var rowMin = Double.MaxValue;
                for (int c = 0; c < costs.GetLength(1); c++)
                {
                    rowMin = System.Math.Min(rowMin, costs[r, c]);
                }

                root.Bound += rowMin;
            }

            return root;
        }

    }
}
