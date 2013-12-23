using Accord.Imaging;
using Accord.Vision;
using LINE2D;
using LINE2D.QueryImage;
using LINE2D.TemplateMatching;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoreLinq;
using Accord.Math.Geometry;

namespace FastTemplateMatchingDemo
{
    public partial class FastTPDemo : Form
    {
        int threshold = 88;
        int minDetectionsPerGroup = 3; //for match grouping (postprocessing)

        Capture videoCapture;
        List<TemplatePyramid> templPyrs;

        private void initialize()
        {
            TemplatePyramid.BindSpecificTemplateToClass<TemplateWithObjectMask>("OpenHand");
            templPyrs = fromFiles();
        }

        List<TemplatePyramid> fromFiles()
        {
            List<TemplatePyramid> list = new List<TemplatePyramid>();

            string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            string[] files = Directory.GetFiles(resourceDir, "*.bmp");

            object syncObj = new object();
            Parallel.ForEach(files, delegate(string file)
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

        private List<Match> findObjects(Image<Bgr, byte> image)
        {
            var bestRepresentatives = new List<Match>();

            LinearMemoryPyramid linPyr = LinearMemoryPyramid.CreatePyramid(image);
            List<Match> matches = Detector.MatchTemplates(linPyr, templPyrs, threshold);

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

            initialize();

            try
            {
                videoCapture = new Capture(0);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            videoCapture.VideoSize = new Size(320, 200); //set new Size(0,0) for the lowest one

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

            frame = videoCapture.QueryFrame()/*.SmoothGaussian(5)*/; //smoothing <<parallel operation>>

            long start = DateTime.Now.Ticks;

            var bestRepresentatives = findObjects(frame);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            /************************************ drawing ****************************************/

            foreach (var m in bestRepresentatives)
            {
                TemplateWithObjectMask template = (TemplateWithObjectMask)m.Template;
                frame.Draw(m.BoundingRect, new Bgr(0, 0, 255), 1);

                if (RectangleExtensions.IntersectionPercent(m.BoundingRect, new Rectangle(Point.Empty, frame.Size)) > 0.99) //if a match is fully inside an image...
                {
                    var bgrMask = template.BinaryMask.Convert<Bgr, byte>();
                    frame.GetSubRect(m.BoundingRect).Max(bgrMask, inPlace: true);
                }
            }

            frame.Draw(String.Format("Matching {0} templates in: {1} ms", templPyrs.Count, elapsedMs), 
                       font, new PointF(15, 10), new Bgr(0, 255, 0));
          
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
