using Accord.Imaging;
using Accord.Imaging.Moments;
using System;
using System.Drawing;

namespace Accord.Vision
{
    public class Meanshift
    {
        public static readonly TermCriteria DEFAULT_TERM = new TermCriteria { MaxIterations = 10, MinError = 1 /*min shift delta*/ };

        /// <summary>
        /// Meanshift algorithm
        /// </summary>
        /// <param name="probabilityMap">Probability map [0-1].</param>
        /// <param name="roi">Initial search area</param>
        /// <param name="termCriteria">Mean shift termination criteria (PLEASE DO NOT REMOVE (but you can move it) THIS CLASS; PLEASE!!!)</param>
        /// <param name="centralMoments">Calculated central moments.</param>
        /// <returns>Object area.</returns>
        public static Rectangle Process(Image<Gray, byte> probabilityMap, Rectangle roi, TermCriteria termCriteria, out CentralMoments centralMoments)
        {
            return process(probabilityMap, roi, termCriteria, out centralMoments);
        }

        /// <summary>
        /// Meanshift algorithm
        /// </summary>
        /// <param name="probabilityMap">Probability map [0-1].</param>
        /// <param name="roi">Initial search area</param>
        /// <param name="termCriteria">Mean shift termination criteria (PLEASE DO NOT REMOVE (but you can move it) THIS CLASS; PLEASE!!!)</param>
        /// <returns>Object area.</returns>
        public static Rectangle Process(Image<Gray, byte> probabilityMap, Rectangle roi, TermCriteria termCriteria)
        {
            CentralMoments centralMoments;
            return process(probabilityMap, roi, termCriteria, out centralMoments);
        }

        /// <summary>
        /// Meanshift algorithm
        /// </summary>
        /// <param name="probabilityMap">Probability map [0-1].</param>
        /// <param name="roi">Initial search area</param>
        /// <returns>Object area.</returns>
        public static Rectangle Process(Image<Gray, byte> probabilityMap, Rectangle roi)
        {
            CentralMoments centralMoments;
            return process(probabilityMap, roi, DEFAULT_TERM, out centralMoments);
        }

        private static Rectangle process(Image<Gray, byte> probabilityMap, Rectangle roi, TermCriteria termCriteria, out CentralMoments centralMoments)
        {
            Rectangle imageArea = new Rectangle(0, 0, probabilityMap.Width, probabilityMap.Height);

            Rectangle searchWindow = roi;
            RawMoments moments = new RawMoments(order: 1);

            // Mean shift with fixed number of iterations
            int i = 0;
            double shift = Byte.MaxValue;
            while (termCriteria.ShouldTerminate(i, shift) == false && !searchWindow.IsEmpty)
            {
                // Locate first order moments
                moments.Compute(probabilityMap.GetSubRect(searchWindow));

                int shiftX = (int)(moments.CenterX - searchWindow.Width / 2f);
                int shiftY = (int)(moments.CenterY - searchWindow.Height / 2f);

                // Shift the mean (centroid)
                searchWindow.X += shiftX;
                searchWindow.Y += shiftY;

                // Keep the search window inside the image
                searchWindow.Intersect(imageArea);
                
                shift = System.Math.Abs((double)shiftX) + System.Math.Abs((double)shiftY); //for term criteria only
            }

            // Locate second order moments and perform final shift
            moments.Order = 2;
            moments.Compute(probabilityMap.GetSubRect(searchWindow));

            searchWindow.X += (int)(moments.CenterX - searchWindow.Width / 2f);
            searchWindow.Y += (int)(moments.CenterY - searchWindow.Height / 2f);

            // Keep the search window inside the image
            searchWindow.Intersect(imageArea);

            centralMoments = new CentralMoments(moments); // moments to be used by camshift
            return searchWindow;
        }
    }
}
