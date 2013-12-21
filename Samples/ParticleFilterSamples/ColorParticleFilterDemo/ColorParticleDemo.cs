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
        class State : ICloneable
        {
            static NormalDistribution normalDistribution = new NormalDistribution();

            public PointF Position { get; set; }

            public static void Drift(ref State state)
            {
                //we do not have velocity (or something else), so nothing :)
            }

            public static void Difuse(ref State state)
            {
                state.Position = new PointF
                {
                    X = state.Position.X + 25 * (float)normalDistribution.Generate(),
                    Y = state.Position.Y + 25 * (float)normalDistribution.Generate(),
                };
            }

            public object Clone()
            {
                return new State
                {
                    Position = this.Position
                };
            }

            public static State FromArray(double[] arr)
            {
                return new State
                {
                    Position = new PointF((float)arr[0], (float)arr[1]),
                };
            }
        }

        Size imgSize = new Size(640, 480);

        Color referenceColor = Color.Red; //User defined color
        ParticleFilter<Particle<State>, State> particleFilter;
      
        private void init()
        {
            var particleInitializer = FilterMethods<Particle<State>, State>.UnifromParticleSpreadInitializer(new DoubleRange[] 
                            { 
                                new DoubleRange(0, imgSize.Width), 
                                new DoubleRange(0, imgSize.Height)
                            },
                            State.FromArray);

            particleFilter = new ParticleFilter<Particle<State>, State>
            {
                //Initialize
                ParticlesCount = 1000,
                Initializer = particleInitializer,

                //Predict
                Drift = State.Drift,
                Diffuse = State.Difuse,

                //Update
                WeightAssigner = particleWeightUpdateFunc,
                Resampler = FilterMethods<Particle<State>, State>.SimpleResampler(),
                Normalizer = FilterMethods<Particle<State>, State>.SimpleNormalizer()
            };

            particleFilter.Initialize();
        }

        NormalDistribution prob = new NormalDistribution(mean: 0, stdDev: 50);

        private double particleWeightUpdateFunc(ParticleFilter<Particle<State>, State> filter, State state)
        { 
            double[] distanceVector = new double[] { 255, 255, 255 };
            var location = System.Drawing.Point.Round(state.Position);

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

            return probability;
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

            particleFilter.Predict();
            particleFilter.Update();

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            drawParticles(particleFilter.Particles, frame);
            frame.Draw("Processed: " + elapsedMs + " ms", font, new PointF(15, 10), new Bgr(0, 255, 0));
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared)

            GC.Collect();
        }

        private void drawParticles(IEnumerable<Particle<State>> particles, Image<Bgr, byte> img)
        {
            var circles = particles.Select(x => new CircleF { X = x.State.Position.X, Y = x.State.Position.Y, Radius = 1.5f });
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
