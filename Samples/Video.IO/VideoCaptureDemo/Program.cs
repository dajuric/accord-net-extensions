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

            Console.WriteLine(@"********** (1 / 3) Image directory reader sample ****************");
            Application.Run(new DirectoryReaderSample());

            Console.WriteLine(@"********** (2 / 3) Video capture and recording sample ****************");
            Application.Run(new VideoCaptureAndRecordingSample());

            Console.WriteLine(@"********** (3 / 3) Video extraction sample (see 'bin\OutputSamples\' ****************");
            ExtractVideoSample.Test();
        }
    }
}
