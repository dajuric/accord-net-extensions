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

#define FILE_CAPTURE //comment it to enable camera capture

using System;
using System.IO;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Moments;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Statistics.Filters;
using Accord.Extensions.Imaging;
using Accord.Math;
using AForge;
using ModelState = Accord.Extensions.Statistics.Filters.ConstantVelocity2DModel;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace KalmanObjectTracking
{
    public partial class KalmanTrackingDemo : Form
    {
        ImageStreamReader videoCapture;
        DenseHistogram originalObjHist, backgroundHist;
        KalmanFilter<ModelState, PointF> kalman;

        private void initalizeHistograms()
        { 
            int[] binSizes = new int[] { 32, 32 }; //X bins per channel

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

            var initialState = new ModelState { Position = startPoint, Velocity = new PointF(0.2f, -1.5f)};
            var initialStateError = ModelState.GetProcessNoise(10);

            kalman = new DiscreteKalmanFilter<ModelState, PointF>(initialState, initialStateError, 
                                                                  measurementDimension /*(position)*/, 0 /*no control*/,
                                                                  x => ModelState.ToArray(x), x => ModelState.FromArray(x), x => new double[] { x.X, x.Y });

            kalman.ProcessNoise = ModelState.GetProcessNoise(1);
            kalman.MeasurementNoise = Matrix.Diagonal<double>(kalman.MeasurementVectorDimension, 10000.0);

            kalman.MeasurementMatrix = ModelState.GetPositionMeasurementMatrix();
            kalman.TransitionMatrix = ModelState.GetTransitionMatrix();
        }

        DenseHistogram ratioHist = null;
        private void initTracking(Bgr<byte>[,] frame)
        {
            initializeKalman(roi.Center());

            //get hue channel from search area
            var hsvImg = frame.ToHsv();
            //user constraints...

            Gray<byte>[,] mask = hsvImg.InRange(new Hsv<byte>(0, 0, (byte)minV), new Hsv<byte>(0, 0, (byte)maxV), Byte.MaxValue, 2);

            originalObjHist.Calculate(hsvImg.SplitChannels<Hsv<byte>, byte>(roi, 0, 1), false, mask, roi.Location);
            originalObjHist.Scale((float)1 / roi.Area());
            //originalObjHist.Normalize(Byte.MaxValue);

            var backgroundArea = roi.Inflate(1.5, 1.5, frame.Size());
            var backgroundMask = mask.Clone(backgroundArea);
            backgroundMask.SetValue<Gray<byte>>(0, new Rectangle(Point.Subtract(roi.Location, backgroundArea.Location), roi.Size));

            backgroundHist.Calculate(hsvImg.SplitChannels<Hsv<byte>, byte>(backgroundArea, 0, 1), false, mask, backgroundArea.Location);
            backgroundHist.Scale((float)1 / (backgroundArea.Area() - roi.Area()));
            //backgroundHist.Normalize(Byte.MaxValue);
            
            //how good originalObjHist and objHist match (suppresses possible selected background)
            ratioHist = originalObjHist.CreateRatioHistogram(backgroundHist, Byte.MaxValue, 3);

            searchArea = roi;
            roi = Rectangle.Empty;
        }

        Rectangle searchArea;
        int nonVisibleCount = 0;
        private void trackOneStep(Bgr<byte>[,] frame, out Gray<byte>[,] probabilityMap, out Box2D foundBox)
        {
            const float SEARCH_AREA_INFLATE_FACTOR = 0.05f;

            /**************************** KALMAN predict **************************/
            kalman.Predict(); 
            searchArea = createRect(kalman.State.Position, searchArea.Size, frame.Size());
            /**************************** KALMAN predict **************************/

            trackCamshift(frame, searchArea, out probabilityMap, out foundBox);

            if (!foundBox.IsEmpty)
            {
                /**************************** KALMAN correct **************************/
                kalman.Correct(new PointF(foundBox.Center.X, foundBox.Center.Y)); //correct predicted state by measurement
                /**************************** KALMAN correct **************************/
            
                var foundArea = Rectangle.Round(foundBox.GetMinArea());
                searchArea = foundArea.Inflate(SEARCH_AREA_INFLATE_FACTOR, SEARCH_AREA_INFLATE_FACTOR, frame.Size()); //inflate found area for search (X factor)...
                nonVisibleCount = 0;
            }
            else
            {
                nonVisibleCount++;
                if (nonVisibleCount == 1) //for the first time 
                {
                    searchArea = searchArea.Inflate(-SEARCH_AREA_INFLATE_FACTOR * 1.5, -SEARCH_AREA_INFLATE_FACTOR * 1.5, frame.Size()); //shrink (hysteresis)
                }

                searchArea = createRect(kalman.State.Position, searchArea.Size, frame.Size()); 
            }

            if (nonVisibleCount > 100) //if not visible for a longer time => reset tracking
            {
                nonVisibleCount = 0;
                isROISelected = false;
            }
        }

        private void trackCamshift(Bgr<byte>[,] frame, Rectangle searchArea, out Gray<byte>[,] probabilityMap, out Box2D foundBox)
        {
            const int PROBABILITY_MIN_VAL = (int)(0.3f * Byte.MaxValue);

            //convert to HSV
            var hsvImg = frame.ToHsv(); 
            //back-project ratio hist => create probability map
            probabilityMap = ratioHist.BackProject(hsvImg.SplitChannels<Hsv<byte>, byte>(0, 1)); //or new Image<Gray<byte>>[]{ hsvImg[0], hsvImg[1]...} 

            //user constraints...
            Gray<byte>[,] mask = hsvImg.InRange(new Hsv<byte>(0, 0, (byte)minV), new Hsv<byte>(0, 0, (byte)maxV), Byte.MaxValue, 2);
            probabilityMap.AndByte(mask, inPlace: true);

            //run Camshift algorithm to find new object position, size and angle
            CentralMoments centralMoments;
            foundBox = Camshift.Process(probabilityMap, searchArea, Meanshift.DEFAULT_TERM, out centralMoments); 

             //stopping conditions
            float avgIntensity = centralMoments.Mu00 / (foundBox.Size.Area() + Single.Epsilon);
            if (avgIntensity < PROBABILITY_MIN_VAL || foundBox.Size.IsEmpty || foundBox.Size.Height < 12)
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

#if FILE_CAPTURE
            roi = new Rectangle(115, 220, 30, 15); //user defined rectangle for sample video
            isROISelected = true;
#endif
            bar_ValueChanged(null, null); //write values to variables
            initalizeHistograms(); //create histograms

            try
            {
#if FILE_CAPTURE
                string videoDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources", "Sequence");
                videoCapture = new ImageDirectoryReader(videoDir, "*.jpg");
#else
                videoCapture = new CameraCapture(0);
#endif
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            this.FormClosing += CamshiftDemo_FormClosing;
            Application.Idle += videoCapture_InitFrame;
            videoCapture.Open();
        }

        Bgr<byte>[,] frame = null;
        void videoCapture_InitFrame(object sender, EventArgs e)
        {
            videoCapture.ReadTo(ref frame);
            if (frame == null)
                return;

            videoCapture.Seek(-1, SeekOrigin.Current);

            if (isROISelected)
            { 
                initTracking(frame);
                Application.Idle -= videoCapture_InitFrame;
                Application.Idle += videoCapture_NewFrame;
            }
            else
            {
                frame.Draw(roi, Bgr<byte>.Blue, 3); 
            }
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared)

            GC.Collect();
        }

        Accord.Extensions.Imaging.Font font = Accord.Extensions.Imaging.Font.Normal; 
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            videoCapture.ReadTo(ref frame);
            if (frame == null)
                return;

            if (!isROISelected)
            {
                Application.Idle += videoCapture_InitFrame;
                Application.Idle -= videoCapture_NewFrame;
                return;
            }

            long start = DateTime.Now.Ticks;

            Rectangle prevSearchArea = searchArea; //processImage overwrites searchArea
            bool isPredicted = nonVisibleCount > 0;

            Gray<byte>[,] probabilityMap;
            Box2D foundBox;
            trackOneStep(frame, out probabilityMap, out foundBox);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + elapsedMs + " ms", font, new Point(25, 20), Bgr<byte>.Green);
            frame.Draw(prevSearchArea, Bgr<byte>.Blue, 3);
            frame.Draw(foundBox, Bgr<byte>.Green, 5); Console.WriteLine("angle: " + foundBox.Angle);
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared) 24bpp color
            this.pbProbabilityImage.Image = probabilityMap.ToBitmap(); //it will be just casted (data is shared) 8bpp gray

            GC.Collect();
            System.Threading.Thread.Sleep(25);
        }
    
        void CamshiftDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null) 
                videoCapture.Dispose();
        }

        Rectangle roi = Rectangle.Empty;
        bool isROISelected = false;
        Point ptFirst;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            ptFirst = e.Location.ToPt();
            isROISelected = false;
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            roi.Intersect(new Rectangle(new Point(), frame.Size()));
            isROISelected = true;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Point ptSecond = e.Location.ToPt();

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
