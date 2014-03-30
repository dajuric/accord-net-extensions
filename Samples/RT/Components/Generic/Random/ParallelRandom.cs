using System;
using System.Security.Cryptography;

namespace RT
{
    public static class ParallelRandom<TRandom>
    {
        private static RNGCryptoServiceProvider global = new RNGCryptoServiceProvider();
        [ThreadStatic]
        private static TRandom localRandomGenerator;

        static Func<int, TRandom> localRandCreator;

        public static void Initialize(Func<int, TRandom> localRandCreator)
        {
            ParallelRandom<TRandom>.localRandCreator = localRandCreator;
        }

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

    public static class ParallelRandom
    {
        static ParallelRandom()
        {
            ParallelRandom<Random>.Initialize((seed) => new Random(seed));
        }

        public static int Next()
        {
            return ParallelRandom<Random>.Local.Next();
        }
    }
}
