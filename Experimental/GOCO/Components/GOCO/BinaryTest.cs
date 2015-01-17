using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Point = AForge.IntPoint;
using System.Runtime.InteropServices;
 
namespace GOCO
{
    [Serializable]
    public struct BinTestCode
    {
        public static int SizeInBytes = Marshal.SizeOf(default(BinTestCode));

        public Point<sbyte> Location;
        public byte Orientation;

        public BinTestCode(IList<sbyte> code)
        {
            if (code.Count != SizeInBytes)
                throw new ArgumentException("The number of bytes must be equal to the PackedLength!");

            Location.Y  = code[0]; Location.X  = code[1];
            Orientation = (byte)(code[2] % 128 + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool Test(Image<Gray, byte> image, Point regionCenter, Size regionSize)
        {
            const int NORMALIZATION_CONST = Byte.MaxValue + 1;

            var x = (NORMALIZATION_CONST * regionCenter.X + Location.X * regionSize.Width)  / NORMALIZATION_CONST;
            var y = (NORMALIZATION_CONST * regionCenter.Y + Location.Y * regionSize.Height) / NORMALIZATION_CONST;

            var val = *(byte*)image.GetData(y, x);

            return (val & Orientation) > 0;
        }

        public bool Test(Image<Gray, byte> image, Rectangle window)
        {
            var center = new Point(window.X + window.Width / 2, window.Y + window.Height / 2);
            return Test(image, center, window.Size);
        }

        public bool Test(Image<Gray, byte> image)
        {
            var center = new Point(image.Width / 2, image.Height / 2);
            return Test(image, center, image.Size);
        }

        static System.Security.Cryptography.RNGCryptoServiceProvider rand = new System.Security.Cryptography.RNGCryptoServiceProvider();
        public static BinTestCode[] CreateRandom(int numberOfTests)
        {
            byte[] buffer = new byte[SizeInBytes];

            return EnumerableExtensions.Create(numberOfTests, (_) =>
            {
                rand.GetBytes(buffer);
                return new BinTestCode((sbyte[])(Array)buffer);
            });
        }
    }
}
