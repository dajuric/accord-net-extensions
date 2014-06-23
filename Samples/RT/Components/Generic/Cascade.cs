#define LOG

using Accord.Extensions;
using Accord.Extensions.Math.Geometry;
using Accord.Math.Geometry;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RT
{
    public class SamplingResult<TSample>
    {
        public IList<ClassifiedSample<TSample>> ClassifedSamples { get; set; }
        public float PrecisionRate { get; set; }
    }

    public class ClassifiedSample<TSample>
    {
        public const int POSITIVE = 1;
        public const int NEGATIVE = -1;

        public TSample Sample { get; set; }
        public float Confidence { get; set; }
        public int ClassLabel { get; set; }
    }

    public class ImageSample<TColor>
        where TColor: IColor
    {
        public Image<TColor, byte> Image { get; set; }
        public Rectangle ROI { get; set; }
    }

    /// <summary>
    /// Represents the container for array of stages and their thresholds, and provides helper methods for cascade training and classification.
    /// </summary>
    /// <typeparam name="TStage">Stage classifier type.</typeparam>
    public class Cascade<TStage>
    {
        private List<TStage> stageClassifiers;
        private List<float> stageThresholds;

        /// <summary>
        /// Creates an empty cascade.
        /// </summary>
        public Cascade()
        {
            this.stageClassifiers = new List<TStage>();
            this.stageThresholds = new List<float>();
        }

        /// <summary>
        /// Loads a cascade by using provided data.
        /// </summary>
        /// <param name="stageClassifiers">The collection of stages.</param>
        /// <param name="stageThresholds">The collection of stage thresholds.</param>
        /// <exception cref="Exception">The number of stages and thresholds must be the same.</exception>
        public Cascade(IEnumerable<TStage> stageClassifiers, IEnumerable<float> stageThresholds)
        {
            this.stageClassifiers = stageClassifiers.ToList();
            this.stageThresholds = stageThresholds.ToList();

            if (this.stageClassifiers.Count != this.stageThresholds.Count)
                throw new Exception("The number of stages and stages thresholds must be the same!");
        }

        /// <summary>
        /// Gets the read-only collection of stage classifiers.
        /// </summary>
        public IReadOnlyList<TStage> StageClassifiers { get { return stageClassifiers.AsReadOnly(); } }
        /// <summary>
        /// Gets the read-only collection of stage thresholds.
        /// </summary>
        public IReadOnlyList<float> StageThresholds { get { return stageThresholds.AsReadOnly(); } }
        /// <summary>
        /// Gets the number of stages that this cascade contains.
        /// </summary>
        public int NumberOfStages { get { return stageClassifiers.Count; } }

        /// <summary>
        /// Classifies a sample by using stage classification function.
        /// </summary>
        /// <param name="stageClassifierOutputFunc">
        /// Stage classification function. 
        /// Parameters: stage classifier.
        /// Returns: classifier output.
        /// </param>
        /// <param name="confidence">Summed output of stages.</param>
        /// <returns>If the classification run through all stages, false otherwise.</returns>
        public bool Classify(Func<TStage, float> stageClassifierOutputFunc, out float confidence)
        {
            confidence = 0f;
            
            int stageIdx = 0;
            float stageThreshold = 0;
            for (stageIdx = 0; stageIdx < NumberOfStages; stageIdx++)
            {
                var stageOutput = stageClassifierOutputFunc(stageClassifiers[stageIdx]);
                stageThreshold = stageThresholds[stageIdx];

                confidence += stageOutput;

                if (confidence <= stageThreshold)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Adds trained stage to the current cascade.
        /// </summary>
        /// <param name="stage">Trained stage classifier.</param>
        /// <param name="threshold">Stage thresholds. Can be obtained by using: <see cref="SearchMinTPR"/> function.</param>
        public void AddStage(TStage stage, float threshold) 
        {
            this.stageClassifiers.Add(stage);
            this.stageThresholds.Add(threshold);
        }

        /// <summary>
        /// Searches Receiver Operating Characteristic curve for minimum true positive rate by changing content-depended threshold parameter.
        /// </summary>
        /// <param name="classes">Sample classes.</param>
        /// <param name="minimumTPR">The minimum true positive rate that must be found.</param>
        /// <param name="startThreshold">Start threshold.</param>
        /// <param name="thresholdStep">Threshold step. (negative threshold step decrements threshold).</param>
        /// <param name="stageSelector">Stage selector function for which to find threshold.</param>
        /// <param name="classifyFunc">
        /// Classify function. 
        /// Parameters: (stage classifier, sample index, current threshold). 
        /// Output: class label.
        /// </param>
        /// <param name="truePositiveRate">Found true positive rate that is greater or equal <see cref="minimumTPR"/>.</param>
        /// <param name="falsePositiveRate">Found false positive rate for which <see cref="truePositiveRate"/> is greater than <see cref="minimumTPR"/>.</param>
        /// <returns>Threshold for which <see cref="truePositiveRate"/> is greater than <see cref="minimumTPR"/>.</returns>
        public static float SearchMinTPR(IList<bool> classes, float minimumTPR, 
                                      float startThreshold, float thresholdStep, 
                                      TStage stage, 
                                      Func<TStage, int, float, bool> classifyFunc, 
                                      out float truePositiveRate, out float falsePositiveRate)
        {
            var threshold = startThreshold;

            do
            {
                threshold += thresholdStep;

                GetErrors(classes, stage, (_stage, index) => classifyFunc(_stage, index, threshold), 
                                                                          out truePositiveRate, out falsePositiveRate);
            }
            while (truePositiveRate < minimumTPR);

            return threshold;
        }
 
        /// <summary>
        /// Gets true positive and false positive rate from provided data.
        /// </summary>
        /// <param name="classes">Class labels.</param>
        /// <param name="stage">Stage classifier.</param>
        /// <param name="classifyFunc">
        /// Classify function. 
        /// Parameters: (stage classifier, sample index, current threshold). 
        /// Output: class label.
        /// </param>
        /// <param name="truePositiveRate">Obtained true positive rate.</param>
        /// <param name="falsePositiveRate">Obtained false positive rate.</param>
        public static void GetErrors(IList<bool> classes, TStage stage, Func<TStage, int, bool> classifyFunc, 
                                     out float truePositiveRate, out float falsePositiveRate)
        {
            int nPositives = classes.Count(x => x == true);
            int nNegatives = classes.Count - nPositives;

            int nTPs = 0, nFPs = 0;

            Parallel.For(0, classes.Count, (int i) =>
            //for (int i = 0; i < classes.Count; i++)
            {
                var isPositive = classifyFunc(stage, i);

                if (classes[i] == true && isPositive)
                    Interlocked.Increment(ref nTPs);
                if (classes[i] == false && isPositive)
                    Interlocked.Increment(ref nFPs);
            });

            truePositiveRate = nTPs / (float)nPositives;
            falsePositiveRate = nFPs / (float)nNegatives;
        }

        public SamplingResult<ImageSample<TColor>> SampleFalsePositives<TColor>(IEnumerable<Image<TColor, byte>> positives, Func<int, Rectangle> windowCreator, IList<List<Rectangle>> forbidenImageRegions,
                                                                                Func<int, ImageSample<TColor>, ClassifiedSample<ImageSample<TColor>>> classificationFunc,
                                                                                Func<int, bool> terminationFunc
                                                                                )
            where TColor : IColor
        {
            Func<int, Rectangle> allowedWindowCreator = (sampleIdx) => 
            {
                var sample = positives.ElementAt(sampleIdx);
                var forbidenRegions = forbidenImageRegions[sampleIdx];

                Rectangle window;
                while (true)
                {
                    window = windowCreator(sampleIdx);

                    if (forbidenRegions.All(x => !x.IntersectsWith(window)))
                        break;
                }

                return window;
            };

            return SampleFalsePositives(positives,
                                        allowedWindowCreator,
                                        classificationFunc,
                                        terminationFunc);
        }

        /// <summary>
        /// Samples false positives from negative images.
        /// </summary>
        /// <typeparam name="TImage">Image type.</typeparam>
        /// <param name="negatives">Negative image collection.</param>
        /// <param name="windowCreator">
        /// Window creation function.
        /// Parameters: sample index.
        /// Returns: random window.
        /// </param>
        /// <param name="classificationFunc">
        /// Classification function.
        /// Parameters: image index, image sample
        /// Returns: classified sample. 
        /// </param>
        /// <param name="terminationFunc">
        /// Termination function. 
        /// Parameters: current number of sampled false positive samples.
        /// Returns: True if the termination is wanted, false otherwise.
        /// </param>
        /// <returns>The collection of selected false positive images (image, window), the classifier confidence  and false positive rate.</returns>
        public SamplingResult<ImageSample<TColor>> SampleFalsePositives<TColor>(IEnumerable<Image<TColor, byte>> negatives, Func<int, Rectangle> windowCreator, 
                                                                                Func<int, ImageSample<TColor>, ClassifiedSample<ImageSample<TColor>>> classificationFunc, 
                                                                                Func<int, bool> terminationFunc)
            where TColor: IColor
        {
            var falsePositives = new ConcurrentBag<ClassifiedSample<ImageSample<TColor>>>();

            /*************************** load false-positives *************************************/
            var nTotalNegatives = negatives.Count();
            var nFPs = 0;
            long nPickedNegatives = 0;

            ParallelExtensions.While(() => terminationFunc(nFPs) == false,
                (loopState) =>
                {
                    //pick random background image
                    var idx = (int)(ParallelRandom.Next() % nTotalNegatives);
                    var negativeSample = negatives.ElementAt(idx);

                    //pick random window
                    var window = windowCreator(idx);

                    //DEBUG
                    /*if (window.X < 0 || window.Y < 0 || window.Right > negativeSample.Width || window.Bottom > negativeSample.Height)
                        Console.WriteLine();*/

                    //classify the region
                    var classificationResult = classificationFunc(idx, new ImageSample<TColor> { Image = negativeSample, ROI = window });

                    //it is a false-positive
                    if (classificationResult.ClassLabel == ClassifiedSample<ImageSample<TColor>>.POSITIVE)
                    {
                        classificationResult.ClassLabel = ClassifiedSample<ImageSample<TColor>>.NEGATIVE; //correct class label
                        falsePositives.Add(classificationResult);

                        /*lock (this)
                        {
                            if (this.NumberOfStages > 0)
                                negativeSample.GetSubRect(window).ToBitmap().Save(String.Format("img-{0}.png", falsePositives.Count));
                        }*/

                        Interlocked.Increment(ref nFPs);
                    }

                    Interlocked.Increment(ref nPickedNegatives);

#if LOG
                    if(nPickedNegatives % 1000 == 0)
                        Console.Write("\rSampling negatives. nFPs / nTotalNegatives: {0} / {1}", nFPs, nPickedNegatives);
#endif
                });
            /*************************** load false-positives *************************************/


            var falsePositiveRate = nFPs / (float)nPickedNegatives;

#if LOG
            Console.Write("\rSampling negatives. nFPs / nTotalNegatives: {0} / {1}", nFPs, nPickedNegatives);
            Console.WriteLine(" - FPR: {0}", falsePositiveRate);
#endif

            return new SamplingResult<ImageSample<TColor>> 
            { 
                ClassifedSamples = falsePositives.ToList(), 
                PrecisionRate = falsePositiveRate 
            };
        }

        /// <summary>
        /// Gets true positive samples from positive samples.
        /// </summary>
        /// <param name="positives">Object samples.</param>
        /// <param name="objectRegions">Object regions within an image.</param>
        /// <returns>True positive samples and true positive rate.</returns>
        public SamplingResult<ImageSample<TColor>> GetTruePositives<TColor>(IEnumerable<ImageSample<TColor>> positives,
                                                                            Func<ImageSample<TColor>, ClassifiedSample<ImageSample<TColor>>> classificationFunc)
            where TColor: IColor
        {
#if LOG
            int nSampled = 0;
#endif

            int nTPs = 0;
            var truePositives = new ConcurrentBag<ClassifiedSample<ImageSample<TColor>>>();

            /*************************** load positive samples **************************************/
            var nTotalPositives = positives.Count();

            Parallel.For(0, nTotalPositives, (i) =>
            {
                //debug
                /*var img = positives.ElementAt(i).Image;
                var window = positives.ElementAt(i).ROI;
                if (window.X < 0 || window.Y < 0 || window.Right > img.Width || window.Bottom > img.Height)
                    Console.WriteLine();*/
                
                var classificationResult = classificationFunc(positives.ElementAt(i));

                if (classificationResult.ClassLabel == ClassifiedSample<ImageSample<TColor>>.POSITIVE)
                {
                    truePositives.Add(classificationResult);
                    Interlocked.Increment(ref nTPs);
                }
#if LOG
                Interlocked.Increment(ref nSampled);

                if (nSampled % 1000 == 0)
                    Console.Write("\rSampling positives. nTPs / nTotalPositives: {0} / {1}", nTPs, nTotalPositives);
#endif
            });

            var truePositiveRate = nTPs / (float)nTotalPositives;

#if LOG
            Console.Write("\rSampling positives. nTPs / nTotalPositives: {0} / {1}", nTPs, nTotalPositives);
            Console.WriteLine(" - TPR: {0}", truePositiveRate);
#endif
            /*************************** load positive samples **************************************/

            return new SamplingResult<ImageSample<TColor>>
            {
                ClassifedSamples = truePositives.ToList(),
                PrecisionRate = truePositiveRate
            };
        }
    }
}
