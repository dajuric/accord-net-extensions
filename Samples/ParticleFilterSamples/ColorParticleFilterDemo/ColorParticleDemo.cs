using Accord.Imaging;
using Accord.Math;
using Accord.Math.Geometry;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Filters;
using Accord.Vision;
using AForge;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleParticleFilterDemo
{
    public partial class SimpleParticleDemoForm : Form
    {
        class ColorParticle: IParticle
        {
            static NormalDistribution normalDistribution = new NormalDistribution();

            public double Weight { get; set; }
            public PointF Position { get; set; }

            public static void Drift(ColorParticle p)
            {
                //we do not have velocity (or something else), so nothing :)
            }

            public static void Difuse(ColorParticle p)
            {
                p.Position = new PointF
                {
                    X = p.Position.X + 25 * (float)normalDistribution.Generate(),
                    Y = p.Position.Y + 25 * (float)normalDistribution.Generate(),
                };
            }

            public static ColorParticle FromArray(double[] arr)
            {
                return new ColorParticle
                {
                    Position = new PointF((float)arr[0], (float)arr[1]),
                };
            }

            object ICloneable.Clone()
            {
                return new ColorParticle 
                {
                     Position = this.Position,
                     Weight = this.Weight
                };
            }
        }

        Size imgSize = new Size(640, 480);

        Color referenceColor = Color.Red; //User defined color
        List<ColorParticle> particleFilter;
      
        private void init()
        {
            particleFilter = ParticleFilter.UnifromParticleSpreadInitializer<ColorParticle>
                                                    (
                                                        //particles' count
                                                        1000,
                                                        //position range
                                                        new DoubleRange[] 
                                                        { 
                                                            new DoubleRange(0, imgSize.Width), 
                                                            new DoubleRange(0, imgSize.Height)
                                                        },
                                                        //convert arr => position (create from array)
                                                        ColorParticle.FromArray
                                                    )
                                                    .ToList();

        }

        private void predict()
        {
            particleFilter.Predict
               (
                   //drift
                   p => ColorParticle.Drift(p),
                   //difuse
                   p => ColorParticle.Difuse(p)
               );
        }

        private void update()
        {
           particleFilter = particleFilter.Update
               (
                   //measure
                   p => particleWeightUpdateFunc(p),
                   //normalize weights
                   particles => ParticleFilter.SimpleNormalizer(particles),
                   //resample (if necessary)
                   (particles, normalizedWeights) => ParticleFilter.SimpleResampler(particles.ToList(), normalizedWeights.ToList())
               )
               .ToList();
        }

        NormalDistribution prob = new NormalDistribution(mean: 0, stdDev: 50);

        private void particleWeightUpdateFunc(ColorParticle p)
        { 
            double[] distanceVector = new double[] { 255, 255, 255 };
            var location = System.Drawing.Point.Round(p.Position);

            //check if a particle got outside the image boundaries
            if (location.X >= 0 && location.X < imgSize.Width &&
                location.Y >= 0 && location.Y < imgSize.Height)
            {
                Bgr8 particleColor;
                unsafe
                {
                    particleColor = *(Bgr8*)frame.GetData(location.Y, location.X); //much faster than frame[row, col]
                }

                distanceVector = new double[]{referenceColor.R - particleColor.R,
                                              referenceColor.G - particleColor.G,
                                              referenceColor.B - particleColor.B};
            }

            var distance = distanceVector.Multiply(distanceVector.Transpose())[0];

            //applied log function on normal distribution:
            /*double constAddFactor = -Math.Log(Math.Sqrt(2 * Math.PI) * stdDev);
            double constMulFactor = -1 / (2 * stdDev * stdDev); 
            double probability = constAddFactor + constMulFactor * distance;*/

            var probability = prob.ProbabilityDensityFunction(Math.Sqrt(distance));
            p.Weight = probability;       
        }

        #region GUI...

        Capture videoCapture;

        public SimpleParticleDemoForm()
        {
            InitializeComponent();
            init();

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

            predict();
            update();
           
            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            drawParticles(particleFilter, frame);
            frame.Draw("Processed: " + elapsedMs + " ms", font, new PointF(15, 10), new Bgr(0, 255, 0));
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared)

            GC.Collect();
        }

        private void drawParticles(IEnumerable<ColorParticle> particles, Image<Bgr, byte> img)
        {
            var circles = particles.Select(x => new CircleF { X = x.Position.X, Y = x.Position.Y, Radius = 1.5f });
            img.Draw(circles, new Bgr(Color.Blue), 5);
        }

        void ColorParticleDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null)
                videoCapture.Stop();
        }

        #endregion
    }
}
