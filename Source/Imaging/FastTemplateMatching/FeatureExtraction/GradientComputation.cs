using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Filters;
using Accord.Extensions.Math;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions;

namespace LINE2D
{
    public unsafe static class GradientComputation
    {
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
                        *orientImgPtr = MathFunctions.Atan2Aprox(*dyPtr, *dxPtr); //faster
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
                        *orientImgPtr = MathFunctions.Atan2Aprox(*dyPtr, *dxPtr);
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
