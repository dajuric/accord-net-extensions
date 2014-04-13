using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using System;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions;

namespace VideoCapture
{
    public partial class VideoCaptureDemo : Form
    {
        StreamableSource capture;
        StreamableDestination<Image<Bgr, byte>> writer;

        public VideoCaptureDemo()
        {
            InitializeComponent();

            //capture = new ImageDirectoryReader("C:/images", "*.png");

            //capture = new CameraCapture(0);
            capture = new FileCapture(@"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv");
            capture.Open();

            writer = new VideoWriter(@"output.avi", new Size(1280, 720), 30, VideoCodec.IntelYUV);
            writer.Open();

            var im = new Image<Bgr, byte>(640, 480, Bgr8.Red);
            //im.Save("sks.bmp");

            for (int i = 0; i < 1; i++)
            {
                writer.Write(im);
            }

            writer.Dispose();
            return;

            Application.Idle += capture_NewFrame;
        }

        async void capture_NewFrame(object sender, EventArgs e)
        {
            //var frame = await capture.ReadAsync(); //faster (not for live streams)
            var frame = capture.ReadAs<Bgr, byte>();

            //frame = (frame as Image).Convert<Bgr, byte>().Convert<Hsv, byte>();
            if (frame == null)
            {
                Application.Idle -= capture_NewFrame;
                return;
            }

            this.pictureBox.Image = frame.ToBitmap();

            bool isFrameWritten = writer.Write(frame);
            Console.WriteLine("Is frame written ? " + isFrameWritten);
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
