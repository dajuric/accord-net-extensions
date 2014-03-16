using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using StageClassifier = RT.GentleBoost<RT.RegressionTree<RT.BinTestCode>>;
using WeakLearner = RT.RegressionTree<RT.BinTestCode>;

namespace RT
{
    public class PicoClassifier
    {                    
        public PicoClassifier(RectangleF normalizedRegion, Cascade<StageClassifier> cascade)
        {
            this.NormalizedRegion = normalizedRegion;
            this.Cascade = cascade;
        }

        public Cascade<StageClassifier> Cascade { get; private set; }
        public RectangleF NormalizedRegion { get; private set; }

        #region Misc

        public Rectangle GetRegion(PointF regionCenter, float regionScale)
        {
            Point center = new Point
            {
                X = (int)(regionCenter.X + regionScale * NormalizedRegion.X),
                Y = (int)(regionCenter.Y + regionScale * NormalizedRegion.Y)
            };

            Size size = GetSize(regionScale);
            
            return new Rectangle
            {
                X = center.X - size.Width / 2,
                Y = center.Y - size.Height / 2,
                Width = size.Width,
                Height = size.Height
            };
        }

        public Size GetSize(float regionScale)
        {
            Size size = new Size
            {
                Width = (int)(regionScale * NormalizedRegion.Width),
                Height = (int)(regionScale * NormalizedRegion.Height)
            };

            return size;
        }

        #endregion

        #region Classification

        public bool ClassifyRegion(Image<Gray, byte> image, Rectangle region, int angleDeg, out float confidence)
        {          
            return ClassifyRegion(image, region.Center(), region.Size, angleDeg, out confidence);
        }

        public bool ClassifyRegion(Image<Gray, byte> image, Point regionCenter, Size regionSize, int angleDeg, out float confidence)
        {
            if (BinTestCode.IsInBounds(image.Size, regionCenter, regionSize, angleDeg != 0) == false)
            {
                confidence = 0;
                return false;
            }

            return Cascade.Classify(stageClassifier =>
                                    stageClassifier.GetOutput(weakLearner =>
                                                              weakLearner.GetOutput(binTest =>
                                                                                    binTest.Test(image, regionCenter, regionSize, angleDeg))),

                                     out confidence);
        }

        #endregion

        #region Training

        public void TrainStage(IEnumerable<Image<Gray, byte>> allPositiveSamples, IEnumerable<Image<Gray, byte>> allNegativeSamples, List<Rectangle> allPositiveSampleWindows, 
                               float minTPR, float maxFPR,
                               int treeMaxDepth = 6, int numberOfBinaryTests = 1024)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            StageClassifier stageClassifier = new StageClassifier();

            List<Image<Gray, byte>> images;
            List<Rectangle> windows;
            List<bool> classLabels;
            List<float> confidences;

            int maxNumberOfNegativeSamples = 2 * allPositiveSamples.Count();
            sampleTrainingData(allPositiveSamples, allPositiveSampleWindows, allNegativeSamples, maxNumberOfNegativeSamples,
                               out images, out windows, out classLabels, out confidences);

            float[] targetValues = classLabels.Select(x => (x == true) ? +1f : -1f).ToArray();

            float threshold = 0;
            stageClassifier.Train(targetValues,

                                  //create and train weak learner
                                  (sampleWeights) => weakLearnerTrain(treeMaxDepth, 
                                                                      () => weakLearnerProvideFeatures(numberOfBinaryTests, () => rand.Next()),
                                                                      targetValues, 
                                                                      sampleWeights,
                                                                      images, 
                                                                      windows
                                                                      ),
                                 
                                  //get output from trained learner
                                  (learner, sampleIndex) => weakLearnerClassify(learner, 
                                                                                images[sampleIndex].Convert<Gray, byte>(),
                                                                                windows[sampleIndex]),
                                     
                                  //after each weak classifier training check whether the training process should be stopped
                                  (learners, learnerOutputs) => 
                                  {
                                      float truePositiveRate, falsePositiveRate;
                                      float startThreshold = 5f, thresholdDelta = -0.005f;

                                      threshold = Cascade<StageClassifier>.SearchMinTPR(classLabels, minTPR, 
                                                                                        startThreshold, thresholdDelta, 
                                                                                        stageClassifier, 
                                                                                        (classifier, idx, th) => learnerOutputs[idx] > th, 
                                                                                        out truePositiveRate, out falsePositiveRate);

                                      return falsePositiveRate < maxFPR;
                                  }
                                );

