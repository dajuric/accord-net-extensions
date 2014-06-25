using System;
using System.IO;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;

namespace VideoCapture
{
    public partial class VideoCaptureAndRecordingSample : Form
    {
        VideoCaptureBase reader;
        StreamableDestination writer;

        public VideoCaptureAndRecordingSample() 
        {
            InitializeComponent();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            //reader = new CameraCapture(0); //capture from camera
            reader = new FileCapture(Path.Combine(getResourceDir(), "Welcome Accord.NET Extensions.mp4"));
            reader.Open();
         
            writer = new VideoWriter(@"output.avi", reader.FrameSize, reader.FrameRate);
            writer.Open();

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

            bool isFrameWritten = writer.Write(frame);
            if (!isFrameWritten)
                MessageBox.Show("Frame is not written!", "Frame writing error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            GC.Collect();
        }

        private void VideoCaptureDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (reader != null)
                reader.Dispose();

            if (writer != null)
                writer.Dispose();
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
