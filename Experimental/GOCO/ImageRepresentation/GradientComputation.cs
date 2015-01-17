using System;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Filters;
using Accord.Extensions.Math;

namespace GOCO
{
    /// <summary>
    /// Contains methods for gray and color gradient magnitude and  orientation computation.
    /// </summary>
    public unsafe static class GradientComputation
    {
        /// <summary>
        /// Computes gradient orientations from the color image. Orientation from the channel which has the maximum gradient magnitude is taken as the orientation for a location.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="frame">Image.</param>
        /// <param name="magnitudeSqrImage">Squared magnitude image.</param>
        /// <param name="minValidMagnitude">Minimal valid magnitude.</param>
        /// <returns>Orientation image (angles are in degrees).</returns>
        public static Image<Gray, int> Compute<TColor>(Image<TColor, Byte> frame, out Image<Gray, int> magnitudeSqrImage, int minValidMagnitude)
            where TColor: IColor3
        {
            int minValidMagSqr = minValidMagnitude * minValidMagnitude;

            Image<TColor, short> dx = frame.Sobel(1, 0, 3);
            Image<TColor, short> dy = frame.Sobel(0, 1, 3);

            short* dxPtr = (short*)dx.ImageData;
            short* dyPtr = (short*)dy.ImageData;
            int dxyImgAllignStride = dx.Stride / sizeof(short) - dx.Width * dx.ColorInfo.NumberOfChannels;

            var orientImg = new Image<Gray, int>(dx.Width, dx.Height, 0); //TODO: low priority - rewrite to remove constrain stride = 0
            int* orientImgPtr = (int*)orientImg.ImageData;

            magnitudeSqrImage = new Image<Gray, int>(dx.Width, dx.Height, 0); //TODO: low priority - rewrite to remove constrain stride = 0
            int* magSqrImgPtr = (int*)magnitudeSqrImage.ImageData;

            int imgWidth = dx.Width;
            int imgHeight = dx.Height;

            for (int j = 0; j < imgHeight; j++)
            {
                for (int i = 0; i < imgWidth; i++)
                {
                    int B_mag = dxPtr[0] * dxPtr[0] + dyPtr[0] * dyPtr[0];
                    int G_mag = dxPtr[1] * dxPtr[1] + dyPtr[1] * dyPtr[1];
                    int R_mag = dxPtr[2] * dxPtr[2] + dyPtr[2] * dyPtr[2];

                    int maxDx, maxDy;

                    if (B_mag > G_mag && B_mag > R_mag)
                    {
                        maxDx = dxPtr[0];
                        maxDy = dyPtr[0];
                    }
                    else if (G_mag > B_mag && G_mag > R_mag)
                    {
                        maxDx = dxPtr[1];
                        maxDy = dyPtr[1];
                    }
                    else
                    {
                        maxDx = dxPtr[2];
                        maxDy = dyPtr[2];
                    }

                    int magSqr = maxDx * maxDx + maxDy * maxDy;
                    if (magSqr < minValidMagSqr)
                        *orientImgPtr = FeatureMap.INVALID_ORIENTATION;
                    else
                    {
                        //*orientImgPtr = (int)(Math.Atan2(maxDy, maxDx) * 180 / Math.PI);
                        //if (*orientImgPtr < 0)
                        //    *orientImgPtr += 360;
                        *orientImgPtr = MathExtensions.Atan2Aprox(*dyPtr, *dxPtr); //faster
                        *magSqrImgPtr = magSqr;
                    }

                    dxPtr += 3; dyPtr += 3;
                    orientImgPtr += 1; magSqrImgPtr += 1;
                }

                dxPtr += dxyImgAllignStride;
                dyPtr += dxyImgAllignStride;
            }

            //magImg = frame.Canny(70, GlobalParameters.MIN_GRADIENT_THRESHOLD, 3).SmoothGaussian(3).Convert<Gray, byte>().Convert<Gray, int>();
            //magImg = frame.Convert<Gray, byte>().Canny(70, GlobalParameters.MIN_GRADIENT_THRESHOLD, 3).SmoothGaussian(3).Convert<Gray, int>();
            return orientImg;
        }

        /// <summary>
        /// Computes gradient orientations from the gray image.
        /// </summary>
        /// <param name="frame">Image.</param>
        /// <param name="magnitudeSqrImage">Squared magnitude image.</param>
        /// <param name="minValidMagnitude">Minimal valid magnitude.</param>
        /// <returns>Orientation image (angles are in degrees).</returns>
        public static Image<Gray, int> Compute(Image<Gray, Byte> frame, out Image<Gray, int> magnitudeSqrImage, int minValidMagnitude)
        {
            int minValidMagSqr = minValidMagnitude * minValidMagnitude;

            Image<Gray, short> dx = frame.Sobel(1, 0, 3);
            Image<Gray, short> dy = frame.Sobel(0, 1, 3);

            short* dxPtr = (short*)dx.ImageData;
            short* dyPtr = (short*)dy.ImageData;
            int dxyImgAllignStride = dx.Stride / sizeof(short) - dx.Width;

            Image<Gray, int> orientImg = new Image<Gray, int>(frame.Size, 0); //TODO: low priority - rewrite to remove constrain stride = 0
            int* orientImgPtr = (int*)orientImg.ImageData;

            magnitudeSqrImage = new Image<Gray, int>(dx.Width, dx.Height, 0); //TODO: low priority - rewrite to remove constrain stride = 0
            int* magSqrImgPtr = (int*)magnitudeSqrImage.ImageData;

            int imgWidth = frame.Width;
            int imgHeight = frame.Height;

            for (int j = 0; j < imgHeight; j++)
            {
                for (int i = 0; i < imgWidth; i++)
                {
                    int magSqr = dxPtr[0] * dxPtr[0] + dyPtr[0] * dyPtr[0];

                    if (magSqr < minValidMagSqr)
                        *orientImgPtr = FeatureMap.INVALID_ORIENTATION;
                    else
                    {
                        //*orientImgPtr = (int)(Math.Atan2(*dyPtr, *dxPtr) * 180 / Math.PI);
                        *orientImgPtr = MathExtensions.Atan2Aprox(*dyPtr, *dxPtr);
                        /*if (*orientImgPtr < 0)
                            *orientImgPtr += 360;*/
                        //ne treba ?

                        *magSqrImgPtr = magSqr;
                        //*magSqrImgPtr = (int)Math.Sqrt(magSqr); if (*magSqrImgPtr > 255) *magSqrImgPtr = 255;
                    }

                    dxPtr += 1; dyPtr += 1;
                    orientImgPtr += 1; magSqrImgPtr += 1;
                }

                dxPtr += dxyImgAllignStride;
                dyPtr += dxyImgAllignStride;
            }

            return orientImg;
        }
    }
}
