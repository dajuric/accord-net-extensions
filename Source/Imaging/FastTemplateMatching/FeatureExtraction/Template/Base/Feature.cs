using System;

namespace LINE2D
{
    public class Feature : ICloneable
    {
        public int X;
        public int Y;
        public readonly byte AngleBinaryRepresentation;
        public readonly byte AngleIndex;

        private Feature() { }

        public Feature(int x, int y, byte angleBinaryRepresentation)
        {
            this.X = x;
            this.Y = y;
            this.AngleBinaryRepresentation = angleBinaryRepresentation;
            this.AngleIndex = GetAngleIndex(angleBinaryRepresentation);
        }

        public Feature Clone()
        {
            return new Feature(X, Y, AngleBinaryRepresentation);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Calculate Log2(angleBinRepr)
        /// </summary>
        public static byte GetAngleIndex(byte angleBinRepr)
        {
            const int MAX_NUM_OF_SHIFTS = 8; //number of bits per byte
            byte numRightShifts = 0;

            while ((angleBinRepr & 1) == 0 && numRightShifts < MAX_NUM_OF_SHIFTS)
            {
                angleBinRepr = (byte)(angleBinRepr >> 1);
                numRightShifts++;
            }

            if (numRightShifts == MAX_NUM_OF_SHIFTS)
                return 0;
            else
                return numRightShifts;
        }

        public static byte GetAngleBinaryForm(byte angleIndex)
        {
            return (byte)(1 << angleIndex);
        }
    }
}
