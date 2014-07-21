using System.Collections.Generic;
using Accord.Imaging;
using AForge;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Harris corners extensions.
    /// </summary>
    public static class HarrisExtensions
    {
        /// <summary>
        /// Harris Corners Detector.
        /// </summary>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="im">Image.</param>
        /// <param name="measure">Corners measures.</param>
        /// <param name="threshold">Harris threshold.</param>
        /// <param name="sigma">Gaussian smoothing sigma.</param>
        /// <param name="suppression">Non-maximum suppression window radius.</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///       P. D. Kovesi. MATLAB and Octave Functions for Computer Vision and Image Processing.
        ///       School of Computer Science and Software Engineering, The University of Western Australia.
        ///       Available in: http://www.csse.uwa.edu.au/~pk/Research/MatlabFns/Spatial/harris.m </description></item>
        ///     <item><description>
        ///       C.G. Harris and M.J. Stephens. "A combined corner and edge detector", 
        ///       Proceedings Fourth Alvey Vision Conference, Manchester.
        ///       pp 147-151, 1988.</description></item>
        ///     <item><description>
        ///       Alison Noble, "Descriptions of Image Surfaces", PhD thesis, Department
        ///       of Engineering Science, Oxford University 1989, p45.</description></item>
        ///   </list>
        /// </para>
        /// </remarks>
        public static List<IntPoint> HarrisCorners<TDepth>(this Image<Gray, TDepth> im, HarrisCornerMeasure measure = HarrisCornerMeasure.Harris, float threshold = 20000f, double sigma = 1.2, int suppression = 3)
            where TDepth : struct
        {
            HarrisCornersDetector harris = new HarrisCornersDetector(measure, threshold, sigma, suppression);
            var points = harris.ProcessImage(im.ToAForgeImage(false, true));

            return points;
        }
    }
}
