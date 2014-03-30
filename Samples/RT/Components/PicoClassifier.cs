#define LOG

using Match = System.Tuple<Accord.Extensions.Imaging.Image<Accord.Extensions.Imaging.Gray, byte>, Accord.Extensions.Rectangle, float>;

using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using StageClassifier = RT.GentleBoost<RT.RegressionTree<RT.BinTestCode>>;
using WeakLearner = RT.RegressionTree<RT.BinTestCode>;
using System.Threading.Tasks;

namespace RT
{
    /// <summary>
    /// Pixel Intensity Comparison-based Object detection (PICO).
    /// For details see: <see cref="https://github.com/nenadmarkus/pico"/>.
    /// </summary>
    public class PicoClassifier
    {
        /// <summary>
        /// Constructs an empty classifier for training.
        /// </summary>
        /// <param name="normalizedRegion">Bounding box shape and offset (e.g. [X: 0, Y: 0, Width: 1, Height: 1]).</param>
        public PicoClassifier(RectangleF normalizedRegion)
            :this(normalizedRegion, new Cascade<StageClassifier>())
        {}
       
        /// <summary>
        /// Loads a trained classifier.
        /// </summary>
        /// <param name="normalizedRegion">Bounding box shape and offset.</param>
        /// <param name="cascade">Trained cascade.</param>
        public PicoClassifier(RectangleF normalizedRegion, Cascade<StageClassifier> cascade)
        {
            this.NormalizedRegion = normalizedRegion;
            this.Cascade = cascade;
        }

        /// <summary>
        /// Gets the cascade (classifier).
        /// </summary>
        public Cascade<StageClassifier> Cascade { get; private set; }
        /// <summary>
        /// Gets the bounding box shape and offset (e.g. [X: 0, Y: 0, Width: 1, Height: 1]).
        /// </summary>
        public RectangleF NormalizedRegion { get; private set; }

        #region Misc

        /// <summary>
        /// Creates a new rectangle by using center point, region size and <see cref="NormalizedRegion"/>.
        /// </summary>
        /// <param name="regionCenter">Center point.</param>
        /// <param name="regionScale">Region size (edge length).</param>
        /// <returns>A new rectangle created from specified data.</returns>
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

        /// <summary>
        /// Creates size by using specified scale and <see cref="NormalizedRegion"/>.
        /// </summary>
        /// <param name="regionScale">Region scale (must be greater than 1).</param>
        /// <returns>A new size created from specified data.</returns>
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

        /// <summary>
        /// Classifies an image portion specified by the region which shape and offset is determined by <seealso cref="NormalizedRegion"/>.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="region">Region to classify. The region can be obtained by using <see cref="GetRegion"/>.</param>
        /// <param name="confidence">Classification confidence. The value is not normalized.</param>
        /// <param name="angleDeg">To detect rotated object, specify the value different than 0. The angle is specified in degrees.</param>
        /// <returns>True if the confidence is greater than 0, false if this is not the case or the region is specified outside valid image boundaries.</returns>
        public bool ClassifyRegion(Image<Gray, byte> image, Rectangle region, out float confidence, int angleDeg = 0)
        {          
            return ClassifyRegion(image, region.Center(), region.Size, angleDeg, out confidence);
        }

        /// <summary>
        /// Classifies an image portion specified by the region which shape and offset is determined by <seealso cref="NormalizedRegion"/>.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="regionCenter">Center of the region.</param>
        /// <param name="regionSize">Region size.</param>
        /// <param name="angleDeg">To detect rotated object, specify the value different than 0. The angle is specified in degrees.</param>
        /// <param name="confidence">Classification confidence. The value is not normalized.</param>
        /// <returns>True if the confidence is greater than 0, false if this is not the case or the region is specified outside valid image boundaries.</returns>
        public bool ClassifyRegion(Image<Gray, byte> image, Point regionCenter, Size regionSize, int angleDeg, out float confidence)
        {
            return Cascade.Classify(stageClassifier =>
                                    stageClassifier.GetOutput(weakLearner =>
                                                              weakLearner.GetOutput(binTest =>
                                                                                    binTest.Test(image, regionCenter, regionSize, angleDeg))),

                                     out confidence);
        }

        #endregion

        #region Training

