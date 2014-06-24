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

        private static long measure(Action action, int numberOfTimes, bool writeMessage = false)
        {
            long s = DateTime.Now.Ticks;

            for (int i = 0; i < numberOfTimes; i++)
            {
                action();
            }

            long e = DateTime.Now.Ticks;
            long elapsed = (e - s) / TimeSpan.TicksPerMillisecond;

            if (writeMessage)
                Console.WriteLine("Per call ~{0} ms", (float)elapsed / numberOfTimes);

            return elapsed;
        }

        private static void measure(Action action1, Action action2, int numberOfTimes, string action1Name, string action2Name)
        {
            Console.WriteLine("Measuring {0}", action1Name);
            var elapsed1 = measure(action1, numberOfTimes);

            Console.WriteLine("Measuring {0}", action2Name);
            var elapsed2 = measure(action2, numberOfTimes);

            if (elapsed1 < elapsed2)
                Console.WriteLine("{0} is faster than {1} ~{2} times. Per call: ~{3} ms", action1Name, action2Name, (float)elapsed2 / elapsed1, (float)elapsed1 / numberOfTimes);
            else
                Console.WriteLine("{0} is slower than {1} ~{2} times. Per call: ~{3} ms", action1Name, action2Name, (float)elapsed1 / elapsed2, (float)elapsed1 / numberOfTimes);
        }
    }

}
