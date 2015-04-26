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

using Accord.Extensions;
using Accord.Extensions.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// Contains extension methods for 8-bit and 16-vector addition by suing fast SIMD arithmetics.
    /// <para>The class depends on unmanaged project SIMDArrayInstructions.</para>
    /// </summary>
    public unsafe static class SIMDArithemtics
    {
        /// <summary>
        /// Adds two byte vectors.
        /// </summary>
        /// <param name="srcAddr">Source address.</param>
        /// <param name="dstAddr">Destination address.</param>
        /// <param name="numOfElemsToAdd">The number of elements (bytes) to add.</param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("SIMDArrayInstructions.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AddByteToByteVector(byte* srcAddr, byte* dstAddr, int numOfElemsToAdd);

        /// <summary>
        /// Adds two 8-bit gray images.
        /// <para>Source and destination image must have the size.</para>
        /// </summary>
        /// <param name="src">Source image.</param>
        /// <param name="dst">Destination image.</param>
        /// <param name="srcOffset">The point in source image.</param>
        public static void AddTo(this Image<Gray<byte>> src, Image<Gray<byte>> dst, Point srcOffset)
        {
            Debug.Assert(src.Width == dst.Width && src.Height == dst.Height);

            byte* srcRow = (byte*)src.GetData(srcOffset.Y);
            byte* dstRow = (byte*)dst.ImageData;

            if (src.Stride == src.Width && dst.Stride == dst.Width)
            {
                int numElemsToAdd = src.Width * src.Height - (srcOffset.Y * src.Width + srcOffset.X);
                AddByteToByteVector(srcRow + srcOffset.X, dstRow, numElemsToAdd);
            }
            else
            {
                //first row
                int nElemsToAddFirstRow = src.Width - srcOffset.X;
                AddByteToByteVector(srcRow + srcOffset.X, dstRow, nElemsToAddFirstRow);

                //other rows
                srcRow += src.Stride;
                dstRow += dst.Stride;

                for (int r = srcOffset.Y + 1; r < src.Height; r++)
                {
                    AddByteToByteVector(srcRow, dstRow, src.Width);
                    srcRow += src.Stride;
                    dstRow += dst.Stride;
                }
            }
        }

        /// <summary>
        /// Adds 8-bit to 16-bit vector using SIMD instructions.
        /// </summary>
        /// <param name="srcAddr">Source address.</param>
        /// <param name="dstAddr">Destination address.</param>
        /// <param name="numOfElemsToAdd">Number of elements to add.</param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("SIMDArrayInstructions.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AddByteToShortVector(byte* srcAddr, short* dstAddr, int numOfElemsToAdd);

        /// <summary>
        /// Adds 8-bit gray image to 16-bit destination image.
        /// <para>Source and destination image must have the size.</para>
        /// </summary>
        /// <param name="src">Source image.</param>
        /// <param name="dst">Destination image.</param>
        public static void AddTo(this Image<Gray<byte>> src, Image<Gray<short>> dst)
        {
            Debug.Assert(src.Width == dst.Width && src.Height == dst.Height);

            byte* srcRow = (byte*)src.ImageData;
            short* dstRow = (short*)dst.ImageData;

            if (src.Stride == src.Width && dst.Stride == dst.Width * sizeof(short))
            {
                int numElemsToAdd = src.Width * src.Height;
                AddByteToShortVector(srcRow, dstRow, numElemsToAdd);
            }
            else
            {
                for (int r = 0; r < src.Height; r++)
                {
                    AddByteToShortVector(srcRow, dstRow, src.Width);
                    srcRow += src.Stride;
                    dstRow = (short*)((byte*)dstRow + dst.Stride);
                }
            }
        }

        /// <summary>
        /// Initializes SIMD arithmetics by adding unmanaged library directory to the search path.
        /// </summary>
        static SIMDArithemtics()
        {
            Platform.AddDllSearchPath();
        }
    }
}
