using Accord.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Accord.Imaging
{
    public static class NonMaximaSupressionExtensions
    {
        delegate void SupressNonMaximaFunc(IImage src, IImage dest, int radius, int discardVal);
        static Dictionary<Type, SupressNonMaximaFunc> supressNonMaximaFuncs;

        static NonMaximaSupressionExtensions()
        {
            supressNonMaximaFuncs = new Dictionary<Type, SupressNonMaximaFunc>();

            supressNonMaximaFuncs.Add(typeof(float), supressNonMaxima_Float);
        }

        public static Image<Gray, TDepth> SupressNonMaxima<TDepth>(this Image<Gray, TDepth> img, int radius = 3, int discardValue = 0)
           where TDepth : struct
        {
            var dest = new Image<Gray, TDepth>(img.Size);
            SupressNonMaxima(img, dest, radius);

            return dest;
        }

        public static void SupressNonMaxima<TDepth>(this Image<Gray, TDepth> img, Image<Gray, TDepth> dest, int radius = 3, int discardValue = 0)
            where TDepth:struct
        {
            SupressNonMaximaFunc supressNonMaximaFunc = null;
            if (supressNonMaximaFuncs.TryGetValue(img.ColorInfo.ChannelType, out supressNonMaximaFunc) == false)
                throw new NotSupportedException(string.Format("Can not perform non-maxima supression on an image of type {0}", img.ColorInfo.ChannelType.Name));

            var proc = new ParallelProcessor<IImage, bool>(img.Size,
                                                          () => true,
                                                          (_src, _, area) =>
                                                          {
                                                              Rectangle srcArea = new Rectangle
                                                              {
                                                                  X = 0,
                                                                  Y = area.Y,
                                                                  Width = _src.Width,
                                                                  Height = area.Height + 2 * radius
                                                              };
                                                              srcArea.Intersect(new Rectangle(Point.Empty, img.Size));

                                                              supressNonMaximaFunc(img.GetSubRect(srcArea), dest.GetSubRect(srcArea), radius, discardValue);
                                                          },
                                                          new ParallelOptions {  /*ForceSequential = true*/ });

            proc.Process(img);
        }

        private unsafe static void supressNonMaxima_Float(IImage src, IImage dest, int radius, int discardVal)
        {
            float* srcPtr = (float*)src.ImageData;
            float* dstPtr = (float*)dest.ImageData;

            int width = src.Width;
            int height = src.Height;

            int srcStride = src.Stride;
            int dstStride = dest.Stride;

            for (int row = 0; row < (height - 2 * radius); row++)
            {
                for (int col = 0; col < (width - 2 * radius); col++)
                {
                    supressNonMaximaPatch_Float(&srcPtr[col], srcStride, 
                                                &dstPtr[col], dstStride, 
                                                radius, discardVal);
                }

                srcPtr = (float*)((byte*)srcPtr + srcStride);
                dstPtr = (float*)((byte*)dstPtr + dstStride);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static void supressNonMaximaPatch_Float(float* srcPtr, int srcStride, 
                                                               float* dstPtr, int dstStride, 
                                                               int radius, int discardVal)
        {
            float centerVal = *((float*)((byte*)srcPtr + radius * srcStride) + radius); //[x + radius, y + radius]
            if(centerVal == discardVal)
                return;
           
            for (int row = 0; row < 2 * radius; row++)
            {
                for (int col = 0; col < 2 * radius; col++)
                {
                    var srcVal = srcPtr[col];

                    if (srcVal > centerVal)
                    {
                       return;
                    }
                }

                srcPtr = (float*)((byte*)srcPtr + srcStride);
            }

            //if centerVal is max value...
            var dstCenterPtr = (float*)((byte*)dstPtr + radius * dstStride) + radius; //[x + radius, y + radius]
            *dstCenterPtr = centerVal;
        }
    }
}
