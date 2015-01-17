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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accord.Extensions;

namespace MethodCaching
{
    class Program
    {
        static void Main(string[] args)
        {
            var nTimes = 10;

            var elapsedNonCached = Diagnostics.MeasureAverageTime((i) => 
            {
                var arg = i % 2;
                Console.Write("Executing expensive func ({0})...", arg);
                var result = expensiveFunc(arg);
                Console.WriteLine(" Result: " + result);
            }, 
            nTimes);
            Console.WriteLine();

            //fill cache
            expensiveFuncCached(0);
            expensiveFuncCached(1);

            var elapsedCached = Diagnostics.MeasureAverageTime((i) =>
            {
                var arg = i % 2;
                Console.Write("Executing expensive func ({0}) cached...", arg);
                var result = expensiveFuncCached(arg);
                Console.WriteLine(" Result: " + result);
            }, 
            nTimes);

            Console.WriteLine("Average execution time of the non-cached and cached function: {0}, {1} ms.", elapsedNonCached, elapsedCached);
        }

        static string expensiveFuncCached(int arg)
        {
            return MethodCache.Global.Invoke(expensiveFunc, arg);
        }

        static string expensiveFunc(int arg)
        {
            Thread.Sleep(100);
            return "processed " + arg;
        }
    }
}
