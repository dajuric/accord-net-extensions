using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using Point = AForge.IntPoint;
 
namespace RT
{
    /// <summary>
    /// Represents normalized point pair [0..255] used as binary test code in <see cref="PicoClassifier"/>.
    /// </summary>
    public struct BinTestCode<TColor>
        where TColor : IColor
    {
        public Point<sbyte> FirstLocation;
        public byte FirstChannel;

        public Point<sbyte> SecondLocation;
        public byte SecondChannel;

        public unsafe BinTestCode(IList<sbyte> code)
        {
            if (code.Count != PackedLength)
                throw new ArgumentException("The number of bytes must be equal to the PackedLength!");

            FirstLocation.Y  = code[0]; FirstLocation.X  = code[1];
            SecondLocation.Y = code[2]; SecondLocation.X = code[3];

            if (!isGrayscale) //the code size will be 4
            {
                var nChannels = ColorInfo.GetInfo<TColor, byte>().NumberOfChannels;

                FirstChannel  = (byte)((byte)code[4] % nChannels);
                SecondChannel = (byte)((byte)code[5] % nChannels);
            }
            else
            {
                FirstChannel = 0; SecondChannel = 0;
            }
        }

        public unsafe bool Test(Image<TColor, byte> image, Point regionCenter, Size regionSize)
        {
            const int NORMALIZATION_CONST = Byte.MaxValue + 1;

            var xA = (NORMALIZATION_CONST * regionCenter.X + FirstLocation.X * regionSize.Width)  / NORMALIZATION_CONST;
            var yA = (NORMALIZATION_CONST * regionCenter.Y + FirstLocation.Y * regionSize.Height) / NORMALIZATION_CONST;

            var xB = (NORMALIZATION_CONST * regionCenter.X + SecondLocation.X * regionSize.Width) / NORMALIZATION_CONST;
            var yB = (NORMALIZATION_CONST * regionCenter.Y + SecondLocation.Y * regionSize.Height) / NORMALIZATION_CONST;

            var valA = ((byte*)image.GetData(yA, xA))[FirstChannel];
            var valB = ((byte*)image.GetData(yB, xB))[SecondChannel];

            return valA <= valB;
        }

        #region Serialization

        static bool isGrayscale = typeof(TColor).Equals(typeof(Gray));

        public IList<sbyte> ToBytes()
        {
            List<sbyte> bytes = new List<sbyte>();

            bytes.Add(FirstLocation.Y);  bytes.Add(FirstLocation.X);
            bytes.Add(SecondLocation.Y); bytes.Add(SecondLocation.X);

            if (!isGrayscale) //make compatible with Markuš implementation
            {
                bytes.Add((sbyte)FirstChannel);
                bytes.Add((sbyte)SecondChannel);
            }

            return bytes;
        }

        public static int PackedLength
        {
            get
            {
                return !isGrayscale ? 
                       (2 /*first location*/ + 2 /*second location*/ + 1 /*first channel*/ + 1 /*second channel*/) 
                       : sizeof(int);
            }
        }

        #endregion
    }
}
