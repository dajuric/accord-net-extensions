using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions
{
    public class Diagnostics
    {
        /// <summary>
        /// Executes a provided action and measures time that is consumed by provided action.
        /// </summary>
        /// <param name="action">User specified action.</param>
        /// <returns></returns>
        public static long MeasureTime(Action action)
        {
            long s = DateTime.Now.Ticks;

            action();

            long e = DateTime.Now.Ticks;
            long elapsed = (e - s) / TimeSpan.TicksPerMillisecond;

            return elapsed;
        }
    }
}
