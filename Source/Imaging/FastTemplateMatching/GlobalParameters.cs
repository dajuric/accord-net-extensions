using System;
using System.Diagnostics;

namespace Accord.Extensions.Imaging.Algorithms.LNE2D
{
    /// <summary>
    /// Contains global parameters for LINE2D template matching.
    /// <para>Values that are defined work well in majority of situations (but can be changed if an user wants to - recompilation).</para>
    /// </summary>
    public static class GlobalParameters
    {
        /// <summary>
        /// Number of quantized orientations. Needed for feature extraction.
        /// <para>Maximum value is 8 (number of bits in byte). 3rd i 4th quadrant are mapped to 2nd and 1st</para>
        /// <para>Although this number can be lower than 8 precision can be lost, but the preprocessing stage can be speeded-up (lower number of response maps).</para>
        /// </summary>
        public const int NUM_OF_QUNATIZED_ORIENTATIONS = 8; 
        /// <summary>
        /// Max feature similarity. Default value is 4. Needed for feature (template) comparison.
        /// This constant can be interpreted as max angle similarity.
        /// </summary>
        public const int MAX_FEATURE_SIMILARITY = 4; 

        /// <summary>
        /// Max number of features per template. It can be used for template feature extraction.
        /// </summary>
        public const int MAX_NUM_OF_FEATURES = short.MaxValue / MAX_FEATURE_SIMILARITY;

        /// <summary>
        /// Max number of features that can be added to a buffer of type <see cref="System.Byte"/>.
        /// </summary>
        public const int MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE = Byte.MaxValue / GlobalParameters.MAX_FEATURE_SIMILARITY;

        static GlobalParameters()
        {
            Debug.Assert(NUM_OF_QUNATIZED_ORIENTATIONS <= 8 /*(num of bits in byte)*/);
        }

    }
}
