using Accord.Imaging;
using AForge;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    ///  Histograms of Oriented Gradients extensions.
    /// </summary>
    public static class HistogramsOfOrientedGradientsExtensions
    {
        /// <summary>
        ///  Histograms of Oriented Gradients.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging. HistogramsOfOrientedGradients"/> for details.</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="numberOfBins">The number of histogram bins.</param>
        /// <param name="blockSize">he size of a block, measured in cells.</param>
        /// <param name="cellSize">The size of a cell, measured in pixels.</param>
        /// <param name="normalize"> True to normalize final histogram feature vectors, false otherwise.</param>
        /// <returns>Histograms of oriented gradients result.</returns>
        public static AlgorithmResult<HistogramsOfOrientedGradients, List<double[]>> HistogramsOfOrientedGradients(this Image<Gray, byte> im, int numberOfBins = 9, int blockSize = 3, int cellSize = 6, bool normalize = true)
        {
            HistogramsOfOrientedGradients hog = new HistogramsOfOrientedGradients(numberOfBins, blockSize, cellSize);
            hog.Normalize = normalize;

            var points = hog.ProcessImage(im.ToAForgeImage(copyAlways: false, failIfCannotCast: true));

            return new AlgorithmResult<HistogramsOfOrientedGradients,List<double[]>>(hog, points);
        }
    }
}
