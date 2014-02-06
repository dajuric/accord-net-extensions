using Accord.Extensions.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Accord.Extensions.Vision
{
    public abstract class EventBasedSource<TImage>: StreamableSource<TImage>
        where TImage: IImage
    {
        public delegate void NewFrameHandler(object sender, EventArgs e);

        /// <summary>
        /// New frame event.
        /// </summary>
        public event NewFrameHandler NewFrame;
        protected bool frameQueried = true;

        protected EventBasedSource()
        {}

        protected object sync = new object();
        protected TImage buffer = default(TImage);

        protected void OnFrameReceive(TImage image, bool copyImage = true)
        {
            if (!WaitUserCall || frameQueried) //if waitUserCall is false always udate the buffer, otherwise just if a user called Read()
            { 
                lock (sync)
                {
                    fillBuffer(image, copyImage);

                    frameQueried = false;
                    if (NewFrame != null) NewFrame(this, new EventArgs());
                }
            }

            TotalReceivedFrames++;
        }

        private void fillBuffer(TImage image, bool copyImage)
        {
            if (copyImage == false)
            {
                buffer = image;
            }
            else
            {
                if (buffer != null && buffer.Size.Equals(image.Size))
                {
                    buffer.SetValue(image);
                }
                else
                {
                    buffer = (TImage)image.Clone();
                }
            }
        }

        protected override bool Read(out TImage image)
        {
            if (buffer == null || frameQueried)
            {
                image = default(TImage);
                return false;
            }

            image = buffer;
            frameQueried = true;
            this.Position++;

            return true;
        }

        public int TotalReceivedFrames { get; private set; }

        public bool WaitUserCall { get; set; }
    }
}
