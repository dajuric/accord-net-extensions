using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using System;
using System.Windows.Forms;

namespace VideoCapture
{
    public partial class VideoCaptureDemo : Form
    {
        VideoCaptureBase capture;
        StreamableDestination writer;

        public VideoCaptureDemo()
        {
            InitializeComponent();

            //capture = new CameraCapture(0);
            capture = new FileCapture(@"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv");
            capture.Open();

            writer = new VideoWriter(@"output.avi", capture.FrameSize, capture.FrameRate);
            writer.Open();

            Application.Idle += capture_NewFrame;
        }

        void capture_NewFrame(object sender, EventArgs e)
        {
            //var frame = await capture.ReadAsync(); //faster (does not apply for live streams)
            var frame = capture.ReadAs<Bgr, byte>();

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
            if (capture != null)
                capture.Dispose();

            if (writer != null)
                writer.Dispose();
        }
    }
}
