using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Vision
{
    public class FileCapture: VideoCaptureBase
    {
        string fileName = null;

        /// <summary>
        /// Creates capture from video file.
        /// </summary>
        /// <param name="fileName">Video file name.</param>
        public FileCapture(string fileName)
        {
            this.CanSeek = true;
        
            if (System.IO.File.Exists(fileName) == false)
                throw new System.IO.FileNotFoundException();

            this.fileName = fileName;
            this.Open(); //to enable property change
        }

        public override void Open()
        {
            if (capturePtr != IntPtr.Zero)
                return;

            capturePtr = CvHighGuiInvoke.cvCreateFileCapture(fileName);
            if (capturePtr == IntPtr.Zero)
                throw new Exception("Cannot open FileStream!");
        }

        public override long Position
        {
            get { return (long)CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.PosFrames); }
            protected set { }  
        }

        public double FramesPerSecond
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FPS); }
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin = SeekOrigin.Current)
        {
            var frameIndex = base.Seek(offset, origin);
            CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.PosFrames, frameIndex);

            return Position;
        }
    }
}
