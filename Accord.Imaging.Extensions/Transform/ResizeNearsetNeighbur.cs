using Accord.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging.Filters
{
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

        public static Image<TColor, TDepth> Resize<TColor, TDepth>(this Image<TColor, TDepth> img, Size newSize)
            where TColor : IColor
            where TDepth : struct
        {
            var resizedIm = new Image<TColor, TDepth>(newSize);
            Resize((IImage)img, resizedIm);
            return resizedIm;
        }

        public static void Resize<TColor, TDepth>(this Image<TColor, TDepth> img, Image<TColor, TDepth> destImg)
            where TColor:IColor
            where TDepth:struct
        {
            Resize((IImage)img, destImg);
        }

        internal static void Resize(this IImage img, IImage destImg)
        {
            if (ColorInfo.Equals(img.ColorInfo, destImg.ColorInfo, ColorInfo.ComparableParts.Castable) == false)
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
