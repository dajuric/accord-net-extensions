#define LOG

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
//using StageClassifier = RT.GentleBoost<RT.RegressionTree<RT.BinTestCode>>;
//using WeakLearner = RT.RegressionTree<RT.BinTestCode>;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RT
{
    /// <summary>
    /// Pixel Intensity Comparison-based Object detection (PICO).
    /// For details see: <see cref="https://github.com/nenadmarkus/pico"/>.
    /// </summary>
    public class PicoClassifier<TColor>
        where TColor: IColor
    {
        /// <summary>
        /// Constructs an empty classifier for training.
        /// </summary>
        /// <param name="windowWidthMultiplier">Bounding box width multiplier (1 for square window).</param>
        public PicoClassifier(float windowWidthMultiplier = 1)
            : this(windowWidthMultiplier, new Cascade<GentleBoost<RegressionTree<BinTestCode<TColor>>>>())
        {}
       
        /// <summary>
        /// Loads a trained classifier.
        /// </summary>
        /// <param name="windowWidthMultiplier">Bounding box width multiplier (1 for square window).</param>
        /// <param name="cascade">Trained cascade.</param>
        public PicoClassifier(float windowWidthMultiplier, Cascade<GentleBoost<RegressionTree<BinTestCode<TColor>>>> cascade)
        {
            this.WindowWidthMultiplier = windowWidthMultiplier;
            this.Cascade = cascade;
        }

        /// <summary>
        /// Gets the cascade (classifier).
        /// </summary>
        public Cascade<GentleBoost<RegressionTree<BinTestCode<TColor>>>> Cascade { get; private set; }
        /// <summary>
        /// Gets the bounding box width multiplier (for non-square windows).
        /// </summary>
        public float WindowWidthMultiplier { get; private set; }

        #region Misc

        /// <summary>
        /// Creates a new rectangle by using center point, region size and <see cref="NormalizedRegion"/>.
        /// </summary>
        /// <param name="regionCenter">Center point.</param>
        /// <param name="regionScale">Region size (edge length).</param>
        /// <returns>A new rectangle created from specified data.</returns>
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

        /// <summary>
        /// Creates size by using specified scale and <see cref="NormalizedRegion"/>.
        /// </summary>
        /// <param name="regionScale">Region scale (must be greater than 1).</param>
        /// <returns>A new size created from specified data.</returns>
        public SizeF GetSize(float regionScale)
        {
            SizeF size = new SizeF
            {
                Width = regionScale * this.WindowWidthMultiplier,
                Height = regionScale
            };

            return size;
        }

        #endregion

        #region Classification

        /// <summary>
        /// Classifies an image portion specified by the region which shape and offset is determined by <seealso cref="NormalizedRegion"/>.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="regionCenter">Center of the region.</param>
        /// <param name="regionSize">Region size. The width / height ratio must be equal to <see cref="WindowWidthMultiplier"/>.</param>
        /// <param name="confidence">Classification confidence. The value is not normalized.</param>
        ///// <param name="angleDeg">To detect rotated object, specify the value different than 0. The angle is specified in degrees.</param>
        /// <returns>True if the confidence is greater than 0, false if this is not the case or the region is specified outside valid image boundaries.</returns>
        public bool ClassifyRegion(Image<TColor, byte> image, Point regionCenter, Size regionSize, out float confidence)
        {
            if (Math.Abs((float)regionSize.Width / regionSize.Height - this.WindowWidthMultiplier) > 1.5E-1)
            {
                throw new ArgumentException("Region size width-height ratio must be equal to WindowWidthMultiplier! (tolerance +/- 0.1)");
            }

            return Cascade.Classify(stageClassifier =>
                                    stageClassifier.GetOutput(weakLearner =>
                                                              weakLearner.GetOutput(binTest =>
                                                                                    binTest.Test(image, regionCenter, regionSize))),

                                     out confidence);
        }

        #endregion

        #region Training

        public bool AddStage(IList<Image<TColor, byte>> allPositiveSamples, IList<List<Rectangle>> windows, 
                             float targetFPR = 1e-6f, float minTPR = 0.980f, float maxFPR = 0.5f, int maxTrees = 1, int treeMaxDepth = 6, int numberOfBinaryTests = 1024)
        {
            Size minSize = new Size(10, 10);

            var positives = new List<ImageSample<TColor>>();
            for (int imgIdx = 0; imgIdx < allPositiveSamples.Count; imgIdx++)
            {
                var imageWindows = windows[imgIdx];
                for (int annIdx = 0; annIdx < imageWindows.Count; annIdx++)
                {
                    var p = new ImageSample<TColor> { Image = allPositiveSamples[imgIdx], ROI = imageWindows[annIdx] };
                    positives.Add(p);
                }          
            }

            var tpSamplingResult = Cascade.GetTruePositives(positives, classify);

            var nFPsToPick = 2 * positives.Count - tpSamplingResult.ClassifedSamples.Count;

            var fpSamplingResult = Cascade.SampleFalsePositives
                                            (
                                            positives:             allPositiveSamples,
                                            windowCreator:         (sampleIdx) => allPositiveSamples[sampleIdx].Size.CreateRandomRegion(this.WindowWidthMultiplier, minSize), 
                                            forbidenImageRegions:  windows,
                                            classificationFunc:    (imgIdx, imgSample) => classify(imgSample),
                                            terminationFunc:       (nFPs) => nFPs >= nFPsToPick
                                            );

            return AddStage
                (
                //true-positive sampler
                   tpSamplingResult,
                //false-positive sampler
                   fpSamplingResult,
                //parameters
                   targetFPR, minTPR, maxFPR, maxTrees, treeMaxDepth, numberOfBinaryTests
                );
        }

        /// <summary>
        /// Trains and adds new stage to the current cascaded classifier.
        /// </summary>
        /// <param name="allPositiveSamples">Positive samples.</param>
        /// <param name="allPositiveSampleWindows">Positive sample windows.</param>
        /// <param name="allNegativeSamples">Negative samples.</param>
        /// <param name="targetFPR">Target false positive rate. If the calculated FPR is less than the specified number, new stage will not be appended. (e.g. 1e-3).</param>
        /// <param name="minTPR">Minimum stage true positive rate (e.g. 0.98).</param>
        /// <param name="maxFPR">Maximum stage false positive rate (e.g. 0.5).</param>
        /// <param name="maxTrees">Maximum number of trees (weak learners) per stage. (e.g. 1-5).</param>
        /// <param name="treeMaxDepth">Maximum depth of the regression tree (weak learner in GentleBoost). (e.g. 6)</param>
        /// <param name="numberOfBinaryTests">Number of generated binary tests per regression tree node. Only one of the specified number of tests will be selected. (e.g. 1024)</param>
        /// <returns>True if the stage is appended, false otherwise.</returns>
        public bool AddStage(IList<Image<TColor, byte>> allPositiveSamples, IList<Rectangle> allPositiveSampleWindows, 
                             IList<Image<TColor, byte>> allNegativeSamples, 
                             float targetFPR = 1e-6f, float minTPR = 0.980f, float maxFPR = 0.5f, int maxTrees = 1, int treeMaxDepth = 6, int numberOfBinaryTests = 1024)
        {
           Size minSize = new Size(24, 24);
            
            var positives = allPositiveSamples
                            .Select((_, index) => new ImageSample<TColor>
                            {
                                Image =  allPositiveSamples[index], 
                                ROI = allPositiveSampleWindows[index] 
                            })
                            .ToList();

            var tpSamplingResult = Cascade.GetTruePositives(positives, classify);

            var nFPsToPick = 2 * positives.Count - tpSamplingResult.ClassifedSamples.Count;

            var fpSamplingResult = Cascade.SampleFalsePositives
                                            (
                                                allNegativeSamples,
                                                (sampleIdx) => allNegativeSamples[sampleIdx].Size.CreateRandomRegion(this.WindowWidthMultiplier, minSize), 
                                                (imgIdx, imgSample) => classify(imgSample),
                                                (nFPs) => nFPs >= nFPsToPick
                                            );

            return AddStage
                (
                //true-positive sampler
                   tpSamplingResult,
                //false-positive sampler
                   fpSamplingResult,
                //parameters
                   targetFPR, minTPR, maxFPR, maxTrees, treeMaxDepth, numberOfBinaryTests
                );
        }

        public bool AddStage(SamplingResult<ImageSample<TColor>> tpSamplingResult, SamplingResult<ImageSample<TColor>> fpSamplingResult,                       
                             float targetFPR = 1e-6f, float minTPR = 0.980f, float maxFPR = 0.5f, int maxTrees = 1, int treeMaxDepth = 6, int numberOfBinaryTests = 1024)
        {
            var stageClassifier = new GentleBoost<RegressionTree<BinTestCode<TColor>>>();
            
#if LOG
            Console.WriteLine();
            Console.WriteLine("Stage training {0} ---------------------------------------------------------------", this.Cascade.NumberOfStages + 1);
            Console.WriteLine();
#endif
            if (fpSamplingResult.PrecisionRate <= targetFPR)
                return false;

            var matches = tpSamplingResult.ClassifedSamples.Concat(fpSamplingResult.ClassifedSamples).ToList();
            var classLabels = matches.Select(x => x.ClassLabel > 0).ToList(); //true for positives, false for negatives

            return addStage
                (
                   matches.Select(x => x.Sample.Image).ToList(),
                   matches.Select(x => x.Sample.ROI).ToList(),
                   matches.Select(x => x.Confidence).ToList(),
                   classLabels,
                   minTPR, maxFPR, maxTrees, treeMaxDepth, numberOfBinaryTests
                );
        }

        private ClassifiedSample<ImageSample<TColor>> classify(ImageSample<TColor> imgSample)
        {
            float confidence;
            bool isTruePositive = this.ClassifyRegion(imgSample.Image, imgSample.ROI.Center(), imgSample.ROI.Size, out confidence);

            return new ClassifiedSample<ImageSample<TColor>>
            {
                Sample = imgSample,
                Confidence = confidence,
                ClassLabel = isTruePositive ? ClassifiedSample<ImageSample<TColor>>.POSITIVE : ClassifiedSample<ImageSample<TColor>>.NEGATIVE
            };
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
        private bool addStage(IList<Image<TColor, byte>> images, IList<Rectangle> windows, IList<float> confidences, IList<bool> classLabels, float minTPR, float maxFPR, int maxTrees, int treeMaxDepth, int numberOfBinaryTests)
        {
            var stageClassifier = new GentleBoost<RegressionTree<BinTestCode<TColor>>>();

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
                                      threshold = Cascade<GentleBoost<RegressionTree<BinTestCode<TColor>>>>
                                                                          .SearchMinTPR(classLabels, minTPR,
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
        public static BinTestCode<TColor>[] weakLearnerProvideFeatures(int numberOfBinaryTests)
        {
            Random rand = new Random();
            byte[] buffer = new byte[BinTestCode<TColor>.PackedLength];

            return EnumerableExtensions.Create(numberOfBinaryTests, (_) => 
                            { 
                                rand.NextBytes(buffer);
                                return new BinTestCode<TColor>((sbyte[])(Array)buffer);
                            });
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
        private static RegressionTree<BinTestCode<TColor>> weakLearnerTrain(int treeMaxDepth, Func<BinTestCode<TColor>[]> nodeFeatureProvider, float[] targetValues, float[] sampleWeights, IEnumerable<Image<TColor, byte>> images, IList<Rectangle> windows)
        {
            var learner = new RegressionTree<BinTestCode<TColor>>(treeMaxDepth);
            learner.Train
                (
                //get features for each node (random binary codes)
                   nodeFeatureProvider: nodeFeatureProvider,
                //classify each image using binary test
                   rightNodeSelector: (binTest, sampleIndex) =>
                   {
                       var image = images.ElementAt(sampleIndex);
                       var window = windows[sampleIndex];

                       return binTest.Test(image, window.Center(), window.Size);
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
        private static float weakLearnerClassify(RegressionTree<BinTestCode<TColor>> learner, Image<TColor, byte> image, Rectangle window)
        {
            var output = learner.GetOutput(binTest =>
                            {
                                //classify each image using binary test
                                return binTest.Test(image, window.Center(), window.Size);
                            });

            return output;
        }
   
        #endregion
    }
}
