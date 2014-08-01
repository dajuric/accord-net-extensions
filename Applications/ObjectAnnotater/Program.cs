using System;
using System.IO;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using Point = AForge.IntPoint;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using System.Collections.ObjectModel;

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
            ObservableCollection<Annotation> o = new ObservableCollection<Annotation>();
            o.Add(new Annotation());
            o.CollectionChanged += o_CollectionChanged;

            o[0].Label = "kkk";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ImageStreamReader capture = null;
            string databaseFileName = null;

            /*using (var wizard = new Wizard())
            {
                wizard.ShowDialog();

                capture = wizard.CaptureObj;
                databaseFileName = wizard.DatabaseFileName;
            }*/

            capture = new ImageDirectoryReader(@"C:\Users\Darko-Home\Desktop\HandDatabase\prepared\Nenad_Darko\Tomislav 1\", "*.jpg");
            databaseFileName = @"C:\Users\Darko-Home\Desktop\HandDatabase\prepared\Nenad_Darko\Tomislav 1.xml";

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
                ObjectAnnotater form = null;
                //try
                {
                    form = new ObjectAnnotater(capture, databaseFileName);
                    Application.Run(form);
                }
                /*catch (Exception)
                {
                    var fInfo = new FileInfo(databaseFileName);
                    var autosaveName = fInfo.Name.Replace(fInfo.Extension, String.Empty) + "-autosave" + fInfo.Extension;

                    autosaveName = Path.Combine(fInfo.DirectoryName, autosaveName);
                    form.Database.Save(autosaveName);

                    var msg = "Unfortunately not your fault :/" + "\r\n" +
                              "However your work is successfully saved to:" + "\r\n" +
                              autosaveName;

                    MessageBox.Show(msg, "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }*/
            }

            capture.Close();
        }

        static void o_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
