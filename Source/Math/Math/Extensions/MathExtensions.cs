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
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Math
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides additional math functions.
    /// </summary>
    public static class MathExtensions
    {
        #region ATan2 Approximation

        /// <summary>
        /// Calculates an octant and brings x and y to the first octant
        /// </summary>
        /// <returns> Octant [0..7]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateOctant(ref int x, ref int y)
        { 
            int o = 0, t;

            if (y < 0)
            {
                x = -x;
                y = -y;
                o += 4;
            }
            if (x <= 0)
            {
                t = x; x = y;
                y = -t;
                o += 2;
            }
            if (x <= y)
            {
                t = y - x; x = x + y;
                y = t;
                o += 1;
            }

            return o;
        }

        const int PI_DEG = 180;

        const int Y_MUL_CONST = 64;
        /// <summary>
        /// Degree resolution.
        /// </summary>
        public static float DEG_RESOULTION = (float)(PI_DEG / 4) / Y_MUL_CONST;


        static int[] angleTable;

        static MathExtensions()
        {
            angleTable = CalculateAngleTable();
        }

        private static int[] CalculateAngleTable()
        {
            int[] angleTable = new int[Y_MUL_CONST + 1];

            for (int i = 0; i < angleTable.Length; i++)
            {
                double angle = System.Math.Atan((double)i / Y_MUL_CONST) * 180 / System.Math.PI;
                angleTable[i] = (int)System.Math.Round(angle);
            }

            return angleTable;
        }

        //TODO - medium: check maxium error!
        /// <summary>
        /// Approximates Atan2 function. Maximum error is 90 / Y_MUL_CONST (1.4 degrees). 
        /// </summary>
        /// <param name="dY">Vertical offset.</param>
        /// <param name="dX">Horizontal offset.</param>
        /// <returns>Angle in degrees.</returns>
        public static int Atan2Aprox(this int dY, int dX)
        {
            if (dY == 0) return (dX >= 0 ? 0 : PI_DEG);

            int octant = CalculateOctant(ref dX, ref dY);

            int tableIdx = dY * Y_MUL_CONST / dX;
            int degFirstOctant = angleTable[tableIdx];

            return (PI_DEG / 4) * octant + degFirstOctant;
        }

        #endregion

        #region Sqrt Approximation

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public int tmp;
        }

        /// <summary>
        /// Approximates Sqrt function.
        /// see: http://blog.wouldbetheologian.com/2011/11/fast-approximate-sqrt-method-in-c.html
        /// </summary>
        /// <param name="z">Input number</param>
        /// <returns>Square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(this float z)
        {
            if (z == 0) return 0;

            FloatIntUnion u;
            u.tmp = 0;
            float xhalf = 0.5f * z;
            u.f = z;
            u.tmp = 0x5f375a86 - (u.tmp >> 1);
            u.f = u.f * (1.5f - xhalf * u.f * u.f);
            return u.f * z;
        }

        #endregion

        /// <summary>
        /// Calculates Jacobian matrix.
        /// The size of an Jacobian matrix is [func output length x arg length]
        /// </summary>
        /// <param name="evalFunc">
        /// Evaluation function.
        /// arg: starting point 
        /// returns: function result.
        /// </param>
        /// <param name="arg">Starting point.</param>
        /// <param name="eps">Delta between two points.</param>
        /// <returns>Jacobian matrix.</returns>
        public static double[,] CalculateJacobian(Func<double[], double[]> evalFunc, double[] arg, double eps = 1e-8)
        {
            var originalResult = evalFunc(arg);
            var jacobian = new double[originalResult.Length, arg.Length];

            //derivations for arg parts
            for (int j = 0; j < arg.Length; j++)
            {
                var argShift = (double[])arg.Clone();
                argShift[j] += eps;

                //derivations for func
                var result = evalFunc(argShift);
                for (int i = 0; i < result.Length; i++)
                {
                    jacobian[j, i] = (result[i] - originalResult[i]) / eps;
                }
            }

            return jacobian;
        }
    }

    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides additional math functions for integer numbers to check whether they are power of two or not.
    /// </summary>
    public static class MathPowerofTwoExtensions
    {
        /// <summary>
        /// Checks whether the specified value is power of 2. 
        /// It uses fast arithemtics to avoid using: Math.Floor(Math.Log(x, 2))
        /// </summary>
        /// <param name="x">Value to check.</param>
        /// <returns>True if the number is power of 2, false otherwise.</returns>
        public static bool IsPowerOfTwo(this ulong x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        /// <summary>
        /// Checks whether the specified value is power of 2. 
        /// It uses fast arithemtics to avoid using: Math.Floor(Math.Log(x, 2))
        /// </summary>
        /// <param name="x">Value to check.</param>
        /// <returns>True if the number is power of 2, false otherwise.</returns>
        public static bool IsPowerOfTwo(this uint x)
        {
            return IsPowerOfTwo((ulong)x);
        }

        /// <summary>
        /// Checks whether the specified value is power of 2. 
        /// It uses fast arithmetics to avoid using: Math.Floor(Math.Log(x, 2)).
        /// </summary>
        /// <param name="x">Value to check.</param>
        /// <returns>True if the number is power of 2, false otherwise.</returns>
        /// <exception cref="ArgumentException">The specified value must be greater or equal to zero.</exception>
        public static bool IsPowerOfTwo(this int x)
        {
            if (x < 0) throw new ArgumentException("The number must be greater or equal to zero!");

            return IsPowerOfTwo((ulong)x);
        }
    }

    /// <summary>
    /// Provides additional math functions for collections.
    /// <para>Defined functions can be used as object extensions.</para>
    /// </summary>
    public static class MathEnumerableExtensions
    {
        /// <summary>
        /// Calculates weighted average. 
        /// <para>In case where sum of weights is equal to zero, zero will be returned. Use a function overload to change this behavior.</para>
        /// </summary>
        /// <param name="collection">Collections of elements.</param>
        /// <param name="weights">Weights.</param>
        /// <returns>Weighted average of a collection.</returns>
        public static double WeightedAverage(this IEnumerable<float> collection, IList<float> weights)
        {
            return collection.WeightedAverage((e, _) => e, (_, i) => weights[i]);
        }

        /// <summary>
        /// Calculates weighted average. 
        /// <para>In case where sum of weights is equal to zero, zero will be returned. Use a function overload to change this behavior.</para>
        /// </summary>
        /// <param name="collection">Collections of elements.</param>
        /// <param name="weights">Weights.</param>
        /// <returns>Weighted average of a collection.</returns>
        public static double WeightedAverage(this IEnumerable<double> collection, IList<double> weights)
        {
            return collection.WeightedAverage((e, _) => e, (_, i) => weights[i]);
        }

        /// <summary>
        /// Calculates weighted average. 
        /// <para>In case where sum of weights is equal to zero, zero will be returned. Use a function overload to change this behavior.</para>
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="collection">Collections of elements.</param>
        /// <param name="valueSelector">Value selector. Parameters are the selected element and index of an element.</param>
        /// <param name="weightSelector">Weight selector. Parameters are the selected element and index of an element.</param>
        /// <returns>Weighted average of a collection.</returns>
        public static double WeightedAverage<T>(this IEnumerable<T> collection, Func<T, int, double> valueSelector, Func<T, int, double> weightSelector)
        {
            return collection.WeightedAverage(valueSelector, weightSelector, () => 0);
        }

        /// <summary>
        /// Calculates weighted average.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="collection">Collections of elements.</param>
        /// <param name="valueSelector">Value selector. Parameters are the selected element and index of an element.</param>
        /// <param name="weightSelector">Weight selector. Parameters are the selected element and index of an element.</param>
        /// <param name="divisionByZeroResolver">Division by zero case resolver.</param>
        /// <returns>Weighted average of a collection.</returns>
        public static double WeightedAverage<T>(this IEnumerable<T> collection, Func<T, int, double> valueSelector, Func<T, int, double> weightSelector, Func<double> divisionByZeroResolver)
        {
            double weightedSum = 0, sumOfWeights = 0;

            int idx = 0;
            foreach (var item in collection)
            {
                var val = valueSelector(item, idx);
                var w = weightSelector(item, idx);

                weightedSum += val * w;
                sumOfWeights += w;

                idx++;
            }

            if (weightedSum == 0)
                return divisionByZeroResolver();
            else
                return weightedSum / sumOfWeights;
        }
    }
}
