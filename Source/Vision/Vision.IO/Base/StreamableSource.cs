using Accord.Extensions.Imaging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Accord.Extensions.Vision
{
    public abstract class StreamableSource<TImage>: IDisposable
        where TImage: IImage
    {
        protected StreamableSource()
        {
            this.CanSeek = false;
            this.IsLiveStream = false;
            this.ReadTimeout = 100;
        }

        public abstract long Length { get; }

        public virtual long Position { get; protected set; }

        public virtual bool IsLiveStream { get; protected set; }

        public virtual bool CanSeek { get; protected set; }

        public int ReadTimeout { get; set; }

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

        public Task<TImage> ReadAsync()
        {
            var readTask = new Task<TImage>(() =>
            {
                TImage result;
                Read(out result);
                return result;
            });

            readTask.Start();
            return readTask;
        }

        public TImage Read(out bool isExpired)
        {
            var readTask = ReadAsync();
            readTask.Wait(this.ReadTimeout);

            isExpired = !readTask.IsCompleted;
            return readTask.Result;
        }

        public TImage Read() 
        {
            bool isExpired;
            return Read(out isExpired);
        }

        public virtual void Dispose()
        {
            this.Close();
        }

        public abstract void Open();

        public abstract void Close();

        protected abstract bool Read(out TImage image);
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
