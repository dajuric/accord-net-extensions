using Accord.Imaging;
using Accord.Vision;
using LINE2D;
using LINE2D.QueryImage;
using LINE2D.TemplateMatching;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastTemplateMatchingDemo
{
    public partial class FastTPDemo : Form
    {
        int threshold = 88;
        int minDetectionsPerGroup = 0; //for match grouping (postprocessing)

        Capture videoCapture;
        List<TemplatePyramid> templPyrs;

        private void initialize()
        {
            //templPyrs = fromFiles();
            templPyrs = fromXML();
        }

        List<TemplatePyramid> fromFiles()
        {
            List<TemplatePyramid> list = new List<TemplatePyramid>();

            string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            string[] files = Directory.GetFiles(resourceDir, "*.bmp");

            object syncObj = new object();
            Parallel.ForEach(files, delegate(string file)
            //foreach(var file in files)
            {
                Image<Gray, Byte> preparedBWImage = Bitmap.FromFile(file).ToImage<Gray, byte>();

                try
                {
                    TemplatePyramid tp = TemplatePyramid.CreatePyramidFromPreparedBWImage(preparedBWImage, "OpenHand");
                    lock (syncObj)
                    { list.Add(tp); };
                }
                catch (Exception)
                {}
            });

            //TemplateSerializer.Save(list, "bla.xml");
            return list;
        }

        List<TemplatePyramid> fromXML()
        {
            List<TemplatePyramid> list = new List<TemplatePyramid>();

            string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            list = TemplateSerializer.Load(Path.Combine(resourceDir, "OpenHand_Right.xml"))/*.Take(500)*/.ToList();

            return list;
        }

        private List<Match> findObjects(Image<Bgr, byte> image, out long preprocessTime, out long matchTime)
        {
            var grayIm = image.Convert<Gray, byte>();

            var bestRepresentatives = new List<Match>();
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            LinearizedMapPyramid linPyr = LinearizedMapPyramid.CreatePyramid(grayIm); //prepeare linear-pyramid maps
            stopwatch.Stop(); preprocessTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            //List<Match> matches = new List<Match>();
            List<Match> matches = Detector.MatchTemplates(linPyr, templPyrs, threshold);
            stopwatch.Stop(); matchTime = stopwatch.ElapsedMilliseconds;

            var matchGroups = new MatchGroupMatching().Group(matches.ToArray());

            foreach (var group in matchGroups)
            {
                if (group.Detections.Length < minDetectionsPerGroup)
                    continue;

                var bestMatch = group.Detections.MaxBy(delegate(Match a) { return a.BoundingRect.Size.Width * a.BoundingRect.Size.Height; });
                //group.Detections.Sort(delegate(Match a, Match b) { return a.Score.CompareTo(b.Score) * (-1); });

                bestRepresentatives.Add(bestMatch);
            }

            return bestRepresentatives;
        }

        #region User interface...

        public FastTPDemo()
        {
            InitializeComponent();

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "SIMDArrayInstructions.dll")) == false)
            {
                MessageBox.Show("Copy SIMDArrayInstructions.dll to your bin directory!");
                return;
            }

            initialize();

            /*frame = Bitmap.FromFile("C:/proba.jpg").ToImage<Bgr, byte>();
            long preprocessTime, matchTime;
            var bestRepresentatives = findObjects(frame, out preprocessTime, out matchTime);
            return;*/

            try
            {
                videoCapture = new Capture(/*"proba.wmv"*/);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            videoCapture.VideoSize = new Size(640 / 2, 480 / 2); //set new Size(0,0) for the lowest one
          
            this.FormClosing += FastTPDemo_FormClosing;
            Application.Idle += videoCapture_NewFrame;
            videoCapture.Start();
        }

        Image<Bgr, byte> frame;
        Font font = new Font("Arial", 12);
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            bool hasNewFrame = videoCapture.WaitForNewFrame(); //do not process the same frame
            if (!hasNewFrame)
                return;

            frame = videoCapture.QueryFrame().Clone();
            //frame = Bitmap.FromFile("C:/proba.jpg").ToImage<Bgr, byte>();

            long preprocessTime, matchTime;
            var bestRepresentatives = findObjects(frame, out preprocessTime, out matchTime);

            /************************************ drawing ****************************************/

            foreach (var m in bestRepresentatives)
            {
                frame.Draw(m.BoundingRect, new Bgr(0, 0, 255), 1);
                frame.Draw(m, new Bgr(Color.Blue), 3, 3);
            }

            frame.Draw(String.Format("Matching {0} templates in: {1} ms", templPyrs.Count, matchTime), 
                       font, new PointF(5, 10), new Bgr(0, 255, 0));
          
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared) 24bpp color
          
            GC.Collect();
        }

        void FastTPDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null)
                videoCapture.Stop();
        }

        #endregion
    }
}
