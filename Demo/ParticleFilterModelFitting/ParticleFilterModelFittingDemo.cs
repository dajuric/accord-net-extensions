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
                               /* //position
                                new DoubleRange(0, imgSize.Width), 
                                new DoubleRange(0, imgSize.Height),

                                //scale
                                new DoubleRange(250, 300),

                                //rotation
                                new DoubleRange(-5, 5)*/

                                new DoubleRange(55, 55), 
                                new DoubleRange(5, 5),
                                new DoubleRange(310, 310),
                                new DoubleRange(0, 0)
                            },
                            ParticleState.FromArray);

            particleFilter = new ParticleFilter<Particle<ParticleState>, ParticleState>
            {
                //Initialize
                ParticlesCount = 100,
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

            //for (int i = 0; i < 1000; i++)
            {
                particleFilter.Predict();
                particleFilter.Update();
            }

            var sortedParticles = particleFilter.Particles.OrderByDescending(x => x.Weight);
            sortedParticles.Take(1).ForEach(x => x.State.HandTemplate.Draw(img));
            //Console.WriteLine("Best particle: " + sortedParticles.First().Weight);
        }

        Capture videoCapture;

        public ParticleFilterModelFittingDemo()
        {
            InitializeComponent();

            init();

            /*frame = Bitmap.FromFile("C:/probaBW.jpg").ToImage<Bgr, byte>();
            exec(frame);
            pictureBox.Image = frame.ToBitmap();
            return;*/

            try
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
            videoCapture.Start();
            return;
            

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

            //frame = videoCapture.QueryFrame().GetSubRect(new Rectangle(0,0,250,250));

            long start = DateTime.Now.Ticks;

            frame = Bitmap.FromFile("C:/probaBW.jpg").ToImage<Bgr, byte>();
            exec(frame);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + elapsedMs + " ms", font, new System.Drawing.PointF(15, 10), new Bgr(0, 255, 0));
            //this.pictureBox.Image = edge.ToBitmap(); //it will be just casted (data is shared)
            this.pictureBox.Image = frame.ToBitmap();

            GC.Collect();
        }

        void ColorParticleDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null)
                videoCapture.Stop();
        }

    }
}
