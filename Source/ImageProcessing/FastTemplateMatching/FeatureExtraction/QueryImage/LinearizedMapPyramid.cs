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
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Filters;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// Computes linearized maps for each image in the image pyramid.. See <see cref="LinearizedMaps"/> for details.
    /// </summary>
    public class LinearizedMapPyramid
    {
        /// <summary>
        /// Default neighborhood spread per level.
        /// </summary>
        public static int[] DEFAULT_NEGBORHOOD_PER_LEVEL = new int[] { 5/*, 8*/}; //bigger image towards smaller one

        /// <summary>
        /// Gets linearized maps.
        /// </summary>
        public LinearizedMaps[] PyramidalMaps { get; private set; }

        private LinearizedMapPyramid(LinearizedMaps[] responseMaps)
        {
            this.PyramidalMaps = responseMaps;
        }

        /// <summary>
        /// Creates linearized maps pyramid.
        /// </summary>
        /// <param name="sourceImage">Source image.</param>
        /// <param name="minGradientMagnitude">Minimal gradient value threshold.</param>
        /// <param name="neigborhoodPerLevel">Neighborhood per level. If null default neighborhood is going to be used.</param>
        /// <returns>Pyramid of linearized maps.</returns>
        public static LinearizedMapPyramid CreatePyramid(Gray<byte>[,] sourceImage, int minGradientMagnitude = 35, params int[] neigborhoodPerLevel)
        {
            return CreatePyramid(sourceImage, 
                                 source => 
                                 {
                                     Gray<int>[,] sqrMagImg;
                                     return GradientComputation.Compute(sourceImage, out sqrMagImg, minGradientMagnitude);
                                 },
                                 neigborhoodPerLevel);
        }

        /// <summary>
        /// Creates linearized maps pyramid.
        /// </summary>
        /// <param name="sourceImage">Source image.</param>
        /// <param name="minGradientMagnitude">Minimal gradient value threshold.</param>
        /// <param name="neigborhoodPerLevel">Neighborhood per level. If null default neighborhood is going to be used.</param>
        /// <returns>Pyramid of linearized maps.</returns>
        public static LinearizedMapPyramid CreatePyramid(Bgr<byte>[,] sourceImage, int minGradientMagnitude = 35, params int[] neigborhoodPerLevel)
        {
            return CreatePyramid(sourceImage,
                                  source =>
                                  {
                                      Gray<int>[,] sqrMagImg;
                                      return GradientComputation.Compute(sourceImage, out sqrMagImg, minGradientMagnitude);
                                  },
                                 neigborhoodPerLevel);
        }

        /// <summary>
        /// Creates linearized maps pyramid.
        /// </summary>
        /// <param name="sourceImage">Source image.</param>
        /// <param name="orientationImgCalc">Orientation image calculation function.</param>
        /// <param name="neigborhoodPerLevel">Neighborhood per level. If null default neighborhood is going to be used.</param>
        /// <returns>Pyramid of linearized maps.</returns>
        public static LinearizedMapPyramid CreatePyramid<TColor>(TColor[,] sourceImage, Func<TColor[,], Gray<int>[,]> orientationImgCalc, params int[] neigborhoodPerLevel)
            where TColor: struct, IColor<byte>
        {
            neigborhoodPerLevel = (neigborhoodPerLevel == null || neigborhoodPerLevel.Length == 0) ? DEFAULT_NEGBORHOOD_PER_LEVEL : neigborhoodPerLevel;

            int nPyramids = neigborhoodPerLevel.Length;
            LinearizedMaps[] responseMaps = new LinearizedMaps[nPyramids];
            var image = sourceImage;

            for (int pyrLevel = 0; pyrLevel < nPyramids; pyrLevel++)
            {
                if (pyrLevel > 0)
                { 
                    image = image.PyrDown();
                }

                Gray<int>[,] orientationImg = orientationImgCalc(sourceImage);
                responseMaps[pyrLevel] = new LinearizedMaps(orientationImg, neigborhoodPerLevel[pyrLevel]);
            }

            return new LinearizedMapPyramid(responseMaps);
        }
    }
}
