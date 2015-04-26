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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Statistics.Filters;
using Accord.Extensions.Imaging;
using AForge;
using Accord.Extensions.Imaging.Algorithms.LINE2D;
using MoreLinq;
using Font = Accord.Extensions.Imaging.Font;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Univariate;

namespace ParticleFilterModelFitting
{
    public partial class ParticleFilterModelFittingDemo : Form
    {
        const int NUMBER_OF_PARTICLES = 250;
        const int MATCHING_MIN_THRESHOLD = 85;
        Size imgSize = new Size(640 / 2, 480 / 2);

        List<ModelParticle> particleFilter;

        /************************************** "tracking" reset ******************************************************/
        //particle are spreading after time to cover as much states as possible, but if we lost object I want to start from initial states (hand upright)
        List<ModelParticle> initialParticles;
        Stopwatch resetClock;
        const int MAX_NONOBJ_TIME = (int)(2.5 * 1000); //X seconds => set it to Int32.MaxValue to avoid resetting particle states
        /************************************** "tracking" reset ******************************************************/

        LinearizedMapPyramid linPyr = null;
        static MatchClustering matchClustering = new MatchClustering();

        private void init()
        {
            var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).FullName, "Resources", "PrototypeTemplatesBW");
            var generatedScales = EnumerableExtensions.GetRange(60, 170, 3);
            var generatedOrientations = EnumerableExtensions.GetRange(-90, +90, (int)((180f / GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS) / 2 / 2 /*user factor - last "2"*/));

            Console.WriteLine("Generating templates.... Please wait!");
            var templates = OpenHandTemplate.CreateRange(path, "*.bmp", generatedScales, generatedOrientations);
            ModelRepository.Initialize(templates);
            //drawTemplatesToFiles(templates, "C:/generatedTemplates");
            
            Console.WriteLine("Initializing particle filter!");

            particleFilter = new List<ModelParticle>();
            particleFilter.CreateParticles(NUMBER_OF_PARTICLES,  //particle count
                                          ModelParticle.FromParameters, //convert arr => position (create from array)
                                          new ISampleableDistribution<double>[]  //position range
                                           { 
                                               //template type
                                               new UniformContinuousDistribution(0, ModelRepository.PrototypeCount - 1),
                                               //scale
                                               new UniformContinuousDistribution(70, 150),
                                               //rotation
                                               new UniformContinuousDistribution(-15, 15)
                                           });

