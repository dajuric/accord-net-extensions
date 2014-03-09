using Accord.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using Point = AForge.IntPoint;


namespace RT
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            PicoClassifier picoDetector;

            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RTDemo());*/

            PicoDetectorHexLoader.FromHexFile("facefinder.ea", out picoDetector);

            ImageDirectoryReader reader = new ImageDirectoryReader("C:/", "*.jpg");
            reader.Open();

            var image = reader.ReadAs<Gray, byte>();

            //var image = System.Drawing.Bitmap.FromFile("C:/face.bmp").ToImage<Gray, byte>();
            var region = new Rectangle(155, 198, 245, 245);

            Detector<PicoClassifier> detector = new Detector<PicoClassifier>(picoDetector);
            detector.StartSize = picoDetector.GetSize(50);

            //var s = DateTime.Now.Ticks;

            float confidence;
            bool result = picoDetector.ClassifyRegion(image, new Point(155, 198), new Size(245, 245), out confidence);


            var detections = detector.Detect(image, (im, window, classifier) =>
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

            var debugImage = image.Convert<Bgr, byte>();
            foreach (var groupMatch in groupMatches)
            {
                debugImage.Draw(groupMatch.Representative, Bgr8.Red, 3);
            }

            /*List<Rectangle> detections = null;
            for (int i = 0; i < 100; i++)
            {
                detections = detector.Detect(image, (im, window, classifier) => classifier.ClassifyRegion(im, window));           
            }
            var result = detections.Count != 0;*/

            //var e = DateTime.Now.Ticks;

            Console.WriteLine(detections);
            Console.WriteLine(result);
            //Console.WriteLine("Elapsed {0} ms.", (e - s) / TimeSpan.TicksPerMillisecond);
            ImageBox.Show(debugImage.ToBitmap(), PictureBoxSizeMode.AutoSize);
        }
    }
}
