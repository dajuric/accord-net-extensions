using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Statistics.Filters;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Accord.Extensions.Vision;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using PointF = AForge.Point;
using State = Accord.Extensions.Statistics.Filters.ConstantVelocity2DModel;
using Track = JPDAF_Demo.Track<Accord.Extensions.Statistics.Filters.DiscreteKalmanFilter<Accord.Extensions.Statistics.Filters.ConstantVelocity2DModel, AForge.Point>, JPDAF_Demo.TrackTag>;
using Tracker = Accord.Extensions.Statistics.Filters.DiscreteKalmanFilter<Accord.Extensions.Statistics.Filters.ConstantVelocity2DModel, AForge.Point>;

namespace JPDAF_Demo
{
    public partial class JPDAF_Kalman_DemoForm : Form
    {
        public const float MAX_ENTROPY_INCREASE = 0.15f;
        public const float MIN_ENTROPY_DECREASE = 0.3f;
       
        List<Track> tracks = new List<Track>();

        private Track createTrack(PointF location)
        {
            var measurementDimension = 2; //just coordinates

            var measurementNoise = 5d;
            var processNoise = 10d;
            var dt = 0.66;

            var procNoiseCov = State.GetProcessNoise(processNoise, dt); 
            var initialState = new State { Position = location, Velocity = default(PointF) };
            var initialErrorCovariance = new double[] 
                                        { 
                                            measurementNoise, processNoise, measurementNoise, processNoise
                                        }
                                        .ToDiagonalMatrix();
            
            var kalman = new DiscreteKalmanFilter<State, PointF>(initialState, initialErrorCovariance,
                                                                 measurementDimension /*(position)*/, 0 /*no control*/,
                                                                 x => State.ToArray(x), x => State.FromArray(x), m => new double[] { m.X, m.Y });

            kalman.ProcessNoise = procNoiseCov;
            kalman.MeasurementNoise = Matrix.Diagonal<double>(kalman.MeasurementVectorDimension, Math.Pow(measurementNoise, 2));

            kalman.MeasurementMatrix = State.GetPositionMeasurementMatrix();
            kalman.TransitionMatrix = State.GetTransitionMatrix(timeInterval: dt);

            return new Track(kalman);
        }

        private void predict()
        {
            var kalmanFilters = tracks.Select(x => x.Tracker);
            
            foreach (var kalmanFilter in kalmanFilters)
            {
                kalmanFilter.Predict();
            }
        }

        private void update(List<PointF> measurements)
        {
            var kalmanFilters = tracks.Select(x => x.Tracker).ToList();
        
            //update filters
            var assocProbs = kalmanFilters.Update<Tracker, State, PointF>(measurements); 
            
            //add if necessary
            kalmanFilters.AddFilters<Tracker, State, PointF>(assocProbs, (measurementIdx) =>  
            {
                var track = createTrack(measurements[measurementIdx]);
                tracks.Add(track);
            });

            //remove if necessary
            var tracksToRemove = tracks.Where(track =>
            {
                var kf = track.Tracker;
                var entropy = kf.CalculateEntropy();

                if (track.Tag.InitialEntropy.HasValue == false)
                    track.Tag = new TrackTag { InitialEntropy = entropy };

                var initialEntropy = track.Tag.InitialEntropy;

                if (entropy <= (initialEntropy - Math.Abs((double)initialEntropy) * MIN_ENTROPY_DECREASE))
                {
                    track.IsTentative = false;
                    return false;
                }

                if (entropy >= (initialEntropy + Math.Abs((double)initialEntropy) * MAX_ENTROPY_INCREASE))
                {
                    return true; //select for removal
                }

                return false;
            })
            .ToList();

            tracks.Remove(tracksToRemove); 
        }

        #region GUI
        StreamableSource capture = null;
        Database annDatabase = null;

        public JPDAF_Kalman_DemoForm()
        {
            capture = new ImageDirectoryReader(@"S:\Svjetla - baza podataka - Boris\Prednja + stražnja\1\", "*.jpg");
            //capture = new FileCapture(@"S:\Svjetla - baza podataka - Boris\Prednja + stražnja\1.mxf");
            capture.Seek(183, SeekOrigin.Begin);
            capture.Open();

            annDatabase = new Database();
            annDatabase.Load(@"S:\Svjetla - baza podataka - Boris\Prednja + stražnja\1.xml");
            //string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            //annDatabase.Load(Path.Combine(resourceDir, "trajectory.xml"));

            InitializeComponent();
           
            Application.Idle += Application_Idle;
        }

        Image<Bgr, byte> debugImg = null;
        System.Drawing.Font font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
        void Application_Idle(object sender, EventArgs e)
        {
            if (capture.Position == capture.Length) 
                return;

            var frameKey = new FileInfo((capture as ImageDirectoryReader).CurrentImageName).Name;
            //var frameKey = capture.Position.ToString();

            List<Rectangle> rects = new List<Rectangle>();

            if (annDatabase.ContainsKey(frameKey))
            {
                List<Annotation> imgAnnotations = annDatabase[frameKey];
                rects = imgAnnotations/*.Where(x => x.Label.Contains("Car3"))*/.Select(ann => ann.Polygon.BoundingRect()).ToList();
            }

            if (capture.Position == 187)
                rects.Add(new Rectangle(739, 145, 69, 32));

            debugImg = capture.ReadAs<Bgr, byte>();
        
            List<PointF> measurements = new List<PointF>();
            var elapsedMs = Accord.Extensions.Diagnostics.MeasureTime(() => 
            {
                predict();

                measurements = rects.Select(x => ((RectangleF)x).Center()).ToList();
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
                //draw Kalman error cov. matrix (0.99 confidence interval)
                var ellipse = track.Tracker.GetEllipse(f => f.Position);
                debugImg.Draw((Box2D)ellipse, Bgr8.Green, 1);

                //draw annotation
                var text = "ID: " + track.ID; //+ (track.IsTentative ? "-tentative" : "");

                var annWidth = TextRenderer.MeasureText(text, font);
                var rect = new Box2D(track.Tracker.State.Position, new SizeF(5, 5), 0).GetMinArea();
                debugImg.DrawAnnotation(Rectangle.Round(rect), text, annWidth.Width);
            }

            pictureBox.Image = debugImg.ToBitmap();
            Thread.Sleep(1500);
        }

        #endregion

        private void JPDAF_Kalman_DemoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (capture != null)
                capture.Dispose();
        }
    }
}
