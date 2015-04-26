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
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Moments;
using Accord.Extensions.Math.Geometry;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Mean shift is a non-parametric feature-space analysis technique for locating the maxima of a density function. 
    /// Application domains include various image-procesing and computer vision applications such as: image segmentation and object tracking.
    /// <para>
    /// See <a href="http://en.wikipedia.org/wiki/Mean-shift#Mean_shift_for_visual_tracking">Mean-shift</a> for details.
    /// </para>
    /// </summary>
    public class Meanshift
    {
        /// <summary>
        /// Gets the default termination criteria for the mean-shift algorithm: max-iterations: 10, min-error: 1.
        /// </summary>
        public static readonly TermCriteria DEFAULT_TERM = new TermCriteria { MaxIterations = 10, MinError = 1 /*min shift delta*/ };

        /// <summary>
        /// Meanshift algorithm
        /// </summary>
        /// <param name="probabilityMap">Probability map [0-255].</param>
        /// <param name="roi">Initial search area</param>
        /// <param name="termCriteria">Mean shift termination criteria</param>
        /// <param name="centralMoments">Calculated central moments (up to order 2).</param>
        /// <returns>Object area.</returns>
        public static Rectangle Process(Gray<byte>[,] probabilityMap, Rectangle roi, TermCriteria termCriteria, out CentralMoments centralMoments)
        {
            return process(probabilityMap, roi, termCriteria, out centralMoments);
        }

        /// <summary>
        /// Meanshift algorithm
        /// </summary>
        /// <param name="probabilityMap">Probability map [0-1].</param>
        /// <param name="roi">Initial search area</param>
        /// <param name="termCriteria">Mean shift termination criteria.</param>
        /// <returns>Object area.</returns>
        public static Rectangle Process(Gray<byte>[,] probabilityMap, Rectangle roi, TermCriteria termCriteria)
        {
            CentralMoments centralMoments;
            return process(probabilityMap, roi, termCriteria, out centralMoments);
        }

        /// <summary>
        /// Meanshift algorithm
        /// </summary>
        /// <param name="probabilityMap">Probability map [0-1].</param>
        /// <param name="roi">Initial search area</param>
        /// <returns>Object area.</returns>
        public static Rectangle Process(Gray<byte>[,] probabilityMap, Rectangle roi)
        {
            CentralMoments centralMoments;
            return process(probabilityMap, roi, DEFAULT_TERM, out centralMoments);
        }

        private static Rectangle process(Gray<byte>[,] probabilityMap, Rectangle roi, TermCriteria termCriteria, out CentralMoments centralMoments)
        {
            Rectangle imageArea = new Rectangle(0, 0, probabilityMap.Width(), probabilityMap.Height());

            Rectangle searchWindow = roi;
            RawMoments moments = new RawMoments(order: 1);

            // Mean shift with fixed number of iterations
            int i = 0;
            double shift = Byte.MaxValue;
            while (termCriteria.ShouldTerminate(i, shift) == false && !searchWindow.IsEmptyArea())
            {
                // Locate first order moments
                moments.Compute(probabilityMap, searchWindow);

                int shiftX = (int)(moments.CenterX - searchWindow.Width / 2f);
                int shiftY = (int)(moments.CenterY - searchWindow.Height / 2f);

                // Shift the mean (centroid)
                searchWindow.X += shiftX;
                searchWindow.Y += shiftY;

                // Keep the search window inside the image
                searchWindow.Intersect(imageArea);
                
                shift = System.Math.Abs((double)shiftX) + System.Math.Abs((double)shiftY); //for term criteria only
                i++;
            }

            if (searchWindow.IsEmptyArea() == false)
            {
                // Locate second order moments and perform final shift
                moments.Order = 2;
                moments.Compute(probabilityMap, searchWindow);

                searchWindow.X += (int)(moments.CenterX - searchWindow.Width / 2f);
                searchWindow.Y += (int)(moments.CenterY - searchWindow.Height / 2f);

                // Keep the search window inside the image
                searchWindow.Intersect(imageArea);
            }

            centralMoments = new CentralMoments(moments); // moments to be used by camshift
            return searchWindow;
        }
    }
}
