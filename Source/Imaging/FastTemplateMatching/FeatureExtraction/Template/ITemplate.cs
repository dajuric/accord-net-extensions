using System;
using System.Drawing;

namespace LINE2D.TemplateMatching
{
    public class Feature
    {
        public int X;
        public int Y;

        private byte angleBinRepr;
        public byte AngleBinaryRepresentation
        {
            get { return angleBinRepr; }
            set
            {
                angleBinRepr = value;
                AngleIndex = CalcAngleLabel(angleBinRepr);
            }
        }

        public byte AngleIndex
        {
            get;
            private set;
        }

        public Feature Clone()
        {
            return new Feature
            {
                X = this.X,
                Y = this.Y,
                AngleBinaryRepresentation = this.AngleBinaryRepresentation
            };
        }

        /// <summary>
        /// Calculate Log2(angleBinRepr)
        /// </summary>
        private static byte CalcAngleLabel(byte angleBinRepr)
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

        public static byte CalcAngleBinRepresentation(byte angleLabel)
        {
            return (byte)Math.Pow(2, angleLabel);
        }
    }

    public interface ITemplate
    {
        Feature[] Features { get; }
        Size Size { get; }
        string ClassLabel { get; }
    }
}
