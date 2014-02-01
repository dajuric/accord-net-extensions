using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Converters;
using Accord.Extensions.Imaging.Filters;
using Accord.Extensions.Math;
using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Test
{
    public partial class Test
    {
        public unsafe void Bla()
        {
            var resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            var imgGray = Bitmap.FromFile(Path.Combine(resourceDir, "testColorBig.jpg")).ToImage<Gray, float>();

            Image<Gray, float> kernel = new float[,] { { 5, 2, 1 }, { 6, 0, 5 }, { 5, 6, 4 } }.ToImage();
            //float* kernelPtr = (float*)kernel.ImageData;
            var bla = imgGray.Convert<Bgr, byte>();

            measure(() =>
            {
                var a = bla.Convert<Hsv, byte>();
            },
            () =>
            {
               var b = bla.ProcessPatch<Bgr, byte, byte>((srcPtr, dstPtr) =>
                {
                    convertBgrToHsv_Byte((Bgr8*)srcPtr, (Hsv8*)dstPtr);
                });

               //b.Save("C:/bla.bmp");
            },

            100, "Old", "New");
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
    }

}
