using Accord.Imaging;
using System;
using Accord.Imaging.Filters;
using Accord.Core;

namespace LINE2D.QueryImage
{
    public class LinearizedMapPyramid: IDisposable
    {
        private LinearizedMaps[] pyrMaps = null;

        public LinearizedMaps this[int pyrLevel]
        {
            get { return pyrMaps[pyrLevel]; }
        }

        public int Count { get { return this.pyrMaps.Length; } }

        private LinearizedMapPyramid(LinearizedMaps[] responseMaps)
        {
            this.pyrMaps = responseMaps;
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

        public void Dispose()
        {
            foreach (var responseMap in pyrMaps)
            {
                responseMap.Dispose();
            }
        }
    }
}
