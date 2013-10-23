using Accord.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
    public static class GetRectSubPixExtensions
    {
        delegate void InterpolateRectFunc(IImage image, PointF startPt, IImage destImage);
        static Dictionary<Type, InterpolateRectFunc> interpolateRectFuncs;

        static GetRectSubPixExtensions()
        {
            interpolateRectFuncs = new Dictionary<Type, InterpolateRectFunc>();
            interpolateRectFuncs.Add(typeof(short), getRectSubPix_Short);
            interpolateRectFuncs.Add(typeof(float), getRectSubPix_Float);
        }

        /*public static void GetRectSubPix<TColor>(this Image<TColor, short> img, PointF startPt, Image<TColor, float> destImg)
            where TColor : IColor
        {
            GetRectSubPix((IImage)img, startPt, destImg);
        }*/

        public static Image<TColor, float> GetRectSubPix<TColor, TDepth>(this Image<TColor, TDepth> img, RectangleF area)
            where TColor : IColor
            where TDepth : struct
        {
            var destImg = new Image<TColor, float>(area.Size.ToSize());
            GetRectSubPix((IImage)img, area.Location, destImg);
            return destImg;
        }

        public static void GetRectSubPix<TColor, TDepth>(this Image<TColor, TDepth> img, PointF startPt, Image<TColor, float> destImg)
            where TColor : IColor
            where TDepth : struct
        {
            GetRectSubPix((IImage)img, startPt, destImg);
        }

        private static void GetRectSubPix(IImage img, PointF startPt, IImage destImg)
        {
            if(destImg.ColorInfo.ChannelType != typeof(float))
                throw new Exception("Destination image channels type are not float.");

            Type depthType = img.ColorInfo.ChannelType;

            InterpolateRectFunc interpolateFunc = null;
            if (interpolateRectFuncs.TryGetValue(depthType, out interpolateFunc) == false)
            {
                throw new Exception(string.Format("GetRectSubPix function of color depth type {0}", depthType));
            }

            interpolateFunc(img, startPt, destImg);
        }

        private unsafe static void getRectSubPix_Short(IImage img, PointF startPt, IImage destImg)
        {
            int xt = (int)startPt.X;
            int yt = (int)startPt.Y;
            float ax = startPt.X - xt;
            float ay = startPt.Y - yt;

            float bx = 1.0f - ax;
            float by = 1.0f - ay;

            float a0 = bx * by;
            float a1 = ax * by;
            float a2 = ax * ay;
            float a3 = bx * ay;

            int width = destImg.Width;
            int height = destImg.Height;

            if (xt + width > img.Width || yt + height > img.Height)
                throw new Exception("Requested region is out of bounds!");

            if (xt + width == img.Width)
            {
                width--;
                //TODO: handle right border
            }

            if (yt + height == img.Height)
            {
                height--;
                //TODO: handle bottom border
            }

            int nChannels = img.ColorInfo.NumberOfChannels;

            int srcStride = img.Stride;
            int dstStride = destImg.Stride;
            int srcOffset = img.Stride - width * img.ColorInfo.Size;
            int dstOffset = destImg.Stride - width * destImg.ColorInfo.Size;

            for (int ch = 0; ch < nChannels; ch++)
            {
                short* imgPtr = (short*)img.GetData(yt, xt) + ch;
                float* dstPtr = (float*)destImg.ImageData + ch;

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        short* nextRowPtr = (short*)((byte*)imgPtr + srcStride);

                        float x0y0 = *imgPtr;
                        float x1y0 = *(imgPtr + nChannels);
                        float x0y1 = *nextRowPtr;
                        float x1y1 = *(nextRowPtr + nChannels);

                        *dstPtr = a0 * x0y0 + a1 * x1y0 + a2 * x1y1 + a3 * x0y1;

                        imgPtr += nChannels;
                        dstPtr += nChannels;
                    }

                    imgPtr = (short*)((byte*)imgPtr + srcOffset);
                    dstPtr = (float*)((byte*)dstPtr + dstOffset);
                }
            }  
        }

        private unsafe static void getRectSubPix_Float(IImage img, PointF startPt, IImage destImg)
        {
            int xt = (int)startPt.X;
            int yt = (int)startPt.Y;
            float ax = startPt.X - xt;
            float ay = startPt.Y - yt;

            float bx = 1.0f - ax;
            float by = 1.0f - ay;

            float a0 = bx * by;
            float a1 = ax * by;
            float a2 = ax * ay;
            float a3 = bx * ay;

            int width = destImg.Width;
            int height = destImg.Height;

            if (xt + width > img.Width || yt + height > img.Height)
                throw new Exception("Requested region is out of bounds!");

            if (xt + width == img.Width)
            {
                width--;
                //TODO: handle right border
            }

            if (yt + height == img.Height)
            {
                height--;
                //TODO: handle bottom border
            }

            int nChannels = img.ColorInfo.NumberOfChannels;

            int srcStride = img.Stride;
            int dstStride = destImg.Stride;
            int srcOffset = img.Stride - width * img.ColorInfo.Size;
            int dstOffset = destImg.Stride - width * destImg.ColorInfo.Size;

            for (int ch = 0; ch < nChannels; ch++)
            {
                float* imgPtr = (float*)img.GetData(yt, xt) + ch;
                float* dstPtr = (float*)destImg.ImageData + ch;

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        float* nextRowPtr = (float*)((byte*)imgPtr + srcStride);

                        float x0y0 = *imgPtr;
                        float x1y0 = *(imgPtr + nChannels);
                        float x0y1 = *nextRowPtr;
                        float x1y1 = *(nextRowPtr + nChannels);

                        *dstPtr = a0 * x0y0 + a1 * x1y0 + a2 * x1y1 + a3 * x0y1;

                        imgPtr += nChannels;
                        dstPtr += nChannels;
                    }

                    imgPtr = (float*)((byte*)imgPtr + srcOffset);
                    dstPtr = (float*)((byte*)dstPtr + dstOffset);
                }
            }
        }
    }
}
