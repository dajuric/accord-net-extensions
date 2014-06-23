using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Statistics.Filters;
using Accord.Extensions.Vision;
using AForge;
using LINE2D;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PointF = AForge.Point;

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
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources", "PrototypeTemplatesBW");
            var generatedScales = EnumerableExtensions.GetRange(60, 170, 3);
            var generatedOrientations = EnumerableExtensions.GetRange(-90, +90, (int)((180f / GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS) / 2 / 2 /*user factor - last "2"*/));

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
                                                        new DoubleRange(0, ModelRepository.PrototypeCount - 1),

                                                        //scale
                                                        new DoubleRange(70, 150),

                                                        //rotation
                                                        new DoubleRange(-15, 15)
                                                    },
                                                    ModelParticle.FromParameters).ToList();

            initialParticles = particleFilter.Select(x => (ModelParticle)x.Clone()).ToList();
            resetClock = new Stopwatch(); resetClock.Start();
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
                        var sampledParticles = ParticleFilter.SimpleResampler(particles.ToList(), 
                                                                              normalizedWeights.ToList());

                        return sampledParticles;
                    }
                ).ToList();
        }

        private void measure(LinearizedMapPyramid linPyr, List<ModelParticle> particles)
        {
            particles.ForEach(x => x.Weight = 0);

            IDictionary<ModelParams, IEnumerable<ModelParticle>> nonDistinctMembers;
            var uniqueParticles = getDistinctParticles(particles, out nonDistinctMembers); //get distint particles (there is no need to match the same templates)

            var matches = Detector.MatchTemplates(linPyr.PyramidalMaps.First(), uniqueParticles, MATCHING_MIN_THRESHOLD);
            if (matches.Count == 0)
                return;

            var groups = matchClustering.Group(matches.ToArray(), MatchClustering.COMPARE_BY_SIZE);

            //var bestGroup = groups/*.Where(x=>x.Neighbours > 3)*/.MaxBy(x => x.Neighbours); //for now
            var bestGroup = groups.MaxBy(x => x.Representative.BoundingRect.Area());
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
                    particle.MetaDataRef = new WeakReference<Match>(m); //avoid circular reference (template inside match is particle again)
                }
            }

            updateNonDistintParticleData(uniqueParticles, nonDistinctMembers); //update the rest of particles (which are the same) if any
        }

        private void processFrame(Image<Bgr, byte> img, out long matchTimeMs)
        {
            var grayIm = img.Convert<Gray, byte>();
            linPyr = LinearizedMapPyramid.CreatePyramid(grayIm); //prepare linear-pyramid maps

            /******************************* match templates + particle filter stuff ******************************/
            matchTimeMs = measureTime
            (
                () => 
                {
                    predict();
                    update();
                }
            );
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
            if (metaData != null)
            {
                //img.Draw(p.MetaData, new Bgr(Color.Blue), 1);
                img.Draw(metaData.Points, Bgr8.Blue, 3);

                var text = String.Format("W: {0:0.00}, \nS:{1:00}, A:{2:00}",
                                         p.Weight, p.ModelParameters.Scale, p.ModelParameters.Angle);
                img.DrawAnnotation(metaData.BoundingRect, text, annotationWidth: 80);
            }

            Console.WriteLine(String.Format("W: {0:0.00}, S:{1:00}, A:{2:00}",
                                      p.Weight, p.ModelParameters.Scale, p.ModelParameters.Angle));
            /********************************* output **************************************/
        }

        #region GUI

        StreamableSource videoCapture;

        public ParticleFilterModelFittingDemo()
        {
            InitializeComponent();

            init();

            try
            {
                string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");

                videoCapture = new ImageDirectoryReader(Path.Combine(resourceDir, "SampleVideos", "1" /*"2"*/), ".jpg"); 
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }
         
            if(videoCapture is CameraCapture)
                (videoCapture as CameraCapture).FrameSize = imgSize;

            this.FormClosing += ColorParticleDemo_FormClosing;
            Application.Idle += videoCapture_ProcessFrame;
            videoCapture.Open();
        }

        Image<Bgr, byte> frame;
        System.Drawing.Font font = new System.Drawing.Font("Arial", 12); //int a = 0;
        void videoCapture_ProcessFrame(object sender, EventArgs e)
        {
            frame = videoCapture.ReadAs<Bgr, byte>();
            if (frame == null)
                return;

            frame.StretchContrast(true);
            //frame.Save("C:/image_" + a + ".jpg");

            long start = DateTime.Now.Ticks;

            long matchTimeMs;
            processFrame(frame, out matchTimeMs);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + /*matchTimeMs*/ elapsedMs + " ms", font, new PointF(15, 10), new Bgr(0, 255, 0));
            this.pictureBox.Image = frame.ToBitmap();
            //frame.Save("C:/imageAnn_" + a + ".jpg");
            //a++;
            GC.Collect();

            //Application.RaiseIdle(new EventArgs());
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
                using (var img = new Image<Bgr, byte>(template.Size.Width + 2 * BORDER_OFFSET, template.Size.Height + 2 * BORDER_OFFSET))
                {
                    img.Draw(template, new PointF(BORDER_OFFSET, BORDER_OFFSET), Bgr8.Red, 2, true, Bgr8.Green);
                    img.Save(fileName);
                }
            }
        }

        private static long measureTime(Action action)
        {
            long start = DateTime.Now.Ticks;

            action();

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;
            return elapsedMs;
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

        private static IEnumerable<ModelParticle> getDistinctParticles(IEnumerable<ModelParticle> particles, out  IDictionary<ModelParams, IEnumerable<ModelParticle>> otherGroupMembers)
        {
            var groups = from p in particles
                         group p by p.ModelParameters into uniqueGroup
                         select new
                         {
                             Representative = uniqueGroup.First(),
                             OtherMembers = uniqueGroup.Skip(1)
                         };

            otherGroupMembers = groups.ToDictionary(x => x.Representative.ModelParameters, x => x.OtherMembers);
            return groups.Select(x => x.Representative);
        }

        private static void updateNonDistintParticleData(IEnumerable<ModelParticle> uniqueParticles, IDictionary<ModelParams, IEnumerable<ModelParticle>> nonDistinctParticles)
        {
            foreach (var uniqueParticle in uniqueParticles)
            {
                foreach (var p in nonDistinctParticles[uniqueParticle.ModelParameters])
                {
                    p.MetaDataRef = new WeakReference<Match>(getData(uniqueParticle.MetaDataRef));
                    p.Weight = uniqueParticle.Weight;
                }
            }
        }

        #endregion
    }
}
