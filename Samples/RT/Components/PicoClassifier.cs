#define LOG

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
        /// <param name="angleDeg">To detect rotated object, specify the value different than 0. The angle is specified in degrees.</param>
        /// <param name="confidence">Classification confidence. The value is not normalized.</param>
        /// <returns>True if the confidence is greater than 0, false if this is not the case or the region is specified outside valid image boundaries.</returns>
        public bool ClassifyRegion(Image<Gray, byte> image, Rectangle region, int angleDeg, out float confidence)
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
            /*if (BinTestCode.IsInBounds(image.Size, regionCenter, regionSize, angleDeg != 0) == false)
            {
                confidence = 0;
                return false;
            }*/

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
        public bool AddStage(IList<Image<Gray, byte>> allPositiveSamples, IList<Image<Gray, byte>> allNegativeSamples, IList<Rectangle> allPositiveSampleWindows, float targetFPR = 1e-6f, float minTPR = 0.980f, float maxFPR = 0.5f, int maxTrees = 1, int treeMaxDepth = 6, int numberOfBinaryTests = 1024)
        {
            Random rand = new Random(0);
            StageClassifier stageClassifier = new StageClassifier();

            List<Image<Gray, byte>> images;
            List<Rectangle> windows;
            List<bool> classLabels;
            List<float> confidences;
            float tpr, fpr;

#if LOG
            Console.WriteLine();
            Console.WriteLine("Stage training {0} ---------------------------------------------------------------", this.Cascade.NumberOfStages + 1);
            Console.WriteLine();
#endif

            sampleTrainingData(allPositiveSamples, allPositiveSampleWindows, allNegativeSamples, (nSelectedPositives) => (2 * allPositiveSamples.Count() - nSelectedPositives),
                               out images, out windows, out classLabels, out confidences, out tpr, out fpr, () => rand.Next() /*get_random()*/);

            if (fpr <= targetFPR)
                return false;

            float[] targetValues = classLabels.Select(x => (x == true) ? +1f : -1f).ToArray();
           
            float threshold = 0;
            stageClassifier.Train(targetValues, confidences.ToArray(),

                                  //create and train weak learner
                                  (sampleWeights) => weakLearnerTrain(treeMaxDepth, 
                                                                      () => weakLearnerProvideFeatures(numberOfBinaryTests, () => (int)rand.Next() /*get_random()*/),
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
                                      float startThreshold = 5f;//learnerOutputs.Max();
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
        /// <param name="nextRandom">Function of the used random generator.</param>
        /// <returns>An array of features (binary tests) to select from.</returns>
        private static BinTestCode[] weakLearnerProvideFeatures(int numberOfBinaryTests, Func<int> nextRandom)
        {
            return EnumerableExtensions.Create(numberOfBinaryTests, (_) => new BinTestCode(nextRandom()));
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
        /// Gets the true positive samples, and the subset of false positives where the negative samples are chosen by specified random function.
        /// </summary>
        /// <param name="positives">Positive sample images.</param>
        /// <param name="objectRegions">Positive sample regions.</param>
        /// <param name="negatives">Negative sample images.</param>
        /// <param name="nNegativesCountSelector">
        /// Number of negatives to selector.
        /// Parameters: number of selected positives.
        /// Returns: number of negatives to select.
        /// </param>
        /// <param name="samples">Chosen positive and negative samples.</param>
        /// <param name="windows">Chosen positive sample regions and generated regions for negatives.</param>
        /// <param name="classLabels">Class labels (true - positive, false - negative).</param>
        /// <param name="classifierOutputs">Classifier outputs (confidences). If the function is used for the first time a classifier must return true regardless of a sample.</param>
        /// <param name="truePositiveRate">True positive rate.</param>
        /// <param name="falsePositiveRate">False positive rate.</param>
        /// <param name="nextRand">The user-random function.</param>
        private void sampleTrainingData(IEnumerable<Image<Gray, byte>> positives,    IList<Rectangle> objectRegions,
                                        IEnumerable<Image<Gray, byte>> negatives, Func<int, int> nNegativesCountSelector,

                                        out List<Image<Gray, byte>> samples, out List<Rectangle> windows,
                                        out List<bool> classLabels         , out List<float> classifierOutputs,
                                        out float truePositiveRate,          out float falsePositiveRate,
            
                                        Func<int> nextRand)
        {
#if LOG
            int nTPs = 0;
#endif

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
#if LOG
                    nTPs++;
#endif
                }
#if LOG
                Console.Write("\rSampling positives. nTPs / nSampled: {0}, {1}", nTPs, nTotalPositives);
#endif
            }

            var nSelectedPositives = classLabels.Where(x => x == true).Count();
            truePositiveRate = nSelectedPositives / (float)nTotalPositives;

#if LOG
            Console.WriteLine(" - TPR: {0}", truePositiveRate);
#endif
            /*************************** load positive samples **************************************/

            /*************************** load false-positives *************************************/
            const int MIN_WINDOW_SIZE = 24;

            var nTotalNegatives = negatives.Count();
            var nFPs = 0;
            var nPickedNegatives = 0;

            int nNegativesToSelect = nNegativesCountSelector(nSelectedPositives);
            while (nFPs < nNegativesToSelect)
            { 
                //pick random background image
                var idx = (int)(nextRand() % nTotalNegatives);
                var negativeSample = negatives.ElementAt(idx);

                //pick random window
                var row   = nextRand() % negativeSample.Height;
                var col   = nextRand() % negativeSample.Width;
                var scale = nextRand() % ( 2 * Math.Min(Math.Min(row, negativeSample.Height - row), Math.Min(col, negativeSample.Width - col)) + 1);

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

#if LOG
                Console.Write("\rSampling negatives. nFPs / nSampled: {0}, {1}", nFPs, nPickedNegatives);
#endif
            }
            /*************************** load false-positives *************************************/

           
            falsePositiveRate = classLabels.Where(x => x == false).Count() / (float)nPickedNegatives;

#if LOG
            Console.WriteLine(" - FPR: {0}", falsePositiveRate);
#endif
        }

        #endregion
    }

}
