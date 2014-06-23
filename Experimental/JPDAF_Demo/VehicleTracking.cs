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
using State = Accord.Extensions.Statistics.Filters.PerspectiveProjectionCoordinateTransform;
using Track = JPDAF_Demo.Track<Accord.Extensions.Statistics.Filters.DiscreteKalmanFilter<Accord.Extensions.Statistics.Filters.PerspectiveProjectionCoordinateTransform, JPDAF_Demo.VehicleTracking.Measurement>, JPDAF_Demo.TrackTag>;
using Tracker = Accord.Extensions.Statistics.Filters.DiscreteKalmanFilter<Accord.Extensions.Statistics.Filters.PerspectiveProjectionCoordinateTransform, JPDAF_Demo.VehicleTracking.Measurement>;

namespace JPDAF_Demo
{
    public partial class VehicleTracking : Form
    {
        public struct Measurement
        {
            public static void Initialize(Size imageSize)
            {
                ImageSize = imageSize;
                TranslationFactor = new PointF(-ImageSize.Width / 2 + 100, /*-ImageSize.Height / 2*/ -400);
            }

            public static Size ImageSize { get; private set; }
            public static PointF TranslationFactor { get; private set; }

            public double X;
            public double Y;
            public double ObjectWidth;

            public static explicit operator Measurement(Rectangle rect)
            {
                var pt = rect.Location;//rect.Center();

                return new Measurement 
                {
                    X = pt.X,
                    Y = pt.Y,
                    ObjectWidth = rect.Width
                };
            }

            public Measurement Translate(PointF offset)
            {
                return new Measurement
                {
                    X = this.X + offset.X,
                    Y = this.Y + offset.Y,
                    ObjectWidth = this.ObjectWidth
                };
            }

            public Measurement Translate()
            {
                return Translate(TranslationFactor);
            }

            public Measurement TranslateBack()
            {
                return Translate(TranslationFactor.Negate());
            }
        }

        public const float MAX_ENTROPY_INCREASE = 0.55f;
        public const float MIN_ENTROPY_DECREASE = 0.15f;

        double dt = 0.33;
        double velocityMultiplier = 0.00000433 * 2.75;

        List<Track> tracks = new List<Track>();

        private Track createTrack(Measurement measurement)
        {
            var measurementDimension = 3; 

            var procNoiseCov = new double[]
            {
              //x, y, width, velocity
                6, 6, 6,     8
            }
            .ElementwisePower(2).ToDiagonalMatrix();


            var initialState = new State { X = measurement.X, Y = measurement.Y, ObjectWidth = measurement.ObjectWidth, Velocity = -(30 + 35) };
            
            var kalman = new DiscreteKalmanFilter<State, Measurement>(initialState, procNoiseCov,
                                                                      measurementDimension /*(position)*/, 0 /*no control*/,
                                                                      x => State.ToArray(x), x => State.FromArray(x), m => new double[] { m.X, m.Y, m.ObjectWidth });

            kalman.ProcessNoise = procNoiseCov;
            kalman.MeasurementNoise = Matrix.Diagonal<double>(kalman.MeasurementVectorDimension, Math.Pow(5, 2));

            kalman.MeasurementMatrix = State.GetMeasurementMatrix();
            kalman.TransitionMatrix = initialState.EstimateTransitionMatrix(velocityMultiplier, dt);

            return new Track(kalman);
        }

        private void predict()
        {
            var kalmanFilters = tracks.Select(x => x.Tracker);
            foreach (var kalmanFilter in kalmanFilters)
            {
                kalmanFilter.TransitionMatrix = kalmanFilter
                                                .State
                                                .EstimateTransitionMatrix(velocityMultiplier, dt);
                kalmanFilter.Predict();
            }
        }

        private void update(List<Measurement> measurements)
        {
            /*if (tracks.Count > 0)
            {
                measurements = new List<Measurement>();
            }*/

            var kalmanFilters = tracks.Select(x => x.Tracker).ToList();
        
            //update filters
            measurements = measurements.Select(x => (Measurement)x).ToList();
            var assocProbs = kalmanFilters.Update<Tracker, State, Measurement>(measurements); 
            
            //add if necessary
            kalmanFilters.AddFilters<Tracker, State, Measurement>(assocProbs, (measurementIdx) =>  
            { 
                var track = createTrack(measurements[measurementIdx]);
                tracks.Add(track);
            });

            //remove if necessary
            var tracksToRemove = tracks.Where(track =>
            {
                var translatedState = track.Tracker.State.Translate(Measurement.TranslationFactor.Negate());
                if (translatedState.X < 0 || translatedState.X >= Measurement.ImageSize.Width ||
                    translatedState.Y < 0 || translatedState.Y >= Measurement.ImageSize.Height)
                {
                    return true;
                }

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

        public VehicleTracking()
        {
            capture = new ImageDirectoryReader(@"S:\Svjetla - baza podataka - Boris\Prednja + stražnja\1\", "*.jpg");
            //capture = new FileCapture(@"S:\Svjetla - baza podataka - Boris\Prednja + stražnja\1.mxf");
            //capture.Seek(6740, SeekOrigin.Begin);
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

            debugImg = capture.ReadAs<Bgr, byte>();
            Measurement.Initialize(debugImg.Size);

            var measurements = new List<Measurement>();

            if (annDatabase.ContainsKey(frameKey))
            {
                List<Annotation> imgAnnotations = annDatabase[frameKey];
                var rects = imgAnnotations/*.Where(x => x.Label.Contains("Car3"))*/.Select(ann => ann.Polygon.BoundingRect());
                measurements = rects.Select(x => ((Measurement)x).Translate()).ToList();
            }

            var elapsedMs = Accord.Extensions.Diagnostics.MeasureTime(() => 
            {
                predict();
                update(measurements);
            });
            Console.WriteLine("Elapsed ms: {0}", elapsedMs);
           
            foreach (var m in measurements)
            {
                var circles = measurements.Select(x=> x.TranslateBack()).Select(c => new CircleF((float)c.X, (float)c.Y, 5));
                debugImg.Draw(circles, Bgr8.Red, 3);
            }

            foreach (var track in tracks)
            {
                var restoredCoordinate = track.Tracker.State.Translate(Measurement.TranslationFactor.Negate()).GetCoordinate();

                //draw Kalman error cov. matrix (0.99 confidence interval)
                var ellipse = track.Tracker.GetEllipse(_ => restoredCoordinate, 0.99, new double[,] 
                {
                    {1, 0, 0, 0},
                    {0, 1, 0, 0}
                });
                debugImg.Draw((Box2D)ellipse, Bgr8.Green, 1);

                //draw annotation
                var text = "ID: " + track.ID + (track.IsTentative ? "-tentative" : "");

                var annWidth = TextRenderer.MeasureText(text, font);
                var rect = new Box2D(restoredCoordinate, new SizeF(5, 5), 0).GetMinArea();
                debugImg.DrawAnnotation(Rectangle.Round(rect), text, annWidth.Width);
            }

            pictureBox.Image = debugImg.ToBitmap();
            //Thread.Sleep(100);
        }

        #endregion

        private void JPDAF_Kalman_DemoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (capture != null)
                capture.Dispose();
        }
    }
}
