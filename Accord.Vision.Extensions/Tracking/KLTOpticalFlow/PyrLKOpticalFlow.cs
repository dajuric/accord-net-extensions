using Accord.Core;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Accord.Vision
{
    /// <summary>
    /// Pyramidal Kanade-Lucas-Tomasi optical flow.
    /// </summary>
    /// <typeparam name="TColor">Image color. This implementation supports all color types.</typeparam>
    public static class PyrLKOpticalFlow<TColor>
        where TColor: IColor
    {
        /// <summary>
        /// Estimates LK optical flow.
        /// </summary>
        /// <param name="prevImg">Previous image.</param>
        /// <param name="currImg">Current image.</param>
        /// <param name="prevFeatures">Previous features.</param>
        /// <param name="currFeatures">Current features.</param>
        /// <param name="status">Feature status.</param>
        /// <param name="error">Normalized tracking error [0..1].</param>
        /// <param name="windowSize">Aperture size.</param>
        /// <param name="iterations">Maximal number of iterations. If <see cref="minFeatureShift"/> is reached then number of iterations will be lower.</param>
        /// <param name="minFeatureShift">Minimal feature shift in horizontal or vertical direction.</param>
        /// <param name="minEigenValue">Minimal eigen value. 
        /// Eigen values could be interpreted as lengths of ellipse's axes. 
        /// If zero ellipse turn into line therefore there is no corner.</param>
        /// <param name="maxError">Maximal allowable error for <see cref="error"/>.</param>
        /// <param name="pyrLevels">Number of pyramid levels. By using 0 pyramid levels pyramidal implementation reduces to non-pyramidal one.</param>
        public static void EstimateFlow(Image<TColor, float> prevImg, Image<TColor, float> currImg,
                                       PointF[] prevFeatures, out PointF[] currFeatures,
                                       out KLTFeatureStatus[] status, out float[] error,
                                       int windowSize = 15, int iterations = 30, float minFeatureShift = 0.1f, float minEigenValue = 0.01f, float maxError = 0.1f, int pyrLevels = 1)
        {
            var storage = new PyrLKStorage<TColor>(pyrLevels);
            storage.Process(prevImg, currImg);

            EstimateFlow(storage,
                         prevFeatures, out currFeatures,
                         out status, out error,
                         windowSize, iterations, minFeatureShift, minEigenValue, maxError);

            storage.Dispose();
        }

        /// <summary>
        /// Estimates LK optical flow.
        /// </summary>
        /// <param name="storage">Used storage. Number of pyramid levels is specified within storage. Use storage to gain performance in video* by 2x! </param>
        /// <param name="prevImg">Previous image.</param>
        /// <param name="currImg">Current image.</param>
        /// <param name="prevFeatures">Previous features.</param>
        /// <param name="currFeatures">Current features.</param>
        /// <param name="status">Feature status.</param>
        /// <param name="error">Normalized tracking error [0..1].</param>
        /// <param name="windowSize">Aperture size.</param>
        /// <param name="iterations">Maximal number of iterations. If <see cref="minFeatureShift"/> is reached then number of iterations will be lower.</param>
        /// <param name="minFeatureShift">Minimal feature shift in horizontal or vertical direction.</param>
        /// <param name="minEigenValue">Minimal eigen value. 
        /// Eigen values could be interpreted as lengths of ellipse's axes. 
        /// If zero ellipse turn into line therefore there is no corner.</param>
        /// <param name="maxError">Maximal allowable error for <see cref="error"/>.</param>
        public static void EstimateFlow(PyrLKStorage<TColor> storage,
                                        PointF[] prevFeatures, out PointF[] currFeatures,
                                        out KLTFeatureStatus[] status, out float[] error,
                                        int windowSize = 15, int iterations = 30, float minFeatureShift = 0.1f, float minEigenValue = 0.001f, float maxError = 0.1f)
        {
            var initialEstimate = new PointF[prevFeatures.Length];

            currFeatures = new PointF[prevFeatures.Length];
            error = new float[prevFeatures.Length];
            status = new KLTFeatureStatus[prevFeatures.Length];

            var scaledPrevFeatures = prevFeatures.Apply(x => x.PyrDown(storage.PyrLevels));
            var usedIndicies = Enumerable.Range(0, prevFeatures.Length).ToArray(); 

            for (int pyrLevel = storage.PyrLevels; pyrLevel >= 0; pyrLevel--)
            {
                PointF[] levelCurrFeatures;
                float[] levelError;
                KLTFeatureStatus[] levelStatus;
                LKOpticalFlow<TColor>.EstimateFlow(storage, 
                                                   scaledPrevFeatures.Get(usedIndicies), out levelCurrFeatures, out levelStatus, out levelError, 
                                                   windowSize, iterations, minFeatureShift, minEigenValue, maxError, initialEstimate, pyrLevel);

                //update data
                currFeatures.Set(usedIndicies, levelCurrFeatures);
                error.Set(usedIndicies, levelError);
                status.Set(usedIndicies, levelStatus);
                
                if (pyrLevel != 0)
                {
                    scaledPrevFeatures.ApplyInPlace(usedIndicies, x => x.PyrUp());
                    currFeatures.ApplyInPlace(usedIndicies, x => x.PyrUp());

                    initialEstimate = Sub(currFeatures, scaledPrevFeatures);
                    usedIndicies = getValidFeatureIndicies(status);
                }
            }
        }

        private static int[] getValidFeatureIndicies(KLTFeatureStatus[] status)
        {
            return status.Where(x => x == KLTFeatureStatus.Success).Select((x, idx) => idx).ToArray();
        }

        private static PointF[] Sub(PointF[] arrA, PointF[] arrB)
        {
            PointF[] diff = new PointF[arrA.Length];

            for (int i = 0; i < diff.Length; i++)
            {
                diff[i] = new PointF(arrA[i].X - arrB[i].X, arrA[i].Y - arrB[i].Y);
            }

            return diff;
        }
    }

    public static class IEnumerableExtensions
    {
        public static T[] Get<T>(this T[] src, IEnumerable<int> indicies)
        {
            T[] arr = new T[indicies.Count()];

            int i = 0;
            foreach (var idx in indicies)
            {
                arr[i++] = src[idx];
            }

            return arr;
        }

        public static void Set<T>(this T[] src, IEnumerable<int> indicies, T[] newValues)
        {
            int i = 0;
            foreach (var idx in indicies)
            {
                src[idx] = newValues[i++];
            }
        }

        public static void ApplyInPlace<T>(this T[] src, IEnumerable<int> indicies, Func<T,T> func)
        {
            foreach (var idx in indicies)
            {
                src[idx] = func(src[idx]);
            }
        }
    }
}
