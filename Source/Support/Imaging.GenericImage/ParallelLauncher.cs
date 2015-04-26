using System;
using System.Runtime.CompilerServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Kernel thread structure which represents the working point.
    /// </summary>
    public struct KernelThread
    {
        /// <summary>
        /// Horizontal offset.
        /// </summary>
        public int X;
        /// <summary>
        /// Vertical offset.
        /// </summary>
        public int Y;
    }

    /// <summary>
    /// Provides a launch method and extension methods for parallel array processing.
    /// </summary>
    public static class ParallelLauncher
    {      
        /// <summary>
        /// launches the specified kernel function in parallel.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array.</param>
        /// <param name="kernel">Kernel function.</param>
        /// <param name="gridX">Horizontal grid size.</param>
        /// <param name="gridY">Vertical grid size</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Launch<T>(this T[,] array, Action<KernelThread, T[,]> kernel, int gridX, int gridY)
        {
            Launch(thread => 
            {
                kernel(thread, array);
            }, 
            gridX, gridY);
        }

        /// <summary>
        /// Launches the specified kernel function in parallel.
        /// </summary>
        /// <param name="kernel">Kernel function.</param>
        /// <param name="gridX">Horizontal grid size.</param>
        /// <param name="gridY">Vertical grid size</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Launch(Action<KernelThread> kernel, int gridX, int gridY)
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
}
