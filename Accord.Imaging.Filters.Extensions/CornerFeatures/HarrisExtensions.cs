using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging.Filters
{
    public static class HarrisExtensions
    {
        public static List<IntPoint> HarrisCorners<TDepth>(this Image<Gray, TDepth> im, HarrisCornerMeasure measure = HarrisCornerMeasure.Harris, float threshold = 20000f, double sigma = 1.2, int suppression = 3)
            where TDepth : struct
        {
            HarrisCornersDetector harris = new HarrisCornersDetector(measure, threshold, sigma, suppression);
            var points = harris.ProcessImage(im.ToAForgeImage(false, true));

            return points;
        }
    }
}
