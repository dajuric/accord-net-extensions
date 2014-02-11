using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accord.Extensions.Vision
{
    public abstract class StreamableSource<TImage>
        where TImage: IImage
    {
        protected StreamableSource()
        {
            this.CanSeek = false;
            this.IsLiveStream = false;
            this.ReadTimeout = 100;
            this.ReadBetweenPause = 1;
        }

        public virtual long Length { get; protected set; }

        public virtual long Position { get; protected set; }

        public virtual bool IsLiveStream { get; protected set; }

        public virtual bool CanSeek { get; protected set; }

        public virtual long Seek(long offset, System.IO.SeekOrigin origin = SeekOrigin.Current)
        {
            if (!this.CanSeek)
                throw new NotSupportedException();

            long newPosition = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;
                case SeekOrigin.Current:
                    newPosition = this.Position + offset;
                    break;
                case SeekOrigin.End:
                    newPosition = this.Length + offset;
                    break;
            }

            var currentFrame = System.Math.Min(this.Length, System.Math.Max(0, newPosition));
            return currentFrame;
        }

        public abstract void Open();

        public abstract void Close();

        public TImage Read() 
        {
            TImage image;
            bool isSuccess = tryReadFrame(out image);

            if (isSuccess)
                return image;
            else
                throw new TimeoutException();
        }

        public int ReadTimeout
        {
            get;
            set;
        }

        protected int ReadBetweenPause { get; set; }

        protected abstract bool Read( out TImage image);

        /// <summary>
        /// Blocks a calling thread until new frame arrives. <see cref="QueryFrame"/> is monitored.
        /// </summary>
        /// <returns>Whether new frame arrived or not after <see cref="maxWaitTimeMs"/>.</returns>
        private bool tryReadFrame(out TImage image)
        {
            image = default(TImage);

            //if end of stream do not even try to read
            if (!this.IsLiveStream)
            {
                if (this.Position >= this.Length)
                    return false;
            }

            var maxTime = this.ReadTimeout;

            bool isSuccess = Read(out image);
            while (!isSuccess && maxTime > 0)
            {
                isSuccess = Read(out image);

                Thread.Sleep(ReadBetweenPause);
                maxTime -= ReadBetweenPause;
            }
          
            return isSuccess;
        }

    }

    public static class StreamableSourceExtensions
    {
        public static Image<TColor, TDepth> ReadAs<TColor, TDepth>(this StreamableSource<IImage> imageStream)
            where TColor: IColor
            where TDepth: struct
        {
            var image = imageStream.Read();
            if (image == null)
                return null;

            return ((GenericImageBase)image).Convert<TColor, TDepth>();
        }
    }
}
