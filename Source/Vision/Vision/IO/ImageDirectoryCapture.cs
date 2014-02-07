using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using Accord.Extensions;

namespace Accord.Extensions.Vision
{
    public sealed class ImageDirectoryCapture : CaptureBase, IDisposable
    {
        int frameDelay = 0;
        Timer timer;
        ImageDirectoryReader imageSource;

        #region Initialization

        public ImageDirectoryCapture(string filePath, string extension, Func<string, IImage> loader, int frameDelayMilliseconds = 1, bool useNaturalSorting = true)
        {
            imageSource = new ImageDirectoryReader(filePath, extension, loader, useNaturalSorting);

            this.frameDelay = frameDelayMilliseconds;
        }

        #endregion

        public override void Open()
        {
            imageSource.Open(); 

            timer = new Timer(frameDelay);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
        }

        public override void Close()
        {
            timer.Stop();
            imageSource.Close(); 
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var image = imageSource.Read();
            base.OnFrameReceive(image, false);
        }

        /// <summary>
        /// Gets the image size. This information is available after the initial frame.
        /// Setting the video size is not supported; the <see cref="NotSupportedException"/> will be thrown.
        /// </summary>
        public override Size VideoSize
        {
            get
            {
                return buffer.Size;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        ~ImageDirectoryCapture()
        {
            Dispose();
        }

        #region Overridden base functions

        public override long Length
        {
            get
            {
                return imageSource.Length;
            }
        }

        public override bool IsLiveStream
        {
            get
            {
                return imageSource.IsLiveStream;
            }
        }

        public override long Position
        {
            get
            {
                return imageSource.Position;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return imageSource.CanSeek;
            }
        }

        public override long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            return imageSource.Seek(offset, origin);
        }

        public string CurrentImageName
        {
            get { return imageSource.CurrentImageName; }
        }

        #endregion
    }
}
