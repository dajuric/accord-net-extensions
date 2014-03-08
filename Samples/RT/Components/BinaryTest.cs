using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Point = AForge.IntPoint;
using Accord.Extensions.Math.Geometry;
using NGenerics.DataStructures.Trees;
 
namespace RT
{
    public struct BytePoint
    {
        public byte X;
        public byte Y;
    }

    public class Pair<T>
    {
        public T First;
        public T Second;

        public override bool Equals(object obj)
        {
            if (obj == null || obj is Pair<T> == false)
                return false;

            var pair = obj as Pair<T>;

            if (pair.First.Equals(this.First) && 
                pair.Second.Equals(this.Second))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("<{0}, {1}>", First, Second);
        }
    }

    public class BinTestCode: Pair<BytePoint>
    {
        public unsafe BinTestCode(int binaryCode)
        { 
            byte* ptr = (byte*)&binaryCode;

            First.Y  = ptr[0]; First.X  = ptr[1];
            Second.Y = ptr[2]; Second.X = ptr[3];
        }

        public void ToRealCoordinates(Rectangle region, out Point ptA, out Point ptB)
        {
            ptA = toRealCoordinates(this.First, region);
            ptB = toRealCoordinates(this.Second, region);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Point toRealCoordinates(BytePoint point, Rectangle region)
        {
            const int NORMALIZATION_CONST = Byte.MaxValue + 1;

            var pt = new Point 
            {
                X = (NORMALIZATION_CONST * region.X + region.Width  * point.X) / NORMALIZATION_CONST,
                Y = (NORMALIZATION_CONST * region.Y + region.Height * point.Y) / NORMALIZATION_CONST
            };

            return pt;
        }

        public unsafe bool Test(Image<Gray, byte> image, Rectangle region, bool testImageBounds = false)
        {
            return false;

            Point ptA, ptB;
            ToRealCoordinates(region, out ptA, out ptB);
        
            if (testImageBounds)
            {
                ptA = ptA.Intersect(image.Size);
                ptB = ptB.Intersect(image.Size);
            }

            var valA = *(byte*)image.GetData(ptA.Y, ptA.X);
            var valB = *(byte*)image.GetData(ptB.Y, ptB.X);

            return valA <= valB;
        }
    }

    public static class IntPointExtensions
    {
        public static Point Intersect(this Point point, Size size)
        {
            return new Point 
            {
                X = Math.Min(Math.Max(0, point.X), size.Width - 1),
                Y = Math.Min(Math.Max(0, point.Y), size.Height - 1)
            };
        }
    }

}
