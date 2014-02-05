using Accord.Extensions.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Video capture class base. 
    /// </summary>
    public abstract class CaptureBase
    {
        public delegate void NewFrameHandler(object sender, EventArgs e);

        /// <summary>
        /// New frame event.
        /// </summary>
        public event NewFrameHandler NewFrame;

        /// <summary>
        /// Starts capture.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops capture.
        /// </summary>
        public abstract void Stop();

        public bool CanPause { get; protected set; }
        protected virtual void Pause() { }
        protected virtual void Resume() { }

        protected CaptureBase()
        {
            CanPause = false;
            FlipDirection = Imaging.FlipDirection.None;
        }

        protected bool frameQueried = true;
        /// <summary>
        /// Queries a frame. 
        /// </summary>
        /// <returns>Frame.</returns>
        public virtual Image<Bgr, byte> QueryFrame()
        {
            if (buffer == null)
                return null;

            if (CanPause)
                Resume();

            frameQueried = true;

            FlipImage(ref buffer);

            var destColor = ColorInfo.GetInfo<Bgr, byte>();
            return ((GenericImageBase)buffer).Convert(destColor, false) as Image<Bgr, byte>; //it will (probably) be casted (not copied) from <Color3, byte> => <Bgr, byte>
        }

        private bool HasNewFrame { get { return buffer != null && frameQueried == false; } }

        /// <summary>
        /// Block a calling thread until new frame arrives. <see cref="QueryFrame"/> is monitored.
        /// </summary>
        /// <param name="maxWaitTimeMs">Max wait time in milliseconds.</param>
        /// <returns>Whether new frame arrived or not after <see cref="maxWaitTimeMs"/>.</returns>
        public bool WaitForNewFrame(int maxWaitTimeMs = 100)
        {
            while (!HasNewFrame && maxWaitTimeMs > 0)
            {
                Thread.Sleep(1);
                maxWaitTimeMs--;
            }

            return (maxWaitTimeMs > 0) ? true : false;
        }

        /// <summary>
        /// Get current video resolution. 
        /// Set the user defined resolution or the closest one if exact is not supported.
        /// </summary>
        public abstract Size VideoSize
        {
           get;
           set;
        }

        //Image<Bgr, byte> buffer = null;
        protected object sync = new object();
        protected IImage buffer = null;

        protected void OnVideoFrame(IImage image, bool copyImage = true)
        {
            if (!SupressNewFrameEvent && frameQueried)
            { 
                lock (sync)
                {
                    fillBuffer(image, copyImage);
                    
                    frameQueried = false;

                    if (NewFrame != null) NewFrame(this, new EventArgs());
                }
            }
            else
            {
                if (CanPause)
                    Pause();
            }
        }

        private void fillBuffer(IImage image, bool copyImage)
        {
            if (copyImage == false)
            {
                buffer = image;
            }
            else
            {
                if (buffer != null)
                {
                    buffer.SetValue(image);
                }
                else
                {
                    buffer = image.Clone();
                }
            }
        }

        /// <summary>
        /// Suppresses New frame event. Can be useful if using <see cref="NewFrame"/> event.
        /// </summary>
        public bool SupressNewFrameEvent { get; set; }


        /// <summary>
        /// Returns whether the stream supports random access.
        /// Default is false.
        /// </summary>
        public bool CanSeek { get; protected set; }

        /// <summary>
        /// When overridden in a class moves the current position in the stream.
        /// If the stream does not support random access it throws an <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="offset">Current frame offset.</param>
        /// <param name="origin">The seek origin.</param>
        /// <returns>A new position in the stream.</returns>
        public virtual long Seek(long offset, SeekOrigin origin) 
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a class returns the total number of frames in sequence.
        /// If a stream is continuous returns the number of received frames.
        /// </summary>
        public virtual long Length
        {
            get;
            protected set;
        }

        /// <summary>
        /// When overridden in a class returns the total number of frames received.
        /// If a stream is continuous the value is equal to the <see cref="Length"/>.
        /// </summary>
        public virtual long Position
        {
            get;
            protected set;
        }

        /// <summary>
        /// Flips an image. Default option is none.
        /// </summary>
        public FlipDirection FlipDirection { get; set; }

        /// <summary>
        /// Flips an image with default image flip function.
        /// </summary>
        /// <param name="image">An image to flip in place.</param>
        protected virtual void FlipImage(ref IImage image)
        {
            if (FlipDirection == Imaging.FlipDirection.None)
                return;

            image.FlipImage(this.FlipDirection, inPlace: true);
        }
    }
}
