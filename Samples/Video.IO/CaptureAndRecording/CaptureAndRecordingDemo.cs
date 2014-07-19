using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using System;
using System.IO;
using System.Windows.Forms;

namespace CaptureAndRecording
{
    public partial class CaptureAndRecordingDemo : Form
    {
        VideoCaptureBase reader; //use specific class to set specific properties (e.g. for camera, for video, for image dir)
        ImageStreamWriter writer;

        public CaptureAndRecordingDemo()
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

        private void CaptureAndRecordingDemo_FormClosing(object sender, FormClosingEventArgs e)
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
