using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Test
{
    public partial class Test
    {
        public void TestLKFlow()
        {
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources", "LKFlow");

            var im1 = Bitmap.FromFile(Path.Combine(path, "1.bmp")).ToImage<Gray, float>();
            var im2 = Bitmap.FromFile(Path.Combine(path, "2.bmp")).ToImage<Gray, float>();

            var pts = new List<PointF>();
            pts.Add(new PointF(272, 82)); //-> 277,83

            PointF[] currFeatures;
            float[] error;
            KLTFeatureStatus[] featureStatus;
            /*LKOpticalFlow<Gray>.EstimateFlow(lkStorage, pts.ToArray(), 
                                                  out currFeatures, out featureStatus, out error);*/

            PyrLKOpticalFlow<Gray>.EstimateFlow(im1, im2, pts.ToArray(),
                                                 out currFeatures, out featureStatus, out error, 15, 15, 0.1f, 0.05f, 0.1f, 0);

            var debug = im2.Convert<Bgr, byte>();
            debug[(int)currFeatures.First().Y, (int)currFeatures.First().X] = Bgr8.Red;
            debug.Save("bla.bmp");
        }

       
    }
}
