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

namespace VideoCapture
{
    public partial class VideoCaptureDemo : Form
    {
        StreamableSource<IImage> capture;

        public VideoCaptureDemo()
        {
            InitializeComponent();

            /*capture = new ImageDirectoryReader<IImage>("C:/images", "*.png", 
                                                        (x) => Bitmap.FromFile(x).ToImage<Bgr, byte>());*/

            //capture = new CameraCapture(0);
            capture = new FileCapture(@"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv");
            capture.Open();
            Application.Idle += capture_NewFrame;
        }

        void capture_NewFrame(object sender, EventArgs e)
        {
            var frame = capture.Read();
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
