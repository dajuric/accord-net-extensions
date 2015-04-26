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
using Accord.Extensions.Imaging;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents the base class for video capture that shares common functions and properties with camera and file capture. 
    /// </summary>
    public abstract class VideoCaptureBase : ImageStreamReader
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
        /// <summary>
        /// Reads the next image in the stream and advances the position by one.
        /// </summary>
        /// <param name="image">Read image.</param>
        /// <returns>True if the reading operation was successful, false otherwise.</returns>
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
        /// Gets or sets whether to force conversion of an input image to Bgr color type.
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
