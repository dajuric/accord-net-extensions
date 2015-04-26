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

using AForge.Imaging.Filters;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Canny extensions.
    /// </summary>
    public static class CannyExtensions
    {
        /// <summary>
        /// Applies Canny filter on specified image. (uses AForge implementation)
        /// </summary>
        /// <param name="im">image</param>
        /// <param name="lowThreshold">Low threshold value used for hysteresis</param>
        /// <param name="highThreshold">High threshold value used for hysteresis</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <param name="gaussianSize">Gaussian filter size</param>
        /// <returns>Processed image with Canny filter</returns>
        public static Gray<byte>[,] Canny(this Gray<byte>[,] im, byte lowThreshold = 20, byte highThreshold = 100, double sigma = 1.4, int gaussianSize = 5)
        {
            CannyEdgeDetector canny = new CannyEdgeDetector(lowThreshold, highThreshold, sigma);
            canny.GaussianSize = gaussianSize;

            return im.ApplyFilter(canny);
        }
    }
}
