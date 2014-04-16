using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Point = AForge.IntPoint;
using Accord.Extensions;
using System.Xml.Serialization;
using Accord.Extensions.Math.Geometry;

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
            
            StreamableSource capture = null;
            string databaseFileName = null;

            using (var wizard = new Wizard())
            {
                wizard.ShowDialog();

                capture = wizard.CaptureObj;
                databaseFileName = wizard.DatabaseFileName;
            }

            /*capture = new ImageDirectoryReader("S:/images/", "*.jpg");
            databaseFileName = "S:/imagesAnnotations.xml";*/

            /*capture = new FileCapture(@"S:\Detekcija_Ruke\WIN_20140311_121008.mp4");
            databaseFileName = @"\\darko-pc\Users\Darko\Google disk\bla.xml";*/

            if (capture == null && databaseFileName == null) //a user clicked "X" without data selection
            {
                //MessageBox.Show("Capture or database file name is null!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            capture.Open();

            if (capture.Length == 0)
            {
                MessageBox.Show("The directory does not contain images!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(capture != null && databaseFileName != null)
                Application.Run(new ObjectAnnotater(capture, databaseFileName));

            capture.Close();
        }
    }
}
