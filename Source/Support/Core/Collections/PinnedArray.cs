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
using System.Runtime.InteropServices;

namespace Accord.Extensions
{
    /// <summary>
    /// Class that represents the pinned array.
    /// </summary>
    /// <typeparam name="T">Generic type of an structure. The structure must have blittable types.</typeparam>
    public class PinnedArray<T>: IDisposable, IEquatable<PinnedArray<T>> where T: struct
    {
        GCHandle handle;

        /// <summary>
        /// Constructs pinned array. (allocation)
        /// </summary>
        /// <param name="length">Number of elements.</param>
        public PinnedArray(int length)
        {
            Array = new T[length];
            initialize(Array);
        }

        /// <summary>
        /// Constructs pinned array. (pinns array - data is not copied)
        /// </summary>
        /// <param name="array">Input array</param>
        public PinnedArray(T[] array)
        {
            this.Array = array;
            initialize(Array);
        }

        /// <summary>
        /// Constructs pinned array. (data is copied from data source)
        /// </summary>
        /// <param name="length">Number of elements.</param>
        /// <param name="dataSource">Pointer to data.</param>
        public PinnedArray(int length, IntPtr dataSource)
        {
            Array = new T[length];
            initialize(Array);

            AForge.SystemTools.CopyUnmanagedMemory(this.Data, dataSource, this.SizeInBytes);
        }

        private void initialize(T[] array)
        {
            handle = GCHandle.Alloc(Array, GCHandleType.Pinned);

            this.Data = handle.AddrOfPinnedObject();
            this.SizeInBytes = Array.Length * Marshal.SizeOf(default(T));
        }

        /// <summary>
        /// Disposes pinned array (frees alocated handle). 
        /// </summary>
        public void Dispose()
        {
            if (handle.IsAllocated) //this function is called for the first time
            {
                handle.Free();
                this.Array = null;
                this.Data = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Destructs pinned array (releases pinning handle).
        /// </summary>
        ~PinnedArray()
        {
            Dispose();
        }

        /// <summary>
        /// Internal pinned array.
        /// </summary>
        public T[] Array { get; private set; }
        /// <summary>
        /// Length of the array in bytes.
        /// </summary>
        public int SizeInBytes { get; private set; }
        /// <summary>
        /// Unmanaged data pointer.
        /// </summary>
        public IntPtr Data { get; private set; }

        /// <summary>
        /// Compares two arrays by checking address and length. (no data comparing).
        /// </summary>
        /// <param name="other">Second array.</param>
        /// <returns>Are equal or not.</returns>
        public bool Equals(PinnedArray<T> other)
        {
            if (other.Data != null &&
                this.Data == other.Data &&
                this.SizeInBytes == other.SizeInBytes)
            {
                return true;
            }

            return false;
        }
    }
}