        /// <summary>
        /// Trains and adds new stage to the current cascaded classifier.
        /// </summary>
        /// <param name="allPositiveSamples">Positive samples.</param>
        /// <param name="allNegativeSamples">Negative samples.</param>
        /// <param name="allPositiveSampleWindows">Positive sample windows.</param>
        /// <param name="targetFPR">Target false positive rate. If the calculated FPR is less than the specified number, new stage will not be appended. (e.g. 1e-3).</param>
        /// <param name="minTPR">Minimum stage true positive rate (e.g. 0.98).</param>
        /// <param name="maxFPR">Maximum stage false positive rate (e.g. 0.5).</param>
        /// <param name="maxTrees">Maximum number of trees (weak learners) per stage. (e.g. 1-5).</param>
        /// <param name="treeMaxDepth">Maximum depth of the regression tree (weak learner in GentleBoost). (e.g. 6)</param>
        /// <param name="numberOfBinaryTests">Number of generated binary tests per regression tree node. Only one of the specified number of tests will be selected. (e.g. 1024)</param>
        /// <returns>True if the stage is appended, false otherwise.</returns>
        public bool AddStage(IList<Image<Gray, byte>> allPositiveSamples, IList<Image<Gray, byte>> allNegativeSamples, IList<Rectangle> allPositiveSampleWindows, float targetFPR = 1e-6f, float minTPR = 0.980f, float maxFPR = 0.5f, int maxTrees = 1, int treeMaxDepth = 6, int numberOfBinaryTests = 1024 * 4)
        {
            Random rand = new Random(0);
            StageClassifier stageClassifier = new StageClassifier();

#if LOG
            Console.WriteLine();
            Console.WriteLine("Stage training {0} ---------------------------------------------------------------", this.Cascade.NumberOfStages + 1);
            Console.WriteLine();
#endif
            float tpr;
            var truePositives = getTruePositives(allPositiveSamples, allPositiveSampleWindows, out tpr);

            float fpr;
            var nFPsToPick = 2 * allPositiveSamples.Count - truePositives.Count(); 
            var falsePositives = sampleFalsePositives(allNegativeSamples, (nFPs) => nFPs >= nFPsToPick, out fpr);

            if (fpr <= targetFPR)
                return false;

            var matches = truePositives.Concat(falsePositives).ToList();

            return addStage
                (
                   matches.Select(x => x.Item1).ToList(),
                   matches.Select(x => x.Item2).ToList(),
                   matches.Select(x => x.Item3).ToList(),
                   EnumerableExtensions.Create(truePositives.Count(), (_) => true).Concat(EnumerableExtensions.Create(falsePositives.Count(), (_) => false)).ToList(),
                   minTPR, maxFPR, maxTrees, treeMaxDepth, numberOfBinaryTests
                );
        }

