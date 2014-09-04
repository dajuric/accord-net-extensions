using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions.Math;
using Accord.Extensions.Statistics;
using Accord.Extensions;

namespace Test
{
    public partial class Test
    {
        public void TestRunningWeightedVariance()
        {
            var arr = new List<double[]>();
            arr.AddRange(EnumerableExtensions.Create(25, _ => new double[] { 0 }));
            arr.AddRange(EnumerableExtensions.Create(25, _ => new double[] { 10 }));
            arr.AddRange(EnumerableExtensions.Create(25, _ => new double[] { 0 }));
            arr.AddRange(EnumerableExtensions.Create(25, _ => new double[] { 1 }));

            double[] w = EnumerableExtensions.Create(100, _ => (double)1).ToArray();

            Console.WriteLine("Incremental: ");
            arr.RunningVarianceIncDec
                (
                        (idx, avgInc, varInc, avgDec, varDec) => Console.WriteLine("{0}: Var: {1}", idx, varInc + varDec)
                        ,w
                        ,true
                );

            /*double[] arr = new double[] { 1, 2, 3, 4, 5 };
            double[] w = new double[] { 1, 1, 2, 3, 3 };

            Console.WriteLine("Incremental: ");
            arr.RunningVarianceIncremental
                (
                        (idx, avg, var) => Console.WriteLine("Idx: {0} \t Avg: {1} \t Var: {2}", idx, avg, var) 
                        ,w
                );

            Console.WriteLine("Decremental: ");
            arr.RunningVarianceDecremental
                (
                        (idx, avg, var) => Console.WriteLine("Idx: {0} \t Avg: {1} \t Var: {2}", idx, avg, var)
                        ,w
                );*/
        }
    }
}
