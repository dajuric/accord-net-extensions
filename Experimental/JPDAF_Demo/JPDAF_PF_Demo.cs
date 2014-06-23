using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Statistics.Filters;
using Accord.Statistics.Distributions.Univariate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PointF = AForge.Point;
using Tracker = System.Collections.Generic.List<JPDAF_Demo.MotionParticle>;
using Track = JPDAF_Demo.Track<System.Collections.Generic.List<JPDAF_Demo.MotionParticle>, JPDAF_Demo.TrackTag>;

namespace JPDAF_Demo
{
    public partial class JPDAF_PF_DemoForm : Form
    {
        public const float MAX_ENTROPY_INCREASE = 0.2f;
        public const float MIN_ENTROPY_DECREASE = 0.2f;
        public const int NUMBER_OF_PARTICLES = 100;

        List<TrajectoryData>[] trajectories = null;
        Image<Bgr, byte> debugImg = new Image<Bgr, byte>(640, 480);

        List<Track> tracks = new List<Track>();

        private Track createTrack(PointF location)
        {
            NormalDistribution normalDistribution = new NormalDistribution();

            var track = new Track(new Tracker());
            for (int i = 0; i < NUMBER_OF_PARTICLES; i++)
            {
                var particle = new MotionParticle
                {
                    State = new ConstantVelocity2DModel
                    {
                        Position = new PointF
                        {
                            X = location.X + 5f * (float)normalDistribution.Generate(),
                            Y = location.Y + 5f * (float)normalDistribution.Generate()
                        },
                        Velocity = new PointF 
                        {
                            X = 1f * (float)normalDistribution.Generate(),
                            Y = 1f * (float)normalDistribution.Generate()
                        }
                    }
                };

                track.Tracker.Add(particle);
            }

            return track;
        }

        private void predict()
        {
            var particleFilters = tracks.Select(x => x.Tracker);

            foreach (var particleFilter in particleFilters)
            {
                particleFilter.Predict
                (
                    //drift
                    p => p.Drift(),
                    //diffuse
                    p => p.Difuse()
                );
            }
        }


        NormalDistribution distanceProbability = new NormalDistribution(mean: 0, stdDev: 3);

        private void update(List<PointF> measurements)
        {
            var particleFilters = tracks.Select(x => x.Tracker).ToList();

            double normalizationFactor = 1 / distanceProbability.ProbabilityDensityFunction(0); //just to make bigger numbers :-)

            //update filters
            var assocProbs = particleFilters.Update(measurements, 
                                                    (particle, measurement) => 
                                                         JPDAF.DistanceLikelihood(particle, measurement, x => distanceProbability.ProbabilityDensityFunction(x) * normalizationFactor),
                                                    (particles, normalizedWeights) => 
                                                         ParticleFilter.SimpleResampler(particles.ToList(), normalizedWeights.ToList()));

            //add if necessary
            particleFilters.AddFilters(assocProbs, (measurementIdx) =>  
            {
                var track = createTrack(measurements[measurementIdx]);
                tracks.Add(track);
            });

            //remove if necessary
            var tracksToRemove = tracks.Where(track => 
            {
                var pf = track.Tracker;
                var entropy = pf.CalculateEntropy(p => p.State.ToArray());

                if (track.Tag.InitialEntropy.HasValue == false)
                    track.Tag = new TrackTag { InitialEntropy = entropy };

                var initialEntropy = track.Tag.InitialEntropy;

                if (entropy <= initialEntropy * (1 - MIN_ENTROPY_DECREASE))
                {
                    track.IsTentative = false;
                    return false;
                }

                if (entropy >= initialEntropy * (1 + MAX_ENTROPY_INCREASE))
                {
                    return true; //select for removal
                }

                return false;
            });

            tracks.Remove(tracksToRemove); 
        }

        private void initialize()
        {
            string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            var area = new Rectangle(0, 0, debugImg.Width, debugImg.Height);
            area.Inflate(-10, -10);

            trajectories = TrajectoryData.Load(resourceDir);

            TrajectoryData.Normalize(trajectories, area);
            TrajectoryData.Sync(trajectories);
        }

        #region GUI

        public JPDAF_PF_DemoForm()
        {
            InitializeComponent();
            initialize();
           
            Application.Idle += Application_Idle;
        }

        int stepIdx = 0;

        System.Drawing.Font font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
        void Application_Idle(object sender, EventArgs e)
        {
            debugImg.Clear();

            var measurements = new List<PointF>();
            foreach (var t in trajectories)
            {
                if (stepIdx >= t.Count)
                    continue;

                var tData = t[stepIdx];

                if (tData == null) continue;

                measurements.Add(new PointF(tData.X, tData.Y));
            }
            stepIdx++;

            var elapsedMs = Accord.Extensions.Diagnostics.MeasureTime(() =>
            {
                predict();
                update(measurements);
            });
            Console.WriteLine("Elapsed ms: {0}", elapsedMs);

            foreach (var m in measurements)
            {
                var circles = measurements.Select(c => new CircleF(c.X, c.Y, 5));
                debugImg.Draw(circles, Bgr8.Red, 3);
            }

            foreach (var track in tracks)
            {
                var pf = track.Tracker;

                var circles = pf.Select(x => x.State.Position).Select(y => new CircleF(y.X, y.Y, 3));
                debugImg.Draw(circles, Bgr8.Green, 1);

                var text = "ID: " + track.ID + (track.IsTentative ? "-tentative" : "");

                var annWidth = TextRenderer.MeasureText(text, font);
                debugImg.DrawAnnotation(Rectangle.Round(pf.BoundingBox()), text, annWidth.Width);
            }

            pictureBox.Image = debugImg.ToBitmap();
        }

        #endregion
    }
}