        /// <summary>
        /// Trains and adds new stage to the current cascaded classifier.
        /// </summary>
        /// <param name="images">Positive and negative sample images.</param>
        /// <param name="windows">Positive and negative sample object regions.</param>
        /// <param name="confidences">Classifier output confidences.</param>
        /// <param name="classLabels">Class labels: true for positive samples, false for negative samples.</param>
        /// <param name="minTPR">Minimum stage true positive rate (e.g. 0.98).</param>
        /// <param name="maxFPR">Maximum stage false positive rate (e.g. 0.5).</param>
        /// <param name="maxTrees">Maximum number of trees (weak learners) per stage. (e.g. 1-5).</param>
        /// <param name="treeMaxDepth">Maximum depth of the regression tree (weak learner in GentleBoost). (e.g. 6)</param>
        /// <param name="numberOfBinaryTests">Number of generated binary tests per regression tree node. Only one of the specified number of tests will be selected. (e.g. 1024)</param>
        /// <returns>True if the stage is appended, false otherwise.</returns>
        private bool addStage(IList<Image<Gray, byte>> images, IList<Rectangle> windows, IList<float> confidences, IList<bool> classLabels, float minTPR, float maxFPR, int maxTrees, int treeMaxDepth, int numberOfBinaryTests)
        {
            StageClassifier stageClassifier = new StageClassifier();

            float[] targetValues = classLabels.Select(x => (x == true) ? +1f : -1f).ToArray();

            float threshold = 0;
            stageClassifier.Train(targetValues, confidences.ToArray(),

                                  //create and train weak learner
                                  (sampleWeights) => weakLearnerTrain(treeMaxDepth,
                                                                      () => weakLearnerProvideFeatures(numberOfBinaryTests),
                                                                      targetValues,
                                                                      sampleWeights,
                                                                      images,
                                                                      windows
                                                                      ),

                                  //get output from trained learner
                                  (learner, sampleIndex) => weakLearnerClassify(learner,
                                                                                images[sampleIndex],
                                                                                windows[sampleIndex]),

                                  //after each weak classifier training check whether the training process should be stopped
                                  (learners, learnerOutputs) =>
                                  {
                                      float startThreshold = learnerOutputs.Max();
                                      const float THRESHOLD_SEARCH_STEP = -0.005f; //decrease by x

#if LOG
                                      Console.Write("\r\tStage: Searching for best threshold... ");
#endif
                                      float truePositiveRate, falsePositiveRate;
                                      threshold = Cascade<StageClassifier>.SearchMinTPR(classLabels, minTPR,
                                                                                        startThreshold, THRESHOLD_SEARCH_STEP,
                                                                                        stageClassifier,
                                                                                        (classifier, idx, th) => learnerOutputs[idx] > th,
                                                                                        out truePositiveRate, out falsePositiveRate);

#if LOG
                                      Console.WriteLine("{0}", threshold);
                                      Console.WriteLine("\r\tGentleBoost: Termination condition check - nLearners: {0}, TPR: {1}, FPR: {2}", learners.Count, truePositiveRate, falsePositiveRate);
#endif

                                      return learners.Count >= maxTrees || falsePositiveRate <= maxFPR; //if true then stop training
                                  }
                                );

            //add trained stage to the cascade
            this.Cascade.AddStage(stageClassifier, threshold);

#if LOG
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();
#endif

            return true;
        }

        /// <summary>
        /// Creates an array of features (binary tests) used for selection in a regression tree node during training procedure.
        /// </summary>
        /// <param name="numberOfBinaryTests">Number of generated binary tests per regression tree node. Only one of the specified number of tests will be selected.</param>
        /// <returns>An array of features (binary tests) to select from.</returns>
        private static BinTestCode[] weakLearnerProvideFeatures(int numberOfBinaryTests)
        {
            var rand = new Random();
            return EnumerableExtensions.Create(numberOfBinaryTests, (_) => new BinTestCode(rand.Next()));
        }

        /// <summary>
        /// Trains a GentleBoost learner (regression tree).
        /// </summary>
        /// <param name="treeMaxDepth">Maximum depth of the regression tree (weak learner in GentleBoost).</param>
        /// <param name="nodeFeatureProvider">The node feature provider (<see cref="weakLearnerProvideFeatures"/>).</param>
        /// <param name="targetValues">Target function values that needs to be approximated (for classification the values are +1 - positive sample, -1 negative sample).</param>
        /// <param name="sampleWeights">The weights of samples used in GentleBoost algorithm.</param>
        /// <param name="images">The array of positive and negative samples.</param>
        /// <param name="windows">The array of regions of positive and negative samples.</param>
        /// <returns>Trained weak learner (regression tree).</returns>
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

        /// <summary>
        /// Classifies an specified image region by using weak learner (used by GentleBoost).
        /// </summary>
        /// <param name="learner">Weak learner.</param>
        /// <param name="image">Input image.</param>
        /// <param name="window">Region to classify.</param>
        /// <returns>Weak learner confidence.</returns>
        private static float weakLearnerClassify(WeakLearner learner, Image<Gray, byte> image, Rectangle window)
        {
            var output = learner.GetOutput(binTest =>
                            {
                                //classify each image using binary test
                                return binTest.Test(image, window.Center(), window.Size, angleDeg: 0, clipToImageBounds: false);
                            });

            return output;
        }
   
