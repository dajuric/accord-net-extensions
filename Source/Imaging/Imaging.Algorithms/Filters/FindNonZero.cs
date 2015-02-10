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
        delegate void FindNonZeroFunc(IImage img, out List<Point> locations, out IList values);
        static Dictionary<Type, FindNonZeroFunc> findNonZeroFuncs;

        static FindNonZeroExtensions()
        {
            findNonZeroFuncs = new Dictionary<Type, FindNonZeroFunc>();

            findNonZeroFuncs.Add(typeof(float), findNonZero_Float);
        }

        /// <summary>
        /// Find non-zero locations in the image.
        /// </summary>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="img">Image.</param>
        /// <returns>List of found non-zero locations.</returns>
        public static List<Point> FindNonZero<TDepth>(this Image<Gray, TDepth> img)
            where TDepth : struct
        {
            List<TDepth> values;
            return FindNonZero(img, out values);
        }

        /// <summary>
        /// Find non-zero locations in the image.
        /// </summary>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="values">Values for the found locations.</param>
        /// <returns>List of found non-zero locations.</returns>
        public static List<Point> FindNonZero<TDepth>(this Image<Gray, TDepth> img, out List<TDepth> values)
            where TDepth: struct
        {
            FindNonZeroFunc findNonZeroFunc = null;
            if (findNonZeroFuncs.TryGetValue(img.ColorInfo.ChannelType, out findNonZeroFunc) == false)
                throw new NotSupportedException(string.Format("Can not perform FindNonZero on an image of type {0}", img.ColorInfo.ChannelType.Name));

            var locations = new List<Point>();
            var _values = new List<TDepth>();

            var proc = new ParallelProcessor<IImage, bool>(img.Size, 
                                                          () => true, 
                                                          (_src, _, area) => 
                                                          {
                                                              List<Point> locationsPatch;
                                                              IList valuesPatch;
                                                              findNonZeroFunc(img.GetSubRect(area), out locationsPatch, out valuesPatch);

                                                              lock(locations)
                                                              lock (_values)
                                                              {
                                                                  foreach (var x in locationsPatch)
                                                                  {
                                                                      locations.Add(x + area.Location);
                                                                  }

                                                                  _values.AddRange(valuesPatch  as IList<TDepth>);
                                                              }
                                                          });

            proc.Process(img);

            values = _values;
            return locations;
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
