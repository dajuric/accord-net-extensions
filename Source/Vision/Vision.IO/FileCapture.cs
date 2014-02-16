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
        }

        public override void Open()
        {
            capturePtr = CvCaptureInvoke.cvCreateFileCapture(fileName);
        }

        public Size FrameSize
        {
            get { return CvCaptureInvoke.GetImageSize(capturePtr); }
        }

        public override long Position
        {
            get { return (long)CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.PosFrames); }
            protected set { }  
        }

        public double FramesPerSecond
        {
            get { return CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FPS); }
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin = SeekOrigin.Current)
        {
            var frameIndex = base.Seek(offset, origin);
            CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.PosFrames, frameIndex);

            return Position;
        }
    }
}
