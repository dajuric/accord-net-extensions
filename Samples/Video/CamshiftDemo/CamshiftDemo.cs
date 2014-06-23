using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Filters;
using System;
using System.Windows.Forms;
using AForge;
using Accord.Extensions.Imaging.Converters;
using Accord.Extensions.Math.Geometry;
using System.Threading;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Vision
{
    public partial class CamshiftDemo : Form
    {
        StreamableSource videoCapture;
        DenseHistogram originalObjHist, backgroundHist;

        private void init()
        {
            int[] binSizes = new int[] { 64, 64 }; //X bins per channel

            IntRange[] ranges = new IntRange[] 
            { 
                new IntRange(0, 180), //Hue (for 8bpp image hue range is (0-180) otherwise (0-360)
                new IntRange(0, 255)
            };

            originalObjHist = new DenseHistogram(binSizes, ranges);
            backgroundHist = originalObjHist.CopyBlank();
        }

        DenseHistogram ratioHist = null;
        private void initTracking(Image<Bgr, byte> frame)
        {
            //get hue channel from search area
            var hsvImg = frame.Convert<Hsv, byte>(); //<<parallel operation>>
            //user constraints...
            Image<Gray, byte> mask = hsvImg.InRange(new Hsv(0, 0, minV), new Hsv(0, 0, maxV), Byte.MaxValue, 2);

            originalObjHist.Calculate(hsvImg.GetSubRect(roi).SplitChannels(0, 1), !false, mask.GetSubRect(roi));
            originalObjHist.Scale((float)1 / roi.Area());
            //originalObjHist.Normalize(Byte.MaxValue);

            var backgroundArea = roi.Inflate(1.5, 1.5, frame.Size);
            backgroundHist.Calculate(hsvImg.GetSubRect(backgroundArea).SplitChannels(0, 1), !false, mask.GetSubRect(backgroundArea));
            backgroundHist.Scale((float)1 / backgroundArea.Area());
            //backgroundHist.Normalize(Byte.MaxValue);
            
            //how good originalObjHist and objHist match (suppresses possible selected background)
            ratioHist = originalObjHist.CreateRatioHistogram(backgroundHist, Byte.MaxValue, 10);

            searchArea = roi;
            roi = Rectangle.Empty;
        }

        Rectangle searchArea;
        private void processImage(Image<Bgr, byte> frame, out Image<Gray, byte> probabilityMap, out Rectangle prevSearchArea, out Box2D foundBox)
        {
            prevSearchArea = searchArea;

            //convert to HSV
            var hsvImg = frame.Convert<Hsv, byte>(); //<<parallel operation>>
            //back-project ratio hist => create probability map
            probabilityMap = ratioHist.BackProject(hsvImg.SplitChannels(0, 1)); //or new Image<Gray, byte>[]{ hsvImg[0], hsvImg[1]...} //<<parallel operation>>

            //user constraints...
            Image<Gray, byte> mask = hsvImg.InRange(new Hsv(0, 0, minV), new Hsv(0, 0, maxV), Byte.MaxValue, 2);
            probabilityMap.And(mask, inPlace:true);

            //run Camshift algorithm to find new object position, size and angle
            foundBox = Camshift.Process(probabilityMap, searchArea); //<<parallel operation>>
            var foundArea = Rectangle.Round(foundBox.GetMinArea());

            searchArea = foundArea.Inflate(0.05, 0.05, frame.Size); //inflate found area for search (X factor)...
            if (searchArea.IsEmpty) isROISelected = false; //reset tracking
        }

        #region User interface...

        public CamshiftDemo()
        {
            InitializeComponent();
            bar_ValueChanged(null, null); //write values to variables
            init(); //create histograms

            try
            {
                videoCapture = new CameraCapture(0);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            if(videoCapture is CameraCapture)
                (videoCapture as CameraCapture).FrameSize = new Size(640, 480); //set new Size(0,0) for the lowest one

            this.FormClosing += CamshiftDemo_FormClosing;
            Application.Idle += videoCapture_InitFrame;
            videoCapture.Open();
        }

        Image<Bgr, byte> frame;
        void videoCapture_InitFrame(object sender, EventArgs e)
        {
            frame = videoCapture.ReadAs<Bgr, byte>();
            if (frame == null) return;

            if (isROISelected)
            { 
                initTracking(frame);
                Application.Idle -= videoCapture_InitFrame;
                Application.Idle += videoCapture_NewFrame;
                return;
            }
            else
            {
                frame.Draw(roi, new Bgr(0, 0, 255), 3); 
            }
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared)

            GC.Collect();
        }

        System.Drawing.Font font = new System.Drawing.Font("Arial", 12);
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            frame = videoCapture.ReadAs<Bgr, byte>()/*.SmoothGaussian(5)*/; //smoothing <<parallel operation>>
            if (frame == null) return;

            if (!isROISelected)
            {
                Application.Idle += videoCapture_InitFrame;
                Application.Idle -= videoCapture_NewFrame;
                return;
            }

            long start = DateTime.Now.Ticks;

            Image<Gray, byte> probabilityMap;
            Rectangle prevSearchArea;
            Box2D foundBox;
            processImage(frame, out probabilityMap, out prevSearchArea, out foundBox);

            long end = DateTime.Now.Ticks;
            long elapsedMs = (end - start) / TimeSpan.TicksPerMillisecond;

            frame.Draw("Processed: " + elapsedMs + " ms", font, new PointF(15, 10), new Bgr(0, 255, 0));
            frame.Draw(prevSearchArea, new Bgr(0, 0, 255), 3);
            frame.Draw(foundBox, new Bgr(0, 255, 0), 5); Console.WriteLine("angle: " + foundBox.Angle);
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared) 24bpp color
            this.pbProbabilityImage.Image = probabilityMap.ToBitmap(); //it will be just casted (data is shared) 8bpp gray

            GC.Collect();
        }
    
        void CamshiftDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCapture != null) 
                videoCapture.Dispose();
        }

        Rectangle roi = Rectangle.Empty;
        bool isROISelected = false;
        Point ptFirst;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            ptFirst = e.Location.ToPt();
            isROISelected = false;
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            roi.Intersect(new Rectangle(new Point(), frame.Size));
            isROISelected = true;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var ptSecond = e.Location;

            roi = new Rectangle 
            {
                X = System.Math.Min(ptFirst.X, ptSecond.X),
                Y = System.Math.Min(ptFirst.Y, ptSecond.Y),
                Width = System.Math.Abs(ptFirst.X - ptSecond.X),
                Height = System.Math.Abs(ptFirst.Y - ptSecond.Y)
            };
        }

        int minV, maxV;
        private void bar_ValueChanged(object sender, EventArgs e)
        {
            if (barVMin.Value > barVMax.Value)
                barVMax.Value = barVMin.Value;

            minV = barVMin.Value;
            maxV = barVMax.Value;
        }

        #endregion
    }
}
