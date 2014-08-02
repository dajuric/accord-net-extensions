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

        /// <summary>
        ///  Executes a provided action several times and measures average time in milliseconds that was consumed by provided action.
        /// </summary>
        /// <param name="action">User specified action. The parameter is current execution count [0..executionCount-1].</param>
        /// <param name="executionCount">Number of times to execute the specified action.</param>
        /// <returns>Average elapsed time in milliseconds.</returns>
        public static float MeasureAverageTime(Action<int> action, int executionCount = 10)
        {
            var totalElapsed = MeasureTime(() => 
            {
                for (int i = 0; i < executionCount; i++)
                {
                    action(i);
                }
            });

            return totalElapsed / executionCount;
        }
    }
}
