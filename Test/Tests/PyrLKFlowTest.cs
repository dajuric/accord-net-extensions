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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.Extensions.Imaging;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test
{
    /// <summary>
    /// Test for Pyramidal KLT tracker
    /// </summary>
    [TestClass]
    public class PyrLKFlowTest
    {
        [TestMethod]
        public void TestLKFlow()
        {
            var im1 = new Gray<float>[480, 640];
            im1.SetValue<Gray<float>>(System.Byte.MaxValue, new Accord.Extensions.Rectangle(272, 82, 116, 64));

            var im2 = new Gray<float>[480, 640];
            im2.SetValue<Gray<float>>(System.Byte.MaxValue, new Accord.Extensions.Rectangle(277, 83, 116, 64));

            var pts = new List<PointF>();
            pts.Add(new PointF(272, 82)); //-> 277,83

            PointF[] currFeatures;
            KLTFeatureStatus[] featureStatus;
            /*LKOpticalFlow<Gray<float>>.EstimateFlow(lkStorage, pts.ToArray(), 
                                                      out currFeatures, out featureStatus);*/

            PyrLKOpticalFlow<Gray<float>>.EstimateFlow(im1, im2, pts.ToArray(),
                                                       out currFeatures, out featureStatus);

            Assert.IsTrue(featureStatus[0] == KLTFeatureStatus.Success);
            Assert.IsTrue(Math.Round(currFeatures[0].X) == 277 && Math.Round(currFeatures[0].Y) == 83);
        }
    }
}
