using System.Collections.Generic;
using System.Linq;
using Accord.Extensions.Imaging.Filters;
using Accord.Extensions.Imaging.IntegralImage;
using Accord.Extensions.Math.Geometry;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extensions for good features to track.
    /// </summary>
    public static class GoodFeaturesToTrackExtensions
    {
        /// <summary>
        /// Searches the image for the good features to track. 
        /// <para>For each location a Hessian matrix is made and min eig-value is compared against threshold.</para>
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="winSize">Window size.</param>
        /// <param name="minEigVal">Minimum eigen value.</param>
        /// <param name="minimalDistance">Minimum distance from two features.</param>
        /// <returns>List of locations that have eigen value larger than <paramref name="minEigVal"/>.</returns>
        public static List<Point> GoodFeaturesToTrack(this Image<Gray, float> image, int winSize = 10, float minEigVal = 0.3f, float minimalDistance = 3)
        {
            var strengthImg = new Image<Gray, float>(image.Size);

            var Dx = image.Sobel(1, 0, 3);
            var Dy = image.Sobel(0, 1, 3);

            var Dxx = (Dx * Dx).MakeIntegral();
            var Dxy = (Dx * Dy).MakeIntegral();
            var Dyy = (Dy * Dy).MakeIntegral();

            var proc = new ParallelProcessor<bool, bool>(image.Size,
                                                         () => true,
                                                         (_, __, area) =>
                                                         {
                                                             Rectangle srcArea = new Rectangle
                                                             {
                                                                 X = 0,
                                                                 Y = area.Y,
                                                                 Width = image.Width,
                                                                 Height = area.Height + winSize
                                                             };
                                                             srcArea.Intersect(new Rectangle(new Point(), image.Size));
                                                           
                                                             goodFeaturesToTrack(Dxx.GetSubRect(srcArea), Dxy.GetSubRect(srcArea), Dyy.GetSubRect(area),
                                                                                 winSize, minEigVal, strengthImg.GetSubRect(srcArea));
                                                         },
                                                         new ParallelOptions2D {  /*ForceSequential = true*/ },
                                                         winSize);

            proc.Process(true);

            
            var filteredStrengthImg = strengthImg.SupressNonMaxima();
            //var filteredStrengthImg = strengthImg;

            List<float> values;
            var locations = filteredStrengthImg.FindNonZero(out values);

            var sortedFeatures = locations.Zip(values, (f, s) => new { f, s })
                                          .OrderByDescending(x => x.s)
                                          .Select(x => x.f)
                                          .ToList();

            sortedFeatures = sortedFeatures.EnforceMinimalDistance(minimalDistance);

            return sortedFeatures;
        }

        private unsafe static void goodFeaturesToTrack(Image<Gray, float> integralDxx, Image<Gray, float> integralDxy, Image<Gray, float> integralDyy,
                                                                    int winSize, float minEigValue, Image<Gray, float> strengthImg)
        {
            float* ptr = (float*)strengthImg.GetData(winSize / 2, winSize / 2);
            int stride = strengthImg.Stride;

            minEigValue = System.Math.Max(1E-3f, minEigValue);
            int normFactor = winSize * winSize * 255;

            int maxCol = integralDxx.Width - winSize;
            int maxRow = integralDxx.Height - winSize;

            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    var Dxx = integralDxx.GetSum(col, row, winSize, winSize);
                    var Dxy = integralDxy.GetSum(col, row, winSize, winSize);
                    var Dyy = integralDyy.GetSum(col, row, winSize, winSize);

                    var eigenVal = calcMinEigenVal(Dxx, Dxy, Dyy);
                    eigenVal /= normFactor;

                    if (eigenVal > minEigValue)
                    {
                        ptr[col] = eigenVal; //strength image has offset [winSize/2, winSize/2]
                    }
                }

                ptr = (float*)((byte*)ptr + stride);
            }
        }

        private static float calcMinEigenVal(float Dxx, float Dxy, float Dyy)
        {
            //(a-d)^2 + 4 * b* c
            var discriminant = (Dxx - Dyy) * (Dxx - Dyy) + 4 * Dxy * Dxy;

            if (discriminant < 0) 
                return 0;

            var sqrtDiscriminant = (float)System.Math.Sqrt(discriminant);
            var minRealEigVal = ((Dxx + Dyy) - sqrtDiscriminant) / 2;

            return minRealEigVal;
        }
    }
}
