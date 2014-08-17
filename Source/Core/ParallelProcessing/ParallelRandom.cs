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
using System.Security.Cryptography;

namespace Accord.Extensions
{
    /// <summary>
    /// Parallel random class.
    /// </summary>
    /// <typeparam name="TRandom">Random type.</typeparam>
    public static class ParallelRandom<TRandom>
    {
        private static RNGCryptoServiceProvider global = new RNGCryptoServiceProvider();
        [ThreadStatic]
        private static TRandom localRandomGenerator;

        static Func<int, TRandom> localRandCreator;

        /// <summary>
        /// Initializes a new parallel random generator.
        /// </summary>
        /// <param name="localRandCreator">Local random creation function.</param>
        public static void Initialize(Func<int, TRandom> localRandCreator)
        {
            ParallelRandom<TRandom>.localRandCreator = localRandCreator;
        }

        /// <summary>
        /// Gets the random generator of the current thread.
        /// </summary>
        public static TRandom Local 
        {
            get 
            {
                if (localRandomGenerator == null)
                {
                    if (localRandCreator == null)
                        throw new Exception("Use Initialize function to provide local random generator creation function!");

                    byte[] buffer = new byte[4];
                    global.GetBytes(buffer);

                    localRandomGenerator = localRandCreator(BitConverter.ToInt32(buffer, 0));
                }

                return localRandomGenerator;
            }
        }
    }

    /// <summary>
    /// Parallel random class of an System.Random
    /// </summary>
    public static class ParallelRandom
    {
        /// <summary>
        /// Initializes parallel random generator.
        /// </summary>
        static ParallelRandom()
        {
            ParallelRandom<Random>.Initialize((seed) => new Random(seed));
        }

        /// <summary>
        /// Gets the next random number. 
        /// This function encapsulates the rand.Next() function where rand is an instance of System.Random class.
        /// <para>Oppose to rand.Next() this function is thread safe.</para>
        /// </summary>
        /// <returns>New random non-negative number.</returns>
        public static int Next()
        {
            return ParallelRandom<Random>.Local.Next();
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number returned. maxValue must be
        /// greater than or equal to minValue.
        /// </param>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue.</returns>
        public static int Next(int minValue, int maxValue)
        {
            return ParallelRandom<Random>.Local.Next(minValue, maxValue);
        }

        /// <summary>
        /// Fills the provided array of bytes with random values.
        /// </summary>
        /// <param name="buffer">Buffer to fill with random numbers.</param>
        public static void NextBytes(byte[] buffer)
        {
            ParallelRandom<Random>.Local.NextBytes(buffer);
        }
    }
}
