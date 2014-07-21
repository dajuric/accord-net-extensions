using System;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Nearest-neighbor interpolation.
    /// <para>Experimental class.</para> //TODO: finish
    /// </summary>
    public static class ResizeNearsetNeighbur
    {
        delegate void ResizeFunc(IImage image, IImage destImage);
        static Dictionary<Type, ResizeFunc> resizeFuncs;

        static ResizeNearsetNeighbur()
        {
            resizeFuncs = new Dictionary<Type, ResizeFunc>();
            resizeFuncs.Add(typeof(float), resizeFloat);
            /*resizeFuncs.Add(typeof(short), conditionalCopyShort);
            resizeFuncs.Add(typeof(int), conditionalCopyInt);
            resizeFuncs.Add(typeof(float), conditionalCopyFloat);
            resizeFuncs.Add(typeof(double), conditionalCopyDouble);*/
        }

        /// <summary>
        /// Resizes the input image by using nearest neighbor interpolation.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="newSize">New image size.</param>
        /// <returns>Resized image.</returns>
        public static Image<TColor, float> ResizeNN<TColor>(Image<TColor, float> img, Size newSize)
            where TColor : IColor
        {
            return ResizeNN<TColor, float>(img, newSize);
        }

        /// <summary>
        /// Resizes the input image by using nearest neighbor interpolation.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="img">Image.</param>
        /// <param name="newSize">New image size.</param>
        /// <returns>Resized image.</returns>
        internal static Image<TColor, TDepth> ResizeNN<TColor, TDepth>(Image<TColor, TDepth> img, Size newSize)
            where TColor : IColor
            where TDepth : struct
        {
            var resizedIm = new Image<TColor, TDepth>(newSize);
            ResizeNN((IImage)img, resizedIm);
            return resizedIm;
        }

        internal static void ResizeNN(IImage img, IImage destImg)
        {
            if (img.ColorInfo.Equals(destImg.ColorInfo, ColorInfo.ComparableParts.Castable) == false)
                throw new Exception("Image and dest image must be at least castable (the same number of channels, the same channel type)!");

            Type depthType = img.ColorInfo.ChannelType;

            ResizeFunc resizeFunc = null;
            if (resizeFuncs.TryGetValue(depthType, out resizeFunc) == false)
            {
                throw new Exception(string.Format("Resize NN function of color depth type {0}", depthType));
            }

            resizeFunc(img, destImg);
        }

        private unsafe static void resizeFloat(IImage srcImg, IImage dstImg)
        {
            float xFactor = (float) srcImg.Width / dstImg.Width;
            float yFactor = (float) srcImg.Height / dstImg.Height;

            int newWidth = dstImg.Width;
            int newHeight = dstImg.Height;
            int nChannels = srcImg.ColorInfo.NumberOfChannels;

            int dstShift = dstImg.Stride - newWidth * srcImg.ColorInfo.Size;

            float* srcPtr = (float*)srcImg.ImageData;
            float* dstPtr = (float*)dstImg.ImageData;

            for (int r = 0; r < newHeight; r++)
            {
                float* srcRowPtr = (float*)srcImg.GetData((int)(r * yFactor));

                for (int c = 0; c < newWidth; c++)
                {
                    float* srcColPtr = srcRowPtr + nChannels * (int)(c * xFactor);

                    for (int ch = 0; ch < nChannels; ch++)
                    {
                        dstPtr[ch] = srcColPtr[ch];
                    }

                    dstPtr += nChannels;
                }

                dstPtr = (float*)((byte*)dstPtr + dstShift);
            }
        }
    }
}
