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

            Console.WriteLine("Parallel: {0}; Sequential: {1}", elapsedParallel, elapsedSeq);
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
