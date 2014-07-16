using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Statistics
{
    /// <summary>
    /// Contains methods for running variance calculation. Scalar and vectors of type <see cref="System.Double"/> are supported.
    /// <para>Most methods can be used as extensions.</para>
    /// <para> See: <a href="http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance"/> for details.</para>
    /// </summary>
    public static class RunningVariance
    {
        #region Scalar values

        /// <summary>
        /// Calculates incremental and decremental running variance.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition-removal. 
        /// <para>Parameters are: (index, incremental (average, incremental variance, decremental average, decremental variance).</para>
        /// </param>
        public static void RunningVarianceIncDec(this IList<double> data, Action<int, double, double, double, double> onCalculated)
        {
            double avgInc = 0d, varInc = 0d;
            double avgDec = data.Average(), varDec = data.Variance();

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                var prevAvg = avgInc;
                avgInc += RunningAverage.UpdateAverageIncremental(avgInc, i, item);
                var postAvg = avgDec;
                avgDec -= RunningAverage.UpdateAverageDecremental(avgDec, data.Count - i, item);

                varInc += UpdateVarianceIncremental(prevAvg, avgInc, varInc, i, item);
                varDec -= UpdateVarianceDecremental(postAvg, avgDec, varDec, data.Count - i, item);

                onCalculated(i, avgInc, varInc, avgDec, varDec);
            }
        }

        /// <summary>
        /// Calculates incremental running average and variance.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition. 
        /// <para>Parameters are: (index, incremental average, incremental variance).</para>
        /// </param>
        public static void RunningVarianceIncremental(this IList<double> data, Action<int, double, double> onCalculated)
        {
            double avg = 0d, variance = 0d;

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                var prevAvg = avg;
                avg += RunningAverage.UpdateAverageIncremental(avg, i, item);
                variance += UpdateVarianceIncremental(prevAvg, avg, variance, i, item);
                onCalculated(i, avg, variance);
            }
        }

        /// <summary>
        /// Calculates decremental running average and variance.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element removal. 
        /// <para>Parameters are: (index, decremental average, decremental variance).</para>
        /// </param>
        public static void RunningVarianceDecremental(this IList<double> data, Action<int, double, double> onCalculated)
        {
            double avg = data.Average(), variance = data.Variance();

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                var postAvg = avg;
                avg -= RunningAverage.UpdateAverageDecremental(avg, data.Count - i, item);
                variance -= UpdateVarianceDecremental(postAvg, avg, variance, data.Count - i, item);
                onCalculated(i, avg, variance);
            }
        }

        /// <summary>
        /// Updates running variance incrementally.
        /// </summary>
        /// <param name="prevAverage">Previous average value. The initial value is 0.</param>
        /// <param name="average">Updated average value.</param>
        /// <param name="prevVariance">Previous variance value. The initial value is 0.</param>
        /// <param name="nSamples">Number of samples that are included (with current element). Starting number is 1.</param>
        /// <param name="sample">Sample to add.</param>
        /// <returns>New variance.</returns>
        internal static double UpdateVarianceIncremental(double prevAverage, double average, double prevVariance, int nSamples, double sample)
        {
            var normFactor = (double)1 / nSamples;

            return normFactor * (nSamples - 1) * prevVariance + normFactor * (sample - prevAverage) * (sample - average);
        }

        /// <summary>
        /// Updates running variance decrement-ally.
        /// </summary>
        /// <param name="postAverage">Post average value. The initial value is the average of the whole collection.</param>
        /// <param name="average">Updated average value.</param>
        /// <param name="postVariance">Previous variance value. The initial value is the variance of the whole collection.</param>
        /// <param name="nSamples">Number of samples that are included (without current element). Starting number is the total number of samples.</param>
        /// <param name="sample">Sample to remove.</param>
        /// <returns>New variance.</returns>
        internal static double UpdateVarianceDecremental(double postAverage, double average, double postVariance, int nSamples, double sample)
        {
            var normFactor = (double)1 / (nSamples - 1);

            return normFactor * (nSamples) * postVariance - normFactor * (sample - postAverage) * (sample - average);
        }

        /// <summary>
        /// Calculates variance of the provided collection. Division is done using total number of samples.
        /// </summary>
        /// <param name="data">Collection.</param>
        /// <returns>Variance.</returns>
        internal static double Variance(this IEnumerable<double> data)
        {
            var avg = data.Average();

            var variance = 0d; int count = 0;
            foreach (var item in data)
            {
                var delta = (item - avg);
                variance += delta * delta;
                count++;
            }

            return variance / count;
        }

        #endregion

        #region Vectors

        /// <summary>
        /// Calculates incremental and decremental running variance.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition-removal. 
        /// <para>Parameters are: (index, incremental (average, incremental variance, decremental average, decremental variance).</para>
        /// </param>
        public static void RunningVarianceIncDec(this IList<double[]> data, Action<int, double[], double, double[], double> onCalculated)
        {
            var dim = data.First().Length;
            double[] avgInc = new double[dim], avgDec = data.Average();
            double varInc = 0d, varDec = data.Variance().Sum();

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                var prevAvg = avgInc;
                avgInc = avgInc.Add(RunningAverage.UpdateAverageIncremental(avgInc, i, item));
                var postAvg = avgDec;
                avgDec = avgDec.Subtract(RunningAverage.UpdateAverageDecremental(avgDec, data.Count - i, item));

                varInc += UpdateVarianceIncremental(prevAvg, avgInc, varInc, i, item);
                varDec -= UpdateVarianceDecremental(postAvg, avgDec, varDec, data.Count - i, item);

                onCalculated(i, avgInc, varInc, avgDec, varDec);
            }
        }

        /// <summary>
        /// Calculates incremental running average and variance.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition. 
        /// <para>Parameters are: (index, incremental average, incremental variance).</para>
        /// </param>
        public static void RunningVarianceIncremental(this IList<double[]> data, Action<int, double[], double> onCalculated)
        {
            var dim = data.First().Length;
            double[] avg = new double[dim];
            double varianceSum = 0d;

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                var prevAvg = avg;
                avg = avg.Add(RunningAverage.UpdateAverageIncremental(avg, i, item));
                varianceSum += UpdateVarianceIncremental(prevAvg, avg, varianceSum, i, item);
                onCalculated(i, avg, varianceSum);
            }
        }

        /// <summary>
        /// Calculates decremental running average and variance.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element removal. 
        /// <para>Parameters are: (index, decremental average, decremental variance).</para>
        /// </param>
        public static void RunningVarianceDecremental(this IList<double[]> data, Action<int, double[], double> onCalculated)
        {
            double[] avg = data.Average();
            double varianceSum = data.Variance().Sum();

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                var postAvg = avg;
                avg = avg.Subtract(RunningAverage.UpdateAverageDecremental(avg, data.Count - i, item));
                varianceSum -= UpdateVarianceDecremental(postAvg, avg, varianceSum, data.Count - i, item);
                onCalculated(i, avg, varianceSum);
            }
        }

        /// <summary>
        /// Updates running variance incrementally.
        /// </summary>
        /// <param name="prevAverage">Previous average value. The initial value is 0.</param>
        /// <param name="average">Updated average value.</param>
        /// <param name="prevVariance">Previous variance value. The initial value is 0.</param>
        /// <param name="nSamples">Number of samples that are included (with current element). Starting number is 1.</param>
        /// <param name="sample">Sample to add.</param>
        /// <returns>New variance.</returns>
        internal static double UpdateVarianceIncremental(double[] prevAverage, double[] average, double prevVariance, int nSamples, double[] sample)
        {
            var dim = sample.Length;
            var var = 0d;

            for (int i = 0; i < dim; i++)
            {
                var += UpdateVarianceIncremental(prevAverage[i], average[i], prevVariance, nSamples, sample[i]);
            }

            return var;
        }

        /// <summary>
        /// Updates running variance decrement-ally.
        /// </summary>
        /// <param name="postAverage">Post average value. The initial value is the average of the whole collection.</param>
        /// <param name="average">Updated average value.</param>
        /// <param name="postVariance">Previous variance value. The initial value is the variance of the whole collection.</param>
        /// <param name="nSamples">Number of samples that are included (without current element). Starting number is the total number of samples.</param>
        /// <param name="sample">Sample to remove.</param>
        /// <returns>New variance.</returns>
        internal static double UpdateVarianceDecremental(double[] postAverage, double[] average, double postVariance, int nSamples, double[] sample)
        {
            var dim = sample.Length;
            var var = 0d;

            for (int i = 0; i < dim; i++)
            {
                var += UpdateVarianceDecremental(postAverage[i], average[i], postVariance, nSamples, sample[i]);
            }

            return var;
        }

        /// <summary>
        /// Calculates variance of the specified collection. Variance is calculated for each sample dimension. 
        /// Division is done using total number of samples.
        /// </summary>
        /// <param name="data">Collection.</param>
        /// <returns>Variance for each sample dimension.</returns>
        internal static double[] Variance(this IEnumerable<double[]> data)
        {
            var dim = data.First().Length;
            var variance = new double[dim];

            for (int i = 0; i < dim; i++)
            {
                variance[i] = data.Select(x => x[i]).Variance();
            }

            return variance;
        }

        #endregion
    }
}
