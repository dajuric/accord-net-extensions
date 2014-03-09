using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using StageClassifier = RT.GentleBoost<RT.RegressionTree<RT.BinTestCode>>;

namespace RT
{
    public class PicoClassifier
    {                    
        private Cascade<StageClassifier> cascade;
        private RectangleF normalizedRegion;

        public PicoClassifier(RectangleF normalizedRegion, Cascade<StageClassifier> cascade)
        {
            this.normalizedRegion = normalizedRegion;
            this.cascade = cascade;
        }

        public Rectangle GetRegion(PointF regionCenter, float regionSize)
        {
            Point center = new Point
            {
                X = (int)(regionCenter.X + regionSize * normalizedRegion.X),
                Y = (int)(regionCenter.Y + regionSize * normalizedRegion.Y)
            };

            Size size = GetSize(regionSize);
            
            return new Rectangle
            {
                X = center.X - size.Width / 2,
                Y = center.Y - size.Height / 2,
                Width = size.Width,
                Height = size.Height
            };
        }

        public Size GetSize(float regionSize)
        {
            Size size = new Size
            {
                Width = (int)(regionSize * normalizedRegion.Width),
                Height = (int)(regionSize * normalizedRegion.Height)
            };

            return size;
        }

        public bool ClassifyRegion(Image<Gray, byte> image, Rectangle region, out float confidence)
        {
            var regionCenter = new Point
            {
                X = region.X + region.Width / 2,
                Y = region.Y + region.Height / 2
            };

            return ClassifyRegion(image, regionCenter, region.Size, out confidence);
        }

        public bool ClassifyRegion(Image<Gray, byte> image, Point regionCenter, Size regionSize, out float confidence)
        {
            return cascade.Classify(stageClassifier =>
                                    stageClassifier.GetOutput(weakLearner =>
                                                              weakLearner.GetOutput(binTest =>
                                                                                    binTest.Test(image, regionCenter, regionSize))),

                                     out confidence);
        }
    }
}
