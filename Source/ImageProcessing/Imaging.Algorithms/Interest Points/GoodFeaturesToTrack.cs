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
using System.Linq;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Filters;
using Accord.Extensions.Imaging.IntegralImage;
using Accord.Extensions.Math.Geometry;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extensions for good features to track.
    /// </summary>
    public static class GoodFeaturesToTrackExtensions
    {
        /// <summary>
        /// Searches the image for the good features to track. 
        /// <para>For each location a Hessian matrix is made and min eig-value is compared against threshold.</para>
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="winSize">Window size.</param>
        /// <param name="minEigVal">Minimum eigen value.</param>
        /// <param name="minimalDistance">Minimum distance from two features.</param>
        /// <returns>List of locations that have eigen value larger than <paramref name="minEigVal"/>.</returns>
        public static List<Point> GoodFeaturesToTrack(this Gray<float>[,] image, int winSize = 10, float minEigVal = 0.3f, float minimalDistance = 3)
        {
            var strengthImg = image.CopyBlank();

            var Dx = image.Sobel(1, 0, 3);
            var Dy = image.Sobel(0, 1, 3);
           
            var Dxx = Dx.MulFloat(Dx).MakeIntegral();
            var Dxy = Dx.MulFloat(Dy).MakeIntegral();
            var Dyy = Dy.MulFloat(Dy).MakeIntegral();

            goodFeaturesToTrack(Dxx, Dxy, Dyy,
                                winSize, minEigVal, strengthImg);
            
            var filteredStrengthImg = strengthImg.SupressNonMaxima();
            //var filteredStrengthImg = strengthImg;

            IList<float> values;
            var locations = filteredStrengthImg.FindNonZero(out values);

            var sortedFeatures = locations.Zip(values, (f, s) => new { f, s })
                                          .OrderByDescending(x => x.s)
                                          .Select(x => x.f)
                                          .ToList();

            sortedFeatures = sortedFeatures.EnforceMinimalDistance(minimalDistance);

            return sortedFeatures;
        }

        private unsafe static void goodFeaturesToTrack(Gray<float>[,] integralDxx, Gray<float>[,] integralDxy, Gray<float>[,] integralDyy,
                                                       int winSize, float minEigValue, Gray<float>[,] strengthImg)
        {
            minEigValue = System.Math.Max(1E-3f, minEigValue);
            int normFactor = winSize * winSize * 255;

            int maxCol = integralDxx.Width() - winSize;
            int maxRow = integralDxx.Height() - winSize;

            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    var Dxx = integralDxx.GetSum(col, row, winSize, winSize);
                    var Dxy = integralDxy.GetSum(col, row, winSize, winSize);
                    var Dyy = integralDyy.GetSum(col, row, winSize, winSize);

                    var eigenVal = calcMinEigenVal(Dxx, Dxy, Dyy);
                    eigenVal /= normFactor;

                    if (eigenVal > minEigValue)
                    {
                        strengthImg[winSize / 2 + row, winSize / 2 + col] = eigenVal;
                    }
                }
            }
        }

        private static float calcMinEigenVal(float Dxx, float Dxy, float Dyy)
        {
            //(a-d)^2 + 4 * b* c
            var discriminant = (Dxx - Dyy) * (Dxx - Dyy) + 4 * Dxy * Dxy;

            if (discriminant < 0) 
                return 0;

            var sqrtDiscriminant = (float)System.Math.Sqrt(discriminant);
            var minRealEigVal = ((Dxx + Dyy) - sqrtDiscriminant) / 2;

            return minRealEigVal;
        }
    }
}
