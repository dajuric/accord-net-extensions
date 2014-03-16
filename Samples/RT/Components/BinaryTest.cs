using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Point = AForge.IntPoint;
 
namespace RT
{
    /// <summary>
    /// Represents normalized point pair [0..255] used as binary test code in <see cref="PicoClassifier"/>.
    /// </summary>
    public class BinTestCode : Pair<Point>
    {
        private static int[] sinLUT = new int[360];
        private static int[] cosLUT = new int[360];

        /// <summary>
        /// Initializes structures shared between <see cref="BinTestCode"/> instances.
        /// </summary>
        static BinTestCode()
        {
            initializeAngleLUTs();
        }

        private static void initializeAngleLUTs()
        {
            for (int angle = 0; angle < 360; angle++)
            {
                var angleRad = Angle.ToRadians(angle);

                sinLUT[angle] = (int)(Math.Sin(angleRad) * Byte.MaxValue);
                cosLUT[angle] = (int)(Math.Cos(angleRad) * Byte.MaxValue);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void rotatePoint(ref Point point, Size regionSize, int angleDeg)
        {
            if (angleDeg == 0) //for speed-up
            {
                point.X = point.X * regionSize.Width;
                point.Y = point.Y * regionSize.Height;
            }
            else
            {
                point.X = (point.Y * regionSize.Height * sinLUT[angleDeg] + point.X * regionSize.Width * cosLUT[angleDeg]) / Byte.MaxValue;
                point.Y = (point.Y * regionSize.Height * cosLUT[angleDeg] - point.X * regionSize.Width * sinLUT[angleDeg]) / Byte.MaxValue;
            }
        }

        /// <summary>
        /// Checks whether the provided center point along with region size is within image boundaries.
        /// </summary>
        /// <param name="imageSize">Image size.</param>
        /// <param name="regionCenter">Region center.</param>
        /// <param name="regionSize">Region size.</param>
        /// <param name="isRotated">True if the angle is different than zero (<see cref="Test"/> function).</param>
        /// <returns>True if the provided center along with region size is within image boundaries, false otherwise.</returns>
        public static bool IsInBounds(Size imageSize, Point regionCenter, Size regionSize, bool isRotated = false)
        {
            if (isRotated == false)
            {
                return new RectangleF
                {
                    X = regionSize.Width / 2 + 1,
                    Y = regionSize.Height / 2 + 1,
                    Width = imageSize.Width - regionSize.Width / 2,
                    Height = imageSize.Height - regionSize.Height / 2
                }
                .Contains(regionCenter);
            }
            else //get the worst possible case where the region is rotated by 45 degrees
            {
                const float SIN_45 = 0.707107f;
                const float COS_45 = SIN_45;

                return new RectangleF
                {
                    X = COS_45 * regionSize.Width + 1,
                    Y = SIN_45 * regionSize.Height + 1,
                    Width = imageSize.Width - COS_45 * regionSize.Width,
                    Height = imageSize.Height - SIN_45 * regionSize.Height
                }
                .Contains(regionCenter);
            }
        }

        /// <summary>
        /// Creates new binary code from <see cref="Int32"/> structure.
        /// Format is: [y1, x1, y2, x2] where every data component is of type <see cref="Byte"/>.
        /// </summary>
        /// <param name="binaryCode">Binary test code represented as 32-bit integer value.</param>
        public unsafe BinTestCode(int binaryCode)
        { 
            sbyte* ptr = (sbyte*)&binaryCode;

            First.Y  = ptr[0]; First.X  = ptr[1];
            Second.Y = ptr[2]; Second.X = ptr[3];
        }

        public unsafe int ToInt32()
        {
            int binaryCode = 0;

            sbyte* ptr = (sbyte*)&binaryCode;

            checked
            {
                ptr[0] = (SByte)First.Y;  ptr[1] = (SByte)First.X;
                ptr[2] = (SByte)Second.Y; ptr[3] = (SByte)Second.X;
            }

            return binaryCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Point toRealCoordinates(Point point, Point regionCenter, Size regionSize, int angleDeg = 0)
        {
            const int NORMALIZATION_CONST = Byte.MaxValue + 1;

            //ensure that supplied angle is inside boundaries
            Debug.Assert(angleDeg >= 0 && angleDeg < 360);

            rotatePoint(ref point, regionSize, angleDeg);

            var imagePt = new Point
            {
                X = (NORMALIZATION_CONST * regionCenter.X + point.X) / NORMALIZATION_CONST,
                Y = (NORMALIZATION_CONST * regionCenter.Y + point.Y) / NORMALIZATION_CONST
            };

            return imagePt;
        }

        /// <summary>
        /// Test whether the image intensity for the first point is less or equal than the intensity in location provided by the second point.
        /// <para>See <seealso cref="IsInBounds"/> function.</para>
        /// </summary>
        /// <param name="image">Intensity image.</param>
        /// <param name="regionCenter">Region center.</param>
        /// <param name="regionSize">Region size.</param>
        /// <param name="angleDeg">Rotation in degrees for the test.</param>
        /// <param name="clipToImageBounds">
        /// If the created points are outside image boundaries they will be clipped.
        /// <para>Used for training phase. Default value (false) is for testing phase.</para>
        /// </param>
        /// <returns>whether the image intensity for the first point is less or equal than the intensity for the second point.</returns>
        public unsafe bool Test(Image<Gray, byte> image, Point regionCenter, Size regionSize, int angleDeg = 0, bool clipToImageBounds = false)
        {
            Point ptA = toRealCoordinates(this.First, regionCenter, regionSize, angleDeg);
            Point ptB = toRealCoordinates(this.Second, regionCenter, regionSize, angleDeg);
          
            if (clipToImageBounds)
            {
                ptA = ptA.Clamp(image.Size);
                ptB = ptB.Clamp(image.Size);
            }

            var valA = *(byte*)image.GetData(ptA.Y, ptA.X);
            var valB = *(byte*)image.GetData(ptB.Y, ptB.X);

            return valA <= valB;
        }
    }

}
