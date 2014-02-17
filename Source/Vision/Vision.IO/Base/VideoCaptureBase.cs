using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using System.Threading;
using Accord.Extensions.Imaging.Helper;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Represents the base class for video capture. 
    /// </summary>
    public abstract class VideoCaptureBase : StreamableSource
    {
        public const int PROPERTY_NOT_SUPPORTED = 0;

        protected IntPtr capturePtr;

        public override void Close()
        {
            if (capturePtr != IntPtr.Zero)
                CvHighGuiInvoke.cvReleaseCapture(ref capturePtr);
        }

        object syncObj = new object();
        protected override bool Read(out IImage image)
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

        public override long Length
        {
            get { return (long)CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FrameCount); }
        }

        public bool ConvertRgb
        {
            get { return (int)CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.ConvertRGB) != 0; }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.ConvertRGB, value ? 0 : 1); }
        }

        public Size FrameSize
        {
            get { return CvHighGuiInvoke.GetImageSize(capturePtr); }
        }
    }
}
