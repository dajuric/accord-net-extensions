#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using Accord.Extensions.Imaging;
using Accord.Extensions.Math;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// Contains methods for gray and color gradient magnitude and  orientation computation.
    /// </summary>
    public unsafe static class GradientComputation
    {
        /// <summary>
        /// Sobel 3x3 for finding vertical edges.
        /// </summary>
        private static readonly int[,] Sobel_3x3_X = new int[,] 
        {
            {+1, 0, -1},
            {+2, 0, -2},
            {+1, 0, -1}
        };

        /// <summary>
        /// Sobel 3x3 for finding horizontal edges.
        /// </summary>
        private static readonly int[,] Sobel_3x3_Y = new int[,] 
        {
            {+1, +2, +1},
            {+0, +0, +0},
            {-1, -2, -1}
        };

        private static int kernelRadius = 3 / 2;

        /// <summary>
        /// Computes gradient orientations from the color image. Orientation from the channel which has the maximum gradient magnitude is taken as the orientation for a location.
        /// </summary>
        /// <param name="frame">Image.</param>
        /// <param name="magnitudeSqrImage">Squared magnitude image.</param>
        /// <param name="minValidMagnitude">Minimal valid magnitude.</param>
        /// <returns>Orientation image (angles are in degrees).</returns>
        public unsafe static Gray<int>[,] Compute(Bgr<byte>[,] frame, out Gray<int>[,] magnitudeSqrImage, int minValidMagnitude)
        {
            var minSqrMagnitude = minValidMagnitude * minValidMagnitude;
            
            var orientationImage = new Gray<int>[frame.Height(), frame.Width()];
            var _magnitudeSqrImage = orientationImage.CopyBlank();

            using (var uFrame = frame.Lock())
            {
                ParallelLauncher.Launch(thread => 
                {
                    computeColor(thread, (byte*)uFrame.ImageData, uFrame.Stride, orientationImage, _magnitudeSqrImage, minSqrMagnitude);
                }, 
                frame.Width() - 2 * kernelRadius, frame.Height() - 2 * kernelRadius);
            }

            magnitudeSqrImage = _magnitudeSqrImage;
            return orientationImage;
        }

        private unsafe static void computeColor(KernelThread thread, byte* frame, int frameStride, Gray<int>[,] orientationImage, Gray<int>[,] magnitudeSqrImage, int minSqrMagnitude)
        {
            frame = frame + frameStride * thread.Y + thread.X;

            int maxMagSqr = 0, maxDx = 0, maxDy = 0;
            for (int ch = 0; ch < 3; ch++)
            {
                var srcPtr = frame + ch;

                int sumX = 0, sumY = 0;
                for (int r = 0; r < 3; r++)
                {
                    var chPtr = srcPtr;
                    for (int c = 0; c < 3; c++)
                    {
                        sumX += *chPtr * Sobel_3x3_X[r, c];
                        sumY += *chPtr * Sobel_3x3_Y[r, c];

                        chPtr += 3 * sizeof(byte);
                    }

                    srcPtr = (byte*)srcPtr + frameStride;
                }
                //sumX >>= 3; sumY >>= 3; //divide by 8 (normalize kernel)

                var grad = sumX * sumX + sumY * sumY;
                if (grad > maxMagSqr)
                {
                    maxMagSqr = grad;
                    maxDx = sumX;
                    maxDy = sumY;
                }
            }

            if (maxMagSqr < minSqrMagnitude)
            {
                //magnitudeSqrImage[thread.Y + kernelRadius, thread.X + kernelRadius] = 0;  //redundant
                orientationImage[thread.Y + kernelRadius, thread.X + kernelRadius] = FeatureMap.INVALID_ORIENTATION;
            }
            else
            {
                magnitudeSqrImage[thread.Y + kernelRadius, thread.X + kernelRadius] = maxMagSqr;
                orientationImage[thread.Y + kernelRadius, thread.X + kernelRadius] = MathExtensions.Atan2Aprox(maxDy, maxDx);
            }
        }

        /// <summary>
        /// Computes gradient orientations from the color image. Orientation from the channel which has the maximum gradient magnitude is taken as the orientation for a location.
        /// </summary>
        /// <param name="frame">Image.</param>
        /// <param name="magnitudeSqrImage">Squared magnitude image.</param>
        /// <param name="minValidMagnitude">Minimal valid magnitude.</param>
        /// <returns>Orientation image (angles are in degrees).</returns>
        public unsafe static Gray<int>[,] Compute(Gray<byte>[,] frame, out Gray<int>[,] magnitudeSqrImage, int minValidMagnitude)
        {
            var minSqrMagnitude = minValidMagnitude * minValidMagnitude;

            var orientationImage = new Gray<int>[frame.Height(), frame.Width()];
            var _magnitudeSqrImage = orientationImage.CopyBlank();

            using (var uFrame = frame.Lock())
            {
                ParallelLauncher.Launch(thread =>
                {
                    computeGray(thread, (byte*)uFrame.ImageData, uFrame.Stride, orientationImage, _magnitudeSqrImage, minSqrMagnitude);
                },
                frame.Width() - 2 * kernelRadius, frame.Height() - 2 * kernelRadius);
            }

            magnitudeSqrImage = _magnitudeSqrImage; 
            return orientationImage;
        }

        private unsafe static void computeGray(KernelThread thread, byte* frame, int frameStride, Gray<int>[,] orientationImage, Gray<int>[,] magnitudeSqrImage, int minSqrMagnitude)
        {
            frame = frame + frameStride * thread.Y + thread.X;          
            var srcPtr = frame;

            int sumX = 0, sumY = 0;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    sumX += srcPtr[c] * Sobel_3x3_X[r, c];
                    sumY += srcPtr[c] * Sobel_3x3_Y[r, c];
                }

                srcPtr = (byte*)srcPtr + frameStride;
            }
            //sumX >>= 3; sumY >>= 3; //divide by 8 (normalize kernel) //without this

            var grad = sumX * sumX + sumY * sumY;
            if (grad < minSqrMagnitude)
            {
                //magnitudeSqrImage[thread.Y + kernelRadius, thread.X + kernelRadius] = 0;  //redundant
                orientationImage[thread.Y + kernelRadius, thread.X + kernelRadius] = FeatureMap.INVALID_ORIENTATION;
            }
            else
            {
                magnitudeSqrImage[thread.Y + kernelRadius, thread.X + kernelRadius] = grad;
                orientationImage[thread.Y + kernelRadius, thread.X + kernelRadius] = MathExtensions.Atan2Aprox(sumY, sumX);
            }
        }
    }
}
