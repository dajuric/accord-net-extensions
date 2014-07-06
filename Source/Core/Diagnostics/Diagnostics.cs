using System;

namespace Accord.Extensions
{
    /// <summary>
    /// Contains methods for code performance measurement. 
    /// </summary>
    public static class Diagnostics
    {
        /// <summary>
        /// Executes a provided action and measures time in milliseconds that was consumed by provided action.
        /// </summary>
        /// <param name="action">User specified action.</param>
        /// <returns>Elapsed time in milliseconds.</returns>
        public static long MeasureTime(Action action)
        {
            var s = DateTime.Now.Ticks;

            action();

            var e = DateTime.Now.Ticks;
            long elapsed = (e - s) / TimeSpan.TicksPerMillisecond;

            return elapsed;
        }
    }
}
