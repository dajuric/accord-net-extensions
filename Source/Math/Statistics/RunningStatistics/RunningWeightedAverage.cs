#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Statistics //TODO - critical: TEST CLASS
{
    /// <summary>
    /// Contains methods for running weighted average calculation. Scalar and vectors of type <see cref="System.Double"/> are supported.
    /// <para>Most methods can be used as extensions.</para>
    /// <para> See: <a href="http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance"/> for details.</para>
    /// </summary>
    public static class RunningWeightedAverage
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
        /// <param name="weights">Sample weights</param>
        public static void RunningAverageIncDec(this IList<double> data, Action<int, double, double> onCalculated, IList<double> weights)
        {
            var avgInc = 0d;
            var avgDec = data.Average(weights);

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avgInc += UpdateAverageIncremental(avgInc, i, item, weights[i]);
                avgDec -= UpdateAverageDecremental(avgDec, data.Count - i, item, weights[i]);
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
        /// <param name="weights">Sample weights</param>
        public static void RunningAverageIncremental(this IList<double> data, Action<int, double> onCalculated, IList<double> weights)
        {
            var avg = 0d;

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avg += UpdateAverageIncremental(avg, i, item, weights[i]);
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
        /// <param name="weights">Sample weights</param>
        public static void RunningAverageDecremental(this IList<double> data, Action<int, double> onCalculated, IList<double> weights)
        {
            var avg = data.Average(weights);

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avg -= UpdateAverageDecremental(avg, data.Count - i, item, weights[i]);
                onCalculated(i, avg);
            }
        }

        /// <summary>
        /// Updates running average incrementally.
        /// </summary>
        /// <param name="prevAverage">Previous average value. The initial value is 0.</param>
        /// <param name="prevWeightSum">Weight sum. The initial value is 0.</param>
        /// <param name="sample">Sample to add.</param>
        /// <param name="weight">Sample weight.</param>
        /// <returns>New average.</returns>
        internal static double UpdateAverageIncremental(double prevAverage, double prevWeightSum, double sample, double weight)
        {
            return prevAverage + (sample - prevAverage) * weight / (prevWeightSum + weight);
        }

        /// <summary>
        /// Updates running average decrement-ally.
        /// </summary>
        /// <param name="postAverage">Post average value. The initial value is the average of the whole collection.</param>
        /// <param name="postWeightSum">Weight sum. The initial value is the sum of weights.</param>
        /// <param name="sample">Sample to remove.</param>
        /// <param name="weight">Sample weight.</param>
        /// <returns>New average.</returns>
        internal static double UpdateAverageDecremental(double postAverage, double postWeightSum, double sample, double weight)
        {
            return postAverage + (postAverage - sample) * weight / (postWeightSum - weight); 
        }

        /// <summary>
        /// Calculated weighted-average of the provided collection.
        /// </summary>
        /// <param name="samples">Samples.</param>
        /// <param name="weights">Sample weights.</param>
        /// <returns>Weighted average.</returns>
        public static double Average(this IEnumerable<double> samples, IEnumerable<double> weights)
        {
            var samplesEnumerator = samples.GetEnumerator();
            var weightsEnumerator = weights.GetEnumerator();

            double sum = 0, weightSum = 0;

            while (samplesEnumerator.MoveNext())
            {
                if(!weightsEnumerator.MoveNext())
                    throw new Exception("Samples and weights must have the same length.");

                sum += samplesEnumerator.Current * weightsEnumerator.Current;
                weightSum += weightsEnumerator.Current;
            }

            return sum / weightSum;
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
        /// <param name="weights">Sample weights.</param>
        public static void RunningAverageIncDec(this IList<double[]> data, Action<int, double[], double[]> onCalculated, IList<double> weights)
        {
            var dim = data.First().Length;

            var avgInc = new double[dim];
            var avgDec = data.Average(weights);

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avgInc = avgInc.Add(UpdateAverageIncremental(avgInc, i, item, weights[i]));
                avgDec = avgDec.Subtract(UpdateAverageDecremental(avgDec, data.Count - i, item, weights[i]));
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
        /// <param name="weights">Sample weights.</param>
        public static void RunningAverageIncremental(this IList<double[]> data, Action<int, double[]> onCalculated, IList<double> weights)
        {
            var dim = data.First().Length;
            double[] avg = new double[dim];

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avg = avg.Add(UpdateAverageIncremental(avg, i, item, weights[i]));
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
        /// <param name="weights">Sample weights.</param>
        public static void RunningAverageDecremental(this IList<double[]> data, Action<int, double[]> onCalculated, IList<double> weights)
        {
            var dim = data.First().Length;
            var avg = data.Average(weights);

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                avg = avg.Subtract(UpdateAverageDecremental(avg, data.Count - i, item, weights[i]));
                onCalculated(i, avg);
            }
        }

        /// <summary>
        /// Updates running average incrementally.
        /// </summary>
        /// <param name="prevAverage">Previous average value. The initial value is 0.</param>
        /// <param name="nSamples">Number of samples that are included (with current element). Starting number is 1.</param>
        /// <param name="sample">Sample to add.</param>
        /// <param name="weight">Sample weight.</param>
        /// <returns>New average.</returns>
        internal static double[] UpdateAverageIncremental(double[] prevAverage, int nSamples, double[] sample, double weight)
        {
            double[] avg = new double[sample.Length];
            for (int i = 0; i < sample.Length; i++)
            {
                avg[i] = UpdateAverageIncremental(prevAverage[i], nSamples, sample[i], weight);
            }

            return avg;
        }

        /// <summary>
        /// Updates running average decrement-ally.
        /// </summary>
        /// <param name="prevAverage">Previous average value. The initial value is the average of the whole collection.</param>
        /// <param name="nSamples">Number of samples that are included (without current element). Starting number is the total number of samples.</param>
        /// <param name="sample">Sample to remove.</param>
        /// <param name="weight">Sample weight.</param>
        /// <returns>New average.</returns>
        internal static double[] UpdateAverageDecremental(double[] prevAverage, int nSamples, double[] sample, double weight)
        {
            double[] avg = new double[sample.Length];
            for (int i = 0; i < sample.Length; i++)
            {
                avg[i] = UpdateAverageDecremental(prevAverage[i], nSamples, sample[i], weight);
            }

            return avg;
        }

        /// <summary>
        /// Calculates average of the specified collection. Average is calculated for each sample dimension.
        /// </summary>
        /// <param name="data">Collection.</param>
        /// <param name="weights">Sample weights.</param>
        /// <returns>Average for each sample dimension.</returns>
        public static double[] Average(this IEnumerable<double[]> data, IEnumerable<double> weights)
        {
            var dim = data.Any() ? data.First().Length : 0;
            var avg = new double[dim];

            for (int i = 0; i < dim; i++)
            {
                avg[i] = data.Select(x => x[i]).Average(weights);
            }

            return avg;
        }

        #endregion
    }
}
