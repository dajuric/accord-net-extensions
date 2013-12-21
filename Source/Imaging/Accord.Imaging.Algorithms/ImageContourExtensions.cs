using System.Collections.Generic;
using Point = AForge.IntPoint;

namespace Accord.Imaging
{
    public static class ImageContourExtensions
    {
        /// <summary>
        /// Extracts the contour from a single object in a grayscale image. (uses Accord built-in function)
        /// </summary>
        /// <param name="threshold">The pixel value threshold above which a pixel
        /// is considered black (belonging to the object). Default is zero.</param>
        public static List<Point> FindContour(this Image<Gray, byte> im, byte threshold = 0)
        {
            BorderFollowing bf = new BorderFollowing(threshold);
            return bf.FindContour(im.ToAForgeImage(copyAlways: false, failIfCannotCast: true));
        }
    }
}
