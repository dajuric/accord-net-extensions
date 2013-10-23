using Accord.Imaging;
using Accord.Vision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public partial class Test
    {
        public void TestLKFlow()
        {
            var im1 = Bitmap.FromFile("1.bmp").ToImage<Gray, float>();
            var im2 = Bitmap.FromFile("2.bmp").ToImage<Gray, float>();

            var pts = new List<PointF>();
            pts.Add(new PointF(272, 82)); //-> 277,83

            PointF[] currFeatures;
            float[] error;
            KLTFeatureStatus[] featureStatus;
            /*LKOpticalFlow<Gray>.EstimateFlow(lkStorage, pts.ToArray(), 
                                                  out currFeatures, out featureStatus, out error);*/

            PyrLKOpticalFlow<Gray>.EstimateFlow(im1, im2, pts.ToArray(),
                                                 out currFeatures, out featureStatus, out error);

            var debug = im2.Convert<Bgr, byte>();
            debug[(int)currFeatures.First().Y, (int)currFeatures.First().X] = new Bgr(Color.Red);
            debug.Save("bla.bmp");
        }

       
    }
}
