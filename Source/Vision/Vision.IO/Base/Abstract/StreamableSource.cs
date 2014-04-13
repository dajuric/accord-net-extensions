using Accord.Extensions.Imaging;
using System;
using System.Collections;
using System.Collections.Generic;
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
    /// Represents image streamable source. 
    /// It is the base class for classes providing image stream reading.
    /// </summary>
    /// <typeparam name="TImage">Image type.</typeparam>
    public abstract class StreamableSource<TImage> : ImageStream<TImage>, IEnumerable<TImage>
        where TImage: IImage
    {
        /// <summary>
        /// Initalizes a new instance of <see cref="StreamableSource"/>.
        /// </summary>
        protected StreamableSource()
        {
            this.ReadTimeout = 100;
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before timing out.
        /// </summary>
        public int ReadTimeout { get; set; }

        /// <summary>
        /// Creates and starts the task responsible for frame reading.
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
                ReadInternal(out result);
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
        /// When overridden in a derived class returns an image and a status.
        /// Position is advanced.
        /// </summary>
        /// <param name="image">Read image.</param>
        /// <returns></returns>
        protected abstract bool ReadInternal(out TImage image);

        #region IEnumerable

        /// <summary>
        /// Gets the enumerator for the stream.
        /// <para>If the stream does not support seek, an exception will be thrown during iteration.</para>
        /// </summary>
        /// <returns>Enumerator for the stream.</returns>
        public IEnumerator<TImage> GetEnumerator()
        {
            return new StreamableSourceEnumerator<TImage>(this);
        }

        /// <summary>
        /// Gets the enumerator for the stream.
        /// <para>If the stream does not support seek, an exception will be thrown during iteration.</para>
        /// </summary>
        /// <returns>Enumerator for the stream.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion
    }

    public class StreamableSourceEnumerator<TImage> : IEnumerator<TImage>
        where TImage : IImage
    {
        StreamableSource<TImage> streamableSource;
        int position;

        public StreamableSourceEnumerator(StreamableSource<TImage> streamableSource)
        {
            this.streamableSource = streamableSource;
            Reset();
        }

        public bool MoveNext()
        {
            position++;

            var oldPosition = streamableSource.Position;
            var newPosition = streamableSource.Seek(position, SeekOrigin.Begin);

            return newPosition > oldPosition || position == 0;
        }

        public void Reset()
        {
            streamableSource.Seek(0, SeekOrigin.Begin);
            position = -1;
        }

        public TImage Current
        {
            get 
            {
                var result = streamableSource.Read();
                streamableSource.Seek(-1, SeekOrigin.Current);

                return result;
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        bool isDisposed = false;
        public void Dispose()
        {
            if (!isDisposed)
            {
                Reset();
                isDisposed = true;
            }
        }
    }

    public static class StreamableSourceExtensions
    {
        /// <summary>
        /// Calls read function defined by the stream and converts an returned image if necessary.
        /// </summary>
        /// <param name="copyAlways">Forces data copy even if a casting is enough.</param>
        /// <param name="failIfCannotCast">If data copy is needed throws an exception.</param>
        /// <returns>Converted image or null if the image can not be read.</returns>
        public static Image<TColor, TDepth> ReadAs<TColor, TDepth>(this StreamableSource<IImage> imageStream, bool copyAlways = false, bool failIfCannotCast = false)
            where TColor: IColor
            where TDepth: struct
        {
            var image = imageStream.Read();
            if (image == null)
                return null;

            return ((Image)image).Convert<TColor, TDepth>(copyAlways, failIfCannotCast);
        }
    }
}
