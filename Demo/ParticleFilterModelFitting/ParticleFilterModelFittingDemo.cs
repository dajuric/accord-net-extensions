using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Math.Geometry;
using PointF = AForge.Point;
using Accord.Vision;
using Accord.Imaging;
using Accord.Imaging.Filters;
using System.Runtime.InteropServices;

namespace ParticleFilterModelFitting
{
    public partial class ParticleFilterModelFittingDemo : Form
    {
        Size imgSize = new Size(640, 480);

        Capture videoCapture;

        public ParticleFilterModelFittingDemo()
        {
            InitializeComponent();
            
           /* try
            {
                videoCapture = new Capture(0);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }
         
            videoCapture.VideoSize = imgSize; //set new Size(0,0) for the lowest one

            this.FormClosing += ColorParticleDemo_FormClosing;
            Application.Idle += videoCapture_ProcessFrame;
            videoCapture.Start();*/

            /*PointF[] coords = new PointF[] 
            {
                new PointF(35, 47),
                new PointF(35, 47),
                new PointF(16, 40),
                new PointF(15, 15),
                new PointF(25, 36),
                new PointF(40, 15),
                new PointF(65, 25),
                new PointF(50, 40),
                new PointF(60, 42),
                new PointF(80, 37),
                new PointF(80, 37)
            };
            coords = coords.FlipVertical(coords.Max(x=>x.Y)).Scale(10, 10).ToArray();*/

            Image<Bgr, byte> img = new Image<Bgr, byte>(1000, 1000);
            //HandModel handModel = new HandModel(coords);
            HandModel handModel = HandModel.Load("hand_nodes.txt");
            handModel.Draw(img);

            this.pictureBox.Image = img.ToBitmap();
            return;
        }

        Image<Bgr, byte> frame;
        Font font = new Font("Arial", 12);
        void videoCapture_ProcessFrame(object sender, EventArgs e)
        {
            bool hasNewFrame = videoCapture.WaitForNewFrame(); //do not process the same frame
            if (!hasNewFrame)
                return;

            frame = videoCapture.QueryFrame();

            long start = DateTime.Now.Ticks;

            var edge = FeatureMap.Compute(frame);
            //var edge = frame.Convert<Gray, short>();
            //var edge2 = edge.Sobel(1, 0, 3).InRange(40, 255);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + elapsedMs + " ms", font, new System.Drawing.PointF(15, 10), new Bgr(0, 255, 0));
            this.pictureBox.Image = edge.ToBitmap(); //it will be just casted (data is shared)

            GC.Collect();
        }

        void ColorParticleDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null)
                videoCapture.Stop();
        }

    }
}
