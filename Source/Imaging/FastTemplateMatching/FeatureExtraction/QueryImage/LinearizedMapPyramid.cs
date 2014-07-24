using System;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Filters;

namespace Accord.Extensions.Imaging.Algorithms.LNE2D
{
    /// <summary>
    /// Computes linearized maps for each image in the image pyramid.. See <see cref="LinearizedMaps"/> for details.
    /// </summary>
    public class LinearizedMapPyramid: IDisposable
    {
        /// <summary>
        /// Default neighborhood spread per level.
        /// </summary>
        public static int[] DEFAULT_NEGBORHOOD_PER_LEVEL = new int[] { 5/*, 8*/}; //bigger image towards smaller one

        /// <summary>
        /// Gets linearized maps.
        /// </summary>
        public LinearizedMaps[] PyramidalMaps { get; private set; }

        private LinearizedMapPyramid(LinearizedMaps[] responseMaps)
        {
            this.PyramidalMaps = responseMaps;
        }

        /// <summary>
        /// Creates linearized maps pyramid.
        /// </summary>
        /// <param name="sourceImage">Source image.</param>
        /// <param name="minGradientMagnitude">Minimal gradient value threshold.</param>
        /// <param name="neigborhoodPerLevel">Neighborhood per level. If null default neighborhood is going to be used.</param>
        /// <returns>Pyramid of linearized maps.</returns>
        public static LinearizedMapPyramid CreatePyramid(Image<Gray, Byte> sourceImage, int minGradientMagnitude = 35, params int[] neigborhoodPerLevel)
        {
            return CreatePyramid(sourceImage, 
                                 source => 
                                 {
                                     Image<Gray, int> sqrMagImg;
                                     return GradientComputation.Compute(sourceImage, out sqrMagImg, minGradientMagnitude);
                                 },
                                 neigborhoodPerLevel);
        }

        /// <summary>
        /// Creates linearized maps pyramid.
        /// </summary>
        /// <param name="sourceImage">Source image.</param>
        /// <param name="minGradientMagnitude">Minimal gradient value threshold.</param>
        /// <param name="neigborhoodPerLevel">Neighborhood per level. If null default neighborhood is going to be used.</param>
        /// <returns>Pyramid of linearized maps.</returns>
        public static LinearizedMapPyramid CreatePyramid(Image<Bgr, Byte> sourceImage, int minGradientMagnitude = 35, params int[] neigborhoodPerLevel)
        {
            return CreatePyramid(sourceImage,
                                  source =>
                                  {
                                      Image<Gray, int> sqrMagImg;
                                      return GradientComputation.Compute(sourceImage, out sqrMagImg, minGradientMagnitude);
                                  },
                                 neigborhoodPerLevel);
        }

        /// <summary>
        /// Creates linearized maps pyramid.
        /// </summary>
        /// <param name="sourceImage">Source image.</param>
        /// <param name="orientationImgCalc">Orientation image calculation function.</param>
        /// <param name="neigborhoodPerLevel">Neighborhood per level. If null default neighborhood is going to be used.</param>
        /// <returns>Pyramid of linearized maps.</returns>
        public static LinearizedMapPyramid CreatePyramid<TColor, TDepth>(Image<TColor, TDepth> sourceImage, Func<Image<TColor, TDepth>, Image<Gray, int>> orientationImgCalc, params int[] neigborhoodPerLevel)
            where TColor: IColor
            where TDepth: struct
        {
            neigborhoodPerLevel = (neigborhoodPerLevel == null || neigborhoodPerLevel.Length == 0) ? DEFAULT_NEGBORHOOD_PER_LEVEL : neigborhoodPerLevel;

            int nPyramids = neigborhoodPerLevel.Length;
            LinearizedMaps[] responseMaps = new LinearizedMaps[nPyramids];
            var image = sourceImage;

            for (int pyrLevel = 0; pyrLevel < nPyramids; pyrLevel++)
            {
                if (pyrLevel > 0)
                {
                    image = image.PyrDown();
                }

                Image<Gray, int> orientationImg = orientationImgCalc(sourceImage);
                responseMaps[pyrLevel] = new LinearizedMaps(orientationImg, neigborhoodPerLevel[pyrLevel]);
            }

            return new LinearizedMapPyramid(responseMaps);
        }

        #region IDisposable Interface

        /// <summary>
        /// Disposes linear map pyramid.
        /// </summary>
        public void Dispose()
        {
            foreach (var responseMap in PyramidalMaps)
            {
                responseMap.Dispose();
            }
        }

        #endregion
    }
}
