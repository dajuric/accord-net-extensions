#define FILE_CAPTURE //comment it to enable camera capture

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

using FlowColor = Accord.Extensions.Imaging.Gray; //switch from color <=> gray optical flow
using System.IO;

namespace PyrKLOpticalFlowDemo
{
    public partial class KLDemo : Form
    {
        Size imgSize = new Size(640, 480);

        ImageStreamReader videoCapture;
        PyrLKStorage<FlowColor> lkStorage;
        int winSize = 21;

        private void processImage(Image<FlowColor, float> prevIm, Image<FlowColor, float> currIm, List<PointF> oldPositions, out List<PointF> newPositions)
        {
            lkStorage.Process(prevIm, currIm);

            PointF[] currFeatures;
            float[] error;
            KLTFeatureStatus[] featureStatus;
            /*LKOpticalFlow<FlowColor>.EstimateFlow(lkStorage, oldPositions.ToArray(), 
                                                  out currFeatures, out featureStatus, out error,
                                                  winSize);*/


            PyrLKOpticalFlow<FlowColor>.EstimateFlow(lkStorage, oldPositions.ToArray(),
                                                     out currFeatures, out featureStatus, out error, 
                                                     winSize);

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

            lkStorage = new PyrLKStorage<FlowColor>(pyrLevels: 1);
            
            try
            {
#if FILE_CAPTURE
                string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
                videoCapture = new ImageDirectoryReader(Path.Combine(resourceDir, "ImageSequence"), "*.jpg");

                prevIm = videoCapture.ReadAs<FlowColor, float>();
                oldPositions = prevIm.
                              Convert<Gray, float>().
                              GoodFeaturesToTrack(winSize, 0.05f)
                              .Select(x => new PointF(x.X, x.Y)).Take(100).ToList();
#else
                videoCapture = new CameraCapture(0);
                oldPositions = new List<PointF>();
                prevIm = new Image<FlowColor, float>(imgSize);
#endif
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            if(videoCapture is CameraCapture)
                (videoCapture as CameraCapture).FrameSize = imgSize; 

            this.FormClosing += CamshiftDemo_FormClosing;
            Application.Idle += videoCapture_NewFrame;
            videoCapture.Open();
        }

        Image<FlowColor, float> prevIm = null;
        List<PointF> oldPositions = null;

        System.Drawing.Font font = new System.Drawing.Font("Arial", 12);
        Image<Bgr, byte> frame = null;
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            frame = videoCapture.ReadAs<Bgr, byte>();
            if (frame == null)
                return;

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
                videoCapture.Dispose();
        }

        private void drawPoints(Image<Bgr, byte> im, List<PointF> points)
        {
            foreach (var pt in points)
            {
                /*im[(int)pt.Y, (int)pt.X] = new Bgr(Color.Red);
                continue;*/
                
                var rect = new RectangleF(pt.X, pt.Y, 1, 1);
                rect.Inflate(winSize / 2, winSize / 2);

                im.Draw(rect, System.Drawing.Color.Red.ToBgr(), 3);
            }
        }

        #endregion

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            lock (oldPositions)
            {
                oldPositions.Add(new PointF(e.Location.X, e.Location.Y));
            }
        }

        private void KLDemo_KeyPress(object sender, KeyPressEventArgs e)
        {
            lock (oldPositions)
            {
                if (e.KeyChar == 'r')
                    //oldPositions = prevIm.Convert<byte>().HarrisCorners().Select(x => new PointF(x.X, x.Y)).Take(100).ToList();
                    oldPositions = prevIm.
                                   Convert<Gray, float>().
                                   GoodFeaturesToTrack(winSize, 0.05f)
                                   .Select(x => new PointF(x.X, x.Y)).Take(100).ToList();
            }
        }
    }
}
