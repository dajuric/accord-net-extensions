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

using System.Collections;
using System.Collections.Generic;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains methods for finding non-zero pixels.
    /// </summary>
    public static class FindNonZeroExtensions
    {
        /// <summary>
        /// Find non-zero locations in the image.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="values">Found non-zero values at the returned positions.</param>
        /// <returns>List of found non-zero locations.</returns>
        public static List<Point> FindNonZero(this Gray<float>[,] img, out IList<float> values)
        {
            List<Point> locationsPatch;
            IList valuesPatch;

            using (var uImg = img.Lock())          
            {
                findNonZero_Float(uImg, out locationsPatch, out valuesPatch);
            }

            values = valuesPatch as IList<float>;
            return locationsPatch;
        }

        private unsafe static void findNonZero_Float(IImage img, out List<Point> locations, out IList values)
        {
            locations = new List<Point>();
            var _values = new List<float>();

            float* ptr = (float*)img.ImageData;
            int stride = img.Stride;

            int width = img.Width;
            int height = img.Height;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var val = ptr[col];
                    
                    if (val != 0)
                    {
                        locations.Add(new Point(col, row));
                        _values.Add(val);
                    }
                }

                ptr = (float*)((byte*)ptr + stride);
            }

            values = _values;
        }
    }
}
