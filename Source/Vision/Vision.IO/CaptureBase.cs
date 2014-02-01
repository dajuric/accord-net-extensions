using Accord.Extensions.Imaging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
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

        public bool SupportsPausing { get; protected set; }
        protected virtual void Pause() { }
        protected virtual void Resume() { }

        protected CaptureBase()
        {
            SupportsPausing = false;
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

            if (SupportsPausing)
                Resume();

            frameQueried = true;

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

        /// <summary>
        /// Flips horizontal.
        /// </summary>
        public abstract bool FlipHorizontal { get; set; } //HOT TO DO IT ? (without making Image<,> methods)

        /// <summary>
        /// Flips vertical.
        /// </summary>
        public abstract bool FlipVertical { get; set; } //HOW TO DO IT ? (without making Image<,> methods)

       
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
                if (SupportsPausing)
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
    }
}
