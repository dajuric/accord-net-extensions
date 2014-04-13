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
    }
}
