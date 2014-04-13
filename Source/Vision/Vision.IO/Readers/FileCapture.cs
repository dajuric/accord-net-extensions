using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Represents video file streamable source and provides functions and properties to access data in a streamable way.
    /// </summary>
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

        /// <summary>
        /// Opens the video file stream.
        /// </summary>
        public override void Open()
        {
            if (capturePtr != IntPtr.Zero)
                return;

            capturePtr = CvHighGuiInvoke.cvCreateFileCapture(fileName);
            if (capturePtr == IntPtr.Zero)
                throw new Exception("Cannot open FileStream!");
        }

        /// <summary>
        /// Gets the current position in the stream as frame offset.
        /// </summary>
        public override long Position
        {
            get { return (long)CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.PosFrames); }
            protected set { }  
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A frame index offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin = SeekOrigin.Current)
        {
            var frameIndex = base.Seek(offset, origin);
            CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.PosFrames, frameIndex);

            return Position;
        }
    }
}