        /// <summary>
        /// Gets true positive samples from positive samples.
        /// </summary>
        /// <param name="positives">Object samples.</param>
        /// <param name="objectRegions">Object regions within an image.</param>
        /// <param name="truePositiveRate">True positive rate.</param>
        /// <returns>True positive samples.</returns>
        private IEnumerable<Match> getTruePositives(IEnumerable<Image<Gray, byte>> positives, IList<Rectangle> objectRegions,
                                                    out float truePositiveRate)
        {
#if LOG
            int nSampled = 0;
#endif

            int nTPs = 0;
            var truePositives = new ConcurrentBag<Match>();

            /*************************** load positive samples **************************************/
            var nTotalPositives = positives.Count();

            Parallel.For(0, nTotalPositives, (i) => 
            {
                var im = positives.ElementAt(i);

                float confidence;
                var classifiedAsPositive = this.ClassifyRegion(im, objectRegions[i], out confidence, 0);

                if (classifiedAsPositive)
                {
                    truePositives.Add(new Match
                        (
                           positives.ElementAt(i),
                           objectRegions.ElementAt(i),
                           confidence)
                        );

                    Interlocked.Increment(ref nTPs);
                }
#if LOG
                Interlocked.Increment(ref nSampled);

                if(nSampled % 1000 == 0)
                    Console.Write("\rSampling positives. nTPs / nTotalPositives: {0} / {1}", nTPs, nTotalPositives);
#endif
            });

            truePositiveRate = nTPs / (float)nTotalPositives;

#if LOG
            Console.Write("\rSampling positives. nTPs / nTotalPositives: {0} / {1}", nTPs, nTotalPositives);
            Console.WriteLine(" - TPR: {0}", truePositiveRate);
#endif
            /*************************** load positive samples **************************************/

            return truePositives;
        }

        /// <summary>
        /// Samples false-positive samples by random selecting the image and region and evaluating the classifier on each picked sample.
        /// </summary>
        /// <param name="negatives">Negative samples (background).</param>
        /// <param name="terminationFunc">
        /// Termination function. 
        /// Parameters: current number of sampled false positive samples.
        /// Returns: True if the termination is wanted, false otherwise.
        /// </param>
        /// <param name="falsePositiveRate">False positive rate.</param>
        /// <returns>False positive samples.</returns>
        private IEnumerable<Match> sampleFalsePositives(IEnumerable<Image<Gray, byte>> negatives, Func<int, bool> terminationFunc,
                                                        out float falsePositiveRate)
        {
            var falsePositives = new ConcurrentBag<Match>();

            /*************************** load false-positives *************************************/
            const int MIN_WINDOW_SIZE = 24;

            var nTotalNegatives = negatives.Count();
            var nFPs = 0;
            var nPickedNegatives = 0;

            ParallelExtensions.While(() => terminationFunc(nFPs) == false,
                (loopState) =>
                {
                    //pick random background image
                    var idx = (int)(ParallelRandom.Next() % nTotalNegatives);
                    var negativeSample = negatives.ElementAt(idx);

                    //pick random window
                    var row = ParallelRandom.Next() % negativeSample.Height;
                    var col = ParallelRandom.Next() % negativeSample.Width;
                    var scale = ParallelRandom.Next() % (2 * Math.Min(Math.Min(row, negativeSample.Height - row), Math.Min(col, negativeSample.Width - col)) + 1);

                    var window = GetRegion(new PointF(col, row), scale);

                    if (scale < MIN_WINDOW_SIZE)
                    {
                        return; //continue;
                    }

                    //classify the region
                    float confidence;
                    var classifiedAsPositive = this.ClassifyRegion(negativeSample, window, out confidence);

                    //it is a false-positive
                    if (classifiedAsPositive)
                    {
                        falsePositives.Add(new Match
                            (
                               negativeSample,
                               window, 
                               confidence
                            ));

                        Interlocked.Increment(ref nFPs);
                    }

                    Interlocked.Increment(ref nPickedNegatives);

#if LOG
                    if(nPickedNegatives % 1000 == 0)
                        Console.Write("\rSampling negatives. nFPs / nTotalNegatives: {0} / {1}", nFPs, nPickedNegatives);
#endif
                });
            /*************************** load false-positives *************************************/


            falsePositiveRate = nFPs / (float)nPickedNegatives;

#if LOG
            Console.Write("\rSampling negatives. nFPs / nTotalNegatives: {0} / {1}", nFPs, nPickedNegatives);
            Console.WriteLine(" - FPR: {0}", falsePositiveRate);
#endif

            return falsePositives;
        }

        #endregion
    }
}
