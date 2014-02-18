using Accord.Extensions.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Represents image streamable source of base image interface <see cref="IImage"/>.
    /// </summary>
    public abstract class StreamableSource: StreamableSource<IImage>
    { }

    /// <summary>
    /// Represents image streamable source. It is the base class for classes providing image stream reading.
    /// </summary>
    /// <typeparam name="TImage">Image type.</typeparam>
    public abstract class StreamableSource<TImage>: IDisposable
        where TImage: IImage
    {
        /// <summary>
        /// Initalizes a new instance of <see cref="StreamableSource"/>.
        /// </summary>
        protected StreamableSource()
        {
            this.CanSeek = false;
            this.IsLiveStream = false;
            this.ReadTimeout = 100;
        }

       /// <summary>
       /// When overridden in a derived class, gets the length in number of frames.
       /// </summary>
        public abstract long Length { get; }

        /// <summary>
        /// When overridden in a derived class, gets the next frame index.
        /// </summary>
        public virtual long Position { get; protected set; }

        /// <summary>
        /// Gets whether the stream is live stream meaning that its length is not constant.
        /// Those streams are usually not seekable <see cref="CanSeek"/>.
        /// </summary>
        public virtual bool IsLiveStream { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public virtual bool CanSeek { get; protected set; }

        /// <summary>
        /// Gets or sets a value, in miliseconds, that determines how long the stream will attempt to read before timing out.
        /// </summary>
        public int ReadTimeout { get; set; }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A frame index offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="NotSupportedException">The stream is not seekable.</exception>
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

        /// <summary>
        /// Creates and starts the task responnsible for frame reading.
        /// If this function is called <see cref="ReadTimeout"/> must be handled by a user itself.
        /// <remarks>
        /// By using this function reading from some streams can be accelerated.
        /// </remarks>
        /// </summary>
        /// <returns>A image reading task.</returns>
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

        /// <summary>
        /// Reads an image from the current stream 
        /// and advances the position within the stream by 1 element.
        /// </summary>
        /// <param name="isExpired">If a null is returned this can be due to <see cref="ReadTimeout"/> has been reached.</param>
        /// <returns>Read image.</returns>
        public TImage Read(out bool isExpired)
        {
            var readTask = ReadAsync();
            readTask.Wait(this.ReadTimeout);

            isExpired = !readTask.IsCompleted;
            return readTask.Result;
        }

        /// <summary>
        /// Reads an image from the current stream 
        /// and advances the position within the stream by usually 1 element.
        /// </summary>
        /// <returns>Read image.</returns>
        public TImage Read() 
        {
            bool isExpired;
            return Read(out isExpired);
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Opens the current stream. 
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Closes the current stream and releases any resources associated with the current stream.
        /// This function is internaly called by <see cref="Dispose"/>.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// When overriden in a derived class returns an image and a status.
        /// Position is advanced.
        /// </summary>
        /// <param name="image">Read image.</param>
        /// <returns></returns>
        protected abstract bool Read(out TImage image);
    }

    public static class StreamableSourceExtensions
    {
        /// <summary>
        /// Calls read function defined in the stream and converts an returned image.
        /// </summary>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <param name="failIfCannotCast">If data copy is needed throws an exception.</param>
        /// <returns>Read converted image.</returns>
        public static Image<TColor, TDepth> ReadAs<TColor, TDepth>(this StreamableSource<IImage> imageStream, bool copyAlways = false, bool failIfCannotCast = false)
            where TColor: IColor
            where TDepth: struct
        {
            var image = imageStream.Read();
            if (image == null)
                return null;

            return ((GenericImageBase)image).Convert<TColor, TDepth>(copyAlways, failIfCannotCast);
        }
    }
}
