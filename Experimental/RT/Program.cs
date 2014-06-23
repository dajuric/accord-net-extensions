#define LOG
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using Color = Accord.Extensions.Imaging.Gray;

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
            trainCarSamples();
            return;
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

            var picoClassifier = new PicoClassifier<Gray>(1);
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

            List<Image<Gray, byte>> negatives = TrainDataLoader.LoadBackgroundImages<Gray>(@"C:\Users\Darko\Desktop\Pico\PICO_LEARNING\nonfaces\list.txt");

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

        private static void getCarSamples<TColor>(out List<Image<TColor, byte>> images, out List<List<Rectangle>> annotations)
            where TColor: IColor
        {
            string dir = @"S:\Svjetla - baza podataka - Boris\Stražnja\";
            //var dir = @"S:\Svjetla - baza podataka - Boris\Prednja\";

            Database db = new Database();
            db.Load(Path.Combine(dir, "straznjaSvjetla1_prepared.xml"));
            //db.Load(Path.Combine(dir, "1.xml"));

            //enable for positive sample preparation
            /*var locationRange = new Range(-0.05f, +0.05f);
            var scaleRange = new Range(0.9f, 1.1f);
            db = db.ProcessSamples(2.2f, new Pair<Range>(locationRange, locationRange), new Pair<Range>(scaleRange, scaleRange), 7);
            db.Save(Path.Combine(dir, "2.xml"));*/

            StreamableSource imageStream = new FileCapture(Path.Combine(dir, "1.mxf"));

            db.Export((imgKey) => 
            {
                //return System.Drawing.Bitmap.FromFile(Path.Combine(dir, "1", imgKey)).ToImage<TColor, byte>();
                imageStream.Seek(Int32.Parse(imgKey), SeekOrigin.Begin);

                var img = imageStream.ReadAs<Bgr, short>();
                //img[Bgr.IdxR].Sub(img[Bgr.IdxG]).ThresholdToZero(0, 255).Convert<TColor, byte>().ToBitmap().Save("bla.bmp");*/

                return img[Bgr.IdxR].Sub(img[Bgr.IdxG]).ThresholdToZero(0, 255).Convert<TColor, byte>();
                //return imageStream.ReadAs<TColor, byte>().Clone();
            }, 
            out images, out annotations, 
            (percentage) => Console.Write("\r Loading - {0} %", (int)(percentage * 100)));
        }

        static void trainCarSamples()
        {
#if LOG
            Stopwatch stopwatch = new Stopwatch();
#endif

            var picoClassifier = new PicoClassifier<Color>(2.2f);
            
#if LOG
            Console.WriteLine();
            Console.Write("Loading samples...");
            stopwatch.Start();
#endif

            List<List<Rectangle>> sampleWindows;
            List<Image<Color, byte>> samples;
            getCarSamples(out samples, out sampleWindows);

            var nBinTests = 1024 * 50;

            picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.980f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.985f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.990f, maxFPR: 0.5f, maxTrees: 2, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
            picoClassifier.ToHexFile("myClassifier.ea");

            picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.995f, maxFPR: 0.5f, maxTrees: 3, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
            picoClassifier.ToHexFile("myClassifier.ea");

            int nStages = 0;
            bool isStageAdded = true;
            while (nStages < 6 && isStageAdded)
            {
                isStageAdded = picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.997f, maxFPR: 0.5f, maxTrees: 10, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
                picoClassifier.ToHexFile("myClassifier.ea");

                nStages++;
            }

            /*nStages = 0;
            while (nStages < 2 && isStageAdded)
            {
                isStageAdded = picoClassifier.AddStage(samples, sampleWindows, targetFPR: 1e-6f, minTPR: 0.999f, maxFPR: 0.5f, maxTrees: 20, treeMaxDepth: 6);
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
