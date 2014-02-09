using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;

namespace ObjectAnnotater
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            ImageDirectoryReader capture = null;
            AnnotationDatabase database = null;

            capture = new ImageDirectoryReader("C:/images/", "*.png",
                                                   (path) => System.Drawing.Bitmap.FromFile(path).ToImage<Bgr, byte>());

            database = AnnotationDatabase.LoadOrCreate("C:/database.txt");

            /*using (var wizard = new Wizard())
            {
                wizard.ShowDialog();
                capture = wizard.CaptureObj;
                annotationWritter = wizard.AnnotationWriter;
            }*/

            //if(capture != null && annotationWritter != null)
                Application.Run(new ObjectAnnotater(capture, database));
        }
    }
}
