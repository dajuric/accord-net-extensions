#define LOG

using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace GOCO
{
    [Serializable]
    public class GocoClassifier
    {
        /// <summary>
        /// Constructs an empty classifier for training.
        /// </summary>
        /// <param name="windowWidthMultiplier">Bounding box width multiplier (1 for square window).</param>
        public GocoClassifier(float windowWidthMultiplier = 1)
            : this(windowWidthMultiplier, new List<Stage<GentleBoost<CART<BinTestCode, float>>>>())
        { }

        /// <summary>
        /// Loads a trained classifier.
        /// </summary>
        /// <param name="windowWidthMultiplier">Bounding box width multiplier (1 for square window).</param>
        /// <param name="cascade">Trained cascade.</param>
        public GocoClassifier(float windowWidthMultiplier, List<Stage<GentleBoost<CART<BinTestCode, float>>>> cascade)
        {
            this.WindowWidthMultiplier = windowWidthMultiplier;
            this.Cascade = cascade;
        }

        /// <summary>
        /// Gets the cascade (classifier).
        /// </summary>
        public List<Stage<GentleBoost<CART<BinTestCode, float>>>> Cascade { get; private set; }
        
        /// <summary>
        /// Gets the bounding box width multiplier (for non-square windows).
        /// </summary>
        public float WindowWidthMultiplier { get; private set; }

        public bool ClassifyRegion(Image<Gray, byte> image, Rectangle area, out float confidence)
        {
            /*if (Math.Abs((float)regionSize.Width / regionSize.Height - this.WindowWidthMultiplier) > 1.5E-1)
            {
                throw new ArgumentException("Region size width-height ratio must be equal to WindowWidthMultiplier! (tolerance +/- 0.1)");
            }*/

            return Cascade.Evaluate(classifier => classifier.Evaluate(image, area), out confidence);           
        }

        public Rectangle GetRegion(PointF regionCenter, float regionScale)
        {
            Size size = Size.Round(GetSize(regionScale));

            return new Rectangle
            {
                X = (int)regionCenter.X - size.Width / 2,
                Y = (int)regionCenter.Y - size.Height / 2,
                Width = size.Width,
                Height = size.Height
            };
        }

        public SizeF GetSize(float regionScale)
        {
            SizeF size = new SizeF
            {
                Width = regionScale * this.WindowWidthMultiplier,
                Height = regionScale
            };

            return size;
        }

        /*************************** Cascade ***************************/
        public bool AddStage(IList<Image<Gray, byte>> positives,
                             IList<Image<Gray, byte>> negatives,
                             float targetFPR = 1e-6f, float minTPR = 0.980f, float maxFPR = 0.5f, int maxTrees = 1, int treeMaxDepth = 6, int numberOfBinaryTests = 1024)
        {
            var windows = positives.Select(x => new Rectangle(0, 0, x.Width, x.Height)).ToList();
            return AddStage(positives, windows, negatives, targetFPR, minTPR, maxFPR, maxTrees, treeMaxDepth, numberOfBinaryTests);
        }

        public bool AddStage(IList<Image<Gray, byte>> positives, IList<Rectangle> windows,
                             IList<Image<Gray, byte>> negatives,
                             float targetFPR = 1e-6f, float minTPR = 0.980f, float maxFPR = 0.5f, int maxTrees = 1, int treeMaxDepth = 6, int numberOfBinaryTests = 1024)
        {
#if LOG
            Console.WriteLine();
            Console.WriteLine("Stage training {0} ---------------------------------------------------------------", this.Cascade.Count + 1);
            Console.WriteLine();
#endif

            float MIN_SCALE = 10;
            var minWindowSize = new SizeF(MIN_SCALE * this.WindowWidthMultiplier, MIN_SCALE);

            Func<GentleBoost<CART<BinTestCode, float>>, ImageSample<Image<Gray, byte>>, float> classificationFunc = (boost, sample) => boost.Evaluate(sample.Image, sample.ROI);

            var tpSamplingResult = this.Cascade.GetTruePositives(classificationFunc, positives, windows);

            var nFPsToPick = 2 * positives.Count - tpSamplingResult.ClassifedSamples.Count;

            var fpSamplingResult = this.Cascade.SampleFalsePositives(classificationFunc, negatives, minWindowSize, nFPsToPick);

            if (fpSamplingResult.PrecisionRate <= targetFPR)
                return false;

            var stageClassifier = new GentleBoost<CART<BinTestCode, float>>();
            var classLabels = tpSamplingResult.ClassifedSamples.Select(x => true)
                              .Concat(fpSamplingResult.ClassifedSamples.Select(x => false))
                              .ToArray();

            var images = tpSamplingResult.ClassifedSamples.Select(x => x.Sample.Image.GetSubRect(x.Sample.ROI))
                         .Concat(fpSamplingResult.ClassifedSamples.Select(x => x.Sample.Image.GetSubRect(x.Sample.ROI)))
                         .ToArray();

            var outputs = tpSamplingResult.ClassifedSamples.Select(x => x.Confidence)
                          .Concat(fpSamplingResult.ClassifedSamples.Select(x => x.Confidence))
                          .ToArray();

            var stage = stageClassifier.Train(classLabels, images, numberOfBinaryTests, treeMaxDepth, maxTrees, minTPR, maxFPR, outputs);
            this.Cascade.Add(stage);
            return true;
        }
    }
}
