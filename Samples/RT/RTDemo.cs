using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
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

            PicoClassifierHexDeserializer.FromHexFile("myClassifier.ea", out picoClassifier);
            //picoClassifier.ToHexFile("faceSerialized.ea");

            detector = new Detector<PicoClassifier>(picoClassifier);
            detector.StartSize = picoClassifier.GetSize(regionScale: 10);
            detector.EndSize = picoClassifier.GetSize(regionScale: 500);

            groupMatching = new RectangleGroupMatching(3);

            //videoStreamSource = new CameraCapture();
            //videoStreamSource = new FileCapture("S:/Detekcija_Ruke/WIN_20140317_120459.mp4");

            //videoStreamSource = new ImageDirectoryReader(@"S:\images\", "*.jpg");
            videoStreamSource = new FileCapture(@"S:\Svjetla - baza podataka - Boris\Prednja svjetla\AA011305.mxf");
            //videoStreamSource.Seek((int)(videoStreamSource.Length * 0.88), System.IO.SeekOrigin.Begin);

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

           /*image = image.Resize(0.5f, InterpolationMode.NearestNeighbor);
           var grayIm = image.Convert<Gray, byte>();

           var detections = detector.Detect(grayIm, (im, window, classifier) =>
           {
               float conf;
               classifier.ClassifyRegion(im, window, out conf);
               if (conf > 0.5)
                   return true;
               else
                   return false;
           });

           var groupMatches = groupMatching.Group(detections);

           foreach (var groupMatch in groupMatches)
           {
              image.Draw(groupMatch.Representative, Bgr8.Red, 3);
           }

           foreach (var d in detections)
           {
               image.Draw(d, Bgr8.Red, 3);
           }*/

           this.pictureBox.Image = image.ToBitmap();
           //Thread.Sleep(1000);
        }

        private void RTDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoStreamSource != null)
                videoStreamSource.Close();
        }
    }
}
