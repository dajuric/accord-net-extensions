using Accord.Imaging;
using Accord.Imaging.Moments;
using Accord.Math;
using Accord.Math.Geometry;
using Accord.Statistics.Filters;
using Accord.Vision;
using AForge;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ModelState = Accord.Statistics.Filters.ConstantVelocity2DModel;
using Point = System.Drawing.Point;
using PointF = AForge.Point;

namespace KalmanObjectTracking
{
    public partial class KalmanTrackingDemo : Form
    {
        CaptureBase videoCapture;
        DenseHistogram originalObjHist, backgroundHist;
        KalmanFilter<ModelState, PointF> kalman;

        private void initalizeHistograms()
        {
            int[] binSizes = new int[] { 64, 64 }; //X bins per channel

            IntRange[] ranges = new IntRange[] 
            { 
                new IntRange(0, 180), //Hue (for 8bpp image hue>> range is (0-180) otherwise (0-360)
                new IntRange(0, 255)
            };

            originalObjHist = new DenseHistogram(binSizes, ranges);
            backgroundHist = originalObjHist.CopyBlank();
        }

        private void initializeKalman(PointF startPoint)
        {
            var measurementDimension = 2; //just coordinates

            var initialState = new ModelState { Position = startPoint, Velocity = new PointF()};
            var initialStateError = ModelState.GetProcessNoise(0, 0);

            kalman = new DiscreteKalmanFilter<ModelState, PointF>(initialState, initialStateError, 
                                                                  measurementDimension /*(position)*/, 0 /*no control*/,
                                                                  x => ModelState.ToArray(x), x => ModelState.FromArray(x), x => new double[] { x.X, x.Y });

            kalman.ProcessNoise = ModelState.GetProcessNoise(5, 10);
            kalman.MeasurementNoise = Matrix.Diagonal<double>(kalman.MeasurementVectorDimension, 5);

            kalman.MeasurementMatrix = new double[,] //just pick point coordinates for an observation [2 x 4] (look at ConstantVelocity2DModel)
                { 
                   //X,  vX, Y,  vY (look at ConstantVelocity2DModel)
                    {1,  0,  0,  0}, //picks X
                    {0,  0,  1,  0}  //picks Y
                };

            kalman.TransitionMatrix = ModelState.GetTransitionMatrix();
        }

        DenseHistogram ratioHist = null;
        private void initTracking(Image<Bgr, byte> frame)
        {
            initializeKalman(roi.Center());

            //get hue channel from search area
            var hsvImg = frame.Convert<Hsv, byte>(); //<<parallel operation>>
            //user constraints...

            Image<Gray, byte> mask = hsvImg.InRange(new Hsv(0, 0, minV), new Hsv(0, 0, maxV), Byte.MaxValue, 2);

            originalObjHist.Calculate(hsvImg.GetSubRect(roi).SplitChannels(0, 1), false, mask.GetSubRect(roi));
            originalObjHist.Scale((float)1 / roi.Area());
            //originalObjHist.Normalize(Byte.MaxValue);

            var backgroundArea = roi.Inflate(1.5, 1.5, frame.Size);
            backgroundHist.Calculate(hsvImg.GetSubRect(backgroundArea).SplitChannels(0, 1), false, mask.GetSubRect(backgroundArea));
            backgroundHist.Scale((float)1 / backgroundArea.Area());
            //backgroundHist.Normalize(Byte.MaxValue);
            
            //how good originalObjHist and objHist match (suppresses possible selected background)
            ratioHist = originalObjHist.CreateRatioHistogram(backgroundHist, Byte.MaxValue, 10);

            searchArea = roi;
            roi = Rectangle.Empty;
        }

        Rectangle searchArea;
        int nonVisibleCount = 0;
        private void trackOneStep(Image<Bgr, byte> frame, out Image<Gray, byte> probabilityMap, out Box2D foundBox)
        {
            const float SEARCH_AREA_INFLATE_FACTOR = 0.05f;

            /**************************** KALMAN predict **************************/
            kalman.Predict(); 
            searchArea = createRect(kalman.State.Position, searchArea.Size, frame.Size);
            /**************************** KALMAN predict **************************/

            trackCamshift(frame, searchArea, out probabilityMap, out foundBox);

            if (foundBox.IsEmpty ==  false)
            {
                /**************************** KALMAN correct **************************/
                kalman.Correct(new PointF(foundBox.Center.X, foundBox.Center.Y)); //correct predicted state by measurement
                /**************************** KALMAN correct **************************/

                var foundArea = Rectangle.Round(foundBox.GetMinArea());
                searchArea = foundArea.Inflate(SEARCH_AREA_INFLATE_FACTOR, SEARCH_AREA_INFLATE_FACTOR, frame.Size); //inflate found area for search (X factor)...
                nonVisibleCount = 0;
            }
            else
            {
                nonVisibleCount++;
                if (nonVisibleCount == 1) //for the first time 
                {
                    searchArea = searchArea.Inflate(-SEARCH_AREA_INFLATE_FACTOR * 1.5, -SEARCH_AREA_INFLATE_FACTOR * 1.5, frame.Size); //shrink (hysteresis)
                }

                searchArea = createRect(kalman.State.Position, searchArea.Size, frame.Size); 
            }

            if (nonVisibleCount > 80) //if not visible for a longer time => reset tracking
            {
                nonVisibleCount = 0;
                isROISelected = false;
            }
        }

