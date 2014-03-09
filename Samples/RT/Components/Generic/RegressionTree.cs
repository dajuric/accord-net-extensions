using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions.Math;

namespace RT
{
    public class RegressionNodeData<TInternalData>
    {
        public TInternalData Data { get; set; }
        public float? OutputValue { get; set; }
    }

    public class RegressionTree<TInternalNodeData>
    {
        private RegressionNodeData<TInternalNodeData>[] treeNodes;

        public RegressionTree(IList<TInternalNodeData> internalNodes, IList<float> leafOutputs)
        {
            if ((internalNodes.Count + 1).IsPowerOfTwo() == false ||
               leafOutputs.Count.IsPowerOfTwo() == false ||
               (internalNodes.Count + 1) != leafOutputs.Count)
            {
                throw new Exception("Incorrect number of node internal nodes or leafs. Cannot create full binary tree!");
            }

            var treeNodes = new List<RegressionNodeData<TInternalNodeData>>();

            foreach (var internalNode in internalNodes)
            { 
                treeNodes.Add(new RegressionNodeData<TInternalNodeData> { Data = internalNode });
            }

            foreach (var  leafOutput in leafOutputs)
            {
                treeNodes.Add(new RegressionNodeData<TInternalNodeData> { OutputValue = leafOutput });
            }

            this.treeNodes = treeNodes.ToArray();
            this.TreeDepth = treeNodes.GetBinaryTreeDepth() - 1 /*do not count child nodes*/; 
        }

        public RegressionTree(int depth)
        {
            int capacity = 1 << (depth + 1); //depth does not take child nodes into account
            treeNodes = new RegressionNodeData<TInternalNodeData>[capacity];
        }

        public float GetOutput(Func<TInternalNodeData, bool> rightNodeSelector)
        {
            var nodeIdx = 0;
            for (int d = 0; d < TreeDepth; d++)
            {
                if (rightNodeSelector(treeNodes[nodeIdx].Data))
                {
                    nodeIdx = treeNodes.RightChildIndex(nodeIdx);
                }
                else 
                {
                    nodeIdx = treeNodes.LeftChildIndex(nodeIdx);
                }
            }

            return treeNodes[nodeIdx].OutputValue.Value;
        }

        /// <summary>
        /// Gets the depth of the tree. 
        /// The returned value does not take leafs into account.
        /// </summary>
        public int TreeDepth
        {
            get;
            private set;
        }
    }

}
