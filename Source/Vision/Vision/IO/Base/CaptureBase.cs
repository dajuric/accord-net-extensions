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
    public abstract class CaptureBase: EventBasedStreamableSource<IImage>
    {
        protected CaptureBase()
        {
            base.UpdateOnDemand = true;
            FlipDirection = Imaging.FlipDirection.None;
        }

        /// <summary>
        /// Get current video resolution. 
        /// If setting a video size is not supported an <see cref="NotSupportedException"/> will be thrown.
        /// </summary>
        public abstract Size VideoSize
        {
           get;
           set;
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
            if (image == null || FlipDirection == Imaging.FlipDirection.None)
                return;

            image.FlipImage(this.FlipDirection, inPlace: true);
        }

        /// <summary>
        /// Starts capture.
        /// </summary>
        public void Start()
        {
            Open();
        }

        /// <summary>
        /// Stops capture.
        /// </summary>
        public void Stop()
        {
            Close();
        }

        /// <summary>
        /// Queries a frame. 
        /// </summary>
        /// <returns>Frame.</returns>
        public IImage QueryFrame()
        {
            return Read();
        }

        /// <summary>
        /// Queries a frame. 
        /// </summary>
        /// <returns>Frame.</returns>
        public IImage QueryFrame<TColor, TDepth>()
            where TColor : IColor
            where TDepth : struct
        {
            return this.ReadAs<TColor, TDepth>();
        }
    }

}
