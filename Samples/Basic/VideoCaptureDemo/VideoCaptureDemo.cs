using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using System.Diagnostics;

namespace VideoCapture
{
    public partial class VideoCaptureDemo : Form
    {
        StreamableSource capture;

        public VideoCaptureDemo()
        {
            InitializeComponent();

            //capture = new ImageDirectoryReader("C:/images", "*.png");

            //capture = new CameraCapture(0);
            capture = new FileCapture(@"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv");
            capture.Open();
            Application.Idle += capture_NewFrame;
        }

        async void capture_NewFrame(object sender, EventArgs e)
        {
            var frame = await capture.ReadAsync(); //faster (not for live streams)
            //var frame = capture.Read();

            if (frame == null)
            {
                Application.Idle -= capture_NewFrame;
                return;
            }

            this.pictureBox.Image = frame.ToBitmap();
            GC.Collect();
        }

        private void VideoCaptureDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (capture != null)
                capture.Close();
        }
    }
}