            initialParticles = particleFilter.Select(x => (ModelParticle)x.Clone()).ToList();
            resetClock = new Stopwatch(); resetClock.Start();
        }

        private void predict()
        {
            particleFilter = particleFilter
                             .Predict(effectiveCountMinRatio: 0.9f, 
                                      sampleCount: NUMBER_OF_PARTICLES /*preserve number of particles*/);
        }

        private void update()
        {
            var uniqueParticles = particleFilter.GroupBy(x => x.ModelParameters).Select(x => x.First()); //get distinct particles (there is no need to match the same templates)

            var matches = linPyr.PyramidalMaps.First().MatchTemplates(uniqueParticles, MATCHING_MIN_THRESHOLD);
            if (matches.Count == 0)
                return;

            var groups = matchClustering.Group(matches.ToArray(), MatchClustering.COMPARE_BY_SIZE);
            var bestGroup = groups.MaxBy(x => x.Representative.BoundingRect.Area());
            var largestSize = bestGroup.Representative.Template.Size;
            var scaleFactor = 1f / (largestSize.Width * largestSize.Height);

            foreach (var m in bestGroup.Detections) 
            {
                var particle = (ModelParticle)m.Template;
                var score = (m.Score / 100) * (particle.Size.Width * particle.Size.Height * scaleFactor); //actual score multiplied with size factor (the bigger the better)
                m.Score = score;
                if (particle.Weight < score) //1 particle may correspond to several matches
                {
                    particle.Weight = score; 
                    particle.MetaDataRef = new WeakReference<Match>(m); //avoid circular reference (template inside match is particle again)
                }
            }
        }

        private void processFrame(Bgr<byte>[,] img, out long matchTimeMs)
        {
            var grayIm = img.ToGray();
            linPyr = LinearizedMapPyramid.CreatePyramid(grayIm); //prepare linear-pyramid maps

            /******************************* match templates + particle filter stuff ******************************/
            matchTimeMs = Diagnostics.MeasureTime(() => 
                {
                    predict();
                    update();
                });
            /******************************* match templates + particle filter stuff ******************************/

            /******************************* reset tracking (if necessary) ******************************/
            var p = particleFilter.MaxBy(x => x.Weight);
            if (p.Weight == 0)
            {
                if (resetClock.ElapsedMilliseconds > MAX_NONOBJ_TIME)
                    particleFilter = initialParticles;

                return;
            }
            resetClock.Restart();
            /******************************* reset tracking (if necessary) ******************************/

            /********************************* output **************************************/
            var metaData = getData(p.MetaDataRef);
            var text = String.Format("S:{0:00}, A:{1:00}",
                                      p.ModelParameters.Scale, p.ModelParameters.Angle);

            if (metaData != null)
            { 
                //img.Draw(p.MetaData, new Bgr(Color.Blue), 1);
                img.Draw(metaData.Points.ToArray(), Bgr<byte>.Blue, 3);
                img.DrawAnnotation(metaData.BoundingRect, text, Accord.Extensions.Imaging.Font.Big);
            }

            Console.WriteLine(text);
            /********************************* output **************************************/
        }

        #region GUI

        ImageStreamReader videoCapture;

        public ParticleFilterModelFittingDemo()
        {
            InitializeComponent();

            init();

            try
            {
                string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");

                videoCapture = new ImageDirectoryReader(Path.Combine(resourceDir, "SampleVideos", "1"), "*.jpg");   //1st sample
                //videoCapture = new ImageDirectoryReader(Path.Combine(resourceDir, "SampleVideos", "2"), "*.jpg"); //2nd sample
            }
            catch (Exception)
            {
                MessageBox.Show("Video stream reading exception!");
                return;
            }
         
            if(videoCapture is CameraCapture)
                (videoCapture as CameraCapture).FrameSize = imgSize;

            this.FormClosing += ColorParticleDemo_FormClosing;
            Application.Idle += videoCapture_ProcessFrame;
            videoCapture.Open();
        }

        Bgr<byte>[,] frame = null;
        Accord.Extensions.Imaging.Font font = new Font(FontTypes.HERSHEY_DUPLEX, 1, 0.2f);
        void videoCapture_ProcessFrame(object sender, EventArgs e)
        {
            videoCapture.ReadTo<Bgr<byte>>(ref frame); 
            if (frame == null)
                return;

            frame.StretchContrast(inPlace: true);

            long start = DateTime.Now.Ticks;

            long matchTimeMs;
            processFrame(frame, out matchTimeMs);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + /*matchTimeMs*/ elapsedMs + " ms", font, new Point(25, 20), Bgr<byte>.Red);
            this.pictureBox.Image = frame.ToBitmap();
            GC.Collect();
        }

        void ColorParticleDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null)
                videoCapture.Dispose();
        }

        #endregion

        #region Debuging

        private static void drawTemplatesToFiles(Dictionary<ModelParams, ITemplate> dict, string path)
        {
            const int BORDER_OFFSET = 10;

            foreach (var pair in dict)
            {
                var mParams = pair.Key;
                var fileName = String.Format("template_{0}_scale{1}_angle_{2}.bmp", mParams.ModelTypeIndex, mParams.Scale, mParams.Angle);
                fileName = Path.Combine(path, fileName);

                var template = pair.Value;
                var img = new Bgr<byte>[template.Size.Height + 2 * BORDER_OFFSET, template.Size.Width + 2 * BORDER_OFFSET];
                img.Draw(template.Features.Select(x => new Point(x.X + BORDER_OFFSET, x.Y + BORDER_OFFSET)).ToArray(), Bgr<byte>.Red, 3);
                img.Save(fileName);
            }
        }

        #endregion

        #region Helper function

        private static T getData<T>(WeakReference<T> wr)
            where T: class
        {
            if (wr == null) return null;

            T val;
            wr.TryGetTarget(out val);
            return val;
        }
       
        #endregion
    }
}
