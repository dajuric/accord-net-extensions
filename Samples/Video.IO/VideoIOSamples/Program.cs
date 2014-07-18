using System;
using System.Windows.Forms;

namespace VideoCapture
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

            Console.WriteLine(@"********** (1 / 2) Image directory reader sample ****************");
            Application.Run(new DirectoryReaderSample());

            Console.WriteLine(@"********** (2 / 2) Video capture and recording sample ****************");
            Application.Run(new VideoCaptureAndRecordingSample());
        }
    }
}
