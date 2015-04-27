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

using Accord.Extensions.Imaging;
using System;
using System.IO;
using System.Windows.Forms;

namespace CaptureAndRecording
{
    public partial class CaptureAndRecordingDemo : Form
    {
        VideoCaptureBase reader; //use specific class to set specific properties (e.g. for camera, for video, for image dir)
        ImageStreamWriter writer;

        public CaptureAndRecordingDemo()
        {
            InitializeComponent();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            reader = new CameraCapture(0); //capture from camera
            //reader = new FileCapture(Path.Combine(getResourceDir(), "Welcome Accord.NET Extensions.mp4"));
            reader.Open();

            writer = new VideoWriter(@"output.avi", reader.FrameSize, /*reader.FrameRate does not work Cameras*/ 30); //TODO: bug: FPS does not work for cameras
            writer.Open();

            Application.Idle += capture_NewFrame;
        }

        Bgr<byte>[,] frame = null;
        void capture_NewFrame(object sender, EventArgs e)
        { 
            reader.ReadTo(ref frame);

            if (frame == null)
            {
                Application.Idle -= capture_NewFrame;
                return;
            }

            this.pictureBox.Image = frame.ToBitmap();

            bool isFrameWritten = writer.Write(frame.Lock()); //TODO: revise
            if (!isFrameWritten)
                MessageBox.Show("Frame is not written!", "Frame writing error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            GC.Collect();
        }

        private void CaptureAndRecordingDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (reader != null)
                reader.Dispose();

            if (writer != null)
                writer.Dispose();
        }

        private static string getResourceDir()
        {
            var directoryInfo = new DirectoryInfo(Environment.CurrentDirectory).Parent;
            if (directoryInfo != null)
                return Path.Combine(directoryInfo.FullName, "Resources");
            return null;
        }
    }
}
