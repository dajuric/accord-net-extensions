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
        EventBasedStreamableSource<IImage> capture;

        public VideoCaptureDemo()
        {
            InitializeComponent();

            capture = new ImageDirectoryCapture("C:/images", "*.png", 
                                                (x) => Bitmap.FromFile(x).ToImage<Bgr, byte>(), 
                                                30);

            capture.NewFrame += capture_NewFrame;
            capture.Open();
        }

        void capture_NewFrame(object sender, EventArgs e)
        {
            var frame = capture.Read();

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
