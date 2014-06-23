using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using LINE2D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PointF = AForge.Point;


//with template mask
/*using Template = LINE2D.ImageTemplateWithMask;
using TemplatePyramid = LINE2D.ImageTemplatePyramid<LINE2D.ImageTemplateWithMask>;*/

//without template mask (there is slightly performance gain during execution while not loading templates with binary masks)
using Template = LINE2D.ImageTemplate;
using TemplatePyramid = LINE2D.ImageTemplatePyramid<LINE2D.ImageTemplate>;

namespace FastTemplateMatchingDemo
{
    public partial class FastTPDemo : Form
    {
        int threshold = 88;
        int minDetectionsPerGroup = 0; //for match grouping (postprocessing)

        StreamableSource videoCapture;
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
            var list = new List<TemplatePyramid>();

            string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources", "OpenHandLeft_BW");
            string[] files = Directory.GetFiles(resourceDir, "*.bmp");

            object syncObj = new object();
            Parallel.ForEach(files, delegate(string file)
            //foreach(var file in files)
            {
                Image<Gray, Byte> preparedBWImage = System.Drawing.Bitmap.FromFile(file).ToImage<Gray, byte>();

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
        private List<Match> findObjects(Image<Bgr, byte> image, out long preprocessTime, out long matchTime)
        {
            var grayIm = image.Convert<Gray, byte>();

            var bestRepresentatives = new List<Match>();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start(); 
            linPyr = LinearizedMapPyramid.CreatePyramid(grayIm); //prepeare linear-pyramid maps
            preprocessTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            List<Match> matches = Detector.MatchTemplates(linPyr, templPyrs, threshold);
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
                videoCapture = new CameraCapture();
                //videoCapture = new ImageDirectoryReader("C:/probaImages", ".jpg"); 
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            if(videoCapture is CameraCapture)
                (videoCapture as CameraCapture).FrameSize = new Size(640 / 2, 480 / 2); //set new Size(0,0) for the lowest one
          
            this.FormClosing += FastTPDemo_FormClosing;
            Application.Idle += videoCapture_NewFrame;
            videoCapture.Open();
        }

        Image<Bgr, byte> frame;
        System.Drawing.Font font = new System.Drawing.Font("Arial", 12);
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            frame = videoCapture.ReadAs<Bgr, byte>();
            if (frame == null)
                return;

            long preprocessTime, matchTime;
            var bestRepresentatives = findObjects(frame, out preprocessTime, out matchTime);

            /************************************ drawing ****************************************/
            foreach (var m in bestRepresentatives)
            {
                frame.Draw(m.BoundingRect, new Bgr(0, 0, 255), 1);
              
                if (m.Template is ImageTemplateWithMask)
                {
                    var mask = ((ImageTemplateWithMask)m.Template).BinaryMask;
                    if (mask == null) continue; //just draw bounding boxes

                    var area = new Rectangle(m.X, m.Y, mask.Width, mask.Height);
                    if (area.X < 0 || area.Y < 0 || area.Right >= frame.Width || area.Bottom >= frame.Height) continue; //must be fully inside

                    using (var someImage = new Image<Bgr, byte>(mask.Width, mask.Height, Bgr8.Red))
                    {
                        someImage.CopyTo(frame.GetSubRect(area), mask);
                    }
                }
                else
                {
                    frame.Draw(m, Bgr8.Blue, 3, true, Bgr8.Red);
                }

                Console.WriteLine("Best template: " + m.Template.ClassLabel + " score: " + m.Score);
            }

            frame.Draw(String.Format("Matching {0} templates in: {1} ms", templPyrs.Count, matchTime), 
                       font, new PointF(5, 10), new Bgr(0, 255, 0));
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
