using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math.Geometry;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using Accord.Core;

namespace Accord.Vision
{
    /// <summary>
    /// Lucas-Kanade optical flow.
    /// </summary>
    /// <typeparam name="TColor">Image color. This implementation supports all color types.</typeparam>
    public static class LKOpticalFlow<TColor> where TColor:IColor
    {
        static PyrLKStorage<TColor> storage;
        static int storagePyrLevel;

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
        /// <param name="initialEstimate">Initial estimate shifts input features by specified amount. Default is zero. 
        /// Used for pyramidal implementation.</param>
        public static void EstimateFlow(Image<TColor, float> prevImg, Image<TColor, float> currImg, 
                                        PointF[] prevFeatures, out PointF[] currFeatures, 
                                        out KLTFeatureStatus[] status, out float[] error, 
                                        int windowSize = 15, int iterations = 30, float minFeatureShift = 0.1f, float minEigenValue = 0.01f, float maxError = 0.1f, PointF[] initialEstimate = null)
        { 
            storage = new PyrLKStorage<TColor>(0);
            storage.Process(prevImg, currImg);

            EstimateFlow(storage, 
                         prevFeatures, out currFeatures, out status, out error, 
                         windowSize, iterations, minFeatureShift, minEigenValue, maxError, initialEstimate, 0);

            storage.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage">Memory storage used for pyramid and derivative calculation. 
        /// By using storage instead images if using sequential image input (e.g. frame by frame video) </param>
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
        /// <param name="initialEstimate">Initial estimate shifts input features by specified amount. Default is zero. 
        /// <param name="storagePyrLevel">Used pyramid level for the input storage. Default is zero.</param>
        public static void EstimateFlow(PyrLKStorage<TColor> storage,
                                        PointF[] prevFeatures, out PointF[] currFeatures,
                                        out KLTFeatureStatus[] status, out float[] error,
                                        int windowSize = 15, int iterations = 30, float minFeatureShift = 0.1f, float minEigenValue = 0.01f, float maxError = 0.1f, 
                                        PointF[] initialEstimate = null, int storagePyrLevel = 0)
        {
            LKOpticalFlow<TColor>.storage = storage;
            LKOpticalFlow<TColor>.storagePyrLevel = storagePyrLevel;
            initialEstimate = initialEstimate ?? new PointF[prevFeatures.Length];

            PointF[] _currFeatures = new PointF[prevFeatures.Length];
            KLTFeatureStatus[] _status = new KLTFeatureStatus[prevFeatures.Length];
            float[] _error = new float[prevFeatures.Length];

            Parallel.For(0, prevFeatures.Length, (int featureIdx) =>
            //for (int featureIdx = 0; featureIdx < prevFeatures.Length; featureIdx++)
            {
                PointF currFeature;
                float featureError;
                KLTFeatureStatus featureStatus;

                EstimateFeatureFlow(storage.PrevImgPyr[storagePyrLevel], storage.CurrImgPyr[storagePyrLevel], 
                                    prevFeatures[featureIdx], out currFeature, out featureStatus, out featureError, windowSize, iterations, minFeatureShift, minEigenValue, maxError, initialEstimate[featureIdx]);

                lock (_currFeatures)
                    lock (_error)
                        lock (_status)
                        {
                            _currFeatures[featureIdx] = currFeature;
                            _error[featureIdx] = featureError;
                            _status[featureIdx] = featureStatus;
                        }
            });

            currFeatures = _currFeatures;
            error = _error;
            status = _status;
        }

        private static void EstimateFeatureFlow(Image<TColor, float> prevImg, Image<TColor, float> currImg, 
                                                PointF prevFeature, 
                                                out PointF currFeature, out KLTFeatureStatus featureStatus, out float featureError, 
                                                int windowSize, int iterations, float minFeatureShift, float minEigenValue, float maxError, PointF initialEstimate = default(PointF))
        {
            minEigenValue = System.Math.Max(1E-4f, minEigenValue);
            currFeature = new PointF(prevFeature.X + initialEstimate.X, prevFeature.Y + initialEstimate.Y); //move searching window (used by pyrOpticalFlow)
            featureError = 0;

            var prevFeatureArea = getFeatureArea(prevFeature, windowSize);
            if (isInsideImage(prevFeatureArea, prevImg.Size) == false)
            {
                featureStatus = KLTFeatureStatus.OutOfBounds;
                return;
            }

            var prevPatch = prevImg.GetRectSubPix(prevFeatureArea);
            var AtA = calcHessian(prevFeatureArea);

            //check min eigen-value
            if (hasValidEigenvalues(AtA, windowSize, minEigenValue) == false)
            {
                featureStatus = KLTFeatureStatus.SmallEigenValue;
                return;
            }

            double det = AtA[0, 0] * AtA[1, 1] - AtA[0, 1] * AtA[0, 1];

            Image<TColor, float> currPatch;
            for (int iter = 0; iter < iterations; iter++)
            {
                var currFeatureArea = getFeatureArea(currFeature, windowSize);
                if (isInsideImage(currFeatureArea, prevImg.Size) == false) // see if it moved outside of the image
                {
                    featureStatus = KLTFeatureStatus.OutOfBounds;
                    return;
                }

                currPatch = currImg.GetRectSubPix(currFeatureArea);
                var Atb = calcE(prevPatch, currPatch, currFeatureArea);

                //solve for D
                var dx = (AtA[1, 1] * Atb[0] - AtA[0, 1] * Atb[1]) / det;
                var dy = (AtA[0, 0] * Atb[1] - AtA[0, 1] * Atb[0]) / det;

                currFeature.X += (float)dx;
                currFeature.Y += (float)dy;

                // see if it has moved more than possible if it is really tracking a target
                // this happens in regions with little texture
                if (System.Math.Abs(currFeature.X - prevFeature.X) > windowSize ||
                    System.Math.Abs(currFeature.Y - prevFeature.Y) > windowSize)
                {
                    featureStatus = KLTFeatureStatus.Drifted;
                    return;
                }

                if (System.Math.Abs(dx) < minFeatureShift && System.Math.Abs(dy) < minFeatureShift)
                    break;
            }

            //TODO: compute error
            //featureError = (currPatch - prevPatch).Abs().Sum();          

            featureStatus = KLTFeatureStatus.Success;
            return;
        }

        private static double[,] calcHessian(RectangleF prevArea)
        {
            var derivX = storage.PrevImgPyrDerivX[storagePyrLevel].GetRectSubPix(prevArea);
            var derivY = storage.PrevImgPyrDerivY[storagePyrLevel].GetRectSubPix(prevArea);

            var corrXY = calcImageCorr(derivX, derivY);

            double[,] AtA = new double[,] 
            { 
                { calcImageCorr(derivX, derivX), corrXY                        },
                { corrXY,                        calcImageCorr(derivY, derivY) }
            };

            return AtA;
        }

        private static double[] calcE(Image<TColor, float> prevPatch, Image<TColor, float> currPatch, RectangleF currArea)
        {
            var pDerivX = storage.CurrImgPyrDerivX[storagePyrLevel].GetRectSubPix(currArea);
            var pDerivY = storage.CurrImgPyrDerivY[storagePyrLevel].GetRectSubPix(currArea);
            var pDerivT = currPatch - prevPatch;

            double[] Atb = new double[] 
            {
                calcImageCorr(pDerivX, pDerivT), calcImageCorr(pDerivY, pDerivT)
            };

            return Atb;
        }

        private static RectangleF getFeatureArea(PointF location, int winSize)
        {
            int halfWinSize = winSize / 2;

            return new RectangleF
            {
                X = location.X - halfWinSize,
                Y = location.Y - halfWinSize,
                Width = winSize,
                Height = winSize
            };
        }

        private static bool isInsideImage(RectangleF featureArea, Size imageSize)
        {
            RectangleF imageArea = new RectangleF(PointF.Empty, imageSize);

            if (featureArea.IntersectionPercent(imageArea) < (1 - 1E-2))
                return false;

            return true;
        }

        private static bool hasValidEigenvalues(double[,] AtA, int winSize, double minValidEigenValue)
        {
            var minEigenValue = calcMinEigenValue(AtA);
            minEigenValue /= (winSize * winSize * 255); //normalize (for the threshold)

            if (minEigenValue < minValidEigenValue)
            {
                return false;
            }

            return true;
        }

        private static double calcMinEigenValue(double[,] mat)
        {
            //(a-d)^2 + 4 * b* c
            var discriminant = (mat[0, 0] - mat[1, 1]) * (mat[0, 0] - mat[1, 1]) + 4 * mat[0, 1] * mat[1, 0];

            if (discriminant < 0) //is this necessary ?
            {
                return 0;
            }

            var sqrtDiscriminant = System.Math.Sqrt(discriminant);
            var minRealEigVal = ((mat[0, 0] + mat[1, 1]) - sqrtDiscriminant) / 2;

            return minRealEigVal;
        }

        private static unsafe float calcImageCorr(Image<TColor, float> im1, Image<TColor, float> im2)
        {
            float val = 0;

            int width = im1.Width;
            int height = im1.Height;
            int nChannels = im1.ColorInfo.NumberOfChannels;

            int im1Offset = im1.Stride - im1.Width * im1.ColorInfo.Size;
            int im2Offset = im2.Stride - im2.Width * im2.ColorInfo.Size;

            float* im1Data = (float*)im1.ImageData;
            float* im2Data = (float*)im2.ImageData;

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        val += *im1Data * *im2Data;

                        im1Data++;
                        im2Data++;
                    }
                }

                im1Data = (float*)((byte*)im1Data + im1Offset);
                im2Data = (float*)((byte*)im2Data + im2Offset);
            }

            return val;
        }

    }
}

