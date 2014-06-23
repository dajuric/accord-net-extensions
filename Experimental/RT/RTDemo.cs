using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Point = AForge.IntPoint;
using Color = Accord.Extensions.Imaging.Bgr;

namespace RT
{
    public partial class RTDemo : Form
    {
        StreamableSource videoStreamSource;
        StreamableDestination videoStreamDest;

        PicoClassifier<Bgr> picoClassifier;
        Detector<PicoClassifier<Bgr>> detector;
        GroupMatching<Rectangle> groupMatching;

        public RTDemo()
        {
            InitializeComponent();

            PicoClassifierHexDeserializer.FromHexFile(@"colorClassifier.ea", out picoClassifier);
            //picoClassifier.ToHexFile("faceSerialized.ea");

            detector = new Detector<PicoClassifier<Bgr>>(picoClassifier);
            detector.Scale = 1.2f;
            detector.StartSize = picoClassifier.GetSize(regionScale: 20);
            detector.EndSize = picoClassifier.GetSize(regionScale: 1000);

            /*var grayIm = System.Drawing.Bitmap.FromFile("open-hand.jpg").ToImage<Gray, byte>();

            var detections = detector.Detect(grayIm, (im, window, classifier) =>
            {
                var center = window.Center();
                var scale = window.Width;

                float conf;
                classifier.ClassifyRegion(im, center, scale, out conf);
                if (conf > 2)
                    return true;
                else
                    return false;
            });
            Console.WriteLine(detections);
            float conf;
            picoClassifier.ClassifyRegion(grayIm, new Point(81, 94), 119, out conf);
            Console.WriteLine(conf);*/

            groupMatching = new RectangleGroupMatching(2);

            //videoStreamSource = new CameraCapture();   
            videoStreamSource = new FileCapture(@"S:\Svjetla - baza podataka - Boris\Stražnja\AA012201.mxf");
            //videoStreamSource = new ImageDirectoryReader(@"S:\Vehicle databases\hriRT\Stream_020\leftImages\", "*.png");
            //videoStreamSource.Seek(1000, System.IO.SeekOrigin.Begin);
            videoStreamSource.Seek((int)(videoStreamSource.Length * 0.5), System.IO.SeekOrigin.Begin);

            if(videoStreamSource is CameraCapture)
                (videoStreamSource as CameraCapture).FrameSize = new Size(640, 480);

            videoStreamSource.Open();

            videoStreamDest = new VideoWriter("output.avi", new Size(1280, 720), 50 * 3);

            Application.Idle += Application_Idle;
        }

        void Application_Idle(object sender, EventArgs e)
        {
           var image = videoStreamSource.ReadAs<Bgr, byte>();
           if (image == null)
               return;

           image = image.Resize(0.5f, InterpolationMode.Bilinear);
           image = image.Clone();
           var convertedIm = image.Convert<Bgr, byte>();

           Rectangle[] detections = null;
           var elapsedTime = Accord.Extensions.Diagnostics.MeasureTime(() => 
           {
               detections = detector.Detect(convertedIm, (im, window, classifier) =>
               {
                   var center = window.Center();

                   float conf;
                   classifier.ClassifyRegion(im, center, window.Size, out conf);
                   if (conf > 0.5)
                       return true;
                   else
                       return false;
               });
           });

           Console.WriteLine("Elapsed: {0} ms", elapsedTime);

           /*foreach (var d in detections)
           {
               image.Draw(d, Bgr8.Blue, 3);
           }*/

           var groupMatches = groupMatching.Group(detections);
           foreach (var groupMatch in groupMatches)
           {
              image.Draw(groupMatch.Representative, Bgr8.Red, 3);
           }

           this.pictureBox.Image = image.ToBitmap();
           //Thread.Sleep(1000);

           //videoStreamDest.Write(image);
           //GC.Collect();
        }

        private void RTDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoStreamSource != null)
                videoStreamSource.Dispose();
        }
    }
}
