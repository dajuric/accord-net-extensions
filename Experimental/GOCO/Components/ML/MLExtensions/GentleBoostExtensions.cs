#define LOG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCO
{
    public static class GentleBoostExtensions
    {
        public static Stage<GentleBoost<CART<TStumpClassifer, float>>> Train<TStumpClassifer>(this GentleBoost<CART<TStumpClassifer, float>> boost,
                                                                                              IList<bool> isPositive, float[] outputs,
                                                                                              Func<float[], CART<TStumpClassifer, float>> learnerCreateAndTrainFunc,
                                                                                              Func<CART<TStumpClassifer, float>, int, float> learnerClassifyFunc,
                                                                                              int maxTrees, float minTPR, float maxFPR)
        {
            float threshold = 0;
            Func<IList<CART<TStumpClassifer, float>>, IList<float>, bool> terminationFunc = (learners, learnerOutputs) =>
            {
                float startThreshold = learnerOutputs.Max();
                const float THRESHOLD_SEARCH_STEP = -0.005f; //decrease by x

#if LOG
                                      Console.Write("\r\tStage: Searching for best threshold... ");
#endif
                float truePositiveRate, falsePositiveRate;
                threshold = Stage<GentleBoost<CART<TStumpClassifer, float>>>.SearchThreshold(isPositive, minTPR,
                                                                                             startThreshold, THRESHOLD_SEARCH_STEP,
                                                                                             (idx, th) => learnerOutputs[idx] > th,
                                                                                             out truePositiveRate, out falsePositiveRate);

#if LOG
                                      Console.WriteLine("{0}", threshold);
                                      Console.WriteLine("\r\tGentleBoost: Termination condition check - nLearners: {0}, TPR: {1}, FPR: {2}", learners.Count, truePositiveRate, falsePositiveRate);
#endif

                return learners.Count >= maxTrees || falsePositiveRate <= maxFPR; //if true then stop training
            };


            boost.Train(isPositive, outputs, learnerCreateAndTrainFunc, learnerClassifyFunc, terminationFunc);
            return new Stage<GentleBoost<CART<TStumpClassifer, float>>>(boost, threshold);
        }
    }
}
