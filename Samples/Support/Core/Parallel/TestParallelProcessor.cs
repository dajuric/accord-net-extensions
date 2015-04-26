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

namespace Parallel
{
    static class TestParallelProcessor 
    {
        public static void Test()
        {
            double[,] field = new double[1080, 1920];
            field.SetValue<double>(1);

            Console.WriteLine("Testing parallel...");
            var elapsedParallel = Diagnostics.MeasureTime(() =>
            {
                field.Launch((thread, _field) =>
                {
                    _field[thread.Y, thread.X] = processFunc(_field[thread.Y, thread.X]);
                },
                field.Width(), field.Height());
            });

            field.SetValue<double>(1);
            Console.WriteLine("Testing sequential...");
            var elapsedSeq = Diagnostics.MeasureTime(() =>
            {
                var w = field.Width();
                var h = field.Height();

                for (int r = 0; r < h; r++)
                {
                    for (int c = 0; c < w; c++)
                    {
                        field[r, c] = processFunc(field[r, c]);
                    }
                }
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
}
