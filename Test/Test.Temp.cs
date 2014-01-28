using Accord.Controls;
using Accord.Imaging;
using Accord.Imaging.Converters;
using Accord.Imaging.Filters;
using Accord.Math;
using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

            Stopwatch s = new Stopwatch();
            s.Start();

            //var a= imgGray.Convolve(kernel);
            var c = 0;
            doParallel(imgGray, imgGray.CopyBlank(), (src, dst, rect) => 
            {
                var kernelPtr = (float*)kernel.ImageData;
                var srcPtr = (float*)src;
                var rez = 0f;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        rez += kernelPtr[j] * srcPtr[j];
                    }

                    kernelPtr += 3;
                    srcPtr += 1024 / sizeof(float);
                }
                c++;
                var dstPtr = (float*)dst;
                *dstPtr = rez;
            });

            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds);
        }

        private unsafe void doParallel(Image<Gray, float> image, Image<Gray, float> dst, Action<IntPtr, IntPtr, Rectangle> func)
        {
            int patchHeight = image.Height / 8;

            var srcPtr = (float*)image.ImageData;
            var dstPtr = (float*)dst.ImageData;
            int stride = image.Stride;

            int w = image.Width;
            int h = image.Height;

            Rectangle rect = Rectangle.Empty;
            Parallel.For(0, 10, (a) =>
            {

                for (int i = 0; i < patchHeight - 3; i++)
                {
                    for (int j = 0; j < w - 3; j++)
                    {
                        func((IntPtr)(srcPtr + i), (IntPtr)(dstPtr + j), rect);
                        //srcPtr += 4;
                        //dstPtr +=4;
                    }

                    //srcPtr += stride;
                    //dstPtr += stride;
                }

            });
        }
    }
}
