#define LOG

using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GOCO
{
    [Serializable]
    public class Stage<TClassifier>
    {
        public Stage(TClassifier classifier, float threshold)
        {
            this.Classifier = classifier;
            this.Threshold = threshold;
        }

        public TClassifier Classifier { get; private set; }
        public float Threshold { get; private set; }

        public bool Evaluate(Func<TClassifier, float> classificationFunc, ref float confidence)
        {
            var classifierOutput = classificationFunc(this.Classifier);
            confidence += classifierOutput;

            return confidence > this.Threshold;
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
        public static float SearchThreshold(IList<bool> classLabels, float minimumTPR,
                                            float startThreshold, float thresholdStep,
                                            Func<int, float, bool> classifyFunc,
                                            out float truePositiveRate, out float falsePositiveRate)
        {
            var threshold = startThreshold;

            do
            {
                threshold += thresholdStep;

                GetRates(classLabels, (index) => classifyFunc(index, threshold),
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
        public static  void GetRates(IList<bool> classes, Func<int, bool> classifyFunc,
                             out float truePositiveRate, out float falsePositiveRate)
        {
            int nPositives = classes.Count(x => x == true);
            int nNegatives = classes.Count - nPositives;

            int nTPs = 0, nFPs = 0;

            Parallel.For(0, classes.Count, (int i) =>
            //for (int i = 0; i < classes.Count; i++)
            {
                var isPositive = classifyFunc(i);

                if (classes[i] == true && isPositive)
                    Interlocked.Increment(ref nTPs);
                if (classes[i] == false && isPositive)
                    Interlocked.Increment(ref nFPs);
            });

            truePositiveRate = nTPs / (float)nPositives;
            falsePositiveRate = nFPs / (float)nNegatives;
        }
    }

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

    public class ImageSample<TImage>
        where TImage: IImage
    {
        public TImage Image { get; set; }
        public Rectangle ROI { get; set; }
    }

    public static class StageExtensions
    {
        public static void Add<TClassifier>(this List<Stage<TClassifier>> cascade, TClassifier classifier, float threshold)
        {
            cascade.Add(new Stage<TClassifier>(classifier, threshold));
        }

        /// <summary>
        /// Classifies a sample by using stage classification function.
        /// </summary>
        /// <param name="cascade">Cascade.</param>
        /// <param name="classificationFunc">
        /// Stage classification function. 
        /// Parameters: stage classifier.
        /// Returns: classifier output.
        /// </param>
        /// <param name="confidence">Summed output of stages.</param>
        /// <returns>True if all stages are passed, false otherwise.</returns>
        public static bool Evaluate<TClassifier>(this IEnumerable<Stage<TClassifier>> cascade, Func<TClassifier, float> classificationFunc, out float confidence)
        {
            confidence = 0f;

            foreach (var stage in cascade)
            {
                bool passed = stage.Evaluate(classificationFunc, ref confidence);
                if (!passed)
                    return false;
            }

            return true;
        }

        public static SamplingResult<ImageSample<TImage>> SampleFalsePositives<TImage, TClassifier>(this IEnumerable<Stage<TClassifier>> cascade,
                                                                                             Func<TClassifier, ImageSample<TImage>, float> classificationFunc,
                                                                                             IList<TImage> negatives, SizeF minWindowSize, int numberOfSamples)
            where TImage: IImage
        {
            var falsePositives = new ConcurrentBag<ClassifiedSample<ImageSample<TImage>>>();

            /*************************** load false-positives *************************************/
            var nTotalNegatives = negatives.Count();
            var nFPs = 0;
            long nPickedNegatives = 0;

            ParallelExtensions.While(() => nFPs <= numberOfSamples,
                (loopState) =>
                {
                    //pick random background image
                    var idx = (int)(ParallelRandom.Next() % nTotalNegatives);
                    var negativeSample = negatives.ElementAt(idx);

                    //pick random window
                    var window = negatives[idx].Size.CreateRandomRegion(minWindowSize);

                    //DEBUG
                    /*if (window.X < 0 || window.Y < 0 || window.Right > negativeSample.Width || window.Bottom > negativeSample.Height)
                        Console.WriteLine();*/

                    //classify the region
                    var imgSample = new ImageSample<TImage> { Image = negativeSample, ROI = window };
                    float confidence;
                    bool isPositive = cascade.Evaluate(classifier => classificationFunc(classifier, imgSample), out confidence);

                    //it is a false-positive
                    if (isPositive)
                    {
                        var classificationResult = new ClassifiedSample<ImageSample<TImage>> 
                        {
                            Sample = imgSample, 
                            Confidence = confidence,
                            ClassLabel = ClassifiedSample<ImageSample<TImage>>.NEGATIVE //correct label
                        };
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

            return new SamplingResult<ImageSample<TImage>>
            {
                ClassifedSamples = falsePositives.ToList(),
                PrecisionRate = falsePositiveRate
            };
        }


        public static SamplingResult<ImageSample<TImage>> GetTruePositives<TImage, TClassifier>(this IEnumerable<Stage<TClassifier>> cascade,
                                                                                        Func<TClassifier, ImageSample<TImage>, float> classificationFunc,
                                                                                        IList<TImage> positives)
             where TImage : IImage
        {
            var windows = positives.Select(x => new Rectangle(0, 0, x.Width, x.Height)).ToList();
            return cascade.GetTruePositives(classificationFunc, positives, windows);
        }

        /// <summary>
        /// Gets true positive samples from positive samples.
        /// </summary>
        public static SamplingResult<ImageSample<TImage>> GetTruePositives<TImage, TClassifier>(this IEnumerable<Stage<TClassifier>> cascade,
                                                                                        Func<TClassifier, ImageSample<TImage>, float> classificationFunc,
                                                                                        IList<TImage> positives, IList<Rectangle> windows)
             where TImage : IImage
        {
#if LOG
            int nSampled = 0;
#endif

            int nTPs = 0;
            var truePositives = new ConcurrentBag<ClassifiedSample<ImageSample<TImage>>>();

            /*************************** load positive samples **************************************/
            var nTotalPositives = positives.Count();

            Parallel.For(0, nTotalPositives, (i) =>
            {
                //debug
                /*var img = positives.ElementAt(i).Image;
                var window = positives.ElementAt(i).ROI;
                if (window.X < 0 || window.Y < 0 || window.Right > img.Width || window.Bottom > img.Height)
                    Console.WriteLine();*/

                //classify the region
                var imgSample = new ImageSample<TImage> { Image = positives[i], ROI = windows[i] };
                float confidence;
                bool isPositive = cascade.Evaluate(classifier => classificationFunc(classifier, imgSample), out confidence);

                if (isPositive)
                {
                    var classificationResult = new ClassifiedSample<ImageSample<TImage>>
                    {
                        Sample = imgSample,
                        Confidence = confidence,
                        ClassLabel = ClassifiedSample<ImageSample<TImage>>.POSITIVE
                    };
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

            return new SamplingResult<ImageSample<TImage>>
            {
                ClassifedSamples = truePositives.ToList(),
                PrecisionRate = truePositiveRate
            };
        }
    }
}
