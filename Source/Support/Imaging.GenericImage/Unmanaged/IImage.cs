#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
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

using Accord.Extensions;
using System;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents interface to the <see cref="Image&lt;TColor&gt;"/> class.
    /// </summary>
    public interface IImage: IDisposable, IEquatable<IImage>
    {
        /// <summary>
        /// Gets unmanaged image data.
        /// </summary>
        IntPtr ImageData { get; }
        /// <summary>
        /// Gets image width.
        /// </summary>
        int Width { get; }
        /// <summary>
        /// Gets image height.
        /// </summary>
        int Height { get; }
        /// <summary>
        /// Gets image stride.
        /// </summary>
        int Stride { get; }
        /// <summary>
        /// Gets image size.
        /// </summary>
        Size Size { get; }
        /// <summary>
        /// Gets image color info.
        /// </summary>
        ColorInfo ColorInfo { get; }
        /// <summary>
        /// Gets image data at specified location.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <returns>Data pointer.</returns>
        IntPtr GetData(int row, int col);
        /// <summary>
        /// Gets image data at specified location.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>Data pointer.</returns>
        IntPtr GetData(int row);
        /// <summary>
        /// Gets sub-image from specified area. Data is shared.
        /// </summary>
        /// <param name="rect">Area of an image for sub-image creation.</param>
        /// <returns>Sub-image.</returns>
        IImage GetSubRect(Rectangle rect);
    }
}
