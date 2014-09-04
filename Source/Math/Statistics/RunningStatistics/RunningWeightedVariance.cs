using Accord.Math;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Accord.Extensions;

namespace Accord.Extensions.Statistics
{
    /// <summary>
    /// Contains methods for running variance calculation. Scalar and vectors of type <see cref="System.Double"/> are supported.
    /// <para>Most methods can be used as extensions.</para>
    /// <para> 
    /// See: <a href="http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance"/>
    /// and <a href="http://nfs-uxsup.csx.cam.ac.uk/~fanf2/hermes/doc/antiforgery/stats.pdf"/> for details.
    /// </para>
    /// </summary>
    public static class RunningWeightedVariance
    {
        #region Scalar values

        /// <summary>
        /// Calculates incremental and decremental running variance.
        /// </summary>
        /// <param name="samples">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition-removal. 
        /// <para>Parameters are: (index, incremental average, incremental variance, decremental average, decremental variance).</para>
        /// </param>
        /// <param name="weights">Sample weights.</param>
        /// <param name="returnSSE">True to return unnormalized variance (SSE). False to return variance instead.</param>
        public static void RunningVarianceIncDec(this IReadOnlyList<double> samples, Action<int, double, double, double, double> onCalculated, IList<double> weights, bool returnSSE = false)
        {
            double sumWeightInc = 0, meanInc = 0, M2_Inc = 0;
            double sumWeightDec = weights.Sum(),
                   meanDec = samples.Average(weights),
                   M2_Dec = samples.Variance(weights) * sumWeightDec;
            
            for (int i = 0; i < samples.Count; i++)
            {
                var varInc = UpdateVarianceIncremental(ref M2_Inc, ref sumWeightInc, ref meanInc, samples[i], weights[i]);
                var varDec = UpdateVarianceDecremental(ref M2_Dec, ref sumWeightDec, ref meanDec, samples[i], weights[i]);

                if(!returnSSE)
                    onCalculated(i, meanInc, varInc, meanDec, varDec);
                else
                    onCalculated(i, meanInc, M2_Inc, meanDec, M2_Dec);
            }
        }

        /// <summary>
        /// Calculates incremental running average and variance.
        /// </summary>
        /// <param name="samples">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition. 
        /// <para>Parameters are: (index, incremental average, incremental variance).</para>
        /// </param>
        /// <param name="weights">Sample weights.</param>
        public static void RunningVarianceIncremental(this IList<double> samples, Action<int, double, double> onCalculated, IList<double> weights)
        {
            double sumWeight = 0, mean = 0, M2 = 0;

            for (int i = 0; i < samples.Count; i++)
            {
                var variance = UpdateVarianceIncremental(ref M2, ref sumWeight, ref mean, samples[i], weights[i]);
                onCalculated(i, mean, variance);
            }
        }

        /// <summary>
        /// Calculates decremental running average and variance.
        /// </summary>
        /// <param name="samples">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element removal. 
        /// <para>Parameters are: (index, decremental average, decremental variance).</para>
        /// </param>
        /// <param name="weights">Sample weights.</param>
        public static void RunningVarianceDecremental(this IList<double> samples, Action<int, double, double> onCalculated, IList<double> weights)
        {
            double sumWeight = weights.Sum(), 
                   mean = samples.Average(weights), 
                   M2 = samples.Variance(weights) * sumWeight;

            for (int i = 0; i < samples.Count; i++)
            {
                var variance = UpdateVarianceDecremental(ref M2, ref sumWeight, ref mean, samples[i], weights[i]);
                onCalculated(i, mean, variance);
            }
        }

        /// <summary>
        /// Adds sample to the total variance.
        /// </summary>
        /// <param name="SSE">The samples SSE (unnormalized variance). The initial value is 0.</param>
        /// <param name="sumWeight">Sum of samples weights. The initial value is 0.</param>
        /// <param name="mean">Samples mean. The initial value is zero.</param>
        /// <param name="sample">Sample to add.</param>
        /// <param name="weight">Sample weight.</param>
        /// <returns>The variance of the sub-collection.</returns>
        public static double UpdateVarianceIncremental(ref double SSE, ref double sumWeight, ref double mean, double sample, double weight)
        {
            var newSumWeight = weight + sumWeight;
            var delta = sample - mean;
            var R = delta * weight / newSumWeight;
            mean = mean + R;

            SSE = SSE + sumWeight * delta * R;
            sumWeight = newSumWeight;

            var variance = SSE / sumWeight;
            return variance;
        }

