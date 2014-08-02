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

using System;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for image value initialization.
    /// </summary>
    public static class ClearImageExtensions
    {
        /// <summary>
        /// Sets each image byte to zero (using kernel function memset).
        /// </summary>
        public static void Clear(this IImage image)
        {
            SetByte(image.ImageData, image.Width * image.ColorInfo.Size, image.Height, image.Stride, 0);
        }

        private unsafe static void SetByte(IntPtr ptr, int rowLengthInBytes, int height, int stride, byte value = 0)
        {
            if (rowLengthInBytes == stride)
                AForge.SystemTools.SetUnmanagedMemory(ptr, value, rowLengthInBytes * height);
            else
            {
                for (int r = 0; r < height; r++)
                {
                    AForge.SystemTools.SetUnmanagedMemory(ptr, value, rowLengthInBytes);
                    ptr += stride;
                }
            }
        }

    }
}
