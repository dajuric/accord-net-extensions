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

using Accord.Extensions;
using Accord.Extensions.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    /// <summary>
    /// Tests for the running variance.
    /// </summary>
    [TestClass]
    public class RunningVarianceTest
    {
        [TestMethod]
        public void TestRunningWeightedVarianceIncremental()
        {
            var arr = new List<double[]>();
            arr.AddRange(EnumerableExtensions.Create(1, _ => new double[] { 0 }));
            arr.AddRange(EnumerableExtensions.Create(1, _ => new double[] { 10 }));
            arr.AddRange(EnumerableExtensions.Create(1, _ => new double[] { 0 }));
            arr.AddRange(EnumerableExtensions.Create(1, _ => new double[] { 1 }));

            double[] w = EnumerableExtensions.Create(arr.Count, _ => (double)1).ToArray();

            var variancesInc = new List<double>();
            arr.RunningVarianceIncremental
                (
                        (idx, avgInc, varInc) =>
                        {
                            variancesInc.Add(varInc);
                        }
                        , w
                );

            Assert.IsTrue(Math.Abs(variancesInc[0] - 0) < 1E-2);
            Assert.IsTrue(Math.Abs(variancesInc[1] - 25) < 1E-2);
            Assert.IsTrue(Math.Abs(variancesInc[2] - 22.22) < 1E-2);
            Assert.IsTrue(Math.Abs(variancesInc[3] - 17.68) < 1E-2);
        }

        [TestMethod]
        public void TestRunningWeightedVarianceDecremental()
        {
            var arr = new List<double[]>();
            arr.AddRange(EnumerableExtensions.Create(1, _ => new double[] { 0 }));
            arr.AddRange(EnumerableExtensions.Create(1, _ => new double[] { 10 }));
            arr.AddRange(EnumerableExtensions.Create(1, _ => new double[] { 0 }));
            arr.AddRange(EnumerableExtensions.Create(1, _ => new double[] { 1 }));

            double[] w = EnumerableExtensions.Create(arr.Count, _ => (double)1).ToArray();

            var variancesDec = new List<double>();
            arr.RunningVarianceDecremental
                (
                        (idx, avgInc, varDec) =>
                        {
                            variancesDec.Add(varDec);
                        }
                        , w
                );

            Assert.IsTrue(Math.Abs(variancesDec[0] - 20.22) < 1E-2);
            Assert.IsTrue(Math.Abs(variancesDec[1] - 0.25) < 1E-2);
            Assert.IsTrue(Math.Abs(variancesDec[2] - 0) < 1E-2);
            Assert.IsTrue(Double.IsInfinity(variancesDec[3]));
        }
    }
}
