#define LOG

using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PointF = AForge.Point;

namespace GOCO
{
    public static class GocoExtensions
    {
        /***************** Stump classifier ********************/
        public static SplitInfo FindBestSplit(this IList<double> targets, IList<BinTestCode> binTests, IList<Image<Gray, byte>> images,
                                              int[] indices = null, IList<double> sampleWeights = null)
        { 
            return targets.ToJaggedMatrix().FindBestSplit(binTests, images, indices, sampleWeights);
        }

        public static SplitInfo FindBestSplit(this IList<double[]> targets, IList<BinTestCode> binTests, IList<Image<Gray, byte>> images,
                                              int[] indices = null, IList<double> sampleWeights = null)
        {
            return targets.FindBestSplit(binTests, (binTest, sampleIdx) => binTest.Test(images[sampleIdx]), indices, sampleWeights);
        }
    }

    public static class GocoClassifierExtensions
    {
        /*****************       Tree       *******************/
        public static void Train(this CART<BinTestCode, float> picoClassifier,
                                 IList<double> targets, IList<Image<Gray, byte>> images,
                                 int nBinTests = 1024, double[] sampleWeights = null)
        {
            Func<int[], double[], Tuple<SplitInfo, BinTestCode>> splitFunc = (indices, weights) => 
            {
                var binTests = BinTestCode.CreateRandom(nBinTests); //generate bin-tests for each node
                var splitInfo = targets.FindBestSplit(binTests, images, indices, weights);

                return new Tuple<SplitInfo,BinTestCode>(splitInfo, binTests[splitInfo.DimensionIndex]);
            };

            picoClassifier.Train(targets.Count, splitFunc,
                                (output) => (float)output[0],
                                sampleWeights);
        }

        public static float Evaluate(this CART<BinTestCode, float> picoClassifier,
                                     Image<Gray, byte> image)
        {
            return picoClassifier.Evaluate(x => x.Test(image));
        }

        public static float Evaluate(this CART<BinTestCode, float> picoClassifier,
                                     Image<Gray, byte> image, Rectangle window)
        {
            return picoClassifier.Evaluate(x => x.Test(image, window));
        }
        /******************************************************/

        /**************    Stage / Gentle Boost    ****************/
        public static Stage<GentleBoost<CART<BinTestCode, float>>> Train(this GentleBoost<CART<BinTestCode, float>> picoClassifier,
                                                                         IList<bool> isPositive, IList<Image<Gray, byte>> images,
                                                                         int nBinTests = 1024, int maxTreeDepth = 6, int maxTreesPerStage = 5,
                                                                         float minTPR = 0.998f, float maxFPR = 0.5f, float[] outputs = null)
        {
            outputs = outputs ?? new float[isPositive.Count];
            var targets = isPositive.Select<bool, double>(x => (x ? +1 : -1)).ToArray();

            Func<float[], CART<BinTestCode, float>> learnerCreateAndTrainFunc = (weights) => 
            {
                var cart = new CART<BinTestCode, float>(maxTreeDepth);
                cart.Train(targets, images, nBinTests, weights.Select(x => (double)x).ToArray());
                return cart;
            };

            Func<CART<BinTestCode, float>, int, float> learnerClassifyFunc = (cart, sampleIdx) =>
            { 
                var sample = images[sampleIdx];
                return cart.Evaluate(sample);
            };

            var stage = picoClassifier.Train(isPositive, outputs, learnerCreateAndTrainFunc, learnerClassifyFunc,
                                             maxTreesPerStage, minTPR, maxFPR);

            return stage;
        }

        public static float Evaluate(this GentleBoost<CART<BinTestCode, float>> pico,
                                     Image<Gray, byte> image, Rectangle window)
        {
            return pico.Evaluate(x => x.Evaluate(image, window));
        }
        /******************************************************/
    }
}
