using Accord.Core;
using Accord.Imaging;
using Accord.Math.Geometry;
using Accord.Statistics.Filters;
using Accord.Vision;
using AForge;
using LINE2D;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ParticleFilterModelFitting
{
    public partial class ParticleFilterModelFittingDemo : Form
    {
        const int NUMBER_OF_PARTICLES = 200;
        Size imgSize = new Size(640 / 2, 480 / 2);
        Image<Bgr, byte> debugImg;

        IEnumerable<ModelParticle> particleFilter;

        LinearizedMapPyramid linPyr = null;
        static MatchClustering matchClustering = new MatchClustering();

        IEnumerable<ModelParticle> initalParticles = null;
        private void init()
        {
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources", "PrototypeTemplatesBW");
            var generatedScales = EnumerableExtensions.GetRange(60, 170, 3);
            var generatedOrientations = EnumerableExtensions.GetRange(-90, +90, (int)((180f / GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS) / 2 / 2 /*user factor*/));

            Console.WriteLine("Generating templates.... Please wait!");
            var templates = OpenHandTemplate.CreateRange(path, "*.bmp", generatedScales, generatedOrientations);
            ModelRepository.Initialize(templates);
            //drawTemplatesToFiles(templates, "C:/generatedTemplates");
            
            Console.WriteLine("Initializing particle filter!");
            particleFilter = ParticleFilter.UnifromParticleSpreadInitializer(
                                                    NUMBER_OF_PARTICLES,
                                                    new DoubleRange[] 
                                                    { 
                                                        //template type
                                                        new DoubleRange(0, 0),

                                                        //scale
                                                        new DoubleRange(70, 150),

                                                        //rotation
                                                        new DoubleRange(-15, 15)
                                                    },
                                                    ModelParticle.FromParameters).Distinct().ToList();

            //particleFilter = templates.Select(x => ModelParticle.FromParameters(x.Key.ModelTypeIndex, x.Key.Scale, x.Key.Angle)).ToList();

            //initalParticles = particleFilter.Select(x => (ModelParticle)x.Clone()).ToList();

            particleFilter2 = particleFilter.Select(x => (ModelParticle)x.Clone()).ToList();
        }

        private void predict()
        {
            particleFilter.Predict
                (
                   //drift
                   (p) => p.Drift(),
                   //diffuse
                   (p) => p.Difuse()
                );

            particleFilter = particleFilter.Distinct(); //some particles can be the same
        }

        private void update()
        {
            particleFilter = particleFilter.Update
                (
                    //measure
                    particles => measure(linPyr, particles.ToList()),
                    //normalize
                    particles => ParticleFilter.SimpleNormalizer(particles),
                    //re-sample
                    (particles, normalizedWeights) => 
                    {
                        var sampledParticles = ParticleFilter.SimpleResampler(particleFilter.ToList(), 
                                                                              normalizedWeights.ToList(), 
                                                                              NUMBER_OF_PARTICLES);

                        return sampledParticles;
                    }
                );
        }

        private void measure(LinearizedMapPyramid linPyr, List<ModelParticle> particles)
        {
            particles.ForEach(x => x.Weight = 0);

            var matches = Detector.MatchTemplates(linPyr.PyramidalMaps.First(), particles, 85);
            if (matches.Count == 0)
            {
                //particleFilter = initalParticles;
                return;
            }

            var groups = matchClustering.Group(matches.ToArray(), MatchClustering.COMPARE_BY_SIZE);

            var bestGroup = groups.MaxBy(x => x.Neighbours); //for now
            var largestSize = bestGroup.Representative.Template.Size;
            var scaleFactor = 1f / (largestSize.Width * largestSize.Height);

            foreach (var m in bestGroup.Detections) 
            {
                var particle = (ModelParticle)m.Template;
                var score = (m.Score / 100) * (particle.Size.Width * particle.Size.Height * scaleFactor); //actual score multiplied with size factor (the bigger the better)
                m.Score = score;
                if (particle.Weight < score)
                {
                    particle.Weight = score; //1 particle may correspond to several matches
                    particle.MetaData = m; //WARNING: circular reference
                }
            }
            bestGroup.Representative = bestGroup.Detections.MaxBy(x => x.Score);
            //frame.Draw(bestGroup.Representative, new Bgr(Color.Blue), 1, true, new Bgr(Color.Red));
            //frame.Save("C:/bla.jpg");
        }

        int a = 0;
        private void exec(Image<Bgr, byte> img)
        {
            var grayIm = img.Convert<Gray, byte>();
            linPyr = LinearizedMapPyramid.CreatePyramid(grayIm); //prepare linear-pyramid maps

            debugImg.Clear();


            exec0(linPyr);
            //exec1(linPyr);
            //exec2(linPyr);
            //frame.Save(String.Format("C:/results/image_{0}.png", a));

            a++;

            this.pictureBoxDebug.Image = debugImg.ToBitmap();
        }

        private void exec0(LinearizedMapPyramid linPyr)
        {
            predict();
            update();

            var sortedParticles = particleFilter.OrderByDescending(x => x.Weight);
            var p = sortedParticles.First();

            if (p.Weight < 0.5) return;
            if (p.MetaData != null)
                frame.Draw(p.MetaData, new Bgr(Color.Blue), 1);

            Console.WriteLine(String.Format("W: {0:0.00}, S:{1:00}, A:{2:00}",
                                       p.Weight, p.ModelParameters.Scale, p.ModelParameters.Angle));
        }

        private void exec1(LinearizedMapPyramid linPyr)
        {
            measure(linPyr, particleFilter.ToList());
            var sortedParticles = particleFilter.OrderByDescending(x => x.Weight);
            var bestP = sortedParticles.First();

            var p = bestP;
            {
                if (p.Weight < 0.5) return;

                var rect = new Rectangle(imgSize.Width / 2, imgSize.Height / 2, 0, 0);
                rect.Inflate(p.Size.Width / 2, p.Size.Height / 2);

                debugImg.Draw(rect, new Bgr(Color.Red), 3);

                if (p.MetaData != null)
                    debugImg.Draw(p.MetaData, new Bgr(Color.Red), 1);
            }//);

            Console.WriteLine(String.Format("W: {0:0.00}, S:{1:00}, A:{2:00}",
                                        bestP.Weight, bestP.ModelParameters.Scale, bestP.ModelParameters.Angle));
        }

        List<ModelParticle> particleFilter2 = null;
        private void exec2(LinearizedMapPyramid linPyr)
        {
            measure(linPyr, particleFilter2);
            var sortedParticles = particleFilter2.OrderByDescending(x => x.Weight);
            var bestP = sortedParticles.First();

            if(bestP.MetaData != null)
                frame.Draw(bestP.MetaData, new Bgr(Color.Blue), 1);

            var newParticles = new List<ModelParticle>();
            for (int i = bestP.ModelParameters.Scale - 15; i <= bestP.ModelParameters.Scale + 15; i += 3)
            {
                //int i = 10;
                for (int j = bestP.ModelParameters.Angle - 15; j <= bestP.ModelParameters.Angle + 15; j += 5)
                {
                    var np = ModelParticle.FromParameters(0, i, j);
                    newParticles.Add(np);
                }
            }

            Console.WriteLine(String.Format("W: {0:0.00}, S:{1:00}, A:{2:00} GeneratedForNext: S: [{3:00}..{4:00}], A: [{5:00}..{6:00}]",
                                          bestP.Weight, bestP.ModelParameters.Scale, bestP.ModelParameters.Angle,
                                          newParticles.Min(x => x.ModelParameters.Scale), newParticles.Max(x => x.ModelParameters.Scale),
                                          newParticles.Min(x => x.ModelParameters.Angle), newParticles.Max(x => x.ModelParameters.Angle)));

            particleFilter2 = newParticles;
        }

        CaptureBase videoCapture;

        public ParticleFilterModelFittingDemo()
        {
            InitializeComponent();

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "SIMDArrayInstructions.dll")) == false)
            {
                MessageBox.Show("Copy SIMDArrayInstructions.dll to your bin directory!");
                return;
            }

            debugImg = new Image<Bgr, byte>(imgSize);
            init();

            try
            {
                videoCapture = new ImageSequenceCapture("C:/probaImages", ".jpg", 1); 
                //videoCapture = new Capture();
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }
         
            videoCapture.VideoSize = imgSize; //set new Size(0,0) for the lowest one

            this.FormClosing += ColorParticleDemo_FormClosing;
            //Application.Idle += videoCapture_ProcessFrame;
            videoCapture.NewFrame += videoCapture_ProcessFrame;
            videoCapture.Start();
        }

        Image<Bgr, byte> frame;
        Font font = new Font("Arial", 12);
        void videoCapture_ProcessFrame(object sender, EventArgs e)
        {
            bool hasNewFrame = videoCapture.WaitForNewFrame(); //do not process the same frame
            if (!hasNewFrame)
                return;

            frame = videoCapture.QueryFrame().Clone();//.GetSubRect(new Rectangle(0, 0, 200, 200));

            long start = DateTime.Now.Ticks;

            exec(frame);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + elapsedMs + " ms", font, new System.Drawing.PointF(15, 10), new Bgr(0, 255, 0));
            this.pictureBox.Image = frame.ToBitmap();

            GC.Collect();
        }

        void ColorParticleDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null)
                videoCapture.Stop();
        }


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
                using (var img = new Image<Bgr, byte>(template.Size.Width + 2 * BORDER_OFFSET, template.Size.Height + 2 * BORDER_OFFSET))
                {
                    img.Draw(template, new System.Drawing.PointF(BORDER_OFFSET, BORDER_OFFSET), new Bgr(Color.Red), 2, true, new Bgr(Color.Green));
                    img.Save(fileName);
                }
            }
        }

        #endregion
    }
}
