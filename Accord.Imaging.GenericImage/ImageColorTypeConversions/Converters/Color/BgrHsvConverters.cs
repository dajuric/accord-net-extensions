using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging.Converters
{
    static class BgrHsvConverters
    {
        #region Bgr8 -> Hsv8

        /// <summary>
        /// see: http://stackoverflow.com/questions/3018313/algorithm-to-convert-rgb-to-hsv-and-hsv-to-rgb
        /// and: http://www.cs.rit.edu/~ncs/color/t_convert.html
        /// range for  HSV is H:[0-180], S:[0-255], V:[0-255]
        /// </summary>
        public unsafe static void ConvertBgrToHsv_Byte(IImage src, IImage dest)
        {
            Bgr8* srcPtr = (Bgr8*)src.ImageData;
            Hsv8* dstPtr = (Hsv8*)dest.ImageData;
            
            int width = src.Width;
            int height = src.Height;

            int srcShift = src.Stride - width * sizeof(Bgr8); //DO NOT divide with sizeof(Bgr8) as reminder may not be 0!!!
            int dstShift = dest.Stride - width * sizeof(Hsv8);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    convertBgrToHsv_Byte(srcPtr, dstPtr);

                    srcPtr++;
                    dstPtr++;
                }

                srcPtr = (Bgr8*)((byte*)srcPtr + srcShift);
                dstPtr = (Hsv8*)((byte*)dstPtr + dstShift);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static void convertBgrToHsv_Byte(Bgr8* bgr, Hsv8* hsv)
        {
            byte rgbMin, rgbMax;

            rgbMin = bgr->R < bgr->G ? (bgr->R < bgr->B ? bgr->R : bgr->B) : (bgr->G < bgr->B ? bgr->G : bgr->B);
            rgbMax = bgr->R > bgr->G ? (bgr->R > bgr->B ? bgr->R : bgr->B) : (bgr->G > bgr->B ? bgr->G : bgr->B);

            hsv->V = rgbMax;
            if (hsv->V == 0)
            {
                hsv->H = 0;
                hsv->S = 0;
                return;
            }

            hsv->S = (byte)(255 * (rgbMax - rgbMin) / rgbMax);
            if (hsv->S == 0)
            {
                hsv->H = 0;
                return;
            }

            int hue = 0;
            if (rgbMax == bgr->R)
            {
                hue = 0 + 60 * (bgr->G - bgr->B) / (rgbMax - rgbMin);
                if (hue < 0)
                    hue += 360;
            }
            else if (rgbMax == bgr->G)
            {
                hue = 120 + 60 * (bgr->B - bgr->R) / (rgbMax - rgbMin);
            }
            else //rgbMax == bgr->B
            {
                hue = 240 + 60 * (bgr->R - bgr->G) / (rgbMax - rgbMin);
            }

            hsv->H = (byte)(hue / 2); //scale [0-360] -> [0-180] (only needed for byte!)

            Debug.Assert(hue >= 0 && hue <= 360);
        }

        public unsafe static Hsv8 ConvertBgrToHsv(Bgr8 bgr)
        { 
            Hsv8 hsv;
            convertBgrToHsv_Byte(&bgr, &hsv);
            return hsv;
        }

        #endregion

        #region Hsv8 -> Bgr8

        /// <summary>
        /// see: http://stackoverflow.com/questions/3018313/algorithm-to-convert-rgb-to-hsv-and-hsv-to-rgb
        /// and: http://www.cbusforums.com/forums/showthread.php?t=8657
        /// range for  HSV is H:[0-180], S:[0-255], V:[0-255]
        /// </summary>
        public unsafe static void ConvertHsvToBgr_Byte(IImage src, IImage dest)
        {
            Hsv8* srcPtr = (Hsv8*)src.ImageData;
            Bgr8* dstPtr = (Bgr8*)dest.ImageData;

            int width = src.Width;
            int height = src.Height;

            int srcShift = src.Stride - width * sizeof(Hsv8); //DO NOT divide with sizeof(Bgr8) as reminder may not be 0!!!
            int dstShift = dest.Stride - width * sizeof(Bgr8);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    convertHsvToBgr_Byte(srcPtr, dstPtr);

                    srcPtr++;
                    dstPtr++;
                }

                srcPtr = (Hsv8*)((byte*)srcPtr + srcShift);
                dstPtr = (Bgr8*)((byte*)dstPtr + dstShift);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static void convertHsvToBgr_Byte(Hsv8* hsv, Bgr8* bgr)
        {
            if (hsv->S == 0)
            {
                bgr->R = hsv->V;
                bgr->G = hsv->V;
                bgr->B = hsv->V;
                return;
            }

            int hue = hsv->H * 2; //move to [0-360 range] (only needed for byte!)

            int hQuadrant = hue / 60; // Hue quadrant 0 - 5 (60deg)
            int hOffset = hue % 60; // Hue position in quadrant
            int vs = hsv->V * hsv->S;

            byte p = (byte)(hsv->V - (vs / 255));
            byte q = (byte)(hsv->V - (vs / 255 * hOffset) / 60);
            byte t = (byte)(hsv->V - (vs / 255 * (60 - hOffset)) / 60);

            switch (hQuadrant)
            {
                case 0:
                    bgr->R = hsv->V; bgr->G = t; bgr->B = p;
                    break;
                case 1:
                    bgr->R = q; bgr->G = hsv->V; bgr->B = p;
                    break;
                case 2:
                    bgr->R = p; bgr->G = hsv->V; bgr->B = t;
                    break;
                case 3:
                    bgr->R = p; bgr->G = q; bgr->B = hsv->V;
                    break;
                case 4:
                    bgr->R = t; bgr->G = p; bgr->B = hsv->V;
                    break;
                default:
                    bgr->R = hsv->V; bgr->G = p; bgr->B = q;
                    break;
            }
        }

        public unsafe static Bgr8 ConvertHsvToBgr(Hsv8 hsv)
        {
            Bgr8 bgr;
            convertHsvToBgr_Byte(&hsv, &bgr);
            return bgr;
        }

        #endregion
    }
}
