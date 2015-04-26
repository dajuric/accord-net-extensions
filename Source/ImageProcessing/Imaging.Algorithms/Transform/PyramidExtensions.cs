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

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extension methods for pyramid image calculation.
    /// </summary>
    public static class PyramidExtensions 
    {
        /// <summary>
        /// Resides the input image by <paramref name="level"/> * <paramref name="downSampleFactor"/>.
        /// <para>It is assumed that input image is already blurred.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="im">Input image.</param>
        /// <param name="level">Pyramid level. If zero an original image will be returned.</param>
        /// <param name="downSampleFactor">Down sample factor.</param>
        /// <returns>Down-sampled image.</returns>
        public static TColor[,] PyrDown<TColor>(this TColor[,] im, int level = 1, int downSampleFactor = 2)
            where TColor : struct, IColor
        {
            if (level < 0)
                new ArgumentOutOfRangeException("level", "Level must be greater or equal than zero.");

            if (level == 0)
                return im;

            double pyrScale = GetPyramidScale(level, downSampleFactor);

            Size newSize = new Size
            {
                Width = (int)(im.Width() * pyrScale),
                Height = (int)(im.Height() * pyrScale)
            };

           return ResizeNearsetNeighbur.Resize(im, newSize);
        }

        /// <summary>
        /// Gets pyramid scale for the specified level.
        /// </summary>
        /// <param name="levelDepth">Pyramid level.</param>
        /// <param name="downSampleFactor">Down sample factor.</param>
        /// <returns>Resize scale.</returns>
        public static double GetPyramidScale(int levelDepth, float downSampleFactor = 2)
        {
            double factor = System.Math.Pow(downSampleFactor, levelDepth);
            factor = 1 / factor;

            return factor;
        }
    }
}
