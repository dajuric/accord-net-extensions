using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using System;
using System.Windows.Forms;

namespace RT
{
    public partial class RTDemo : Form
    {
        StreamableSource videoStreamSource;

        PicoClassifier picoClassifier;
        Detector<PicoClassifier> detector;
        GroupMatching<Rectangle> groupMatching;

        public RTDemo()
        {
            InitializeComponent();

            PicoClassifierHexDeserializer.FromHexFile("face-detector-from-genki-dataset.ea", out picoClassifier);
            //picoClassifier.ToHexFile("faceSerialized.ea");

            detector = new Detector<PicoClassifier>(picoClassifier);
            detector.StartSize = picoClassifier.GetSize(regionScale: 50);
            //detector.InParallel = false;

            groupMatching = new RectangleGroupMatching();

            //videoStreamSource = new CameraCapture();
            videoStreamSource = new FileCapture("S:/Detekcija_Ruke/Nikola.wmv");

            if(videoStreamSource is CameraCapture)
                (videoStreamSource as CameraCapture).FrameSize = new Size(640, 480);

            videoStreamSource.Open();

            Application.Idle += Application_Idle;
        }

        void Application_Idle(object sender, EventArgs e)
        {
           var image = videoStreamSource.ReadAs<Bgr, byte>();
           if (image == null)
               return;

           var grayIm = image.Convert<Gray, byte>();

           var detections = detector.Detect(grayIm, (im, window, classifier) =>
           {
               float conf;
               classifier.ClassifyRegion(im, window, 0, out conf);
               if (conf > 3)
                   return true;
               else
                   return false;
           });

           var groupMatches = groupMatching.Group(detections);

           foreach (var groupMatch in groupMatches)
           {
              image.Draw(groupMatch.Representative, Bgr8.Red, 3);
           }

           this.pictureBox.Image = image.ToBitmap();
        }

        private void RTDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoStreamSource != null)
                videoStreamSource.Close();
        }
    }
}
