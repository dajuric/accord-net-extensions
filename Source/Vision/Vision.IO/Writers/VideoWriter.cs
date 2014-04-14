using Accord.Extensions.Imaging;
using System;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Video writer that writes images into video file.
    /// </summary>
    public class VideoWriter: StreamableDestination
    {
        object syncObj = new object();
        IntPtr videoObjPtr = IntPtr.Zero;

        /// <summary>
        /// Gets the output file name.
        /// </summary>
         public string OutputFileName { get; private set; }

        /// <summary>
        /// Gets whether the frame must consist of 3-channels (color) or from just one (grayscale).
        /// </summary>
         public bool ColorFrames { get; private set; }

        /// <summary>
        /// Gets the codec used to encode frames.
        /// </summary>
         public VideoCodec Codec { get; private set; }

        /// <summary>
        /// Gets the number of frames per second.
        /// </summary>
         public float FrameRate { get; private set; }

        /// <summary>
        /// Gets the frame size.
        /// </summary>
         public Size FrameSize { get; private set; }

        /// <summary>
        /// Creates new video writer (with default codec).
        /// </summary>
        /// <param name="fileName">Video file name.</param>
        /// <param name="frameSize">Video frame size.</param>
         /// <param name="fps">Specifies the number of frames per second.</param>
         /// <param name="isColor">Specifies whether the image is color image (3 channels) or grayscale image (one channel).</param>
         public VideoWriter(string fileName, Size frameSize, float fps = 30, bool isColor = true)
             : this(fileName, frameSize, fps, isColor, VideoCodec.MotionJpeg)
         {}

        /// <summary>
        /// Creates new video writer.
        /// </summary>
        /// <param name="fileName">Video file name.</param>
        /// <param name="frameSize">Video frame size.</param>
         /// <param name="fps">Specifies the number of frames per second.</param>
         /// <param name="isColor">Specifies whether the image is color image (3 channels) or grayscale image (one channel).</param>
        /// <param name="videoCodec">Specifies used codec for video encoding.</param>
         public VideoWriter(string fileName, Size frameSize, float fps, bool isColor, VideoCodec videoCodec)
         {
             this.CanSeek = false;
             this.IsLiveStream = true;

             this.OutputFileName = fileName;
             this.ColorFrames = isColor;
             this.Codec = videoCodec;
             this.FrameSize = frameSize;
             this.FrameRate = fps;

             this.Open(); //to enable property change
         }

        /// <summary>
        /// Opens the video file stream.
        /// </summary>
        public override void Open()
        {
            if (videoObjPtr != IntPtr.Zero)
                return;

            videoObjPtr = CvHighGuiInvoke.cvCreateVideoWriter(OutputFileName, (int)Codec, FrameRate, FrameSize, ColorFrames);
            if (videoObjPtr == IntPtr.Zero)
                throw new Exception(String.Format("Cannot open FileStream! Please check that the selected codec ({0}) is supported.", Codec));
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

        protected unsafe override bool WriteInternal(IImage image)
        {
            bool isSuccessful;

            lock (syncObj)
            {
                if (image.ColorInfo.NumberOfChannels == 3 && !ColorFrames)
                    throw new Exception("Image must be color!");

                if (image.ColorInfo.NumberOfChannels == 1 && ColorFrames)
                    throw new Exception("Image must be grayscale!");

                if (!image.Size.Equals(FrameSize))
                    throw new Exception("Input image must be the same size as defined frame size!");

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
