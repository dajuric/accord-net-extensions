using Accord.Core;
using AForge.Imaging.Filters;

namespace Accord.Imaging
{
    public static class CannyExtensions
    {
        /// <summary>
        /// Applies Canny filter on specified image. (uses AForge implementation)
        /// </summary>
        /// <param name="im">image</param>
        /// <param name="lowThreshold">Low threshold value used for hysteresis</param>
        /// <param name="highThreshold">High threshold value used for hysteresis</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <param name="gaussianSize">Gaussian filter size</param>
        /// <returns>Processed image with Canny filter</returns>
        public static Image<Gray, byte> Canny(this Image<Gray, byte> im, byte lowThreshold = 20, byte highThreshold = 100, double sigma = 1.4, int gaussianSize = 5)
        {
            return Canny<Gray, byte>(im, lowThreshold, highThreshold, sigma, gaussianSize);
        }

        private static Image<TColor, TDepth> Canny<TColor, TDepth>(this Image<TColor, TDepth> im, byte lowThreshold, byte highThreshold, double sigma, int gaussianSize = 5)
            where TColor : IColor
            where TDepth : struct
        {
            var res = im.CopyBlank();

            CannyEdgeDetector canny = new CannyEdgeDetector(lowThreshold, highThreshold, sigma);
            canny.GaussianSize = gaussianSize;
            
            canny.Apply(
                im.ToAForgeImage(copyAlways: false, failIfCannotCast: true),
                res.ToAForgeImage(copyAlways: false, failIfCannotCast: true)
                      );
          
            return res;
        }


    }
}