        private void trackCamshift(Image<Bgr, byte> frame, Rectangle searchArea, out Image<Gray, byte> probabilityMap, out Box2D foundBox)
        {
            const int PROBABILITY_MIN_VAL = (int)(0.5f * 255);

            //convert to HSV
            var hsvImg = frame.Convert<Hsv, byte>(); //<<parallel operation>>
            //back-project ratio hist => create probability map
            probabilityMap = ratioHist.BackProject(hsvImg.SplitChannels(0, 1)); //or new Image<Gray, byte>[]{ hsvImg[0], hsvImg[1]...} //<<parallel operation>>

            //user constraints...
            Image<Gray, byte> mask = hsvImg.InRange(new Hsv(0, 0, minV), new Hsv(0, 0, maxV), Byte.MaxValue, 2);
            probabilityMap.And(mask, inPlace: true);

            //run Camshift algorithm to find new object position, size and angle
            CentralMoments centralMoments;
            foundBox = Camshift.Process(probabilityMap, searchArea, Meanshift.DEFAULT_TERM, out centralMoments); //<<parallel operation>>

             //stopping conditions
            float avgIntensity = centralMoments.Mu00 / (foundBox.Size.Area() + Single.Epsilon);
            if (avgIntensity < PROBABILITY_MIN_VAL || foundBox.Size.IsEmpty)
            {
                foundBox = Box2D.Empty; //invalid box
            }
        }

        private static Rectangle createRect(PointF centerPoint, Size size, Size frameSize)
        {
            return new Rectangle((int)centerPoint.X - size.Width / 2, (int)centerPoint.Y - size.Height / 2, size.Width, size.Height).Intersect(frameSize);
        }

        #region User interface...

        public KalmanTrackingDemo()
        {
            InitializeComponent();

            /********************* MANUAL ROI SELECTION - remove this to select from image (also select camera input) ******************/
            roi = new Rectangle(210, 435, 90, 45); //user defined rectangle for sample video
            isROISelected = true;
            /********************* MANUAL ROI SELECTION - remove this to select from image (also select camera input) ******************/

            bar_ValueChanged(null, null); //write values to variables
            initalizeHistograms(); //create histograms

            try
            {
                string videoDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources", "Sequence");
                videoCapture = new ImageSequenceCapture(videoDir, ".jpg", 30);

                //videoCapture = new Capture(0);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            videoCapture.VideoSize = new Size(640, 480); //set new Size(0,0) for the lowest one

            this.FormClosing += CamshiftDemo_FormClosing;
            Application.Idle += videoCapture_InitFrame;
            videoCapture.Start();
        }

        Image<Bgr, byte> frame;
        void videoCapture_InitFrame(object sender, EventArgs e)
        {
            bool hasNewFrame = videoCapture.WaitForNewFrame(); //do not process the same frame
            if (!hasNewFrame)
                return;

            frame = videoCapture.QueryFrame();

            if (isROISelected)
            { 
                initTracking(frame);
                Application.Idle -= videoCapture_InitFrame;
                Application.Idle += videoCapture_NewFrame;
            }
            else
            {
                frame.Draw(roi, new Bgr(0, 0, 255), 3); 
            }
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared)

            GC.Collect();
        }

        Font font = new Font("Arial", 12);
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            bool hasNewFrame = videoCapture.WaitForNewFrame(); //do not process the same frame
            if (!hasNewFrame)
                return;

            if (!isROISelected)
            {
                Application.Idle += videoCapture_InitFrame;
                Application.Idle -= videoCapture_NewFrame;
                return;
            }

            frame = videoCapture.QueryFrame();

            long start = DateTime.Now.Ticks;

            Rectangle prevSearchArea = searchArea; //processImage overwrites searchArea
            bool isPredicted = nonVisibleCount > 0;

            Image<Gray, byte> probabilityMap;
            Box2D foundBox;
            trackOneStep(frame, out probabilityMap, out foundBox);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + elapsedMs + " ms", font, new System.Drawing.PointF(15, 10), new Bgr(0, 255, 0));
            frame.Draw(prevSearchArea, new Bgr(0, 0, 255), 3);
            frame.Draw(foundBox, new Bgr(0, 255, 0), 5); Console.WriteLine("angle: " + foundBox.Angle);
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared) 24bpp color
            this.pbProbabilityImage.Image = probabilityMap.ToBitmap(); //it will be just casted (data is shared) 8bpp gray

            GC.Collect();
        }
    
        void CamshiftDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null) 
                videoCapture.Stop();
        }

        Rectangle roi = Rectangle.Empty;
        bool isROISelected = false;
        Point ptFirst;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            ptFirst = e.Location;
            isROISelected = false;
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            roi.Intersect(new Rectangle(Point.Empty, frame.Size));
            isROISelected = true;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Point ptSecond = e.Location;

            roi = new Rectangle
            {
                X = System.Math.Min(ptFirst.X, ptSecond.X),
                Y = System.Math.Min(ptFirst.Y, ptSecond.Y),
                Width = System.Math.Abs(ptFirst.X - ptSecond.X),
                Height = System.Math.Abs(ptFirst.Y - ptSecond.Y)
            };
        }

        int minV, maxV;
        private void bar_ValueChanged(object sender, EventArgs e)
        {
            if (barVMin.Value > barVMax.Value)
                barVMax.Value = barVMin.Value;

            minV = barVMin.Value;
            maxV = barVMax.Value;
        }

        #endregion
    }
}
