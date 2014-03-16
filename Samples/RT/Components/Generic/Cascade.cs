using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT
{
    public class Cascade<TStage>
    {
        private List<TStage> stageClassifiers;
        private List<float> stageThresholds;

        public Cascade()
        {
            this.stageClassifiers = new List<TStage>();
            this.stageThresholds = new List<float>();
        }

        public Cascade(IEnumerable<TStage> stageClassifiers, IEnumerable<float> stageThresholds)
        {
            this.stageClassifiers = stageClassifiers.ToList();
            this.stageThresholds = stageThresholds.ToList();

            if (this.stageClassifiers.Count != this.stageThresholds.Count)
                throw new Exception("The number of stages and stages thresholds must be the same!");
        }

        public IReadOnlyList<TStage> StageClassifiers { get { return stageClassifiers.AsReadOnly(); } }
        public IReadOnlyList<float> StageThresholds { get { return stageThresholds.AsReadOnly(); } }
        public int NumberOfStages { get { return stageClassifiers.Count; } }

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

            confidence -= stageThreshold;
            return true;
        }

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
        /// <param name="classifyFunc">Classify function. Parameters: (stage classifier, sample index, current threshold). Output: class label.</param>
        /// <param name="truePositiveRate">Found true positive rate that is greater or equal <see cref="minimumTPR"/>.</param>
        /// <param name="falsePositiveRate">Found false positive rate for which <see cref="truePositiveRate"/> is greater than <see cref="minimumTPR"/>.</param>
        /// <returns>Threshold for which <see cref="truePositiveRate"/> is greater than <see cref="minimumTPR"/>.</returns>
        public static float SearchMinTPR(IList<bool> classes, float minimumTPR, 
                                      float startThreshold, float thresholdStep, 
                                      TStage stage, 
                                      Func<TStage, int, float, bool> classifyFunc, 
                                      out float truePositiveRate, out float falsePositiveRate)
        {
            truePositiveRate=0;
            var threshold = startThreshold;

            do
            {
                threshold -= thresholdStep;

                GetErrors(classes, stage, (_stage, index) => classifyFunc(_stage, index, threshold), out truePositiveRate, out falsePositiveRate);
            }
            while (truePositiveRate < minimumTPR);

            return threshold;
        }

        public static void GetErrors(IList<bool> classes, TStage stage, Func<TStage, int, bool> classifyFunc, out float truePositiveRate, out float falsePositiveRate)
        {
            int nPositives = classes.Count(x => x == true);
            int nNegatives = classes.Count - nPositives;

            int nTPs = 0, nFPs = 0;

            for (int i = 0; i < classes.Count; i++)
            {
                var isPositive = classifyFunc(stage, i);

                if (classes[i] == true && isPositive)
                    nTPs++;
                if (classes[i] == false && isPositive)
                    nFPs++;
            }

            truePositiveRate = nTPs / (float)nPositives;
            falsePositiveRate = nFPs / (float)nNegatives;
        }
    }
}
