using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT
{
    public class GentleBoost<TWeakLearner>
    {
        List<TWeakLearner> learners;

        public GentleBoost(List<TWeakLearner> learners)
        {
            this.learners = learners;
        }

        public GentleBoost()
            :this(new List<TWeakLearner>())
        {}

        public float GetOutput(Func<TWeakLearner, float> learnerClassifyFunc) 
        {
            float output = 0;
            foreach (var learner in learners)
            {
                output += learnerClassifyFunc(learner);
            }

            return output;
        }

        public void Train(Func<TWeakLearner> learnerCreator) {}

    }
}
