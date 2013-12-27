using Accord.Imaging;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using Point = AForge.IntPoint;

namespace LINE2D.QueryImage
{
    public unsafe static class SIMDArithemtics
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("SIMDArrayInstructions.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AddByteToByteVector(byte* srcAddr, byte* dstAddr, int numOfElemsToAdd);

        public static void AddTo(this Image<Gray, byte> src, Image<Gray, byte> dst, Point srcOffset)
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

        [SuppressUnmanagedCodeSecurity]
        [DllImport("SIMDArrayInstructions.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AddByteToShortVector(byte* srcAddr, short* dstAddr, int numOfElemsToAdd);

        public static void AddTo(this Image<Gray, byte> src, Image<Gray, short> dst)
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

        public static int MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE = Byte.MaxValue / GlobalParameters.MAX_FEATURE_SIMILARITY;
    }
}
