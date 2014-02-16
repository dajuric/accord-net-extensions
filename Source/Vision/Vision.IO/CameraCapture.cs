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

            capturePtr = CvCaptureInvoke.cvCreateCameraCapture(cameraIdx);
            if (capturePtr == IntPtr.Zero)
                throw new Exception("Cannot open CameraStream!");
        }

        public double Brightness
        {
            get { return CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Brightness); }
            set { CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Brightness, value); }
        }

        public double Contrast
        {
            get { return CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Contrast); }
            set { CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Contrast, value); }
        }

        public double Exposure
        {
            get { return CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Exposure); }
            set { CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Exposure, value); }
        }

        public double Gain
        {
            get { return CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Gain); }
            set { CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Gain, value); }
        }

        public double hue
        {
            get { return CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Hue); }
            set { CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Hue, value); }
        }

        public double Saturation
        {
            get { return CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Saturation); }
            set { CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Saturation, value); }
        }


        public Size FrameSize
        {
            get { return CvCaptureInvoke.GetImageSize(capturePtr); }
            set { CvCaptureInvoke.SetImageSize(capturePtr, value); }
        }

        public double FramesPerSecond
        {
            get { return CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FPS); }
            set { CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.FPS, value); }
        }
    }
}
