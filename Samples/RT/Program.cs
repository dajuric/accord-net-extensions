#define LOG

using Accord.Controls;
using System.Linq;
using System;
using System.Collections.Generic;
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
            /*Random rand = new Random();

            var windows = new List<Rectangle>[1000];

            for (int imgIdx = 0; imgIdx < windows.Length; imgIdx++)
            {
                var imageWindows = new List<Rectangle>();

                for (int imgWndIdx = 0; imgWndIdx < 1000 * 1000; imgWndIdx++)
                {
                    imageWindows.Add(new Rectangle(rand.Next(), rand.Next(), rand.Next(), rand.Next()));
                }

                windows[imgIdx] = imageWindows;
            }


            Console.WriteLine(windows);
            return;*/

            /*Train();
            return;*/


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
            //PicoClassifier picoClassifier;
            //PicoClassifierHexDeserializer.FromBinaryFile("d", out picoClassifier);

#if LOG
            Console.WriteLine();
            Console.Write("Loading positive samples...");
            stopwatch.Start();
#endif

            List<Rectangle> sampleWindows;
            List<Image<Gray, byte>> samples;
            TrainDataLoader.LoadSampleData(@"C:\Users\Darko\Desktop\Pico\PICO_LEARNING\faces\list.txt", picoClassifier, out samples, out sampleWindows);

#if LOG
            Console.WriteLine(" {0} ms.", stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
#endif

#if LOG
            Console.Write("Loading negative samples...");
            stopwatch.Restart();
#endif

            List<Image<Gray, byte>> negatives = TrainDataLoader.LoadBackgroundImages(@"C:\Users\Darko\Desktop\Pico\PICO_LEARNING\nonfaces\list.txt");

#if LOG
            Console.WriteLine(" {0} ms.\n", stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
#endif

            /*int a = 0;
            while (a < 3)
            {
                picoClassifier.AddStage(samples, negatives, sampleWindows, targetFPR: 1e-6f, minTPR: 0.990f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 4);
                a++;
            }*/

            //picoClassifier.ToBinaryFile("d");
            //picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, negatives, sampleWindows, targetFPR: 1e-6f, minTPR: 0.980f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, negatives, sampleWindows, targetFPR: 1e-6f, minTPR: 0.985f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, negatives, sampleWindows, targetFPR: 1e-6f, minTPR: 0.990f, maxFPR: 0.5f, maxTrees: 2, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, negatives, sampleWindows, targetFPR: 1e-6f, minTPR: 0.995f, maxFPR: 0.5f, maxTrees: 3, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            int nStages = 0;
            bool isStageAdded = true;
            while (nStages < 6 && isStageAdded)
            {
                isStageAdded = picoClassifier.AddStage(samples, negatives, sampleWindows, targetFPR: 1e-6f, minTPR: 0.997f, maxFPR: 0.5f, maxTrees: 10, treeMaxDepth: 6);
                picoClassifier.ToHexFile("myClassifier.ea");

                nStages++;
            }

            /*nStages = 0;
            while (nStages < 10 && isStageAdded)
            {
                isStageAdded = picoClassifier.AddStage(samples, negatives, sampleWindows, targetFPR: 1e-6f, minTPR: 0.999f, maxFPR: 0.5f, maxTrees: 20, treeMaxDepth: 6);
                picoClassifier.ToHexFile("myClassifier.ea");

                nStages++;
            }*/

#if LOG
            stopwatch.Stop();
            Console.WriteLine("Cascade training finished in: {0}.", stopwatch.Elapsed);
#endif
        }
    }
}
