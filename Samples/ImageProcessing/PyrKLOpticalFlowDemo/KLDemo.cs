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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

using FlowColor = Accord.Extensions.Imaging.Gray<float>;
using System.IO;

namespace PyrKLOpticalFlowDemo
{
    public partial class KLDemo : Form
    {
        Size imgSize = new Size(320, 240);

        ImageStreamReader videoCapture;
        PyrLKStorage<FlowColor> lkStorage;
        int winSize = 15;

        private void processImage(FlowColor[,] prevIm, FlowColor[,] currIm, List<PointF> oldPositions, out List<PointF> newPositions)
        {
            lkStorage.Process(prevIm, currIm);

            PointF[] currFeatures;
            KLTFeatureStatus[] featureStatus;
            /*LKOpticalFlow<FlowColor>.EstimateFlow(lkStorage, oldPositions.ToArray(), 
                                                  out currFeatures, out featureStatus,
                                                  winSize);*/


            PyrLKOpticalFlow<FlowColor>.EstimateFlow(lkStorage, oldPositions.ToArray(),
                                                     out currFeatures, out featureStatus, 
                                                     winSize);

            newPositions = new List<PointF>();
            for (int i = 0; i < currFeatures.Length; i++)
            {
                if (featureStatus[i] == KLTFeatureStatus.Success)
                    newPositions.Add(currFeatures[i]);

                Console.WriteLine(featureStatus[i]);
            }
        }

        #region User interface...

        public KLDemo()
        {
            InitializeComponent();

            lkStorage = new PyrLKStorage<FlowColor>(pyrLevels: 1);
            
            try
            {
#if FILE_CAPTURE
                string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
                videoCapture = new ImageDirectoryReader(Path.Combine(resourceDir, "ImageSequence"), "*.jpg");

                videoCapture.ReadTo(ref frame);
                prevIm = frame.ToGray().Cast<float>();
                oldPositions = prevIm.
                              GoodFeaturesToTrack(winSize, 0.05f)
                              .Select(x => new PointF(x.X, x.Y)).Take(100).ToList();
#else
                videoCapture = new CameraCapture(0);
                oldPositions = new List<PointF>();
                prevIm = new Image<FlowColor, float>(imgSize);
#endif
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            if(videoCapture is CameraCapture)
                (videoCapture as CameraCapture).FrameSize = imgSize; 

            this.FormClosing += CamshiftDemo_FormClosing;
            Application.Idle += videoCapture_NewFrame;
            videoCapture.Open();
        }

        FlowColor[,] prevIm = null;
        List<PointF> oldPositions = null;

        Accord.Extensions.Imaging.Font font = Accord.Extensions.Imaging.Font.Normal; 
        Bgr<byte>[,] frame = null;
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            videoCapture.ReadTo(ref frame);
            if (frame == null)
                return;

            var im = frame.ToGray().Cast<float>();

            long start = DateTime.Now.Ticks;
            
            List<PointF> newPositions;
            processImage(prevIm, im, this.oldPositions, out newPositions);
            
            prevIm = im;
            oldPositions = newPositions;

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + elapsedMs + " ms", font, new Point(15, 20), Bgr<byte>.Green);
            drawPoints(frame, newPositions);
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared) 24bpp color
            GC.Collect();
        }

        void CamshiftDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null) 
                videoCapture.Dispose();
        }

        private void drawPoints(Bgr<byte>[,] im, List<PointF> points)
        {
            foreach (var pt in points)
            {                
                var rect = new RectangleF(pt.X, pt.Y, 1, 1);
                rect.Inflate(winSize / 2, winSize / 2);

                im.Draw(rect, Bgr<byte>.Red, 2);
            }
        }

        #endregion

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            lock (oldPositions)
            {
                oldPositions.Add(new PointF(e.Location.X, e.Location.Y));
            }
        }

        private void KLDemo_KeyPress(object sender, KeyPressEventArgs e)
        {
            lock (oldPositions)
            {
                if (e.KeyChar == 'r')
                    oldPositions = prevIm.
                                   GoodFeaturesToTrack(winSize, 0.05f)
                                   .Select(x => new PointF(x.X, x.Y)).Take(100).ToList();
            }
        }
    }
}
