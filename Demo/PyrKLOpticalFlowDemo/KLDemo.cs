using Accord.Imaging;
using Accord.Vision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Imaging.Filters;

using FlowColor = Accord.Imaging.Gray;

namespace PyrKLOpticalFlowDemo
{
    public partial class KLDemo : Form
    {
        Capture videoCapture;
        PyrLKStorage<FlowColor> lkStorage;
        int winSize = 15;

        private void processImage(Image<FlowColor, float> prevIm, Image<FlowColor, float> currIm, List<PointF> oldPositions, out List<PointF> newPositions)
        {
            lkStorage.Process(prevIm, currIm);

            PointF[] currFeatures;
            float[] error;
            KLTFeatureStatus[] featureStatus;
            /*LKOpticalFlow<FlowColor>.EstimateFlow(lkStorage, oldPositions.ToArray(), 
                                                  out currFeatures, out featureStatus, out error);*/


            PyrLKOpticalFlow<FlowColor>.EstimateFlow(prevIm, currIm, oldPositions.ToArray(),
                                          out currFeatures, out featureStatus, out error);

            newPositions = new List<PointF>();
            for (int i = 0; i < currFeatures.Length; i++)
            {
                if (featureStatus[i] == KLTFeatureStatus.Success)
                    newPositions.Add(currFeatures[i]);

                Console.WriteLine(featureStatus[i]);
            }
        }

        #region User interface...

        public KLDemo()
        {
            InitializeComponent();
            lkStorage = new PyrLKStorage<Gray>(pyrLevels: 1);

            /*var i = Bitmap.FromFile("bla.bmp").ToImage<Gray, float>();
            RectangleF r = new RectangleF(260.9459f, 70.6160049f, 25, 25);
            i.GetSubRect(Rectangle.Round(r)).ToBitmap().Save("bla2.bmp");
            i.Mul(-1).GetRectSubPix(r).Mul(-1).ToBitmap().Save("bla3.bmp");*/
            //return;

            var im1 = Bitmap.FromFile("1.bmp").ToImage<Gray, float>();
            var im2 = Bitmap.FromFile("2.bmp").ToImage<Gray, float>();
            var im3=im2.Clone();

            /*List<PointF> newPositions;
            processImage(im1, im2, new List<PointF>(), out newPositions);
            processImage(im2, im3, new List<PointF>(), out newPositions);*/

            var pts = new List<PointF>();
            pts.Add(new PointF(272, 82)); //-> 277,83

            /*im1.SupressNonMaxima(3).ToBitmap().Save("bla.bmp");
            return;*/

            var features = im1.GoodFeaturesToTrack(5, 0.3f);

            var d = new Image<Bgr, byte>(im1.Size);
            drawPoints(d, features.Select(x => new PointF(x.X, x.Y)).ToList());
            d.Save("bla.bmp");

            Console.WriteLine(features);
            //return;

            List<PointF> newPts;
            processImage(im1, im2, pts, out newPts);   
            //return;

            try
            {
                videoCapture = new Capture(0);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            videoCapture.VideoSize = new Size(640, 480); //set new Size(0,0) for the lowest one

            oldPositions = new List<PointF>();
            prevIm = new Image<Gray, float>(videoCapture.VideoSize);

            this.FormClosing += CamshiftDemo_FormClosing;
            Application.Idle += videoCapture_NewFrame;
            videoCapture.Start();
        }

        Image<FlowColor, float> prevIm = null;
        List<PointF> oldPositions = null;

        Font font = new Font("Arial", 12);
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            bool hasNewFrame = videoCapture.WaitForNewFrame(); //do not process the same frame
            if (!hasNewFrame)
                return;

            var frame = videoCapture.QueryFrame();
            //var frame = Bitmap.FromFile("1.bmp").ToImage<Bgr, byte>();
            var im = frame.Convert<FlowColor, float>();//.SmoothGaussian(5); //smoothing <<parallel operation>>;

            long start = DateTime.Now.Ticks;
            
            List<PointF> newPositions;
            processImage(prevIm, im, this.oldPositions, out newPositions);
            
            prevIm = im;
            oldPositions = newPositions;

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + elapsedMs + " ms", font, new PointF(15, 10), new Bgr(0, 255, 0));
            drawPoints(frame, newPositions);
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared) 24bpp color

            GC.Collect();
        }
    
        void CamshiftDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null) 
                videoCapture.Stop();
        }

        private void drawPoints(Image<Bgr, byte> im, List<PointF> points)
        {
            foreach (var pt in points)
            {
                /*im[(int)pt.Y, (int)pt.X] = new Bgr(Color.Red);
                continue;*/
                
                var rect = new RectangleF(pt.X, pt.Y, 1, 1);
                rect.Inflate(winSize / 2, winSize / 2);

                im.Draw(rect, new Bgr(Color.Red), 3);
            }
        }

        #endregion

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            lock (oldPositions)
            {
                oldPositions.Add(e.Location);
            }
        }

        private void KLDemo_KeyPress(object sender, KeyPressEventArgs e)
        {
            lock (oldPositions)
            {
                if (e.KeyChar == 'r')
                    //oldPositions = prevIm.Convert<byte>().HarrisCorners().Select(x => new PointF(x.X, x.Y)).Take(100).ToList();
                    oldPositions = prevIm.GoodFeaturesToTrack(winSize, 0.3f).Select(x => new PointF(x.X, x.Y)).Take(100).ToList();
            }
        }
    }
}
