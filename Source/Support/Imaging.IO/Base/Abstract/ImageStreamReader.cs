#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accord.Extensions.Imaging;
using System;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Image stream reader abstract class. 
    /// See generic class also.
    /// </summary>
    public abstract class ImageStreamReader: ImageStreamReader<IImage>
    { }

    /// <summary>
    /// Image stream writer abstract class. 
    /// It is the base class for classes providing image stream reading.
    /// </summary>
    /// <typeparam name="TImage">Image type.</typeparam>
    public abstract class ImageStreamReader<TImage> : ImageStream<TImage>, IEnumerable<TImage>
    {
        /// <summary>
        /// Initializes a new instance of the image reader class.
        /// </summary>
        protected ImageStreamReader()
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
            return new ImageStreamReaderEnumerator<TImage>(this);
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

    /// <summary>
    /// Enumerator for the image stream.
    /// <para>Stream must support seek operation.</para>
    /// </summary>
    /// <typeparam name="TImage">Image type.</typeparam>
    public class ImageStreamReaderEnumerator<TImage> : IEnumerator<TImage>
    {
        ImageStreamReader<TImage> streamableSource;
        long length = -1;
        int position;

        /// <summary>
        /// Creates new image stream iterator.
        /// </summary>
        /// <param name="streamableSource">Image stream.</param>
        public ImageStreamReaderEnumerator(ImageStreamReader<TImage> streamableSource)
        {
            this.streamableSource = streamableSource;
            this.length = streamableSource.Length;

            Reset();
        }

        /// <summary>
        /// Moves the position of the iterator by 1.
        /// </summary>
        /// <returns>True if the position increment is valid, false otherwise.</returns>
        public bool MoveNext()
        {
            position++;
            return position < length;
        }

        /// <summary>
        /// Resets the enumerator,
        /// </summary>
        public void Reset()
        {
            streamableSource.Seek(0, SeekOrigin.Begin);
            position = -1;
        }

        /// <summary>
        /// Gets the current image within the stream.
        /// </summary>
        public TImage Current
        {
            get 
            {
                var realPos = streamableSource.Position;

                if (position != realPos)
                    streamableSource.Seek(position, SeekOrigin.Begin);

                var currentImage = streamableSource.Read();
                return currentImage;
            }
        }

        /// <summary>
        /// Gets the current image within the stream.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        bool isDisposed = false;
        /// <summary>
        /// Disposes the iterator and resets the position within the stream.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                Reset();
                isDisposed = true;
            }
        }
    }

    /// <summary>
    /// Provides extensions for image stream.
    /// </summary>
    public static class ImageStreamReaderExtensions
    {
        /// <summary>
        /// Calls read function defined by the stream and converts an returned image if necessary.
        /// <para>If the image can not be read (null), null is returned.</para>
        /// </summary>
        /// <param name="imageStream">Image stream.</param>
        /// <returns>Converted image or null if the image can not be read.</returns>
        public static Image<TColor> ReadAs<TColor>(this ImageStreamReader<IImage> imageStream)
            where TColor: struct
        {
            var image = imageStream.Read();
            if (image == null)
                return null;

            return image as Image<TColor>;
        }

        /// <summary>
        /// Reads an element form the input stream and fills the specified buffer.
        /// <para>If the read element does not match the specified type a null value will be written.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="imageStream">Image source stream.</param>
        /// <param name="buffer">
        /// Buffer to write to.
        /// <para>The specified buffer can be null, as it is managed by the function itself.</para>
        /// </param>
        public static void ReadTo<TColor>(this ImageStreamReader<IImage> imageStream, ref TColor[,] buffer)
            where TColor: struct
        { 
            imageStream.ReadAs<TColor>().CopyToOrCreate(ref buffer);
        }
    }
}
