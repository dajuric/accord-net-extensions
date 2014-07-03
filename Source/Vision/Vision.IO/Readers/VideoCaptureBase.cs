using System;
using Accord.Extensions.Imaging;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Represents the base class for video capture that shares common functions and properties with camera and file capture. 
    /// </summary>
    public abstract class VideoCaptureBase : StreamableSource
    {
        /// <summary>
        /// Internal OpenCV pointer for the capture object.
        /// </summary>
        protected IntPtr capturePtr;

        /// <summary>
        /// Releases all resources allocated by capture.
        /// </summary>
        public override void Close()
        {
            if (capturePtr != IntPtr.Zero)
                CvHighGuiInvoke.cvReleaseCapture(ref capturePtr);
        }

        object syncObj = new object();
        protected override bool ReadInternal(out IImage image)
        {
            bool status = false;
            image = default(IImage);

            lock (syncObj)
            {
                IntPtr cvFramePtr;
                cvFramePtr = CvHighGuiInvoke.cvQueryFrame(capturePtr);

                if (cvFramePtr != IntPtr.Zero)
                {
                    image = IplImage.FromPointer(cvFramePtr).AsImage();
                    this.Position++;
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the length in number of frames.
        /// </summary>
        public override long Length
        {
            get { return (long)CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FrameCount); }
        }

        /// <summary>
        /// Gets or sets whether to force conversion of an input image to <see cref="Bgr"/> color type.
        /// </summary>
        public bool ConvertRgb
        {
            get { return (int)CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.ConvertRGB) != 0; }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.ConvertRGB, value ? 0 : 1); }
        }

        /// <summary>
        /// Gets the frame size.
        /// </summary>
        public Size FrameSize
        {
            get { return CvHighGuiInvoke.GetImageSize(capturePtr); }
        }

        /// <summary>
        /// Gets the frame rate.
        /// </summary>
        public float FrameRate
        {
            get { return (float)CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FPS); }
        }
    }
}
