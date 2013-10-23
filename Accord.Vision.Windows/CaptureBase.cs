using Accord.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using System.Threading;
using Accord.Imaging.Helper;

namespace Accord.Vision
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
        /// Creates capture from camera which has index: <see cref="cameraIdx"/>
        /// </summary>
        /// <param name="cameraIdx">Camera index.</param>
        public CaptureBase(int cameraIdx = 0)
        {}

        /// <summary>
        /// Creates capture from video file.
        /// </summary>
        /// <param name="fileName">Video file name.</param>
        public CaptureBase(string fileName)
        {}

        /// <summary>
        /// Starts capture.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops capture.
        /// </summary>
        public abstract void Stop();

        bool frameQueried = true;
        /// <summary>
        /// Queries a frame. 
        /// </summary>
        /// <returns>Frame.</returns>
        public virtual Image<Bgr, byte> QueryFrame()
        {
            if (buffer == null)
                return null;

            frameQueried = true;

            var destColor = ColorInfo.GetInfo<Bgr, byte>();
            return ((GenericImageBase)buffer).Convert(destColor, false) as Image<Bgr, byte>; //it will (probably) be casted (not copied) from <Color3, byte> => <Bgr, byte>
        }

        private bool HasNewFrame { get { return buffer != null && !frameQueried; } }

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

        object sync = new object();
        //Image<Bgr, byte> buffer = null;
        IImage buffer = null;

        protected void OnVideoFrame(Bitmap bmp)
        {
            if (!SupressNewFrameEvent && frameQueried)
            {
                lock (sync)
                {
                    if (buffer != null)
                    {
                        bmp.ToImage(buffer);
                    }
                    else
                    {
                        buffer = bmp.ToImage();
                    }

                    frameQueried = false;

                    if (NewFrame != null) NewFrame(this, new EventArgs());
                }
            }
            //else
            {
                //HOW TO PAUSE ???
            }
        }

        /// <summary>
        /// Suppresses New frame event. Can be useful if using <see cref="NewFrame"/> event.
        /// </summary>
        public bool SupressNewFrameEvent { get; set; }
    }
}
