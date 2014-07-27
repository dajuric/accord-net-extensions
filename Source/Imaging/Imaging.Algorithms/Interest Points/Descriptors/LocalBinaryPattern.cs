using Accord.Imaging;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Local Binary Patterns extensions.
    /// </summary>
    public static class LocalBinaryPatternExtensions
    {
        /// <summary>
        /// Local Binary Patterns.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.LocalBinaryPattern"/> for details.</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="blockSize">The size of a block, measured in cells. </param>
        /// <param name="cellSize">The size of a cell, measured in pixels. If set to zero, the entire image will be used at once, forming a single block.</param>
        /// <param name="normalize">Whether to normalize generated histograms.</param>
        /// <returns>Local Binary Patterns result.</returns>
        public static AlgorithmResult<LocalBinaryPattern, List<double[]>> LocalBinaryPattern(this Image<Gray, byte> im,
                                                                                             int blockSize = 3, int cellSize = 6, bool normalize = true)
        {
            LocalBinaryPattern localBinPattern = new LocalBinaryPattern(blockSize, cellSize, normalize);

            var points = localBinPattern.ProcessImage(im.ToAForgeImage(copyAlways: false, failIfCannotCast: true));

            return new AlgorithmResult<LocalBinaryPattern, List<double[]>>(localBinPattern, points);
        }
    }
}
