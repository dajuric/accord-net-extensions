#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using Accord.Imaging;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

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
                                                               Gray<byte>[][,] images, double threshold = 1e-5)
            where TPoint : IFeatureDescriptor<TFeature>
        {
            var uImages = images.Select(x => x.Lock());

            var featurePoints = bow.Compute
                (
                   uImages.Select(x=> x.AsBitmap()).ToArray(),
                   threshold
                 );

            uImages.ForEach(x => x.Dispose());

            return featurePoints;
        }
    }
}
