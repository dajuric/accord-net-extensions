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

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents camera streamable source and provides functions and properties to access a device in a streamable way.
    /// </summary>
    public class CameraCapture: VideoCaptureBase
    {
        int cameraIdx = 0;

        /// <summary>
        /// Creates capture from camera.
        /// </summary>
        /// <param name="cameraIdx">Camera index.</param>
        public CameraCapture(int cameraIdx = 0)
        {
            this.cameraIdx = cameraIdx;
            this.CanSeek = false;
            this.IsLiveStream = true;
            this.Open(); //to enable property change
        }

        /// <summary>
        /// Opens the camera stream.
        /// </summary>
        public override void Open()
        {
            if (capturePtr != IntPtr.Zero)
                return;
            
            capturePtr = CvHighGuiInvoke.cvCreateCameraCapture(cameraIdx);
            if (capturePtr == IntPtr.Zero)
                throw new Exception("Cannot open camera stream! It seems that camera device can not be found.");
        }

        /// <summary>
        /// Gets or sets the brightness of the camera.
        /// <para>If the property is not supported by device 0 will be returned.</para>
        /// </summary>
        public double Brightness
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Brightness); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Brightness, value); }
        }

        /// <summary>
        /// Gets or sets the contrast of the camera.
        /// <para>If the property is not supported by device 0 will be returned.</para>
        /// </summary>
        public double Contrast
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Contrast); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Contrast, value); }
        }

        /// <summary>
        /// Gets or sets the exposure of the camera.
        /// <para>If the property is not supported by device 0 will be returned.</para>
        /// </summary>
        public double Exposure
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Exposure); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Exposure, value); }
        }

        /// <summary>
        /// Gets or sets the gain of the camera.
        /// <para>If the property is not supported by device 0 will be returned.</para>
        /// </summary>
        public double Gain
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Gain); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Gain, value); }
        }

        /// <summary>
        /// Gets or sets the hue of the camera.
        /// <para>If the property is not supported by device 0 will be returned.</para>
        /// </summary>
        public double Hue
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Hue); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Hue, value); }
        }

        /// <summary>
        /// Gets or sets the saturation of the camera.
        /// <para>If the property is not supported by device 0 will be returned.</para>
        /// </summary>
        public double Saturation
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.Saturation); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.Saturation, value); }
        }

        /// <summary>
        /// Gets or sets the frame size of the camera.
        /// </summary>
        public new Size FrameSize
        {
            get { return CvHighGuiInvoke.GetImageSize(capturePtr); }
            set { CvHighGuiInvoke.SetImageSize(capturePtr, value); }
        }

        /// <summary>
        /// Gets or sets the frame rate of the camera.
        /// <para>If the property is not supported by device 0 will be returned.</para>
        /// </summary>
        public new double FrameRate
        {
            get { return CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FPS); }
            set { CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.FPS, value); }
        }
    }
}
