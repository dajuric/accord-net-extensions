using Accord.Imaging;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Haralick textural feature extractor extensions.
    /// </summary>
    public static class HaralickExtensions
    {
        /// <summary>
        /// Haralick textural feature extractor.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Haralick"/> for details.</para>
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="cellSize">The size of a computing cell, measured in pixels. Default is 0 (use whole image at once).</param>
        /// <param name="normalize">Whether to normalize generated histograms.</param>
        /// <param name="degrees">The angulation degrees on which the <see cref="Accord.Imaging.HaralickDescriptor">Haralick's features</see> should be computed. Default is to use all directions.</param>
        /// <param name="mode">
        /// The mode of operation of this Haralick's textural feature extractor.
        /// The mode determines how the different features captured by the <see cref="Accord.Imaging.HaralickDescriptor"/> are combined.
        /// </param>
        /// <param name="features">The number of features to extract using the <see cref="Accord.Imaging.HaralickDescriptor"/>. By default, only the first 13 original Haralick's features will be used.</param>
        /// <returns>Haralicks's detector result.</returns>
        public static AlgorithmResult<Haralick, List<double[]>> Haralick(this Image<Gray, byte> im, int cellSize = 0, bool normalize = true, CooccurrenceDegree[] degrees = null, HaralickMode mode = HaralickMode.NormalizedAverage, int features = 13)
        {
            Haralick haralick = new Accord.Imaging.Haralick(cellSize, normalize, degrees);
            haralick.Mode = mode;
            haralick.Features = features;

            var points = haralick.ProcessImage(im.ToAForgeImage(copyAlways: false, failIfCannotCast: true));

            return new AlgorithmResult<Haralick, List<double[]>>(haralick, points);
        }
    }
}