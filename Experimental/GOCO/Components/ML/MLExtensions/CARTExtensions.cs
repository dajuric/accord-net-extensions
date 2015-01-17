using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCO
{
    public static class CARTExtensions
    {
        public static double[] Evaluate(this CART<StumpClassifier, double[]> cart, double[] sample)
        {
            return cart.Evaluate(stump => stump.Evaluate(sample));
        }

        public static void Train(this CART<StumpClassifier, double[]> cart, IList<double[]> samples, IList<double[]> targets)
        {
            Func<int[], double[], Tuple<SplitInfo, StumpClassifier>> splitFunc = (indices, weights) => 
            {
                var splitInfo = targets.FindBestSplit(samples, indices, weights);
                return new Tuple<SplitInfo, StumpClassifier>(splitInfo, StumpClassifier.Create(splitInfo));
            };

            cart.Train(targets.Count, splitFunc, 
                       (output) => output);
        }
    }
}
