using Accord.Imaging;
using System;
using Accord.Imaging.Filters;

namespace LINE2D.QueryImage
{
    public class LinearMemoryPyramid
    {
        public Maps[] ResponseMaps = null;

        private LinearMemoryPyramid(Maps[] responseMaps)
        {
            this.ResponseMaps = responseMaps;
        }

        public static LinearMemoryPyramid CreatePyramid(Image<Bgr, Byte> sourceImage)
        {
            Maps[] responseMaps = new Maps[GlobalParameters.NEGBORHOOD_PER_LEVEL.Length];
            Image<Bgr, Byte> image = sourceImage;

            for (int pyrLevel = 0; pyrLevel < GlobalParameters.NEGBORHOOD_PER_LEVEL.Length; pyrLevel++)
            {
                if (pyrLevel > 0)
                {
                    image = image.PyrDown();
                }

                responseMaps[pyrLevel] = Maps.Calculate(image, GlobalParameters.NEGBORHOOD_PER_LEVEL[pyrLevel]);
            }

            return new LinearMemoryPyramid(responseMaps);
        }
    }
}
