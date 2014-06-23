#define LOG

using Accord.Extensions;
using Accord.Extensions.BinaryTree;
using Accord.Extensions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RT
{
    /// <summary>
    /// Regression tree.
    /// See:
    /// <see cref="http://en.wikipedia.org/wiki/Decision_tree_learning"/>,
    /// <see cref="http://www.stat.cmu.edu/~cshalizi/350-2006/lecture-10.pdf"/>,
    /// <see cref="http://www.stat.cmu.edu/~cshalizi/350/lectures/22/lecture-22.pdf"/>,
    /// <see cref="http://www.mathworks.com/help/stats/regressiontreeclass.html"/>,
    /// <see cref="http://www.youtube.com/watch?v=zvUOpbgtW3c"/>
    /// for details.
    /// </summary>
    /// <typeparam name="TFeature">Internal node data type.</typeparam>
    public class RegressionTree<TFeature>
    {
        /// <summary>
        /// Regression tree data container.
        /// </summary>
        /// <typeparam name="TInternalNodeData">Internal node data type.</typeparam>
        class RegressionNodeData<TInternalNodeData>
        {
            /// <summary>
            /// Gets or sets internal node data.
            /// <para>Leafs do not have internal data.</para>
            /// </summary>
            public TInternalNodeData Data { get; set; }
            /// <summary>
            /// Gets or sets leaf output data. Default is null.
            /// <para>Internal nodes do not have output value.</para>
            /// </summary>
            public float? OutputValue { get; set; }
        }

        private RegressionNodeData<TFeature>[] treeNodes;

        /// <summary>
        /// Creates new regression tree from existing data.
        /// </summary>
        /// <param name="internalNodes">Internal nodes. The number of internal nodes should be 2^d - 1, where d is tree depth (without leafs).</param>
        /// <param name="leafOutputs">Leaf nodes. The number of internal nodes should be 2^d, where d is tree depth (without leafs).</param>
        public RegressionTree(IList<TFeature> internalNodes, IList<float> leafOutputs)
        {
            if ((internalNodes.Count + 1).IsPowerOfTwo() == false ||
               leafOutputs.Count.IsPowerOfTwo() == false ||
               (internalNodes.Count + 1) != leafOutputs.Count)
            {
                throw new Exception("Incorrect number of node internal nodes or leafs. Cannot create full binary tree!");
            }

            var treeNodes = new List<RegressionNodeData<TFeature>>();

            foreach (var internalNode in internalNodes)
            { 
                treeNodes.Add(new RegressionNodeData<TFeature> { Data = internalNode });
            }

            foreach (var  leafOutput in leafOutputs)
            {
                treeNodes.Add(new RegressionNodeData<TFeature> { OutputValue = leafOutput });
            }

            this.treeNodes = treeNodes.ToArray();
            this.TreeDepth = treeNodes.GetBinaryTreeDepth() - 1 /*do not count child nodes*/; 
        }

        /// <summary>
        /// Creates new regression tree.
        /// </summary>
        /// <param name="depth">The depth of the regression tree. Does not take leaf nodes into account.</param>
        public RegressionTree(int depth)
        {
            int nNodes = (1 << (depth + 1)) - 1; //depth does not take child nodes into account
            //treeNodes = EnumerableExtensions.Create(nNodes, (_) => new RegressionNodeData<TFeature>()).ToArray();
            treeNodes = new RegressionNodeData<TFeature>[nNodes];

            TreeDepth = depth;
        }

        /// <summary>
        /// Gets the regression tree node output (does regression).
        /// </summary>
        /// <param name="rightNodeSelector">
        /// Function that returns true to some user specified criteria, false otherwise.
        /// <para>If the function returns true a right node in the tree will be selected, left node otherwise.</para>
        /// </param>
        /// <returns>Function value approximation. In the case of classification returned value can be interpreted as confidence.</returns>
        public float GetOutput(Func<TFeature, bool> rightNodeSelector)
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

        /// <summary>
        /// Trains (grows) the regression tree.
        /// </summary>
        /// <param name="nodeFeatureProvider">Function that provides features for each node. 
        /// <para>It can be constant, but some algorithms provide different set of features for each node.</para>
        /// </param>
        /// <param name="rightNodeSelector">
        /// Function that returns true to some user specified criteria, false otherwise.
        /// <para>Parameters: tree internal node, index of the requested sample.</para>
        /// <para>If the function returns true a right node in the tree will be selected, left node otherwise.</para>
        /// </param>
        /// <param name="targetValues">Target function values that should be approximated.</param>
        /// <param name="sampleWeights">Weight for each sample. If using some kind of boosting use this parameter to provide sample weights.</param>
        public void Train(Func<TFeature[]> nodeFeatureProvider, Func<TFeature, int, bool> rightNodeSelector, float[] targetValues, float[] sampleWeights = null)
        {
            sampleWeights = sampleWeights ?? EnumerableExtensions.Create<float>(targetValues.Length, (_) => 1); //or (float)1 / targetValues.Length ? TODO: check

            growTree(sampleWeights, targetValues, nodeFeatureProvider, rightNodeSelector);
        }

        /// <summary>
        /// Function for regression tree training.
        /// </summary>
        /// <param name="sampleWeights">Indices of the input samples. (enumerated array - 0, 1, 2, 3, ... nSamples).</param>
        /// <param name="targetValues">The values that need to be approximated (all values - only indices are changed).</param>
        /// <param name="nodeFeatureProvider">Provider for features for a node. In most cases it is the constant, but can depend on every node (e.g. random features for every node).</param>
        /// <param name="rightNodeSelector">
        /// Function that represents the classifier. 
        /// Parameters: feature, sample index. 
        /// Returns: True if the right node should be selected, false otherwise.
        /// </param>
        private void growTree(float[] sampleWeights, float[] targetValues, Func<TFeature[]> nodeFeatureProvider, Func<TFeature, int, bool> rightNodeSelector)
        {
            var sampleIndices = Enumerable.Range(0, sampleWeights.Length).ToArray();
            growSubTree(sampleIndices, 0, 0, sampleWeights, targetValues, nodeFeatureProvider, rightNodeSelector);
        }

#if LOG
        int nProcessedNodes = 0; 
#endif

        /// <summary>
        /// Recursive function for regression tree training.
        /// </summary>
        /// <param name="indices">Sample indices.</param>
        /// <param name="nodeIndex">A node index (container for nodes is array).</param>
        /// <param name="depth">The current depth (first call: 0).</param>
        /// <param name="sampleWeights">Indices of the input samples. (enumerated array - 0, 1, 2, 3, ... nSamples).</param>
        /// <param name="targetValues">The values that need to be approximated (all values - only indices are changed).</param>
        /// <param name="nodeFeatureProvider">Provider for features for a node. In most cases it is the constant, but can depend on every node (e.g. random features for every node).</param>
        /// <param name="rightNodeSelector">
        /// Function that represents the classifier. 
        /// Parameters: feature, sample index. 
        /// Returns: True if the right node should be selected, false otherwise.
        /// </param>
        private void growSubTree(int[] indices, int nodeIndex, int depth, float[] sampleWeights, float[] targetValues, Func<TFeature[]> nodeFeatureProvider, Func<TFeature, int, bool> rightNodeSelector)
        {
#if LOG
            nProcessedNodes++;
            Console.Write("\r\t\tRegression tree: training node {0} / {1}", nProcessedNodes, this.treeNodes.Length);

            if (nProcessedNodes == this.treeNodes.Length)
                Console.WriteLine();
#endif
            treeNodes[nodeIndex] = new RegressionNodeData<TFeature>(); 
            RegressionNodeData<TFeature> node = treeNodes[nodeIndex];

            if (depth == this.TreeDepth) //compute output: weighted average
            {
                var nodeTargetValues = targetValues.GetAt(indices);
                var nodeSampleWeights = sampleWeights.GetAt(indices);

                node.OutputValue = (float)nodeTargetValues.WeightedAverage(nodeSampleWeights);
                return;
            }
            else if (indices.Length <= 1) //if the data is already split but the maximum depth is not reached propagate to the tree depth (it happens rarely if enough samples are taken)
            {
                var parentIdx = treeNodes.ParentIndex(nodeIndex);
                node.Data = treeNodes[parentIdx].Data;

                growSubTree(indices, treeNodes.LeftChildIndex(nodeIndex),  depth + 1, sampleWeights, targetValues, nodeFeatureProvider, rightNodeSelector);
                growSubTree(indices, treeNodes.RightChildIndex(nodeIndex), depth + 1, sampleWeights, targetValues, nodeFeatureProvider, rightNodeSelector);

                return;
            }

            //else //find error and splitting index => go recursive

            /****************** find the best split error (minimum error) ********************************/
            var features = nodeFeatureProvider();

            //get split error for each feature
            float[] splitErrors = new float[features.Length];

            Parallel.For(0, features.Length, (int i) => 
            //for (int i = 0; i < features.Length; i++)
            {
                splitErrors[i] = getSplitError(indices, features[i], rightNodeSelector, sampleWeights, targetValues);
            });

            var indexOfMinError = splitErrors.IndexOfMin();
            node.Data = features[indexOfMinError]; 
            /****************** find the best split error (minimum error) ********************************/

            //get split index for the selected feature
            var splitIndex = splitTrainingData(node.Data, ref indices, rightNodeSelector);

            //recursive calls for left and right branch
            growSubTree(indices.GetRange(0,          splitIndex),                  treeNodes.LeftChildIndex(nodeIndex),  depth + 1, sampleWeights, targetValues, nodeFeatureProvider, rightNodeSelector);
            growSubTree(indices.GetRange(splitIndex, indices.Length - splitIndex), treeNodes.RightChildIndex(nodeIndex), depth + 1, sampleWeights, targetValues, nodeFeatureProvider, rightNodeSelector);
        }

        /// <summary>
        /// Sorts sample indices and searches for the best splitting index for the specified feature <seealso cref="getSplitError"/>. 
        /// The 
        /// </summary>
        /// <param name="feature">The classifier input (e.g. binary test).</param>
        /// <param name="indices">Sample indices for a node (they are going to be sorted into two clusters according classifier output).</param>
        /// <param name="rightNodeSelector">
        /// Function that represents the classifier. 
        /// Parameters: feature, sample index. 
        /// Returns: True if the right node should be selected, false otherwise.
        /// </param>
        /// <returns>The index for which the splitting error is lowest.</returns>
        private int splitTrainingData(TFeature feature, ref int[] indices, Func<TFeature, int, bool> rightNodeSelector)
        {
            bool stop = false;
            int i = 0, j = indices.Length - 1;

            //sort indices (to make two clusters - left and right branch)
            while (!stop)
            {
                //while binary test gives output zero
                while (!rightNodeSelector(feature, indices[i]))
                {
                    if (i == j)
                        break;
                    else
                        i++;
                }

                //while binary test gives output one
                while (rightNodeSelector(feature, indices[j]))
                {
                    if (i == j)
                        break;
                    else
                        j--;
                }

                if (i == j)
                    stop = true;
                else
                {
                    //swap two indices
                    var temp = indices[i];
                    indices[i] = indices[j];
                    indices[j] = temp;
                }
            }

            /*int splitIndex = 0; //TODO: check: zašto nije jednak i ? da
            foreach (var idx in indices)
            {
                if (!rightNodeSelector(feature, idx)) 
                    splitIndex++;
            }

            if (i != splitIndex)
                Console.WriteLine();

            return splitIndex;*/

            return i; //splitIndex
        }

        /// <summary>
        /// Gets spiting error for a node of the regression tree.
        /// </summary>
        /// <param name="sampleIndicesForNode">Indices of the samples that belong to a node (tree branch).</param>
        /// <param name="feature">Along with sample index represents an input (test) for a classifier.</param>
        /// <param name="rightNodeSelector">
        /// Function that represents the classifier. 
        /// Parameters: feature, sample index. 
        /// Returns: True if the right node should be selected, false otherwise.
        /// </param>
        /// <param name="sampleWeights">Weights of the samples. The length of the array must be equal as the the number of sample indices.</param>
        /// <param name="targetValues">Values that has to be approximated. The length of the array must be equal as the the number of sample indices.</param>
        /// <returns>Splitting error for the specified samples and the specified feature.</returns>
        private float getSplitError(int[] sampleIndicesForNode, TFeature feature, Func<TFeature, int, bool> rightNodeSelector, float[] sampleWeights, float[] targetValues)
        {
            double weightSumL = 0, weightValSumL = 0, weightValSrqSumL = 0;
            double weightSumR = 0, weightValSumR = 0, weightValSrqSumR = 0;

            double weightSum = 0;

            foreach (var idx in sampleIndicesForNode)
            {
                if (rightNodeSelector(feature, idx))
                {
                    weightSumR += sampleWeights[idx];
                    weightValSumR += sampleWeights[idx] * targetValues[idx];
                    weightValSrqSumR += sampleWeights[idx] * targetValues[idx] * targetValues[idx];
                }
                else
                {
                    weightSumL += sampleWeights[idx];
                    weightValSumL += sampleWeights[idx] * targetValues[idx];
                    weightValSrqSumL += sampleWeights[idx] * targetValues[idx] * targetValues[idx];
                }

                weightSum += sampleWeights[idx];
            }

            if (weightSumL == 0 || weightSumR == 0 || weightSum == 0)
                return Single.MaxValue;

            //Nenad
            var errL = weightValSrqSumL - (weightValSumL * weightValSumL) / weightSumL;
            var errR = weightValSrqSumR - (weightValSumR * weightValSumR) / weightSumR;
            var error = (errL + errR) / weightSum;

            //u članku PITATI!!!
            /*var errL = weightValSrqSumL - 2 * weightValSumL + (weightValSumL * weightValSumL) / weightSumL;
            var errR = weightValSrqSumR - 2 * weightValSumR + (weightValSumR * weightValSumR) / weightSumR;
            var error = errL + errR;*/

            if (Math.Abs(error) < 1e-3)
                error = 0;

            return (float)error;
        }

        /// <summary>
        /// Gets the internal node data as read-only collection.
        /// </summary>
        public IReadOnlyList<TFeature> InternalNodeData 
        {
            get 
            {
                return this.treeNodes
                           .Where(x => x.OutputValue.HasValue == false)
                           .Select(x => x.Data)
                           .ToList();
            }
        }

        /// <summary>
        /// Gets the leaf output data as read-only collection.
        /// </summary>
        public IReadOnlyList<float> LeafData
        {
            get
            {
                return this.treeNodes
                           .Where(x => x.OutputValue.HasValue)
                           .Select(x => x.OutputValue.Value)
                           .ToList();
            }
        }
    }

}
