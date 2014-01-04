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
using System.Threading;
using LINE2D;
using Particle = Accord.Statistics.Filters.Particle<ParticleFilterModelFitting.ParticleState>;

namespace ParticleFilterModelFitting
{
    class MyClass: ITemplatePyramid<ParticleState>
    {
        public ParticleState[] Templates
        {
            get;
            set;
        }

        public void Initialize(ParticleState[] templates)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ParticleFilterModelFittingDemo : Form
    {
        Size imgSize = new Size(640, 480);
        Image<Bgr, byte> debugImg;

        ParticleFilter<Particle, ParticleState> particleFilter;
        IEnumerable<Particle> bestParticles;

        LinearizedMapPyramid linPyr = null;
        MatchClustering matchClustering = new MatchClustering();

        private void init()
        {
            OpenHandTemplate.LoadPrototype("myFile.txt");

            var particleInitializer = FilterMethods<Particle, ParticleState>.UnifromParticleSpreadInitializer(new DoubleRange[] 
                            { 
                                //scale
                                new DoubleRange(250, 400),

                                //rotation
                                new DoubleRange(-5, 5)
                            },
                            ParticleState.FromArray);

            particleFilter = new ParticleFilter<Particle, ParticleState>
            {
                //Initialize
                ParticlesCount = 1000,
                Initializer = particleInitializer,

                //Predict
                Drift = ParticleState.Drift,
                Diffuse = difuse,

                //Update
                WeightAssigner = assignWeight,
                Resampler = (particles) => 
                            {
                                //return particles;

                                var nBestParticles = 0;

                                var sortedParticles = particles.OrderByDescending(x => x.Weight).ToList();
                                bestParticles = sortedParticles.Take(nBestParticles).ToList();

                                var sampleParticles = FilterMethods<Particle, ParticleState>.SimpleResampler();
                                var sampledParticles = sampleParticles(sortedParticles.Skip(nBestParticles));

                                return bestParticles.Concat(sampledParticles);
                            },
                Normalizer = (particles) => {} //FilterMethods<Particle<ParticleState>, ParticleState>.SimpleNormalizer()
            };

            particleFilter.Initialize();
        }

        private void difuse(ref ParticleState state)
        {
            if (bestParticles == null || bestParticles.Select(x => x.State).Contains(state) == false)
            {
                ParticleState.Difuse(ref state);
            }
        }

        private void exec(Image<Bgr, byte> img)
        {
            var grayIm = img.Convert<Gray, byte>();
            linPyr = LinearizedMapPyramid.CreatePyramid(grayIm); //prepeare linear-pyramid maps

            /*var tp = new List<MyClass>();
            foreach (var p in particleFilter.Particles)
            {
                tp.Add(new MyClass { Templates = new ParticleState[] { p.State } });
            }

            XMLTemplateSerializer<MyClass, ParticleState>.Save(tp, "C:/bla.xml");
            return;*/
          
            //particleFilter.Particles.ForEach(x => x.Weight = assignWeight(particleFilter, x.State));


            var matchesByParticles = new List<List<Match>>();

            for (int i = 0; i < particleFilter.Particles.Count; i++)
            {
                var matches = Detector.MatchTemplate(this.linPyr.PyramidalMaps.Last(), particleFilter.Particles[i].State, 65);
                matchesByParticles.Add(matches);
            }

            var allMatches = new List<Match>();
            foreach (var mp in matchesByParticles)
            {
                allMatches.AddRange(mp);
            }
        
            var clusters = matchClustering.Group(allMatches.ToArray());
            if (clusters.Length != 0)
            {
                var bestCluster = clusters.MaxBy(x => x.Neighbours);
                var bestP = new Particle<ParticleState> { State = bestCluster.Representative.Template as ParticleState, Weight = bestCluster.Representative.Score };
                bestP.State.MetaData = bestCluster;
                particleFilter.Particles.Clear();
                particleFilter.Particles.Add(bestP);
            }
            //for (int i = 0; i < 10; i++)
            {
                //particleFilter.Predict();
                //particleFilter.Update();
            }

            debugImg.Clear();
            var sortedParticles = particleFilter.Particles.OrderByDescending(x => x.Weight);
            sortedParticles.Take(1).ForEach(x => 
            {
                var rect = new Rectangle(imgSize.Width / 2, imgSize.Height / 2, 0, 0);
                rect.Inflate(x.State.Size.Width, x.State.Size.Height);

                debugImg.Draw(rect, new Bgr(Color.Red), 3);

                if (x.State.MetaData == null)
                    return;

                img.Draw(x.State.MetaData.Representative, new Bgr(Color.Red), 3, 3);
            });
            Console.WriteLine("Best particle: " + sortedParticles.First().Weight);

            this.pictureBoxDebug.Image = debugImg.ToBitmap();


            var best = particleFilter.Particles.MaxBy(x => x.Weight);

            if (best.Weight > 50)
            {
                particleFilter.Particles.Clear();
                for (double i = Math.Max(150, best.State.Scale - 50); i < best.State.Scale + 50; i+=5)
                {
                    //double j = 0;
                    for (double j = best.State.RotationZ - 10; j < best.State.RotationZ + 10; j+=3)
                    {
                        var p = new Particle();
                        p.Weight = 0;
                        p.State = ParticleState.FromArray(i, j);
                        particleFilter.Particles.Add(p);
                    }
                }
            }

        }

        double assignWeight(ParticleFilter<Particle, ParticleState> filter, ParticleState state)
        {
            var template = state as ITemplate;
            var matches = Detector.MatchTemplate(this.linPyr.PyramidalMaps.Last(), template, 60);
            var clusters = matchClustering.Group(matches.ToArray());

            if (clusters.Length == 0)
                return 0;

            var bestCluster = clusters.MaxBy(x => x.Neighbours);

            state.MetaData = bestCluster;

            return bestCluster.Representative.Score / 100;
        }

        CaptureBase videoCapture;

        public ParticleFilterModelFittingDemo()
        {
            InitializeComponent();

            debugImg = new Image<Bgr, byte>(imgSize);
            init();

            /*frame = Bitmap.FromFile("C:/proba2Images/scene00189.png").ToImage<Bgr, byte>().GetSubRect(new Rectangle(0,0, 320, 320));
            exec(frame);
            //exec(frame);
            pictureBox.Image = frame.ToBitmap();
            return;*/

            try
            {
                //videoCapture = new ImageSequenceCapture("C:/proba2Images", ".png", 100); 
                videoCapture = new Capture();
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
            OpenHandTemplate.LoadPrototype("myFile.txt");

            OpenHandTemplate template = null;

            //var s = DateTime.Now.Ticks;

            //Parallel.For(0, 2 * 1000, (int i) => { 
            //for (int i = 0; i < 2 * 1000; i++){
                template = OpenHandTemplate.Create(500, 500,
                                                   0);
            //});

            //var e = DateTime.Now.Ticks;
            //Console.WriteLine((e - s) / TimeSpan.TicksPerMillisecond);
            //return;

            template.Draw(img);
            img.Draw(template.BoundingBox, new Bgr(Color.Green), 3);
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

            frame = videoCapture.QueryFrame().GetSubRect(new Rectangle(0, 0, 200, 200));

            long start = DateTime.Now.Ticks;

            //frame = Bitmap.FromFile("proba.jpg").ToImage<Bgr, byte>();
            //Thread.Sleep(500);
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
