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
using System.Collections.Generic;
using DotImaging.Primitives2D;
using RangeF = AForge.Range;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides extension methods for rectangle.
    /// </summary>
    public static class RectangleFExtensions
    {
        /// <summary>
        /// Randomizes rectangle position and scale and returns randomized rectangles.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <param name="locationOffset">Minimum location offset for horizontal and vertical direction.</param>
        /// <param name="sizeOffset">Minimum size offset for horizontal and vertical direction.</param>
        /// <param name="nRandomizedRectangles">Number of randomized rectangles to generate.</param>
        /// <param name="rand">Random generator. If null the instance will be generated.</param>
        /// <returns>Randomized rectangles.</returns>
        public static IEnumerable<RectangleF> Randomize(this RectangleF rect, RangeF locationOffset, RangeF sizeOffset, int nRandomizedRectangles, Random rand = null)
        {
            return Randomize(rect, new Pair<RangeF>(locationOffset, locationOffset), new Pair<RangeF>(sizeOffset, sizeOffset), nRandomizedRectangles, rand);
        }

        /// <summary>
        /// Randomizes rectangle position and scale and returns randomized rectangles.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <param name="locationOffset">Minimum location offset for horizontal and vertical direction respectively.</param>
        /// <param name="sizeOffset">Minimum size offset for horizontal and vertical direction respectively.</param>
        /// <param name="nRandomizedRectangles">Number of randomized rectangles to generate.</param>
        /// <param name="rand">Random generator. If null the instance will be generated.</param>
        /// <returns>Randomized rectangles.</returns>
        /// <example>
        /// var img = new Image&lt;Bgr, byte&gt;(640, 480);
        ///
        /// var rect = new RectangleF(50, 50, 100, 50);
        ///
        /// var locationOffsets = new Range(-0.05f, +0.05f);
        /// var sizeOffsets = new Range(0.9f, 1.1f);
        /// var randomizedRects = rect.Randomize(new Pair&lt;Range&gt;(locationOffsets, locationOffsets), new Pair&lt;Range&gt;(sizeOffsets, sizeOffsets), 5);
        /// randomizedRects = randomizedRects.Select(x =&gt; x.SetScaleTo(rect.Size));
        ///
        /// img.Draw(rect, Bgr8.Red, 3);
        ///
        /// foreach (var randomizedRect in randomizedRects)
        /// {
        ///    img.Draw(randomizedRect, Bgr8.Green, 1);
        /// }
        ///
        /// ImageBox.Show(img.ToBitmap(), PictureBoxSizeMode.AutoSize);
        /// return;
        /// </example>
        public static IEnumerable<RectangleF> Randomize(this RectangleF rect, Pair<RangeF> locationOffset, Pair<RangeF> sizeOffset, int nRandomizedRectangles, Random rand = null)
        {
            rand = rand ?? new Random();

            for (int i = 0; i < nRandomizedRectangles; i++)
            {
                var randRect = new RectangleF
                {
                    X = rect.X + rect.Width * (float)rand.NextDouble(locationOffset.First.Min, locationOffset.First.Max),
                    Y = rect.Y + rect.Height * (float)rand.NextDouble(locationOffset.Second.Min, locationOffset.Second.Max),

                    Width = rect.Width * (float)rand.NextDouble(sizeOffset.First.Min, sizeOffset.First.Max),
                    Height = rect.Height * (float)rand.NextDouble(sizeOffset.Second.Min, sizeOffset.Second.Max)
                };

                yield return randRect;
            }
        }
    }
}
