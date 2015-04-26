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

using Accord.Extensions.Math;
using Accord.Extensions.Math.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    /// <summary>
    /// Tests for the <see cref="Accord.Extensions.Math.BranchAndBoundAssigmentExtensions"/>.
    /// </summary>
    [TestClass]
    public class BranchAndBoundAssigmentTest
    {
        [TestMethod]
        public void TestEmpty()
        {
            double[,] cost = new double[0, 1];

            var assigmentMatrix = cost.MatchAssigments();

            Assert.IsTrue(assigmentMatrix.RowCount() == 0 && assigmentMatrix.ColumnCount() == 1);
        }

        [TestMethod]
        public void TestRowUnmatched()
        {
            var cost = new double[,] 
                      {
                          {1},
                          {14}
                      };

            var assigmentMatrix = cost.MatchAssigments();

            Assert.IsTrue(assigmentMatrix[0, 0] == true);
            Assert.IsTrue(assigmentMatrix[1, 0] == false);
        }

        [TestMethod]
        public void TestColumnUnmatched()
        {
            var cost = new double[,] 
                      {
                          {14, 1}
                      };

            var assigmentMatrix = cost.MatchAssigments();

            Assert.IsTrue(assigmentMatrix[0, 0] == false);
            Assert.IsTrue(assigmentMatrix[0, 1] == true);
        }

        /// <summary>
        /// The sample is taken from: <a href="http://community.topcoder.com/tc?module=Static&d1=tutorials&d2=hungarianAlgorithm">Assignment Problem and Hungarian Algorithm</a>.
        /// </summary>
        [TestMethod]
        public void TestSquare()
        {
            double[,] cost = new double[,] 
                      {
                           {1, 4, 5},
                           {5, 7, 6}, 
                           {5, 8, 8}
                      };

            var assigmentMatrix = cost.MatchAssigments();

            Assert.IsTrue(assigmentMatrix[0, 0] == false);
            Assert.IsTrue(assigmentMatrix[0, 1] == true);
            Assert.IsTrue(assigmentMatrix[0, 2] == false);

            Assert.IsTrue(assigmentMatrix[1, 0] == false);
            Assert.IsTrue(assigmentMatrix[1, 1] == false);
            Assert.IsTrue(assigmentMatrix[1, 2] == true);

            Assert.IsTrue(assigmentMatrix[2, 0] == true);
            Assert.IsTrue(assigmentMatrix[2, 1] == false);
            Assert.IsTrue(assigmentMatrix[2, 2] == false);
        }
    }
}
