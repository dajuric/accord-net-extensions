#define LOG
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;

using Accord.Controls;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using AForge;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

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
            //trainCarSamples();
            //return;
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

            picoClassifier.AddStage(samples, sampleWindows, negatives, targetFPR: 1e-6f, minTPR: 0.980f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, sampleWindows, negatives, targetFPR: 1e-6f, minTPR: 0.985f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, sampleWindows, negatives, targetFPR: 1e-6f, minTPR: 0.990f, maxFPR: 0.5f, maxTrees: 2, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, sampleWindows, negatives, targetFPR: 1e-6f, minTPR: 0.995f, maxFPR: 0.5f, maxTrees: 3, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            int nStages = 0;
            bool isStageAdded = true;
            while (nStages < 6 && isStageAdded)
            {
                isStageAdded = picoClassifier.AddStage(samples, sampleWindows, negatives, targetFPR: 1e-6f, minTPR: 0.997f, maxFPR: 0.5f, maxTrees: 10, treeMaxDepth: 6);
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

        private static void getCarSamples(out List<Image<Gray, byte>> images, out List<List<Rectangle>> annotations)
        {
            Database db = new Database();
            db.Load("S:/processedImagesAnnotations.xml");

            db.Export((imgKey) => 
            {
                var filePath = "S:" + imgKey;
                var image = System.Drawing.Bitmap.FromFile(filePath).ToImage<Gray, byte>();
                return image;

            }, out images, out annotations);
        }

        static void trainCarSamples()
        {
#if LOG
            Stopwatch stopwatch = new Stopwatch();
#endif

            var picoClassifier = new PicoClassifier(new RectangleF(0, 0, 2.2f, 1));
            
#if LOG
            Console.WriteLine();
            Console.Write("Loading samples...");
            stopwatch.Start();
#endif

            List<List<Rectangle>> sampleWindows;
            List<Image<Gray, byte>> samples;
            getCarSamples(out samples, out sampleWindows);


            picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.980f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.985f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.990f, maxFPR: 0.5f, maxTrees: 2, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.995f, maxFPR: 0.5f, maxTrees: 3, treeMaxDepth: 6);
            picoClassifier.ToHexFile("myClassifier.ea");

            int nStages = 0;
            bool isStageAdded = true;
            while (nStages < 6 && isStageAdded)
            {
                isStageAdded = picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.997f, maxFPR: 0.5f, maxTrees: 10, treeMaxDepth: 6);
                picoClassifier.ToHexFile("myClassifier.ea");

                nStages++;
            }

#if LOG
            stopwatch.Stop();
            Console.WriteLine("Cascade training finished in: {0}.", stopwatch.Elapsed);
#endif
        }
    }
}