        /// <summary>
        /// Removes the sample impact from the total variance.
        /// </summary>
        /// <param name="SSE">The samples SSE (unnormalized variance). The initial value is the variance multiplied by sum of weights.</param>
        /// <param name="sumWeight">Sum of samples weights. The initial value is the sum of weights.</param>
        /// <param name="mean">Samples mean. The initial value is the total mean.</param>
        /// <param name="sample">Sample to remove.</param>
        /// <param name="weight">Sample weight.</param>
        /// <returns>The variance of the sub-collection.</returns>
        public static double UpdateVarianceDecremental(ref double SSE, ref double sumWeight, ref double mean, double sample, double weight)
        {
            /*if (System.Math.Abs(M2) < 1E-5) //if there are no more samples
            {
                mean = 0;
                return 0;
            }*/

            var newSumWeight = sumWeight - weight;
            var delta = mean - sample;
            var R = delta * weight / newSumWeight;
            mean = mean + R;

            SSE = SSE - sumWeight * delta * R;
            sumWeight = newSumWeight;

            var variance = SSE / sumWeight;
            return variance;
        }

        /// <summary>
        /// Calculates weighted variance of the provided collection. Division is done using total number of samples.
        /// <para>See <a href="http://nfs-uxsup.csx.cam.ac.uk/~fanf2/hermes/doc/antiforgery/stats.pdf">weighted variance</a> for details.</para>
        /// </summary>
        /// <param name="samples">Collection.</param>
        /// <param name="weights">Sample weights.</param>
        /// <returns>Variance.</returns>
        public static double Variance(this IEnumerable<double> samples, IEnumerable<double> weights)
        {
            var samplesEnumerator = samples.GetEnumerator();
            var weightsEnumerator = weights.GetEnumerator();

            double weightedAvg = samples.Average(weights);
            double variance = 0d, weightSum = 0d;

            while (samplesEnumerator.MoveNext())
            {
                if (!weightsEnumerator.MoveNext())
                    throw new Exception("Samples and weights must have the same length.");

                var delta = (samplesEnumerator.Current - weightedAvg);
                variance += delta * delta * weightsEnumerator.Current;
                weightSum += weightsEnumerator.Current;
            }

            return variance / weightSum;
        }

        #endregion

        #region Vectors

        /// <summary>
        /// Calculates incremental and decremental running variance.
        /// </summary>
        /// <param name="samples">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition-removal. 
        /// <para>Parameters are: (index, incremental (average, incremental variance, decremental average, decremental variance).</para>
        /// </param>
        /// <param name="weights">Sample weights.</param>
        /// <param name="returnSSE">True to return unnormalized variance (SSE). False to return variance instead.</param>
        public static void RunningVarianceIncDec(this IReadOnlyList<double[]> samples, Action<int, double[], double, double[], double> onCalculated, IReadOnlyList<double> weights, bool returnSSE = false)
        {
            //weights = weights.Select(x => x * 1000).ToArray();

            int sampleDim = samples.First().Length;

            double sumWeightInc = 0;
            double[] meanInc = new double[sampleDim];
            double M2_Inc = 0;
       
            double sumWeightDec = weights.Sum();
            double[] meanDec = samples.Average(weights);
            double M2_Dec = samples.Variance(weights).Multiply(sumWeightDec).Sum();

            for (int i = 0; i < samples.Count; i++)
            {
                var varInc = UpdateVarianceSumIncremental(ref M2_Inc, ref sumWeightInc, ref meanInc, samples[i], weights[i]);
                var varDec = UpdateVarianceSumDecremental(ref M2_Dec, ref sumWeightDec, ref meanDec, samples[i], weights[i]);

                /*var averageInc = samples.Take(i + 1).Average(weights.Take(i + 1));
                var averageDec = samples.Skip(i + 1).Average(weights.Skip(i + 1));

                if (System.Math.Abs(averageInc[0] - meanInc[0]) > 0.001 ||
                   System.Math.Abs(averageDec.FirstOrDefault() - meanDec[0]) > 0.001)
                {
                    Console.WriteLine();
                }*/

                if(!returnSSE)
                    onCalculated(i, meanInc, varInc, meanDec, varDec); 
                else
                    onCalculated(i, meanInc, M2_Inc, meanDec, M2_Dec); 
            }
        }

