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
