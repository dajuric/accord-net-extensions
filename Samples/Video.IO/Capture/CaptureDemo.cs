using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using System;
using System.IO;
using System.Windows.Forms;

namespace Capture
{
    public partial class CaptureDemo : Form
    {
        ImageStreamReader reader;

        public CaptureDemo()
        {
            InitializeComponent();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            //reader = new CameraCapture(0); //capture from camera
            //reader = new FileCapture(Path.Combine(getResourceDir(), "Welcome Accord.NET Extensions.mp4")); //capture from video
            reader = new ImageDirectoryReader(Path.Combine(getResourceDir(), "Sequence"), ".jpg");
            reader.Open();

            Application.Idle += capture_NewFrame;
        }

        Image<Bgr, byte> frame = null;
        void capture_NewFrame(object sender, EventArgs e)
        {
            //frame = await capture.ReadAsync(); //faster (does not apply for live streams)
            frame = reader.ReadAs<Bgr, byte>(); 

            if (frame == null)
            {
                Application.Idle -= capture_NewFrame;
                return;
            }

            this.pictureBox.Image = frame.ToBitmap();
            GC.Collect();
        }

        private void CaptureDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (reader != null)
                reader.Dispose();
        }

        private static string getResourceDir()
        {
            var directoryInfo = new DirectoryInfo(Environment.CurrentDirectory).Parent;
            if (directoryInfo != null)
                return Path.Combine(directoryInfo.FullName, "Resources");
            return null;
        }
    }
}
