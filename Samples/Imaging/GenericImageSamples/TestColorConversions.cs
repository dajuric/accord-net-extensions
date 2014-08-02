#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
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
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Converters;
using Accord.Extensions.Math;

namespace GenericImage
{
    public partial class Test
    {
        public static void WriteAllConversionPaths()
        {
            writeConversionPaths();
        }

        static void writeConversionPaths()
        {
            Console.BufferHeight = 1000;
            Console.BufferWidth = 250;

            var colorTypes = new Type[] { typeof(Color3), typeof(Color4), typeof(Gray), typeof(Bgr), typeof(Hsv), typeof(Complex) };
            var depthTypes = new Type[] { typeof(byte), typeof(short), typeof(int), typeof(float), typeof(double) };

            var allPossibleCombinations = getAllPossibleColors(colorTypes, depthTypes);

            foreach (var colorComb1 in allPossibleCombinations)
            {
                foreach (var colorComb2 in allPossibleCombinations)
                {
                    writeColorConversionPath(colorComb1, colorComb2);
                }
                Console.WriteLine();
            }
        }

        static List<ColorInfo> getAllPossibleColors(Type[] colorTypes, Type[] depthTypes)
        {
            var combinations = new List<ColorInfo>();

            foreach (var colorType in colorTypes)
            {
                foreach (var depthType in depthTypes)
                {
                    var colorInfo = ColorInfo.GetInfo(colorType, depthType);
                    combinations.Add(colorInfo);
                }
            }

            return combinations;
        }

        static void writeColorConversionPath(ColorInfo c1, ColorInfo c2)
        {
            var path = ColorDepthConverter.GetPath(c1, c2);
            //var path = ColorDepthConverter.GetMostInexepnsiveConversionPath(c1, c2);
            Console.Write("{0} => {1}:  ", c1, c2);
            Console.CursorLeft = 40;

            //Console.Write("Cast: {0}", !ColorDepthConverter.ConversionPathCopiesData(path));
            Console.CursorLeft = 55;

            if (path.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("NO PATH");
                Console.ResetColor();
                return;
            }
            else if (path.CopiesData() == false)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("cast");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var stage in path)
            {
                Console.Write("=>  ({0} => {1})", stage.Source, stage.Destination);
            }
            Console.ResetColor();

            Console.WriteLine();
        }
    }
}
