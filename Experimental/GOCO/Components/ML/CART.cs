#define LOG

using Accord.Extensions;
using Accord.Extensions.BinaryTree;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math;
using Accord.Extensions.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GOCO
{
    [Serializable]
    public class CART<TStump, TOutput>
    {
        [Serializable]
        public class Node
        {
            public Node(TStump internalData)
            {
                this.Data = internalData;
                this.Output = default(TOutput);
                this.IsChild = false;
            }

            public Node(TOutput output)
            {
                this.Data = default(TStump);
                this.Output = output;
                this.IsChild = true;
            }

            public TStump Data { get; private set; }
            public TOutput Output { get; private set; }
            public bool IsChild { get; private set; }
        }

        private Node[] treeNodes;

        public CART(IList<TStump> internalNodes, IList<TOutput> leafOutputs)
        {
            if ((internalNodes.Count + 1).IsPowerOfTwo() == false ||
               leafOutputs.Count.IsPowerOfTwo() == false ||
               (internalNodes.Count + 1) != leafOutputs.Count)
            {
                throw new Exception("Incorrect number of node internal nodes or leafs. Cannot create full binary tree!");
            }

            var treeNodes = new List<Node>();

            foreach (var internalNode in internalNodes)
            {
                treeNodes.Add(new Node(internalNode));
            }

            foreach (var  leafOutput in leafOutputs)
            {
                treeNodes.Add(new Node(leafOutput));
            }

            this.treeNodes = treeNodes.ToArray();
            this.TreeDepth = treeNodes.GetBinaryTreeDepth() - 1; //do not count child nodes
        }

        /// <summary>
        /// Creates new classification and regression tree.
        /// </summary>
        /// <param name="depth">The depth of the regression tree. Does not take leaf nodes into account.</param>
        public CART(int depth)
        {
            if (depth < 0 || depth > 16)
                throw new ArgumentOutOfRangeException("Depth range must be between 0 and 16.");

            int nNodes = (1 << (depth + 1)) - 1; //depth does not take child nodes into account
            treeNodes = new Node[nNodes];

            TreeDepth = depth;
        }

        public TOutput Evaluate(Func<TStump, bool> rightNodeSelector)
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

            return treeNodes[nodeIdx].Output;
        }

        public IList<Node> Nodes { get { return treeNodes; } } //revert to IReadOnlyCollection when Accord.NET resolves the bug
       
        public int TreeDepth { get; private set; }

        public void Train(int numberOfSamples, Func<int[], double[], Tuple<SplitInfo, TStump>> splitFunc,
                          Func<double[], TOutput> outputConverter,
                          double[] sampleWeights = null)      
        {
            var sampleIndices = Enumerable.Range(0, numberOfSamples).ToArray();
            sampleWeights = sampleWeights ?? EnumerableExtensions.Create(numberOfSamples, _ => (double)1);

            growSubTree(splitFunc,
                        outputConverter,
                        sampleWeights,
                        0, sampleIndices, 0, null);
        }

#if LOG
        int nProcessedNodes = 0;
#endif

        private void growSubTree(Func<int[], double[], Tuple<SplitInfo, TStump>> splitFunc, 
                                 Func<double[], TOutput> outputConverter,
                                 double[] sampleWeights,
                                 int depth, int[] indices, int nodeIndex, double[] weightedAverage)
        {
#if LOG
            nProcessedNodes++;
            Console.Write("\r\t\tRegression tree: training node {0} / {1}", nProcessedNodes, this.treeNodes.Length);

            if (nProcessedNodes == this.treeNodes.Length)
                Console.WriteLine();
#endif
            if (depth == this.TreeDepth) //compute output: weighted average
            {
                var outputVal = outputConverter(weightedAverage);       
                treeNodes[nodeIndex] = new Node(outputVal);
                return;
            }
            else if (indices.Length <= 1) //if the data is already split but the maximum depth is not reached propagate to the tree depth (it happens rarely if enough samples are taken)
            {
                var parentIdx = treeNodes.ParentIndex(nodeIndex);
                treeNodes[nodeIndex] = new Node(treeNodes[parentIdx].Data);

                growSubTree(splitFunc, outputConverter, sampleWeights, depth + 1,
                            indices,
                            treeNodes.LeftChildIndex(nodeIndex),
                            weightedAverage);

                growSubTree(splitFunc, outputConverter, sampleWeights, depth + 1,
                            indices,
                            treeNodes.RightChildIndex(nodeIndex),
                            weightedAverage);
                return;
            }
        
            //else //find error and splitting index => go recursive

            Tuple<SplitInfo, TStump> splitData = splitFunc(indices, sampleWeights);
            var splitInfo = splitData.Item1;
            treeNodes[nodeIndex] = new Node(splitData.Item2);

            //recursive calls for left and right branch
            growSubTree(splitFunc, outputConverter, sampleWeights, depth + 1,
                        splitInfo.LeftIndices,
                        treeNodes.LeftChildIndex(nodeIndex),
                        splitInfo.LeftAverage);

            growSubTree(splitFunc, outputConverter, sampleWeights, depth + 1,
                        splitInfo.RightIndices,
                        treeNodes.RightChildIndex(nodeIndex),
                        splitInfo.RightAverage);
        }
    }
}
