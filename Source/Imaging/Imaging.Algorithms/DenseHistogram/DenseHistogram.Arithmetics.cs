using System;
using Accord.Extensions.Math;

namespace Accord.Extensions.Imaging
{
    public partial class DenseHistogram
    {
        /// <summary>
        /// Normalizes histogram so that sum of values are <see cref="scale"/>.
        /// </summary>
        /// <param name="scale">Scale</param>
        public unsafe void Normalize(float scale = 1)
        {
            var sum = Sum();

            if (sum != 0)
                this.Scale((1 / sum) * scale);
        }

        /// <summary>
        /// Calculates histogram sum.
        /// </summary>
        /// <returns></returns>
        public unsafe float Sum()
        {
            int numOfElements = this.NumberOfElements;
            float* bins = histPtr;

            float sum = 0;
            for (int i = 0; i < numOfElements; i++)
            {
                sum += bins[0];
                bins++;
            }

            return sum;
        }

        /// <summary>
        /// Calculates histogram min.
        /// </summary>
        /// <returns></returns>
        private unsafe float Min()
        {
            int numOfElements = this.NumberOfElements;
            float* bins = histPtr;

            float min = Single.MaxValue;
            for (int i = 0; i < numOfElements; i++)
            {
                var val = bins[0];
                if (val < min)
                    min = val;

                bins++;
            }

            return min;
        }

        /// <summary>
        /// Calculates histogram max.
        /// </summary>
        /// <returns></returns>
        public unsafe float Max()
        {
            int numOfElements = this.NumberOfElements;
            float* bins = histPtr;

            float max = 0;
            for (int i = 0; i < numOfElements; i++)
            {
                var val = bins[0];
                if (val > max)
                {
                    max = val;
                }

                bins++;
            }

            return max;
        }

        /// <summary>
        /// Scales histogram values by user defined factor.
        /// </summary>
        /// <param name="factor"></param>
        public unsafe void Scale(float factor)
        {
            int numOfElements = this.NumberOfElements;
            float* bins = histPtr;

            bins = histPtr;
            for (int i = 0; i < numOfElements; i++)
            {
                bins[0] *= factor;
                bins++;
            }
        }

        /// <summary>
        /// Calculates the mean value of the histogram.
        /// </summary>
        /// <returns>Mean of the histogram.</returns>
        public float Mean()
        {
            return (float)this.HistogramArray.WeightedAverage((val, idx) => idx, (val, idx) => val);
        }

        /// <summary>
        /// Calculates deviation of the histogram.
        /// </summary>
        /// <param name="mean">Histogram mean value.</param>
        /// <returns>Deviation of the histogram.</returns>
        public float Deviation(float mean)
        {
            return (float)this.HistogramArray.WeightedAverage((val, idx) => (idx - mean), (val, idx) => val);
        }

        /// <summary>
        /// Calculates deviation of the histogram.
        /// <para>If the mean value is already calculated use the function overload to reduce performance penalties.</para>
        /// </summary>
        /// <returns>Deviation of the histogram.</returns>
        public float Deviation()
        {
            var mean = this.Mean();
            return Deviation(mean);
        }

    }
}
