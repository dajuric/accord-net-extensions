#define LOG

using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCO
{
    class Program
    {
        public static void Main(string[] args)
        {
            /*var proba = System.Drawing.Bitmap.FromFile(@"S:\HandDatabase-cropped\ExtractedSamples\croppedImages\Hand_Left\hand-1.jpg").ToImage<Gray, byte>();
            var oi = getOrientationImage(proba);
            oi.Save("out.bmp");*/

            var elapsedMs = Diagnostics.MeasureTime(() =>
            {
                trainSamples();
            });

            Console.ReadLine();
            return;
        }

        static void trainSamples()
        {
#if LOG
            Stopwatch stopwatch = new Stopwatch();
#endif

            var gocoClassifier = new GocoClassifier(0.75f);

#if LOG
            Console.WriteLine();
            Console.WriteLine("Loading samples...");
            stopwatch.Start();
#endif

            List<Image<Gray, byte>> positives;
            List<Image<Gray, byte>> negatives;
            getSamples(out positives, out negatives);

#if LOG
            Console.WriteLine("Done sample loading.");
#endif
            //WARNING
            //gocoClassifier = File.Open("myClassifier.bin", FileMode.Open).FromBinary<GocoClassifier>();

            var nBinTests = 1024 * 1000;

            gocoClassifier.AddStage(positives, negatives, targetFPR: 1e-6f, minTPR: 0.980f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
            gocoClassifier.ToBinary("myClassifier.bin");

            gocoClassifier.AddStage(positives, negatives, targetFPR: 1e-6f, minTPR: 0.985f, maxFPR: 0.5f, maxTrees: 1, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
            gocoClassifier.ToBinary("myClassifier.bin");

            gocoClassifier.AddStage(positives, negatives, targetFPR: 1e-6f, minTPR: 0.990f, maxFPR: 0.5f, maxTrees: 2, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
            gocoClassifier.ToBinary("myClassifier.bin");

            gocoClassifier.AddStage(positives, negatives, targetFPR: 1e-6f, minTPR: 0.995f, maxFPR: 0.5f, maxTrees: 3, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
            gocoClassifier.ToBinary("myClassifier.bin");

            int nStages = 0;
            bool isStageAdded = true;
            while (nStages < 6 && isStageAdded)
            {
                isStageAdded = gocoClassifier.AddStage(positives, negatives, targetFPR: 1e-6f, minTPR: 0.997f, maxFPR: 0.5f, maxTrees: 10, treeMaxDepth: 6, numberOfBinaryTests: nBinTests);
                gocoClassifier.ToBinary("myClassifier.bin");

                nStages++;
            }

            nStages = 0;
            while (nStages < 10 && isStageAdded)
            {
                isStageAdded = gocoClassifier.AddStage(positives, negatives, targetFPR: 1e-6f, minTPR: 0.999f, maxFPR: 0.5f, maxTrees: 20, treeMaxDepth: 6);
                gocoClassifier.ToBinary("myClassifier.bin");

                nStages++;
            }

#if LOG
            stopwatch.Stop();
            Console.WriteLine("Cascade training finished in: {0}.", stopwatch.Elapsed);
#endif
        }

        private static void getSamples(out List<Image<Gray, byte>> positives, out List<Image<Gray, byte>> negatives)
        {
            positives = new List<Image<Gray, byte>>();
            negatives = new List<Image<Gray, byte>>();

            /*positives.AddRange(
                        new ImageDirectoryReader(@"S:\SynteticImages\positives\", "*.bmp", recursive: true)
                       .Select(x => getOrientationImage(x.Convert<Gray, byte>()), (percentage) => Console.Write("\tReading positives: {0} % \r", percentage * 100))
                              );

            negatives.AddRange(
                      new ImageDirectoryReader(@"S:\SynteticImages\negatives\", "*.jpg", recursive: true)
                     .Select(x => getOrientationImage(x.Convert<Gray, byte>()), (percentage) => Console.Write("\tReading negatives: {0} % \r", percentage * 100))
                            );*/

            positives.AddRange(
                       new ImageDirectoryReader(@"S:\HandDatabase-cropped\ExtractedSamples\croppedImages\", "*.jpg", recursive: true)
                      .Select(x => getOrientationImage(x.Convert<Gray, byte>()), (percentage) => Console.Write("\tReading positives: {0} % \r", percentage * 100))
                             );

            negatives.AddRange(
                    new ImageDirectoryReader(@"S:\HandDatabase-cropped\negatives\", "*.jpg", recursive: true)
                    .Select(x => getOrientationImage(x.Convert<Gray, byte>()), (percentage) => Console.Write("\tReading negatives: {0} % \r", percentage * 100))
                    .Where(x=>x != null) //there is a bug in RetainQuantizedOrientations
                        );
        }

        private static Image<Gray, byte> getOrientationImage(Image<Gray, byte> image, int neigborhoodSpread = 7)
        { 
            Image<Gray, int> magnitudeSqrImage = null;
            Image<Gray, int> orientationImage = GradientComputation.Compute(image, out magnitudeSqrImage, minValidMagnitude: 35);

            Image<Gray, byte> featureImage = FeatureMap.Calculate(orientationImage, neigborhoodSpread);
            return featureImage;
        }
    }
}
