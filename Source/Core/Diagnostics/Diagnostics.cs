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
using System.IO;

namespace Accord.Extensions
{
    /// <summary>
    /// Contains methods for code performance measurement. 
    /// </summary>
    public static class Diagnostics
    {
        #region Time measure

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

        #endregion

        #region Logging

        //Taken from http://stackoverflow.com/questions/420429/mirroring-console-output-to-a-file and modified.
        /// <summary>
        /// Combined console stream writer. Enables the cloning the console output.
        /// </summary>
        private class CombinedWriter : StreamWriter
        {
            TextWriter console;
            public CombinedWriter(string path, bool append, TextWriter consoleout)
                : base(File.Open(path, append ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write))
            {
                this.console = consoleout;
                base.AutoFlush = true;
            }
            public override void Write(string value)
            {
                console.Write(value);
                //base.Write(value);//do not log writes without line ends as these are only for console display
            }
            public override void WriteLine()
            {
                console.WriteLine();
                //base.WriteLine();//do not log empty writes as these are only for advancing console display
            }
            public override void WriteLine(string value)
            {
                console.WriteLine(value);
                if (value != String.Empty)
                {
                    base.WriteLine(value);
                }
            }
            public new void Dispose()
            {
                base.Dispose();
            }
        }

        static CombinedWriter combinedWriter = null;

        /// <summary>
        /// Starts to clone the console output to a specified file.
        /// <para>If the logging is active the old instance is replaced by a new one.</para>
        /// </summary>
        /// <param name="fileName">Log-file name</param>
        /// <param name="append">True to append to an existing file, false to overwrite it.</param>
        public static void StartConsoleLogging(string fileName, bool append)
        {
            StopConsoleLogging();

            combinedWriter = new CombinedWriter(fileName, append, Console.Out);
            Console.SetOut(combinedWriter);
        }

        /// <summary>
        /// Stops the logging process.
        /// </summary>
        public static void StopConsoleLogging()
        {
            if (combinedWriter != null)
            {
                combinedWriter.Dispose();
                Console.SetOut(Console.Out);
            }
        }

        #endregion
    }
}
