#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Statistics.Filters;
using Accord.Extensions.Imaging;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using AForge;
using PointF = AForge.Point;
using Point = AForge.IntPoint;
using System.IO;
using Accord.Statistics.Distributions;

namespace SimpleParticleFilterDemo
{
    public partial class SimpleParticleDemoForm : Form
    {
        class ColorParticle: IParticle
        {
            static NormalDistribution normalDistribution = new NormalDistribution();

            public double Weight { get; set; }
            public PointF Position { get; set; }

            public void Drift()
            {
                //we do not have velocity (or something else), so nothing :)
            }

            public void Diffuse()
            {
                this.Position = new PointF
                {
                    X = this.Position.X + 25 * (float)normalDistribution.Generate(),
                    Y = this.Position.Y + 25 * (float)normalDistribution.Generate(),
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

        Size imgSize = new Size(640 / 2, 480 / 2);

        Bgr<byte> referenceColor = Bgr<byte>.Red; //User defined color
        List<ColorParticle> particleFilter;
      
        private void init()
        {
            particleFilter = new List<ColorParticle>();
            particleFilter.CreateParticles(1000,  //particles' count
                                           ColorParticle.FromArray, //convert arr => position (create from array)
                                           new ISampleableDistribution<double>[]  //position range
                                           { 
                                               new UniformContinuousDistribution(0, imgSize.Width),
                                               new UniformContinuousDistribution(0, imgSize.Height)
                                           });
        }

        private void predict()
        {
            particleFilter = particleFilter.Predict();
        }

        private void update()
        {
            particleFilter.Update(measure);
        }

        NormalDistribution prob = new NormalDistribution(mean: 0, stdDev: 50);

        private double measure(ColorParticle p)
        { 
            double[] distanceVector = new double[] { 255, 255, 255 };
            var location = p.Position.Round();

            //check if a particle got outside the image boundaries
            if (location.X >= 0 && location.X < imgSize.Width &&
                location.Y >= 0 && location.Y < imgSize.Height)
            {
                Bgr<byte> particleColor;
                unsafe
                {
                    particleColor = frame[location.Y, location.X];
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

        ImageStreamReader videoCapture; 

        public SimpleParticleDemoForm()
        {
            InitializeComponent();
            init();

            try
            {
                //videoCapture = new CameraCapture(0);
                string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
                videoCapture = new ImageDirectoryReader(Path.Combine(resourceDir, "ImageSequence"), "*.jpg"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if (videoCapture is CameraCapture)
                (videoCapture as CameraCapture).FrameSize = imgSize;

            videoCapture.Open();

            this.FormClosing += ColorParticleDemo_FormClosing;
            Application.Idle += videoCapture_ProcessFrame;
        }

        Bgr<byte>[,] frame;
        Accord.Extensions.Imaging.Font font = new Accord.Extensions.Imaging.Font(FontTypes.HERSHEY_COMPLEX, 1, 0.5f); 
        void videoCapture_ProcessFrame(object sender, EventArgs e)
        {
            videoCapture.ReadTo(ref frame);
            if (frame == null)
                return;

            long start = DateTime.Now.Ticks;

            predict();
            update();
           
            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            drawParticles(particleFilter.Draw(sampleCount: particleFilter.Count / 2), frame); //draw only better particles
            frame.Draw("Processed: " + elapsedMs + " ms", font, new Point(5, 20), Bgr<byte>.Red);
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared)

            GC.Collect();
        }

        private void drawParticles(IEnumerable<ColorParticle> particles, Bgr<byte>[,] img)
        {
            var circles = particles.Select(x => new Circle { X = (int)x.Position.X, Y = (int)x.Position.Y, Radius = 2 });
            img.Draw(circles, Bgr<byte>.Blue, 5);
        }

        void ColorParticleDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null)
                videoCapture.Dispose();
        }

        #endregion
    }
}
