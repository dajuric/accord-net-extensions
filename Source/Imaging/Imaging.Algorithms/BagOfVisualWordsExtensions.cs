using Accord.Imaging;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    ///  Bag of Visual Words extensions.
    /// </summary>
    public static class BagOfVisualWordsExtensions
    {
        /// <summary>
        /// Computes the Bag of Words model.
        /// </summary>
        /// <typeparam name="TPoint">
        /// The <see cref="Accord.Imaging.IFeaturePoint{TFeature}"/> type to be used with this class,
        /// such as <see cref="Accord.Imaging.SpeededUpRobustFeaturePoint"/>.
        /// </typeparam>
        /// <typeparam name="TFeature">
        /// The feature type of the <typeparamref name="TPoint"/>, such
        /// as <see cref="T:double[]"/>.
        /// </typeparam>
        /// <param name="bow">Bag of Visual Words.</param>
        /// <param name="images">The set of images to initialize the model.</param>
        /// <param name="threshold">Convergence rate for the k-means algorithm. Default is 1e-5.</param>
        /// <returns>The list of feature points detected in all images.</returns>
        public static List<TPoint>[] Compute<TPoint, TFeature>(this BagOfVisualWords<TPoint, TFeature> bow,
                                                               Image<Gray, byte>[] images, double threshold = 1e-5)
            where TPoint : IFeatureDescriptor<TFeature>
        {
            var featurePoints = bow.Compute
                (
                   images.Select(x=> x.ToBitmap(copyAlways: false, failIfCannotCast: true)).ToArray(), 
                   threshold
                 );

            return featurePoints;
        }
    }
}
