using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RT
{
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
    }
}
