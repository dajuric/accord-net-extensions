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
using Accord.Imaging.Filters;
using System.Runtime.InteropServices;
using Accord.Statistics.Filters;
using AForge;
using Accord.Statistics.Distributions.Univariate;
using MoreLinq;

namespace ParticleFilterModelFitting
{
    public partial class ParticleFilterModelFittingDemo : Form
    {
        Size imgSize = new Size(640, 480);

        ParticleFilter<Particle<ParticleState>, ParticleState> particleFilter;
        Image<Gray, byte> orientationImage;

        private void init()
        {
            Template.LoadPrototype("myFile.txt");

            var particleInitializer = FilterMethods<Particle<ParticleState>, ParticleState>.UnifromParticleSpreadInitializer(new DoubleRange[] 
                            { 
                                //position
                                new DoubleRange(0, imgSize.Width), 
                                new DoubleRange(0, imgSize.Height),

                                //scale
                                new DoubleRange(100, 200),

                                //rotation
                                new DoubleRange(-5, 5)
                            },
                            ParticleState.FromArray);

            particleFilter = new ParticleFilter<Particle<ParticleState>, ParticleState>
            {
                //Initialize
                ParticlesCount = 1000,
                Initializer = particleInitializer,

                //Predict
                Drift = ParticleState.Drift,
                Diffuse = ParticleState.Difuse,

                //Update
                WeightAssigner = (filter, state) => state.HandTemplate.GetScore(orientationImage),
                Resampler = FilterMethods<Particle<ParticleState>, ParticleState>.SimpleResampler(),
                Normalizer = FilterMethods<Particle<ParticleState>, ParticleState>.SimpleNormalizer()
            };

            particleFilter.Initialize();
        }

        private void exec(Image<Bgr, byte> img)
        {
            orientationImage = FeatureMap.Compute(img);

            particleFilter.Predict();
            particleFilter.Update();

            particleFilter.Particles.ForEach(x => x.State.HandTemplate.Draw(img));
        }

        Capture videoCapture;

        public ParticleFilterModelFittingDemo()
        {
            InitializeComponent();

            init();

            Image<Bgr, byte> proba = Bitmap.FromFile("").ToImage<Bgr, byte>();
            exec(proba);
            return;

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
            Template.LoadPrototype("myFile.txt");

            Template template = null;

            //var s = DateTime.Now.Ticks;

            //Parallel.For(0, 2 * 1000, (int i) => { 
            //for (int i = 0; i < 2 * 1000; i++){
                template = Template.Create(100f, 100f,
                                         500, 500,
                                         0, 0, 45);
            //});

            //var e = DateTime.Now.Ticks;
            //Console.WriteLine((e - s) / TimeSpan.TicksPerMillisecond);
            //return;
            template.Draw(img);
            //img.Draw(new CircleF(template.ControlPoints.Average(x => x.X), template.ControlPoints.Average(x => x.Y), 10), new Bgr(Color.Green), 3);

            /*template = Template.Create(0f, 0f,
                                       500, 500,
                                       0, 45, 90);
            template.Draw(img);*/

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
