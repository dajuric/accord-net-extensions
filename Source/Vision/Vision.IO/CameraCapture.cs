using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Vision
{
    public class CameraCapture: VideoCaptureBase
    {
        int cameraIdx = 0;

        /// <summary>
        /// Creates capture from camera which has index: <see cref="cameraIdx"/>
        /// </summary>
        /// <param name="cameraIdx">Camera index.</param>
        public CameraCapture(int cameraIdx = 0)
        {
            this.cameraIdx = cameraIdx;
            this.CanSeek = false;
            this.IsLiveStream = true;
            this.Open(); //to enable property change
        }

        public override void Open()
        {
            if (capturePtr != IntPtr.Zero)
                return;

            capturePtr = CvHighGuiInvoke.cvCreateCameraCapture(cameraIdx);
            if (capturePtr == IntPtr.Zero)
                throw new Exception("Cannot open CameraStream!");
        }

        public double Brightness
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Brightness); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Brightness, value); }
        }

        public double Contrast
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Contrast); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Contrast, value); }
        }

        public double Exposure
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Exposure); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Exposure, value); }
        }

        public double Gain
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Gain); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Gain, value); }
        }

        public double hue
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Hue); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Hue, value); }
        }

        public double Saturation
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Saturation); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Saturation, value); }
        }


        public new Size FrameSize
        {
            get { return CvHighGuiInvoke.GetImageSize(capturePtr); }
            set { CvHighGuiInvoke.SetImageSize(capturePtr, value); }
        }

        public double FramesPerSecond
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FPS); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.FPS, value); }
        }
    }
}
