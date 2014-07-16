using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Statistics
{
    /// <summary>
    /// Contains methods for running average calculation. Scalar and vectors of type <see cref="System.Double"/> are supported.
    /// <para>Most methods can be used as extensions.</para>
    /// <para> See: <a href="http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance"/> for details.</para>
    /// </summary>
    public static class RunningAverage
    {
        #region Scalar values

        /// <summary>
        /// Calculates incremental and decremental running average.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition-removal. 
        /// <para>Parameters are: (index, incremental average, decremental average).</para>
        /// </param>
        public static void RunningAverageIncDec(this IList<double> data, Action<int, double, double> onCalculated)
        {
            var avgInc = 0d;
            var avgDec = data.Average();

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avgInc += UpdateAverageIncremental(avgInc, i, item);
                avgDec -= UpdateAverageDecremental(avgDec, data.Count - i, item);
                onCalculated(i, avgInc, avgDec);
            }
        }

        /// <summary>
        /// Calculates incremental running average.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition. 
        /// <para>Parameters are: (index, incremental average).</para>
        /// </param>
        public static void RunningAverageIncremental(this IList<double> data, Action<int, double> onCalculated)
        {
            var avg = 0d;

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avg += UpdateAverageIncremental(avg, i, item);
                onCalculated(i, avg);
            }
        }

        /// <summary>
        /// Calculates decremental running average.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element removal. 
        /// <para>Parameters are: (index, decremental average).</para>
        /// </param>
        public static void RunningAverageDecremental(this IList<double> data, Action<int, double> onCalculated)
        {
            var avg = data.Average();

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avg -= UpdateAverageDecremental(avg, data.Count - i, item);
                onCalculated(i, avg);
            }
        }

        /// <summary>
        /// Updates running average incrementally.
        /// </summary>
        /// <param name="prevAverage">Previous average value. The initial value is 0.</param>
        /// <param name="nSamples">Number of samples that are included (with current element). Starting number is 1.</param>
        /// <param name="sample">Sample to add.</param>
        /// <returns>New average.</returns>
        internal static double UpdateAverageIncremental(double prevAverage, int nSamples, double sample)
        {
            return prevAverage + (sample - prevAverage) / nSamples;
        }

        /// <summary>
        /// Updates running average decrement-ally.
        /// </summary>
        /// <param name="postAverage">Post average value. The initial value is the average of the whole collection.</param>
        /// <param name="nSamples">Number of samples that are included (without current element). Starting number is the total number of samples.</param>
        /// <param name="sample">Sample to remove.</param>
        /// <returns>New average.</returns>
        internal static double UpdateAverageDecremental(double postAverage, int nSamples, double sample)
        {
            if (nSamples == 1)
                return sample;

            return postAverage - (sample - postAverage) / (nSamples - 1);
        }

        #endregion

        #region Vectors

        /// <summary>
        /// Calculates incremental and decremental running average.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition-removal. 
        /// <para>Parameters are: (index, incremental average, decremental average).</para>
        /// </param>
        public static void RunningAverageIncDec(this IList<double[]> data, Action<int, double[], double[]> onCalculated)
        {
            var dim = data.First().Length;

            var avgInc = new double[dim];
            var avgDec = data.Average();

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avgInc = avgInc.Add(UpdateAverageIncremental(avgInc, i, item));
                avgDec = avgDec.Subtract(UpdateAverageDecremental(avgDec, data.Count - i, item));
                onCalculated(i, avgInc, avgDec);
            }
        }

        /// <summary>
        /// Calculates incremental running average.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element addition. 
        /// <para>Parameters are: (index, incremental average).</para>
        /// </param>
        public static void RunningAverageIncremental(this IList<double[]> data, Action<int, double[]> onCalculated)
        {
            var dim = data.First().Length;
            double[] avg = new double[dim];

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avg = avg.Add(UpdateAverageIncremental(avg, i, item));
                onCalculated(i, avg);
            }
        }

        /// <summary>
        /// Calculates decremental running average.
        /// </summary>
        /// <param name="data">Sample data.</param>
        /// <param name="onCalculated">
        /// Action callback which fires on each element removal. 
        /// <para>Parameters are: (index, decremental average).</para>
        /// </param>
        public static void RunningAverageDecremental(this IList<double[]> data, Action<int, double[]> onCalculated)
        {
            var dim = data.First().Length;
            var avg = data.Average();

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avg = avg.Subtract(UpdateAverageDecremental(avg, data.Count - i, item));
                onCalculated(i, avg);
            }
        }

        /// <summary>
        /// Updates running average incrementally.
        /// </summary>
        /// <param name="prevAverage">Previous average value. The initial value is 0.</param>
        /// <param name="nSamples">Number of samples that are included (with current element). Starting number is 1.</param>
        /// <param name="sample">Sample to add.</param>
        /// <returns>New average.</returns>
        internal static double[] UpdateAverageIncremental(double[] prevAverage, int nSamples, double[] sample)
        {
            double[] avg = new double[sample.Length];
            for (int i = 0; i < sample.Length; i++)
            {
                avg[i] = UpdateAverageIncremental(prevAverage[i], nSamples, sample[i]);
            }

            return avg;
        }

        /// <summary>
        /// Updates running average decrement-ally.
        /// </summary>
        /// <param name="prevAverage">Previous average value. The initial value is the average of the whole collection.</param>
        /// <param name="nSamples">Number of samples that are included (without current element). Starting number is the total number of samples.</param>
        /// <param name="sample">Sample to remove.</param>
        /// <returns>New average.</returns>
        internal static double[] UpdateAverageDecremental(double[] prevAverage, int nSamples, double[] sample)
        {
            double[] avg = new double[sample.Length];
            for (int i = 0; i < sample.Length; i++)
            {
                avg[i] = UpdateAverageDecremental(prevAverage[i], nSamples, sample[i]);
            }

            return avg;
        }

        /// <summary>
        /// Calculates average of the specified collection. Average is calculated for each sample dimension.
        /// </summary>
        /// <param name="data">Collection.</param>
        /// <returns>Average for each sample dimension.</returns>
        public static double[] Average(this IList<double[]> data)
        {
            var dim = data.First().Length;
            var avg = new double[dim];

            for (int i = 0; i < dim; i++)
            {
                avg[i] = data.Select(x => x[i]).Average();
            }

            return avg;
        }

        #endregion
    }
}
