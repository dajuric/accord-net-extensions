using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions;
using System.Threading;

namespace ContourExtremaDemo
{
    public partial class ContourExtremaForm : Form
    {
        Image<Bgr, byte> image;

        public ContourExtremaForm()
        {
            InitializeComponent();
        }

        private void ContourDemoForm_Shown(object sender, EventArgs e)
        {
            image = ResourceImages.bwHand.ToImage<Bgr, byte>();
            findContour();
        }

        private void findContour()
        {
            var scale = 25; //for bigger humps increase scale

            var contour = image.Convert<Gray, byte>().FindContour();

            //find peaks and valeys
            List<int> peakIndeces, valeyIndeces;
            contour.FindExtremaIndices((angle, isPeak) =>
            {
                if ((isPeak && angle > 0 && angle < 90) ||
                    (!isPeak && angle > 0 && angle < 90))
                    return true;

                return false;
            },
            scale, out peakIndeces, out valeyIndeces);

            //cluster peak and valeys
            var cumulativeDistance = contour.CumulativeEuclideanDistance();
            var peakClusters = contour.ClusterPoints(peakIndeces, scale * 2, cumulativeDistance).Where(x => x.Count > 3);
            var valeyClusters = contour.ClusterPoints(valeyIndeces, scale * 2, cumulativeDistance).Where(x => x.Count > 3);

            //get humps
            List<int> humpPeaks;
            var humps = contour.GetHumps(peakClusters.Select(x => ((int)x.Average() + contour.Count) % contour.Count).ToList() /*get mean value; if it is a negative index make it positive*/,
                                         valeyClusters.Select(x => ((int)x.Average() + contour.Count) % contour.Count).ToList(),
                                         scale, out humpPeaks);


            /******************** DRAWING *************************/

            //draw humps
            for (int i = 0; i < humps.Count; i++)
            {
                var slice = contour.ToCircularList().GetRange(humps[i]);
                image.Draw(slice, Bgr8.Green, 5);
            }

            //draw valeys
            for (int i = 0; i < valeyIndeces.Count; i++)
            {
                image.Draw(new CircleF(contour[valeyIndeces[i]], 3), Bgr8.Red, 3);
            }

            //draw peaks
            for (int i = 0; i < peakIndeces.Count; i++)
            {
                image.Draw(new CircleF(contour[peakIndeces[i]], 3), Bgr8.Blue, 3);
            }

            //draw contour
            //image.Draw(contour.Select(x => new System.Drawing.Point(x.X, x.Y)), new Bgr(Color.Red), 3);
            pictureBox.Image = image.ToBitmap();
        }
    }
}
