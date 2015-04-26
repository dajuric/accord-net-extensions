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
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using AForge;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using System.IO;

namespace Accord.Extensions.Imaging
{
    public partial class CamshiftDemo : Form
    {
        ImageStreamReader videoCapture;
        DenseHistogram originalObjHist, backgroundHist;

        private void init()
        {
            int[] binSizes = new int[] { 64, 64 }; //X bins per channel

            IntRange[] ranges = new IntRange[] 
            { 
                new IntRange(0, 180), //Hue (for 8bpp image hue range is (0-180) otherwise (0-360)
                new IntRange(0, 255)
            };

            originalObjHist = new DenseHistogram(binSizes, ranges);
            backgroundHist = originalObjHist.CopyBlank();
        }

        DenseHistogram ratioHist = null;
        private void initTracking(Bgr<byte>[,] frame)
        {
            //get hue channel from search area
            var hsvImg = frame.ToHsv();
            //user constraints...
            Gray<byte>[,] mask = hsvImg.InRange(new Hsv<byte>(0, 0, (byte)minV), new Hsv<byte>(0, 0, (byte)maxV), Byte.MaxValue, 2);

            originalObjHist.Calculate(hsvImg.SplitChannels<Hsv<byte>, byte>(roi, 0, 1), !false, mask, roi.Location);
            originalObjHist.Scale((float)1 / roi.Area());
            //originalObjHist.Normalize(Byte.MaxValue);

            var backgroundArea = roi.Inflate(1.5, 1.5, frame.Size());
            backgroundHist.Calculate(hsvImg.SplitChannels<Hsv<byte>, byte>(backgroundArea, 0, 1), !false, mask, backgroundArea.Location);
            backgroundHist.Scale((float)1 / backgroundArea.Area());
            //backgroundHist.Normalize(Byte.MaxValue);
            
            //how good originalObjHist and objHist match (suppresses possible selected background)
            ratioHist = originalObjHist.CreateRatioHistogram(backgroundHist, Byte.MaxValue, 10);

            searchArea = roi;
            roi = Rectangle.Empty;
        }

        Rectangle searchArea;
        private void processImage(Bgr<byte>[,] frame, out Gray<byte>[,] probabilityMap, out Rectangle prevSearchArea, out Box2D foundBox)
        {
            prevSearchArea = searchArea;

            //convert to HSV
            var hsvImg = frame.ToHsv(); 
            //back-project ratio hist => create probability map
            probabilityMap = ratioHist.BackProject(hsvImg.SplitChannels<Hsv<byte>, byte>(0, 1)); //or new Image<Gray<byte>>[]{ hsvImg[0], hsvImg[1]...} 

            //user constraints...
            Gray<byte>[,] mask = hsvImg.InRange(new Hsv<byte>(0, 0, (byte)minV), new Hsv<byte>(0, 0, (byte)maxV), Byte.MaxValue, 2);
            probabilityMap.AndByte(mask, inPlace:true);

            //run Camshift algorithm to find new object position, size and angle
            foundBox = Camshift.Process(probabilityMap, searchArea); 
            var foundArea = Rectangle.Round(foundBox.GetMinArea());

            searchArea = foundArea.Inflate(0.05, 0.05, frame.Size()); //inflate found area for search (X factor)...
            if (searchArea.IsEmpty) isROISelected = false; //reset tracking
        }

        #region User interface...

        public CamshiftDemo()
        {
            InitializeComponent();
            bar_ValueChanged(null, null); //write values to variables
            init(); //create histograms

            try
            {
#if FILE_CAPTURE
                string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
                videoCapture = new ImageDirectoryReader(Path.Combine(resourceDir, "ImageSequence"), "*.jpg");
                roi = new Rectangle(80, 130, 50, 70); isROISelected = true;
                this.barVMin.Value = 100;
#else
                videoCapture = new CameraCapture(0);
#endif
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            if(videoCapture is CameraCapture)
                (videoCapture as CameraCapture).FrameSize = new Size(640, 480); //set new Size(0,0) for the lowest one

            this.FormClosing += CamshiftDemo_FormClosing;
            Application.Idle += videoCapture_InitFrame;
            videoCapture.Open();
        }

        Bgr<byte>[,] frame;
        void videoCapture_InitFrame(object sender, EventArgs e)
        {
            videoCapture.ReadTo(ref frame);
            if (frame == null) return;

            if (isROISelected)
            { 
                initTracking(frame);
                Application.Idle -= videoCapture_InitFrame;
                Application.Idle += videoCapture_NewFrame;
                return;
            }
            else
            {
                frame.Draw(roi, Bgr<byte>.Red, 3); 
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

            Gray<byte>[,] probabilityMap;
            Rectangle prevSearchArea;
            Box2D foundBox;
            processImage(frame, out probabilityMap, out prevSearchArea, out foundBox);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + elapsedMs + " ms", font, new Point(25, 20), Bgr<byte>.Green);
            frame.Draw(prevSearchArea, Bgr<byte>.Red, 3);
            frame.Draw(foundBox, Bgr<byte>.Green, 5); Console.WriteLine("angle: " + foundBox.Angle);
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared) 24bpp color
            this.pbProbabilityImage.Image = probabilityMap.ToBitmap(); //it will be just casted (data is shared) 8bpp gray

            GC.Collect();
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

            var ptSecond = e.Location;

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
