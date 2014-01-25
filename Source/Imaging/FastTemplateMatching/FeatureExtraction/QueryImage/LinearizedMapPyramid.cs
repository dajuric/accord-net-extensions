using Accord.Imaging;
using System;
using Accord.Imaging.Filters;
using Accord.Core;
using System.Collections.Generic;

namespace LINE2D
{
    public class LinearizedMapPyramid: IDisposable
    {
        public static int[] DEFAULT_NEGBORHOOD_PER_LEVEL = new int[] { 5/*, 8*/}; //bigger image towards smaller one

        public LinearizedMaps[] PyramidalMaps { get; private set; }

        private LinearizedMapPyramid(LinearizedMaps[] responseMaps)
        {
            this.PyramidalMaps = responseMaps;
        }

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
