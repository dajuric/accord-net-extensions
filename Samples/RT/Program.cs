#define LOG

using Accord.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using Point = AForge.IntPoint;
using System.IO;
using System.Runtime.Caching;
using AForge;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Diagnostics;

namespace RT
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Train();
            //return;


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RTDemo());
        }

        static void Train()
        {
#if LOG
            Stopwatch stopwatch = new Stopwatch();
#endif

            var picoClassifier = new PicoClassifier(new RectangleF(0, 0, 1, 1));

#if LOG
            Console.WriteLine();
            Console.Write("Loading positive samples...");
            stopwatch.Start();
#endif

            List<Rectangle> sampleWindows;
            List<Image<Gray, byte>> samples;
            TrainDataLoader.LoadSampleData(@"C:\Users\Darko\Desktop\Pico\PICO_LEARNING\faces\list.txt", picoClassifier, out samples, out sampleWindows);

#if LOG
            stopwatch.Stop();
            Console.WriteLine(" {0} ms.", stopwatch.ElapsedMilliseconds);
            stopwatch.Start();
#endif

#if LOG
            Console.Write("Loading negative samples...");
            stopwatch.Start();
#endif

            List<Image<Gray, byte>> negatives = TrainDataLoader.LoadBackgroundImages(@"C:\Users\Darko\Desktop\Pico\PICO_LEARNING\nonfaces\list.txt");

#if LOG
            stopwatch.Stop();
            Console.WriteLine(" {0} ms.", stopwatch.ElapsedMilliseconds);
            Console.WriteLine();
#endif

            /*picoClassifier.AddStage(samples, negatives, sampleWindows, minTPR: 0.995f, maxFPR: 0.1f, targetFPR: 1e-6f, maxTrees: 10, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");*/

            picoClassifier.AddStage(samples, negatives, sampleWindows, minTPR: 0.980f, maxFPR: 0.5f, targetFPR: 1e-6f, maxTrees: 1, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, negatives, sampleWindows, minTPR: 0.985f, maxFPR: 0.5f, targetFPR: 1e-6f, maxTrees: 1, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            /*picoClassifier.AddStage(samples, negatives, sampleWindows, minTPR: 0.990f, maxFPR: 0.5f, targetFPR: 1e-6f, maxTrees: 2, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, negatives, sampleWindows, minTPR: 0.995f, maxFPR: 0.5f, targetFPR: 1e-6f, maxTrees: 3, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");*/
        }
    }
}
