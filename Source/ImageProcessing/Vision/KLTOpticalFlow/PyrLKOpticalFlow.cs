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

using System.Linq;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Math;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Pyramidal Kanade-Lucas-Tomasi optical flow.
    /// </summary>
    /// <typeparam name="TColor">Image color. This implementation supports all color types.</typeparam>
    public static class PyrLKOpticalFlow<TColor>
        where TColor: struct, IColor<float>
    {
        /// <summary>
        /// Estimates LK optical flow.
        /// </summary>
        /// <param name="prevImg">Previous image.</param>
        /// <param name="currImg">Current image.</param>
        /// <param name="prevFeatures">Previous features.</param>
        /// <param name="currFeatures">Current features.</param>
        /// <param name="status">Feature status.</param>
        /// <param name="windowSize">Aperture size.</param>
        /// <param name="iterations">Maximal number of iterations. If <paramref name="minFeatureShift"/> is reached then number of iterations will be lower.</param>
        /// <param name="minFeatureShift">Minimal feature shift in horizontal or vertical direction.</param>
        /// <param name="minEigenValue">Minimal eigen value. 
        /// Eigen values could be interpreted as lengths of ellipse's axes. 
        /// If zero ellipse turn into line therefore there is no corner.</param>
        /// <param name="pyrLevels">Number of pyramid levels. By using 0 pyramid levels pyramidal implementation reduces to non-pyramidal one.</param>
        public static void EstimateFlow(TColor[,] prevImg, TColor[,] currImg,
                                       PointF[] prevFeatures, out PointF[] currFeatures,
                                       out KLTFeatureStatus[] status,
                                       int windowSize = 15, int iterations = 30, float minFeatureShift = 0.1f, float minEigenValue = 0.01f, int pyrLevels = 1)
        {
            var storage = new PyrLKStorage<TColor>(pyrLevels);
            storage.Process(prevImg, currImg);

            EstimateFlow(storage,
                         prevFeatures, out currFeatures,
                         out status,
                         windowSize, iterations, minFeatureShift, minEigenValue);
        }

        /// <summary>
        /// Estimates LK optical flow.
        /// </summary>
        /// <param name="storage">Used storage. Number of pyramid levels is specified within storage. Use storage to gain performance in video* by 2x! </param>
        /// <param name="prevFeatures">Previous features.</param>
        /// <param name="currFeatures">Current features.</param>
        /// <param name="status">Feature status.</param>
        /// <param name="windowSize">Aperture size.</param>
        /// <param name="iterations">Maximal number of iterations. If <paramref name="minFeatureShift"/> is reached then number of iterations will be lower.</param>
        /// <param name="minFeatureShift">Minimal feature shift in horizontal or vertical direction.</param>
        /// <param name="minEigenValue">Minimal eigen value. 
        /// Eigen values could be interpreted as lengths of ellipse's axes. 
        /// If zero ellipse turn into line therefore there is no corner.</param>
        public static void EstimateFlow(PyrLKStorage<TColor> storage,
                                        PointF[] prevFeatures, out PointF[] currFeatures,
                                        out KLTFeatureStatus[] status,
                                        int windowSize = 15, int iterations = 30, float minFeatureShift = 0.1f, float minEigenValue = 0.001f)
        {
            var initialEstimate = new PointF[prevFeatures.Length];

            currFeatures = new PointF[prevFeatures.Length];
            status = new KLTFeatureStatus[prevFeatures.Length];

            var scaledPrevFeatures = prevFeatures.Apply(x => x.DownScale(storage.PyrLevels));
            var usedIndicies = Enumerable.Range(0, prevFeatures.Length).ToArray(); 

            for (int pyrLevel = storage.PyrLevels; pyrLevel >= 0; pyrLevel--)
            {
                PointF[] levelCurrFeatures;
                KLTFeatureStatus[] levelStatus;
                LKOpticalFlow<TColor>.EstimateFlow(storage, 
                                                   scaledPrevFeatures.GetAt(usedIndicies), out levelCurrFeatures, out levelStatus, 
                                                   windowSize, iterations, minFeatureShift, minEigenValue, initialEstimate, pyrLevel);

                //update data
                currFeatures.SetAt(usedIndicies, levelCurrFeatures);
                status.SetAt(usedIndicies, levelStatus);
                
                if (pyrLevel != 0)
                {
                    scaledPrevFeatures.ApplyInPlace(usedIndicies, x => x.UpScale());
                    currFeatures.ApplyInPlace(usedIndicies, x => x.UpScale());

                    initialEstimate = Sub(currFeatures, scaledPrevFeatures);
                    usedIndicies = getValidFeatureIndicies(status);
                }
            }
        }

        private static int[] getValidFeatureIndicies(KLTFeatureStatus[] status)
        {
            return status.Where(x => x == KLTFeatureStatus.Success).Select((x, idx) => idx).ToArray();
        }

        private static PointF[] Sub(PointF[] arrA, PointF[] arrB)
        {
            PointF[] diff = new PointF[arrA.Length];

            for (int i = 0; i < diff.Length; i++)
            {
                diff[i] = new PointF(arrA[i].X - arrB[i].X, arrA[i].Y - arrB[i].Y);
            }

            return diff;
        }
    }

}
