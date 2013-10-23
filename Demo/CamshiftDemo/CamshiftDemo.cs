using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Drawing;
using System.Windows.Forms;
using AForge;
using Point = System.Drawing.Point;
using Accord.Imaging.Converters;
using Accord.Math.Geometry;
using System.Threading;

namespace Accord.Vision
{
    public partial class CamshiftDemo : Form
    {
        Capture videoCapture;
        DenseHistogram originalObjHist, backgroundHist;

        private void init()
        {
            int[] binSizes = new int[] { 32, 32 }; //X bins per channel

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
            Image<Gray, byte> mask = hsvImg.InRange(new Hsv(0, 0, minV), new Hsv(0, 0, maxV), 2);

            originalObjHist.Calculate(hsvImg.GetSubRect(roi).SplitChannels(0, 1), false, mask.GetSubRect(roi));
            originalObjHist.Scale((float)1 / roi.Area());
            //originalObjHist.Normalize(Byte.MaxValue);

            var backgroundArea = roi.Inflate(1.5, 1.5, frame.Size);
            backgroundHist.Calculate(hsvImg.GetSubRect(backgroundArea).SplitChannels(0, 1), false, mask.GetSubRect(backgroundArea));
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
            Image<Gray, byte> mask = hsvImg.InRange(new Hsv(0, 0, minV), new Hsv(0, 0, maxV), 2);
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
            /*byte[,] mat = new byte[,] 
            {
                {0, 0, 0, 0},
                {1, 1, 1, 1},
                {0, 0, 0, 0}
            };*/

            //var matImg = mat.AsImage();
            /*var matImg = ((Bitmap)Bitmap.FromFile("C:/elipse.bmp")).ToImage<Gray, byte>();

            Camshift.Process(matImg, new Rectangle(Point.Empty, matImg.Size));
            Accord.Imaging.Moments.CentralMoments c = new Imaging.Moments.CentralMoments(3);
            c.Compute(matImg);
            var size = c.GetSize();*/

            InitializeComponent();
            bar_ValueChanged(null, null); //write values to variables
            init(); //create histograms

            try
            {
                videoCapture = new Capture(0);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot find any camera!");
                return;
            }

            videoCapture.VideoSize = new Size(640, 480); //set new Size(0,0) for the lowest one

            this.FormClosing += CamshiftDemo_FormClosing;
            Application.Idle += videoCapture_InitFrame;
            videoCapture.Start();
        }

        Image<Bgr, byte> frame;
        void videoCapture_InitFrame(object sender, EventArgs e)
        {
            bool hasNewFrame = videoCapture.WaitForNewFrame(); //do not process the same frame
            if (!hasNewFrame)
                return;

            frame = videoCapture.QueryFrame();

            if (isROISelected)
            { 
                initTracking(frame);
                Application.Idle -= videoCapture_InitFrame;
                Application.Idle += videoCapture_NewFrame;
            }
            else
            {
                frame.Draw(roi, new Bgr(0, 0, 255), 3); 
            }
            this.pictureBox.Image = frame.ToBitmap(); //it will be just casted (data is shared)

            GC.Collect();
        }

        Font font = new Font("Arial", 12);
        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            bool hasNewFrame = videoCapture.WaitForNewFrame(); //do not process the same frame
            if (!hasNewFrame)
                return;
            /*if (videoCapture.HasNewFrame == false) //do not use: Application.Idle may not be fired right away which may cause temporary freezing
                return;*/

            if (!isROISelected)
            {
                Application.Idle += videoCapture_InitFrame;
                Application.Idle -= videoCapture_NewFrame;
                return;
            }

            frame = videoCapture.QueryFrame()/*.SmoothGaussian(5)*/; //smoothing <<parallel operation>>

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
                videoCapture.Stop();
        }

        Rectangle roi = Rectangle.Empty;
        bool isROISelected = false;
        Point ptFirst;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            ptFirst = e.Location;
            isROISelected = false;
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            roi.Intersect(new Rectangle(Point.Empty, frame.Size));
            isROISelected = true;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Point ptSecond = e.Location;

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
