using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Core
{
    /// <summary>
    /// Class that represents array that is pinned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
            if (handle != null && handle.IsAllocated) //this function is called for the first time
            {
                handle.Free();
                this.Array = null;
                this.Data = IntPtr.Zero;
            }
        }

        ~PinnedArray()
        {
            Dispose();
        }

        /// <summary>
        /// Internal pinned array.
        /// </summary>
        public T[] Array { get; private set; }
        /// <summary>
        /// Array's length in bytes.
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
