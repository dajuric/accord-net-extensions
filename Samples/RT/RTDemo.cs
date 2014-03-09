using Accord.Controls;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using NGenerics.DataStructures.Trees;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using Accord.Extensions.Math.Geometry;

namespace RT
{
    public partial class RTDemo : Form
    {
        CameraCapture videoStreamSource;

        PicoClassifier picoClassifier;
        Detector<PicoClassifier> detector;

        public RTDemo()
        {
            InitializeComponent();

            PicoClassifierHexLoader.FromHexFile("facefinder.ea", out picoClassifier);
            detector = new Detector<PicoClassifier>(picoClassifier);
            detector.StartSize = picoClassifier.GetSize(50);

            videoStreamSource = new CameraCapture();
            videoStreamSource.FrameSize = new Size(640, 480);
            videoStreamSource.Open();

            Application.Idle += Application_Idle;
        }

        void Application_Idle(object sender, EventArgs e)
        {
           var image = videoStreamSource.ReadAs<Bgr, byte>();
           var grayIm = image.Convert<Gray, byte>();

           var detections = detector.Detect(grayIm, (im, window, classifier) =>
           {
               float conf;
               classifier.ClassifyRegion(im, window, out conf);
               if (conf > 3)
                   return true;
               else
                   return false;
           });

           GroupMatching<Rectangle> groupMatching = new RectangleGroupMatching();
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
