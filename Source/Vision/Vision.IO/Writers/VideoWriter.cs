using Accord.Extensions.Imaging;
using System;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Video writer that writes color images into video file.
    /// </summary>
    public class VideoWriter : VideoWriter<Image<Bgr, byte>>
    {
        /// <summary>
        /// Creates new video writer.
        /// </summary>
        /// <param name="fileName">Video file name.</param>
        /// <param name="frameSize">Video frame size.</param>
        /// <param name="fps">Specifies the number of frames per second.</param>
        public VideoWriter(string fileName, Size frameSize, float fps = 30)
            : base(fileName, frameSize, true, fps)
        { }

        /// <summary>
        /// Creates new video writer.
        /// </summary>
        /// <param name="fileName">Video file name.</param>
        /// <param name="frameSize">Video frame size.</param>
        /// <param name="fps">Specifies the number of frames per second.</param>
        /// <param name="videoCodec">Specifies used codec for video encoding.</param>
        public VideoWriter(string fileName, Size frameSize, float fps, VideoCodec videoCodec)
            : base(fileName, frameSize, true, fps, videoCodec)
        {}
    }

    /// <summary>
    /// Video writer that writes images into video file.
    /// </summary>
    /// <typeparam name="TImage">Image type.</typeparam>
    public class VideoWriter<TImage>: StreamableDestination<TImage>
        where TImage: IImage
    {
        object syncObj = new object();

         string fileName = null;
         IntPtr videoObjPtr = IntPtr.Zero;
         bool isColor;
         int codec;

        /// <summary>
        /// Gets the number of frames per second.
        /// </summary>
         public float FramesPerSecond { get; private set; }

        /// <summary>
        /// Gets the frame size.
        /// </summary>
         public Size FrameSize { get; private set; }

        /// <summary>
        /// Creates new video writer (with default codec).
        /// </summary>
        /// <param name="fileName">Video file name.</param>
        /// <param name="frameSize">Video frame size.</param>
        /// <param name="isColor">Specifies whether the image is color image (3 channels) or grayscale image (one channel).</param>
        /// <param name="fps">Specifies the number of frames per second.</param>
         public VideoWriter(string fileName, Size frameSize, bool isColor = true, float fps = 30)
             :this(fileName, frameSize, isColor, fps, VideoCodec.IntelYUV)
         {}

        /// <summary>
        /// Creates new video writer.
        /// </summary>
        /// <param name="fileName">Video file name.</param>
        /// <param name="frameSize">Video frame size.</param>
        /// <param name="isColor">Specifies whether the image is color image (3 channels) or grayscale image (one channel).</param>
        /// <param name="fps">Specifies the number of frames per second.</param>
        /// <param name="videoCodec">Specifies used codec for video encoding.</param>
         public VideoWriter(string fileName, Size frameSize, bool isColor, float fps, VideoCodec videoCodec)
         {
             this.CanSeek = false;
             this.IsLiveStream = true;

             this.fileName = fileName;
             this.isColor = isColor;
             this.codec = (int)videoCodec;
             this.FrameSize = frameSize;
             this.FramesPerSecond = fps;

             this.Open(); //to enable property change
         }

        /// <summary>
        /// Opens the video file stream.
        /// </summary>
        public override void Open()
        {
            if (videoObjPtr != IntPtr.Zero)
                return;

            videoObjPtr = CvHighGuiInvoke.cvCreateVideoWriter(fileName, codec, FramesPerSecond, FrameSize, isColor);
            if (videoObjPtr == IntPtr.Zero)
                throw new Exception("Cannot open FileStream!");
        }

        /// <summary>
        /// Gets the current position in the stream as frame offset.
        /// </summary>
        public override long Position
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the current stream length which is not constant and is the same as position.
        /// </summary>
        public override long Length
        {
            get { return this.Position; }
        }

        protected unsafe override bool WriteInternal(TImage image)
        {
            bool isSuccessful;

            lock (syncObj)
            {
                /*if (image.ColorInfo.NumberOfChannels == 3 && !isColor)
                    return false;
                    //throw new Exception("Image must be color!");

                if (image.ColorInfo.NumberOfChannels == 1 && isColor)
                    return false;
                    //throw new Exception("Image must be grayscale!");

                if (!image.Size.Equals(FrameSize))
                    return false;
                    //throw new Exception("Input image must be the same size as defined frame size!");*/

                this.Position++;

                var iplImg = image.AsOpenCvImage();
                IplImage* iplImgPtr = (IplImage*)&iplImg;

                isSuccessful = CvHighGuiInvoke.cvWriteFrame(videoObjPtr, (IntPtr)iplImgPtr);
            }

            return isSuccessful;
        }

        /// <summary>
        /// Closes video writer.
        /// <para>Use dispose method to remove any additional resources.</para>
        /// </summary>
        public override void Close()
        {
            if (videoObjPtr != IntPtr.Zero)
                CvHighGuiInvoke.cvReleaseVideoWriter(ref videoObjPtr);
        }
    }
}
