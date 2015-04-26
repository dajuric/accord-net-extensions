#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.IO;

namespace Accord.Extensions.Imaging
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
        /// <para>Warning: the underlying OpenCV function seeks to nearest key-frame, therefore the seek operation may not be frame-accurate.</para>
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
