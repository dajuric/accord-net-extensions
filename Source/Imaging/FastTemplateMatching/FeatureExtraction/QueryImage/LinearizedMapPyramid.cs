using Accord.Imaging;
using System;
using Accord.Imaging.Filters;
using Accord.Core;
using System.Collections.Generic;

namespace LINE2D
{
    public class LinearizedMapPyramid: IDisposable
    {
        public LinearizedMaps[] PyramidalMaps { get; set; }

        private LinearizedMapPyramid(LinearizedMaps[] responseMaps)
        {
            this.PyramidalMaps = responseMaps;
        }

        public static LinearizedMapPyramid CreatePyramid(Image<Gray, Byte> sourceImage)
        {
            return CreatePyramid(sourceImage, (source) =>
            {
                Image<Gray, int> orientationImg;
                GradientOrientation.ComputeGradient(sourceImage, out orientationImg, GlobalParameters.MIN_GRADIENT_THRESHOLD);
                return orientationImg;
            });
        }

        public static LinearizedMapPyramid CreatePyramid(Image<Bgr, Byte> sourceImage)
        {
            return CreatePyramid(sourceImage, (source) => 
            {
                Image<Gray, int> orientationImg;
                GradientOrientation.ComputeGradient(sourceImage, out orientationImg, GlobalParameters.MIN_GRADIENT_THRESHOLD);
                return orientationImg;
            });
        }

        public static LinearizedMapPyramid CreatePyramid<TColor, TDepth>(Image<TColor, TDepth> sourceImage, Func<Image<TColor, TDepth>, Image<Gray, int>> orientationImgCalc)
            where TColor: IColor
            where TDepth: struct
        {
            LinearizedMaps[] responseMaps = new LinearizedMaps[GlobalParameters.NEGBORHOOD_PER_LEVEL.Length];
            var image = sourceImage;

            for (int pyrLevel = 0; pyrLevel < GlobalParameters.NEGBORHOOD_PER_LEVEL.Length; pyrLevel++)
            {
                if (pyrLevel > 0)
                {
                    image = image.PyrDown();
                }

                Image<Gray, int> orientationImg = orientationImgCalc(sourceImage);
                responseMaps[pyrLevel] = new LinearizedMaps(orientationImg, GlobalParameters.NEGBORHOOD_PER_LEVEL[pyrLevel]);
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
