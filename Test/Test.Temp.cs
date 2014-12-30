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
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Accord.Extensions.Imaging;
using System.Threading.Tasks;
using Accord.Extensions;
using System.Runtime.InteropServices;
using System.Threading;
using AForge;

namespace Test
{
    public partial class Test
    {
        public unsafe void Bla()
        {
            /*A<byte>[,] a = new A<byte>[10, 10];
            var h1 = GCHandle.Alloc(a, GCHandleType.Pinned);
            var h2 = GCHandle.Alloc(a, GCHandleType.Pinned);

            Console.WriteLine(h1 + " " + h2);
            return;*/

            var resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            var imgGray = System.Drawing.Bitmap.FromFile(Path.Combine(resourceDir, "testColorBig.jpg")).ToImage<Gray, float>();

            var k = new float[,] { { 5, 2, 1 }, { 6, 0, 5 }, { 5, 6, 4 } }; 
            Image<Gray, float> kernel = k.ToImage();
            //float* kernelPtr = (float*)kernel.ImageData;
            var bla = imgGray.Convert<Bgr, byte>();

            var input = new float[bla.Height, bla.Width];
            var output = new float[bla.Height, bla.Width];

            measure(() =>
            {
                imgGray.SmoothGaussian(3);
            },
            () =>
            {
                process(kernelConvolve, input, output, k, new IntPoint(), bla.Width - 3, bla.Height - 3);

               //b.Save("C:/bla.bmp");
            },

            100, "Old", "New");
        }

        private static long measure(Action action, int numberOfTimes, bool writeMessage = false)
        {
            long s = DateTime.Now.Ticks;

            for (int i = 0; i < numberOfTimes; i++)
            {
                action();
            }

            long e = DateTime.Now.Ticks;
            long elapsed = (e - s) / TimeSpan.TicksPerMillisecond;

            if (writeMessage)
                Console.WriteLine("Per call ~{0} ms", (float)elapsed / numberOfTimes);

            return elapsed;
        }

        private static void measure(Action action1, Action action2, int numberOfTimes, string action1Name, string action2Name)
        {
            Console.WriteLine("Measuring {0}", action1Name);
            var elapsed1 = measure(action1, numberOfTimes);

            Console.WriteLine("Measuring {0}", action2Name);
            var elapsed2 = measure(action2, numberOfTimes);

            if (elapsed1 < elapsed2)
                Console.WriteLine("{0} is faster than {1} ~{2} times. Per call: ~{3} ms", action1Name, action2Name, (float)elapsed2 / elapsed1, (float)elapsed1 / numberOfTimes);
            else
                Console.WriteLine("{0} is slower than {1} ~{2} times. Per call: ~{3} ms", action1Name, action2Name, (float)elapsed1 / elapsed2, (float)elapsed1 / numberOfTimes);
        }

        class KernelThread
        {
            public int X;
            public int Y;
            public int Id;
        }

        private unsafe static void kernelConvert(KernelThread thread, float[,] input, float[,] output, float[,] kernel)
        {
            output[thread.Y, thread.X] = (float)Math.Sin(input[thread.Y, thread.X]) + 5;
        }

        private unsafe static void kernelConvolve(KernelThread thread, float[,] input, float[,] output, float[,] kernel, IntPoint offset)
        {
            float sum = 0;

            fixed (float* inputPtr = &input[thread.Y + offset.Y, thread.X + offset.X], kernelPtr = kernel)
            {
                for (int i = 0; i < 3; i++)
                {
                    var y = thread.Y + i;
                    for (int j = 0; j < 3; j++)
                    {
                        sum += inputPtr[y + thread.X + j] * kernelPtr[j +i];
                    }
                }
            }
           
            output[thread.Y, thread.X] = sum;
        }

        delegate void Convert<TSrc, TDst>(ref TSrc source, ref TDst destination)
            where TSrc: struct
            where TDst: struct;

        private static void convert<TSrc, TDst>(Convert<TSrc, TDst> convert, TSrc[,] source, TDst[,] destination)
            where TSrc: struct
            where TDst: struct
        {
            var gridX = source.GetLength(1);
            var gridY = source.GetLength(0);

            process(th => 
            {
                convert(ref source[th.Y, th.X], ref destination[th.Y, th.X]);
            }, 
            gridX, gridY);
        }

        private static void process(Action<KernelThread, float[,], float[,], float[,], IntPoint> kernel, float[,] arg1, float[,] arg2, float[,] arg3, IntPoint location, int gridX, int gridY)
        {
            process(th => kernel(th, arg1, arg2, arg3, location), gridX, gridY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void process(Action<KernelThread> kernel, int gridX, int gridY)
        {
            System.Threading.Tasks.Parallel.For(0, gridY, (j) =>
            {
                KernelThread th = new KernelThread();

                th.Y = j;
                for (int i = 0; i < gridX; i++)
                {
                    th.X = i;
                    kernel(th);
                }
            });
        }
    }

    public unsafe delegate void UnsafeDel(byte* ptr);

    public struct A<T> : IColor
    { }

    public struct B<T> : IColor
    { }

    public static class SomeExtensions
    {
        public static TDst[,] MyConvert<TSrc, TDst>(this TSrc[,] source)
            where TSrc : IColor
            where TDst : IColor
        {
            return null;
        }
    }

}
