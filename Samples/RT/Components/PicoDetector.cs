using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using StageClassifier = RT.GentleBoost<RT.RegressionTree<RT.BinTestCode>>;

namespace RT
{
    public class PicoDetector
    {
        private static readonly Func<StageClassifier, Image<Gray, byte>, Rectangle, float> stageClassifyFunc = (stage, image, roi) =>
                                                                                                                stage.GetOutput((weakLearner) => 
                                                                                                                                 weakLearner.GetOutput(binTest => 
                                                                                                                                 binTest.Test(image, roi)));
                                
        private Cascade<StageClassifier> cascade;
        private RectangleF normalizedRegion;

        public PicoDetector(RectangleF normalizedRegion, Cascade<StageClassifier> cascade)
        {
            this.normalizedRegion = normalizedRegion;
            this.cascade = cascade;
        }

        public bool ClassifyRegion(Image<Gray, byte> image, Rectangle region)
        {
            float confidence;
            return cascade.Classify(x => stageClassifyFunc(x, image, region), out confidence);
        }
    }
}
