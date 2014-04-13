using System;

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides extension methods for <see cref="Random"/>.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns the random number in user-specified interval.
        /// </summary>
        /// <param name="random">Random generator.</param>
        /// <param name="minimum">Minimum number.</param>
        /// <param name="maximum">MAximujm number.</param>
        /// <returns>Random number in user-specified interval.</returns>
        public static double NextDouble(this Random random, double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
