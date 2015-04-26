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

using System;
using System.IO;
using Accord.Extensions.Imaging;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides the base class for the image stream.
    /// </summary>
    /// <typeparam name="TImage">Image type.</typeparam>
    public abstract class ImageStream<TImage>: IDisposable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ImageStream{TImage}"/>.
        /// </summary>
        protected ImageStream()
        {
            this.CanSeek = false;
            this.IsLiveStream = false;
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
        /// Those streams are usually not seek-able. See also: <see cref="CanSeek"/>.
        /// </summary>
        public virtual bool IsLiveStream { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public virtual bool CanSeek { get; protected set; }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A frame index offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="NotSupportedException">Seek operation is not supported by the current stream.</exception>
        public virtual long Seek(long offset, System.IO.SeekOrigin origin = SeekOrigin.Current)
        {
            if (!this.CanSeek)
                throw new NotSupportedException("Seek operation is not supported by the current stream.");

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

            var currentFrame = System.Math.Min(this.Length - 1, System.Math.Max(0, newPosition));
            return currentFrame;
        }

        /// <summary>
        /// Closes the stream and releases all resources.
        /// <para>Use Dispose function rather than Close function.</para>
        /// </summary>
        public virtual void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// When overridden in a derived class, opens the current stream. 
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// When overridden in a derived class, closes the current stream and releases any resources associated with the current stream.
        /// This function is internally called by <see cref="Dispose"/>.
        /// </summary>
        public abstract void Close();
    }
}