        /// <summary>
        /// Calculates incremental running average and variance.
        /// </summary>
        /// <param name="samples">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition. 
        /// <para>Parameters are: (index, incremental average, incremental variance).</para>
        /// </param>
        /// <param name="weights">Sample weights.</param>
        public static void RunningVarianceIncremental(this IList<double[]> samples, Action<int, double[], double> onCalculated, IList<double> weights)
        {
            int sampleDim = samples.First().Length;

            double   sumWeight = 0;
            double[] mean = new double[sampleDim];
            double M2 = 0;

            for (int i = 0; i < samples.Count; i++)
            {
                var variance = UpdateVarianceSumIncremental(ref M2, ref sumWeight, ref mean, samples[i], weights[i]);
            }
        }

        /// <summary>
        /// Calculates decremental running average and variance.
        /// </summary>
        /// <param name="samples">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element removal. 
        /// <para>Parameters are: (index, decremental average, decremental variance).</para>
        /// </param>
        /// <param name="weights">Sample weights.</param>
        public static void RunningVarianceDecremental(this IList<double[]> samples, Action<int, double[], double> onCalculated, IList<double> weights)
        {
            double   sumWeight = weights.Sum();
            double[] mean = samples.Average(weights);
            double   M2 = samples.Variance(weights).Sum();

            for (int i = 0; i < samples.Count; i++)
            {
                var variance = UpdateVarianceSumDecremental(ref M2, ref sumWeight, ref mean, samples[i], weights[i]);
                onCalculated(i, mean, variance);
            }
        }

        /// <summary>
        /// Adds sample to the total variance.
        /// </summary>
        /// <param name="SSE">The samples SSE (unnormalized variance). The initial value is 0.</param>
        /// <param name="sumWeight">Sum of samples weights. The initial value is 0.</param>
        /// <param name="mean">Samples mean. The initial value is zero vector.</param>
        /// <param name="sample">Sample to add.</param>
        /// <param name="weight">Sample weight.</param>
        /// <returns>The variance of the sub-collection.</returns>
        public static double UpdateVarianceSumIncremental(ref double SSE, ref double sumWeight, ref double[] mean, double[] sample, double weight)
        {
            var dim = sample.Length;
            var var = 0d;

            for (int i = 0; i < dim; i++)
            {
                var += UpdateVarianceIncremental(ref SSE, ref sumWeight, ref mean[i], sample[i], weight);
            }

            return var;
        }

        /// <summary>
        /// Removes the sample impact from the total variance.
        /// </summary>
        /// <param name="SSE">The samples SSE (unnormalized variance). The initial value is the variance multiplied by su of weights.</param>
        /// <param name="sumWeight">Sum of samples weights. The initial value is the sum of weights..</param>
        /// <param name="mean">Samples mean. The initial value is the total mean.</param>
        /// <param name="sample">Sample to remove..</param>
        /// <param name="weight">Sample weight.</param>
        /// <returns>The variance of the sub-collection.</returns>
        public static double UpdateVarianceSumDecremental(ref double SSE, ref double sumWeight, ref double[] mean, double[] sample, double weight)
        {
            var dim = sample.Length;
            var var = 0d;

            for (int i = 0; i < dim; i++)
            {
                var += UpdateVarianceDecremental(ref SSE, ref sumWeight, ref mean[i], sample[i], weight);
            }

            return var;
        }

        /// <summary>
        /// Calculates variance of the specified collection. Variance is calculated for each sample dimension. 
        /// Division is done using total number of samples.
        /// </summary>
        /// <param name="samples">Samples.</param>
        /// <param name="weights">Sample weights.</param>
        /// <returns>Variance for each sample dimension.</returns>
        public static double[] Variance(this IEnumerable<double[]> samples, IEnumerable<double> weights)
        {
            var dim = samples.First().Length;
            var variance = new double[dim];

            for (int i = 0; i < dim; i++)
            {
                variance[i] = samples.Select(x => x[i]).Variance(weights);
            }

            return variance;
        }

        #endregion
    }
}