            //add trained stage to the cascade
            this.Cascade.AddStage(stageClassifier, threshold);
        }

        private static BinTestCode[] weakLearnerProvideFeatures(int numberOfBinaryTests, Func<int> nextRandom)
        {
            return EnumerableExtensions.Create(numberOfBinaryTests, (_) => new BinTestCode(nextRandom()));
        }

        private static WeakLearner weakLearnerTrain(int treeMaxDepth, Func<BinTestCode[]> nodeFeatureProvider, float[] targetValues, float[] sampleWeights, IEnumerable<Image<Gray, byte>> images, IList<Rectangle> windows)
        {
            var learner = new WeakLearner(treeMaxDepth);
            learner.Train
                (
                //get features for each node (random binary codes)
                   nodeFeatureProvider: nodeFeatureProvider,
                //classify each image using binary test
                   rightNodeSelector: (binTest, sampleIndex) =>
                   {
                       var image = images.ElementAt(sampleIndex);
                       var window = windows[sampleIndex];

                       return binTest.Test(image, window.Center(), window.Size, angleDeg: 0, clipToImageBounds: true);
                   },
                //target values
                   targetValues: targetValues,
                //sample weights
                   sampleWeights: sampleWeights
                 );

            return learner;
        }

        private static float weakLearnerClassify(WeakLearner learner, Image<Gray, byte> image, Rectangle window)
        {
            var output = learner.GetOutput(binTest =>
                            {
                                //classify each image using binary test
                                return binTest.Test(image, window.Center(), window.Size, angleDeg: 0, clipToImageBounds: false);
                            });

            return output;
        }

        private void sampleTrainingData(IEnumerable<Image<Gray, byte>> positives,    IList<Rectangle> objectRegions,
                                        IEnumerable<Image<Gray, byte>> negatives, int maxNegatives,

                                        out List<Image<Gray, byte>> samples, out List<Rectangle> windows,
                                        out List<bool> classLabels         , out List<float> classifierOutputs)
        {
            samples = new List<Image<Gray, byte>>();
            windows = new List<Rectangle>();
            classLabels = new List<bool>();
            classifierOutputs = new List<float>();

            /*************************** load positive samples **************************************/
            var nTotalPositives = positives.Count();
            for (int i = 0; i < nTotalPositives; i++)
            {
                var im = positives.ElementAt(i);

                float confidence;
                var classifiedAsPositive = this.ClassifyRegion(im, objectRegions[i], 0, out confidence);

                if (classifiedAsPositive)
                {
                    windows.Add(objectRegions.ElementAt(i));
                    samples.Add(positives.ElementAt(i));
                    classLabels.Add(true);
                    classifierOutputs.Add(confidence);
                }
            }
            /*************************** load positive samples **************************************/

            /*************************** load false-positives *************************************/
            const int MIN_WINDOW_SIZE = 24;

            Random rand = new Random();
            var nTotalNegatives = negatives.Count();
            var nFPs = 0;
            var nPickedNegatives = 0;

            while (nFPs <= maxNegatives)
            { 
                //pick random background image
                var idx = rand.Next(0, nTotalNegatives);
                var negativeSample = negatives.ElementAt(idx);

                //pick random window
                var row = rand.Next(0, negativeSample.Height);
                var col = rand.Next(0, negativeSample.Width);
                var scale = rand.Next(0, 2 * Math.Min(Math.Min(row, negativeSample.Height - row), Math.Min(col, negativeSample.Width - col)));

                var window = GetRegion(new PointF(col, row), scale);

                if (scale < MIN_WINDOW_SIZE)
                {
                    continue;
                }

                //classify the region
                float confidence;
                var classifiedAsPositive = this.ClassifyRegion(negativeSample, window, 0, out confidence);

                //it is a false-positive
                if (classifiedAsPositive)
                {
                    windows.Add(window);
                    samples.Add(negativeSample);
                    classLabels.Add(false);
                    classifierOutputs.Add(confidence);

                    nFPs++;
                }

                nPickedNegatives++;
            }
            /*************************** load false-positives *************************************/

            var truePositiveRate  = classLabels.Where(x => x == true ).Count() / (float)nTotalPositives;
            var falsePositiveRate = classLabels.Where(x => x == false).Count() / (float)nPickedNegatives;
        }

        #endregion

    }

}
