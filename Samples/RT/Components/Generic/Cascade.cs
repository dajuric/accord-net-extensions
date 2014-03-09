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

        public int NumberOfStages { get { return stageClassifiers.Count; } }

        public void AddStage(TStage stage, float threshold) 
        {
            this.stageClassifiers.Add(stage);
            this.stageThresholds.Add(threshold);
        }
    }
}
