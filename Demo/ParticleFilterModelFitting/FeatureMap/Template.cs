using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Point = AForge.IntPoint;

namespace ParticleFilterModelFitting
{
    public static class Template 
    {
        const int MAX_FEATURE_SIMILARITY = 4;

        public static unsafe float GetScore(Image<Gray, byte> featureMap, IList<Point> points, IList<byte> quantizedOrientations)
        {
            if (points.Count != quantizedOrientations.Count)
                throw new NotSupportedException();

            int numOfFeatures = points.Count;
            float scaleFactor = 1f / (MAX_FEATURE_SIMILARITY * numOfFeatures);

            int similarity = 0;
            for (int i = 0; i < numOfFeatures; i++)
            {
                //template
                var featurePt = points[i];
                var featureAngle = quantizedOrientations[i];

                //image
                var imageAngles = *(byte*)featureMap.GetData(featurePt.Y, featurePt.X);

                //score
                var featureSimilarity = getFeatureSimilarity(featureAngle, imageAngles);
                similarity += featureSimilarity;
            }

            return similarity * scaleFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte getFeatureSimilarity(byte templateAngle, byte imageAngles)
        {
            //the closest 1 on the left...
            byte numOfLeftShifts = 0;

            while (((imageAngles << numOfLeftShifts) & templateAngle) == 0 && numOfLeftShifts < MAX_FEATURE_SIMILARITY)
            {
                numOfLeftShifts++;
            }

            //the closest 1 on the right...
            byte numOfRightShifts = 0;

            while (((imageAngles >> numOfRightShifts) & templateAngle) == 0 && numOfRightShifts < MAX_FEATURE_SIMILARITY)
            {
                numOfRightShifts++;
            }

            //the less shifts, the bigger similarity
            byte similarity = (byte)(MAX_FEATURE_SIMILARITY - Math.Min(numOfLeftShifts, numOfRightShifts));

            return similarity;
        }
    }
}
