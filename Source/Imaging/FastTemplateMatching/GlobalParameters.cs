using System.Diagnostics;

namespace LINE2D
{
    public static class GlobalParameters
    {
        public const int MIN_GRADIENT_THRESHOLD = 35; //threshold for gradient
        public const int MIN_FEATURE_STRENGTH = 50; //minimum valid gradient strength for a feature
        public const int NUM_OF_QUNATIZED_ORIENTATIONS = 8; //3. i 4. kvadrant se mapiraju na 2. i 1.

        public const int MAX_FEATURE_SIMILARITY = 4; //== MAX_ANGLE_SIMILARITY
        public const int MAX_NUM_OF_FEATURES = short.MaxValue / MAX_FEATURE_SIMILARITY;

        public static int[] MAX_FEATURES_PER_LEVEL = new int[] { 100/*, 100 / 2*/ };
        public static int[] NEGBORHOOD_PER_LEVEL = new int[] { 5/*, 8*/}; //bigger image towards smaller one

        public static int MIN_NUMBER_OF_FEATURES = 30; //minimal number of features to consider that a template is valid


        static GlobalParameters()
        {
            Debug.Assert(MAX_FEATURES_PER_LEVEL.Length == NEGBORHOOD_PER_LEVEL.Length);
            Debug.Assert(NUM_OF_QUNATIZED_ORIENTATIONS <= 8 /*(num of bits in byte)*/);
        }

    }
}
