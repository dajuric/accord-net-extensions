using Accord.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions;

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
            PicoDetector picoDetector;

            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RTDemo());*/

            PicoDetectorHexLoader.FromHexFile("facefinder.ea", out picoDetector);

            var image = System.Drawing.Bitmap.FromFile("faceBig.jpg").ToImage<Bgr, byte>().Convert<Gray, byte>().Resize(new Size(640, 480), InterpolationMode.Bicubic);
            var region = new Rectangle(0, 0, image.Width, image.Height);

            Detector<PicoDetector> detector = new Detector<PicoDetector>(picoDetector);

            var s = DateTime.Now.Ticks;

            //bool result = picoDetector.ClassifyRegion(image, region);
            List<Rectangle> detections = null;
            for (int i = 0; i < 100; i++)
            {
                detections = detector.Detect(image, (im, window, classifier) => classifier.ClassifyRegion(im, window));           
            }
            var result = detections.Count != 0;

            var e = DateTime.Now.Ticks;
          
            Console.WriteLine(result);
            Console.WriteLine("Elapsed {0} ms.", (e - s) / TimeSpan.TicksPerMillisecond);
            ImageBox.Show(image.ToBitmap(), PictureBoxSizeMode.AutoSize);
        }
    }
}
