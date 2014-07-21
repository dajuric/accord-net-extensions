using System;

namespace LINE2D
{
    /// <summary>
    /// Line2D feature.
    /// </summary>
    public class Feature : ICloneable
    {
        /// <summary>
        /// X location.
        /// </summary>
        public int X;
        /// <summary>
        /// Y location.
        /// </summary>
        public int Y;
        /// <summary>
        /// Quantized angle - binary representation (1, 2, 4,...).
        /// </summary>
        public readonly byte AngleBinaryRepresentation;
        /// <summary>
        /// Quantized angle index.
        /// </summary>
        public readonly byte AngleIndex;

        private Feature() { }

        /// <summary>
        /// Creates new feature.
        /// </summary>
        /// <param name="x">X location.</param>
        /// <param name="y">Y location.</param>
        /// <param name="angleBinaryRepresentation">Angle - binary representation.</param>
        public Feature(int x, int y, byte angleBinaryRepresentation)
        {
            this.X = x;
            this.Y = y;
            this.AngleBinaryRepresentation = angleBinaryRepresentation;
            this.AngleIndex = GetAngleIndex(angleBinaryRepresentation);
        }

        /// <summary>
        /// Clones the feature.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the binary representation of the quantized angle.
        /// </summary>
        /// <param name="angleIndex">Quantized angle index.</param>
        /// <returns>The binary representation of the quantized angle</returns>
        public static byte GetAngleBinaryForm(byte angleIndex)
        {
            return (byte)(1 << angleIndex);
        }
    }
}
