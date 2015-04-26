#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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

using System;
using Accord.Extensions.Math;

namespace Accord.Extensions.Imaging
{
    public partial class DenseHistogram
    {
        /// <summary>
        /// Normalizes histogram so that sum of values are <paramref name="scale"/>.
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
