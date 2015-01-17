using Accord.Math;
using Accord.Extensions;
using Accord.Extensions.Statistics;
using Accord.Extensions.Math;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCO
{
    public class StumpClassifier
    {
        private StumpClassifier(int dimension, double threshold) 
        {
            this.Dimension = dimension;
            this.Threshold = threshold;
        }

        public static StumpClassifier Create(SplitInfo splitInfo)
        {
            return new StumpClassifier(splitInfo.DimensionIndex, splitInfo.SplitThreshold);
        }

        public double Threshold { get; set; }
        public int Dimension { get; set; }

        public bool Evaluate(double[] sample)
        {
            return sample[Dimension] >= Threshold;
        }
    }

    /// <summary>
    /// Stump classifier split data.
    /// </summary>
    public class SplitInfo
    {
        public int[] SortedIndices;
        public int SplitIndex;

        public int[] LeftIndices 
        { 
            get 
            {
                if (SortedIndices == null) return null;

                var length = SplitIndex + 1;
                return SortedIndices.GetRange(0, length); 
            } 
        }
         public int[] RightIndices 
        { 
            get 
            {
                if (SortedIndices == null) return null;

                var length = SortedIndices.Length - (SplitIndex + 1);
                return SortedIndices.GetRange(SplitIndex + 1, length); 
            } 
        }

        /// <summary>
        /// Dimension index.
        /// </summary>
        public int DimensionIndex;
        /// <summary>
        /// Best splitting threshold. The value is obtained as: (sortedSamples[splitIndex][featureDim] + sortedSamples[splitIndex+1][featureDim]) / 2
        /// </summary>
        public double SplitThreshold;

        /// <summary>
        /// Left side average (0..<paramref name="SplitIndex"/>). 
        /// <para>It is calculated for each dimension.</para> 
        /// </summary>
        public double[] LeftAverage;
        /// <summary>
        /// Left sum squared error (0..<paramref name="SplitIndex"/>).
        /// <para>It is the sum of SSE of all target dimensions.</para>
        /// </summary>
        public double LeftSSE;
        /// <summary>
        /// Right side average (<paramref name="SplitIndex"/> + 1 .. end). 
        /// <para>It is calculated for each dimension.</para> 
        /// </summary>
        public double[] RightAverage;
        /// <summary>
        /// Left sum squared error (<paramref name="SplitIndex"/> + 1 .. end). 
        /// <para>It is the sum of SSE of all target dimensions.</para>
        /// </summary>
        public double RightSSE;
    }

    /// <summary>
    /// Contains methods for determining the best split for samples which can be used for stump classifier, node splitting in CART.
    /// <para>Methods can be used as extensions.</para>
    /// </summary>
    public static class StumpClassifierExtensions
    {
        #region Binary features

        public static SplitInfo FindBestSplit<TSplitFeature>(this IList<double[]> targets, IList<TSplitFeature> splitFeatures, Func<TSplitFeature, int, bool> splitDataFunc,
                                                             int[] indices = null, IList<double> weights = null)
        {
            indices = indices ?? Matrix.Indices(0, targets.Count);
            weights = weights ?? EnumerableExtensions.Create(targets.Count, (_) => (double)1);

            SplitInfo bestInfo = new SplitInfo { LeftSSE = Double.MaxValue };
            object syncObj = new object();

            //for (int i = 0; i < features.Length; i++)
            Parallel.For(0, splitFeatures.Count, (int i) =>
            {
                SplitInfo dimInfo = new SplitInfo { DimensionIndex = i, LeftSSE = Double.MaxValue };
                getSplitError(targets, indices, weights, splitFeatures[i], splitDataFunc, ref dimInfo);

                //update the best split info
                lock (syncObj)
                {
                    var bestSSE = bestInfo.LeftSSE + bestInfo.RightSSE;
                    if ((dimInfo.LeftSSE + dimInfo.RightSSE) < bestSSE)
                    {
                        bestInfo = dimInfo;
                    }
                }
            });

            //get split index for the selected feature
            var bestFeature = splitFeatures[bestInfo.DimensionIndex];
            bestInfo.SortedIndices = indices; //clone or not ?
            splitTrainingData(bestFeature, bestInfo.SortedIndices, splitDataFunc, ref bestInfo);

            return bestInfo;
        }

        /// <summary>
        /// Gets spiting error for a node of the regression tree.
        /// </summary>
        /// <param name="indices">Indices of the samples that belong to a node (tree branch).</param>
        /// <param name="feature">Along with sample index represents an input (test) for a classifier.</param>
        /// <param name="rightSideSelector">
        /// Function that represents the classifier. 
        /// Parameters: feature, sample index. 
        /// Returns: True if the right node should be selected, false otherwise.
        /// </param>
        /// <param name="weights">Weights of the samples. The length of the array must be equal as the the number of sample indices.</param>
        /// <param name="targetValues">Values that has to be approximated. The length of the array must be equal as the the number of sample indices.</param>
        /// <returns>Splitting error for the specified samples and the specified feature.</returns>
        private static void getSplitError<TSplitFeature>(IList<double[]> targetValues, int[] indices, IList<double> weights, 
                                                         TSplitFeature splitFeature, Func<TSplitFeature, int, bool> rightSideSelector, 
                                                         ref SplitInfo splitInfo)
        {
            var targetDim  = targetValues.First().Length;

            double sumWeightLeft = 0, SSE_Left = 0; double[] meanLeft = new double[targetDim];
            double sumWeightRight = 0, SSE_Right = 0; double[] meanRight = new double[targetDim];

            foreach (var idx in indices)
            {
                if (rightSideSelector(splitFeature, idx))
                    RunningWeightedVariance.UpdateVarianceSumIncremental(ref SSE_Right, ref sumWeightRight, ref meanRight, targetValues[idx], weights[idx]);
                else
                    RunningWeightedVariance.UpdateVarianceSumIncremental(ref SSE_Left, ref sumWeightLeft, ref meanLeft, targetValues[idx], weights[idx]);
            }

            splitInfo.LeftAverage = meanLeft; splitInfo.RightAverage = meanRight;
            splitInfo.LeftSSE = SSE_Left; splitInfo.RightSSE = SSE_Right;
        }

        /// <summary>
        /// Sorts sample indices and searches for the best splitting index for the specified feature <seealso cref="getSplitError"/>. 
        /// The 
        /// </summary>
        /// <param name="feature">The classifier input (e.g. binary test).</param>
        /// <param name="indices">Sample indices for a node (they are going to be sorted into two clusters according classifier output).</param>
        /// <param name="rightSideSelector">
        /// Function that represents the classifier. 
        /// Parameters: feature, sample index. 
        /// Returns: True if the right node should be selected, false otherwise.
        /// </param>
        /// <returns>The index for which the splitting error is lowest.</returns>
        private static void splitTrainingData<TSplitFeature>(TSplitFeature feature, int[] indices, Func<TSplitFeature, int, bool> rightSideSelector, ref SplitInfo splitInfo)
        {
            Array.Sort<int>(indices, (a, b) => rightSideSelector(feature, a).CompareTo(rightSideSelector(feature, b)));
            splitInfo.SplitIndex = indices.Count((idx) => !rightSideSelector(feature, idx)) - 1;

            if (splitInfo.SplitIndex >= 0 && splitInfo.SplitIndex < (indices.Length - 1))
            {
                splitInfo.SplitThreshold = ((rightSideSelector(feature, indices[splitInfo.SplitIndex + 0]) ? +1d : -1d) + 
                                            (rightSideSelector(feature, indices[splitInfo.SplitIndex + 1]) ? +1d : -1d)) / 2; //will be 0 in most cases
            }
        }

        #endregion

        /// <summary>
        /// Finds the best split (feature dimension + threshold) in terms of the smallest variance. 
        /// </summary>
        /// <param name="targets">Outputs (target function values).</param>
        /// <param name="samples">Samples (source function values).</param>
        /// <param name="indices">Indices of the used samples. If null all samples will be used. Needed for tree training.</param>
        /// <param name="weights">Weights of the used samples. If null all weights will be set to 1 / nSamples.</param>
        /// <returns>Information about the selected dimension.</returns>
        public static SplitInfo FindBestSplit(this IList<double[]> targets, IList<double[]> samples, 
                                              int[] indices = null, IList<double> weights = null)
        {
            var sampleDim = samples.First().Length;

            return FindBestSplit(targets, sampleDim, (dimIdx) => samples.GetColumn(dimIdx).ToArray(), indices);
        }

        /// <summary>
        /// Finds the best split (feature dimension + threshold) in terms of the smallest variance. 
        /// </summary>
        /// <param name="targets">Outputs (target function values).</param>
        /// <param name="samples">Samples (source function values).</param>
        /// <param name="sampleDimension">The dimension of the sample.</param>
        /// <param name="dimensionSamplesFunc">
        /// The provider for samples for a dimension.
        /// <para>Parameters are: dimension index, samples parts for dimension.</para>
        /// </param>
        /// <param name="indices">Indices of the used samples. If null all samples will be used. Needed for tree training.</param>
        /// <param name="weights">Weights of the used samples. If null all weights will be set to 1 / nSamples.</param>
        /// <returns>Information about the selected dimension.</returns>
        public static SplitInfo FindBestSplit(this IList<double[]> targets, int sampleDimension, Func<int, IList<double>> dimensionSamplesFunc, 
                                              int[] indices = null, IList<double> weights = null)
        {
            object syncObj = new object();

            var targetDim = targets.First().Length;

            indices = indices ?? Matrix.Indices(0, targets.Count);
            weights = weights ?? EnumerableExtensions.Create(targets.Count, (_) => (double)1);
          
            //determine the best (smallest) SSE for each dimension
            SplitInfo bestInfo = new SplitInfo { LeftSSE = Double.MaxValue };

            //for (int fDimIdx = 0; fDimIdx < sampleDimension; fDimIdx++)
            Parallel.For(0, sampleDimension, (fDimIdx) =>
            {
                var dimInfo = new SplitInfo { DimensionIndex = fDimIdx, LeftSSE = Double.MaxValue };

                var dimensionSamples = dimensionSamplesFunc(fDimIdx);
                dimInfo.SortedIndices = (int[])indices.Clone(); //clone indices to avoid cross-threading invalid operations
                Array.Sort<int>(dimInfo.SortedIndices, (a, b) => dimensionSamples[a].CompareTo(dimensionSamples[b]));

                var sortedTargets = new IndexedCollection<double[]>(targets, dimInfo.SortedIndices);
                var sortedSamples = new IndexedCollection<double>(dimensionSamples, dimInfo.SortedIndices);
                var sortedWeights = new IndexedCollection<double>(weights, dimInfo.SortedIndices);

                sortedTargets.RunningVarianceIncDec((sampleIdx, leftAvg, leftVarSum, rightAvg, rightVarSum) =>
                {
                    findBestSplitInDimension(ref dimInfo, sortedSamples,
                                             sampleIdx, leftAvg, leftVarSum, rightAvg, rightVarSum);
                },
                sortedWeights,
                returnSSE: true);

                //update the best split info
                lock (syncObj)
                {
                    var bestSSE = bestInfo.LeftSSE + bestInfo.RightSSE;
                    if ((dimInfo.LeftSSE + dimInfo.RightSSE) < bestSSE)
                    {
                        bestInfo = dimInfo;
                    }
                }

                //GC.Collect();
            });

            return bestInfo;
        }

        /// <summary>
        /// Determines the best split for a specified dimension.
        /// The function is called each the running variance is updated.
        /// </summary>
        /// <param name="dimInfo">Dimension info structure.</param>
        /// <param name="sortedSamples">Sorted samples.</param>
        /// <param name="sampleIdx">The index of samples that has been added-removed.</param>
        /// <param name="leftAvg">Left side average.</param>
        /// <param name="leftSSE">Left sum of variance (SSE normalized).</param>
        /// <param name="rightAvg">Right side average.</param>
        /// <param name="rightSSE">Right sum of variance (SSE normalized).</param>
        private static void findBestSplitInDimension(ref SplitInfo dimInfo, IReadOnlyList<double> sortedSamples, 
                                                     int sampleIdx, double[] leftAvg, double leftSSE, double[] rightAvg, double rightSSE)
        {
            //force splitting in the middle of a set if the set contains equal values
            if (sampleIdx == (sortedSamples.Count - 1))
                return; //TODO: insert handling for collections of one sample

            if (sortedSamples[sampleIdx] == sortedSamples[sampleIdx + 1])
                return;

            if ((leftSSE + rightSSE) < (dimInfo.LeftSSE + dimInfo.RightSSE))
            {
                dimInfo.SplitThreshold = (sortedSamples[sampleIdx] + sortedSamples[sampleIdx + 1]) / 2;
                dimInfo.SplitIndex = sampleIdx;

                dimInfo.LeftAverage  = (double[])leftAvg.Clone();  dimInfo.LeftSSE = leftSSE;
                dimInfo.RightAverage = (double[])rightAvg.Clone(); dimInfo.RightSSE = rightSSE;
            }
        }
    }
}
