using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using System;
using System.IO;
using System.Windows.Forms;

namespace VideoCapture
{
    public partial class DirectoryReaderSample : Form
    {
        StreamableSource reader;

        public DirectoryReaderSample() 
        {
            InitializeComponent();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            //reader = new CameraCapture(0); //capture from camera
            reader = new ImageDirectoryReader(Path.Combine(getResourceDir(), "Sequence"), ".png");
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

        private void DirectoryReaderSampleo_FormClosing(object sender, FormClosingEventArgs e)
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
