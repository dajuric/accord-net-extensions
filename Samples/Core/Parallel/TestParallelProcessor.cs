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
using Accord.Extensions;

namespace Parallel
{
    static class TestParallelProcessor
    {
        public static void Test()
        {
            double[,] field = new double[1080, 1920];

            Console.WriteLine("Testing parallel...");
            var elapsedParallel = Diagnostics.MeasureTime(() =>
            {
                var proc = field.GetProcessor(processFunc);
                proc.Process(field);
            });

            Console.WriteLine("Testing sequential...");
            var elapsedSeq = Diagnostics.MeasureTime(() =>
            {
                var proc = field.GetProcessor(processFunc, forceSequential: true);
                proc.Process(field);
            });

            Console.WriteLine("Parallel: {0} ms; Sequential: {1} ms.", elapsedParallel, elapsedSeq);
        }

        static double processFunc(double val)
        {
            //something complex :)
            return Math.Sin(val) + Math.Cos(val) + Math.Atan(val) + Math.Pow(val, 3) +
                   Math.Sin(val) + Math.Cos(val) + Math.Atan(val) + Math.Pow(val, 3) -
                   Math.Sin(val) + Math.Cos(val) + Math.Atan(val) + Math.Pow(val, 3) *
                   Math.Sin(val) + Math.Cos(val) + Math.Atan(val) + Math.Pow(val, 3) /
                   Math.Sin(val) + Math.Cos(val) + Math.Atan(val) + Math.Pow(val, 3) +
                   Math.Sin(val) + Math.Cos(val) + Math.Atan(val) + Math.Pow(val, 3);
        }
    }

    public static class ArrayElementProcessor
    {
        public static ParallelProcessor<T[,], T[,]> GetProcessor<T>(this T[,] field, Func<T, T> processFunc, bool forceSequential = false)
        {
            var fieldSize = new Size(field.GetLength(1), field.GetLength(0));

            ParallelProcessor<T[,], T[,]> proc = new ParallelProcessor<T[,], T[,]>
                                                       (
                                                          fieldSize,
                                                          () =>
                                                              new T[fieldSize.Height, fieldSize.Width],

                                                           (inputArray, outputArray, patch) =>
                                                           {
                                                               for (int y = patch.Top; y < patch.Bottom; y++)
                                                               {
                                                                   for (int x = patch.Left; x < patch.Right; x++)
                                                                   {
                                                                       outputArray[y, x] = processFunc(inputArray[y, x]);
                                                                   }
                                                               }
                                                           },

                                                            new ParallelOptions2D { ForceSequential = forceSequential }
                                                        );

            return proc;
        }
    }
}
