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

using Accord.Extensions;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Algorithms.LINE2D;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = AForge.IntPoint;
using Template = Accord.Extensions.Imaging.Algorithms.LINE2D.ImageTemplate;
using TemplatePyramid = Accord.Extensions.Imaging.Algorithms.LINE2D.ImageTemplatePyramid<Accord.Extensions.Imaging.Algorithms.LINE2D.ImageTemplate>;

namespace FastTemplateMatchingDemo
{
    public partial class FastTPDemo : Form
    {
        int threshold = 88;
        int minDetectionsPerGroup = 0; //for match grouping (postprocessing)

        ImageStreamReader videoCapture;
        List<TemplatePyramid> templPyrs;

        /// <summary>
        /// Switch between two loading methods. 
        /// Creating a template from files or by deserializing.
        /// </summary>
        private void initialize()
        {
            templPyrs = fromFiles();
            //templPyrs = fromXML();
        }

        List<TemplatePyramid> fromFiles()
        {
            Console.WriteLine("Building templates from files...");

            var list = new List<TemplatePyramid>();

            string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources", "OpenHandLeft_BW");
            string[] files = Directory.GetFiles(resourceDir, "*.bmp");

            object syncObj = new object();
            Parallel.ForEach(files, delegate(string file)
            //foreach(var file in files)
            { 
                Gray<byte>[,] preparedBWImage = ImageIO.LoadGray(file).Clone();

                try
                {
                    var tp = TemplatePyramid.CreatePyramidFromPreparedBWImage(preparedBWImage, new FileInfo(file).Name /*"OpenHand"*/);
                    lock (syncObj)
                    { list.Add(tp); };
                }
                catch (Exception)
                {}
            });

            //XMLTemplateSerializer<ImageTemplatePyramid, ImageTemplate>.Save(list, "C:/bla.xml");
            return list;
        }

        List<TemplatePyramid> fromXML()
        {
            List<TemplatePyramid> list = new List<TemplatePyramid>();

            string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            list = XMLTemplateSerializer<TemplatePyramid, Template>.Load(Path.Combine(resourceDir, "OpenHand_Right.xml")).ToList();
            //list = XMLTemplateSerializer<TemplatePyramid, Template>.Load("C:/bla.xml").ToList();

            return list;
        }

        LinearizedMapPyramid linPyr = null;
        private List<Match> findObjects(Bgr<byte>[,] image, out long preprocessTime, out long matchTime)
        {
            var grayIm = image.ToGray();

            var bestRepresentatives = new List<Match>();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start(); 
            linPyr = LinearizedMapPyramid.CreatePyramid(grayIm); //prepare linear-pyramid maps
            preprocessTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            List<Match> matches = linPyr.MatchTemplates(templPyrs, threshold);
            stopwatch.Stop(); matchTime = stopwatch.ElapsedMilliseconds;

            var matchGroups = new MatchClustering(minDetectionsPerGroup).Group(matches.ToArray());
            foreach (var group in matchGroups)
            {
                bestRepresentatives.Add(group.Representative);
            }

            return bestRepresentatives;
        }

        #region User interface...

        public FastTPDemo()
        {
            InitializeComponent();

            initialize();

            try
            {
#if FILE_CAPTURE
                string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
                videoCapture = new ImageDirectoryReader(Path.Combine(resourceDir, "ImageSequence"), "*.jpg");
#else
                videoCapture = new CameraCapture(0);
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if(videoCapture is CameraCapture)
                (videoCapture as CameraCapture).FrameSize = new Size(640 / 2, 480 / 2); //set new Size(0,0) for the lowest one
          
            this.FormClosing += FastTPDemo_FormClosing;
            Application.Idle += videoCapture_NewFrame;
            videoCapture.Open();
        }

        Bgr<byte>[,] frame = null;
        Accord.Extensions.Imaging.Font font = new Font(FontTypes.HERSHEY_DUPLEX, 1, 0.2f);
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            videoCapture.ReadTo(ref frame);
            if (frame == null)
                return;

            long preprocessTime, matchTime;
            var bestRepresentatives = findObjects(frame, out preprocessTime, out matchTime);

            /************************************ drawing ****************************************/
            foreach (var m in bestRepresentatives)
            {
                frame.Draw(m.BoundingRect, Bgr<byte>.Blue, 1);
                frame.Draw(m.Points.Select(x=> new Circle(x.X, x.Y, 5)).ToArray(), Bgr<byte>.Blue, -1);

                Console.WriteLine("Best template: " + m.Template.ClassLabel + " score: " + m.Score);
            }

            frame.Draw(String.Format("Matching {0} templates in: {1} ms", templPyrs.Count, matchTime), 
                       font, new Point(10, 25), Bgr<byte>.Green);
            /************************************ drawing ****************************************/

            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared) 24bpp color

            //frame.Save(String.Format("C:/probaImages/imgMarked_{0}.jpg", i)); b.Save(String.Format("C:/probaImages/img_{0}.jpg", i)); i++;
            GC.Collect();
        }

        void FastTPDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null)
                videoCapture.Dispose();
        }

        #endregion
    }
}
