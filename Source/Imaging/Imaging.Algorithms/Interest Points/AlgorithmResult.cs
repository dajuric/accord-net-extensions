using System;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents algorithm result which is the return value 
    /// of some image processing algorithms which have additional properties and methods.
    /// </summary>
    /// <typeparam name="TAlgorithm">Algorithm type.</typeparam>
    /// <typeparam name="TResult">Result type.</typeparam>
    public class AlgorithmResult<TAlgorithm, TResult>
    {
        /// <summary>
        /// Creates new algorithm result.
        /// </summary>
        /// <param name="algorithm">Algorithm.</param>
        /// <param name="result">Result.</param>
        public AlgorithmResult(TAlgorithm algorithm, TResult result)
        {
            this.Result = result;
            this.Algorithm = algorithm;
        }

        /// <summary>
        /// Gets the underlying algorithm.
        /// </summary>
        public TAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// Gets the result of the feature detector.
        /// </summary>
        public TResult Result { get; private set; }
    }
}
