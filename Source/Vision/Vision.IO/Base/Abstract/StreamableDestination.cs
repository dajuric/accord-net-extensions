using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Represents image streamable source of base image interface <see cref="IImage"/>.
    /// </summary>
    public abstract class StreamableDestination : StreamableDestination<IImage>
    { }

    /// <summary>
    /// Represents image streamable source. 
    /// It is the base class for classes providing image stream reading.
    /// </summary>
    /// <typeparam name="TImage">Image type.</typeparam>
    public abstract class StreamableDestination<TImage> : ImageStream<TImage>
        where TImage : IImage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StreamableSource"/>.
        /// </summary>
        protected StreamableDestination()
        {
            this.WriteTimeout = 500;
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the writer will attempt to write before timing out.
        /// </summary>
        public int WriteTimeout { get; set; }

        /// <summary>
        /// Creates and starts the task responsible for frame writing.
        /// If this function is called <see cref="WriteTimeout"/> must be handled by a user itself.
        /// <remarks>
        /// By using this function writing to some streams can be accelerated.
        /// </remarks>
        /// </summary>
        /// <returns>An image writing task.</returns>
        public Task<bool> WriteAsync(TImage image)
        {
            var writeTask = new Task<bool>(() =>
            {
                bool success = WriteInternal(image);
                return success;
            });

            writeTask.Start();
            return writeTask;
        }

        /// <summary>
        /// Writes an image from the current stream 
        /// and advances the position within the stream by 1 element.
        /// </summary>
        /// <returns>
        /// True if the operation is successfully completed, 
        /// false if the writer failed to write or the <see cref="WriteTimeout"/> has been reached.
        /// </returns>
        public bool Write(TImage image)
        {
            var writeTask = WriteAsync(image);
            writeTask.Wait(this.WriteTimeout);

            return writeTask.IsCompleted && writeTask.Result;
        }

        /// <summary>
        /// When overridden in a derived class returns an image and a status.
        /// Position is advanced.
        /// </summary>
        /// <param name="image">Image to write.</param>
        /// <returns>True if successful, false otherwise.</returns>
        protected abstract bool WriteInternal(TImage image);
    }
}
