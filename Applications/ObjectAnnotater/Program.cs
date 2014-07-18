using System;
using System.IO;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using Point = AForge.IntPoint;

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

            /*var prepareSamplesForm = new SampleGeneration.SamplePreparation();
            prepareSamplesForm.ShowDialog();
            return;*/

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

            if (capture == null) //a user clicked "X" without data selection
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

            if (capture != null && databaseFileName != null)
            {
                if (capture is FileCapture)
                    MessageBox.Show("Due to seek operation issue with opencv/mmpeg seek operation may not work properly. " +  
                                    "If you notice frame/annotation mismatches move slider to the beginning and " + 
                                    "then hold right arrow to move to the desired position." + 
                                    "\nSee: http://code.opencv.org/issues/1419 for details.",
                                    "Seek operation issue",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);

                ObjectAnnotater form = null;
                try
                {
                    form = new ObjectAnnotater(capture, databaseFileName);
                    Application.Run(form);
                }
                catch (Exception)
                {
                    var fInfo = new FileInfo(databaseFileName);
                    var autosaveName = fInfo.Name.Replace(fInfo.Extension, String.Empty) + "-autosave" + fInfo.Extension;

                    autosaveName = Path.Combine(fInfo.DirectoryName, autosaveName);
                    form.Database.Save(autosaveName);

                    var msg = "Unfortunately not your fault :/" + "\r\n" +
                              "However your work is successfully saved to:" + "\r\n" +
                              autosaveName;

                    MessageBox.Show(msg, "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            capture.Close();
        }
    }
}
