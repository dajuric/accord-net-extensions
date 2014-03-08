using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT
{
    public static class MathExtensions
    {
        public static bool IsPowerOfTwo(this ulong x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        public static bool IsPowerOfTwo(this uint x)
        {
            return IsPowerOfTwo((ulong)x);
        }

        public static bool IsPowerOfTwo(this int x)
        {
            if (x < 0) throw new ArgumentException("The number must be greater or equal to zero!");

            return IsPowerOfTwo((ulong)x);
        }
    }

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
        }

        public RegressionTree(int depth)
        {
            int capacity = 1 << (depth + 1); //depth does not take child nodes into account
            treeNodes = new RegressionNodeData<TInternalNodeData>[capacity];
        }

        public float GetOutput(Func<TInternalNodeData, bool> rightNodeSelector)
        {
            var depth = TreeDepth;

            var nodeIdx = 0;
            for (int d = 0; d < depth; d++)
            {
                if (rightNodeSelector(treeNodes[d].Data))
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

        public int TreeDepth 
        {
            get { return treeNodes.GetBinaryTreeDepth() - 1 /*do not count child nodes*/; }
        }
    }

}
