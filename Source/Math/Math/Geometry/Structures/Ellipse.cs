using System;
using System.Runtime.InteropServices;
using PointF = AForge.Point;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Math.Geometry
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Ellipse
    {
        /// <summary>
        /// Area center.
        /// </summary>
        public PointF Center;
        /// <summary>
        /// Area size.
        /// </summary>
        public SizeF Size;
        /// <summary>
        /// Angle in degrees.
        /// </summary>
        public float Angle;

        public static explicit operator Ellipse(Box2D box)
        {
            return new Ellipse { Center = box.Center, Size = box.Size, Angle = box.Angle };
        }

        public static explicit operator Box2D(Ellipse ellipse)
        {
            return new Box2D { Center = ellipse.Center, Size = ellipse.Size, Angle = ellipse.Angle };
        }

        public static Ellipse Fit(double[,] covMatrix, PointF center = default(PointF))
        {
            if (covMatrix.ColumnCount() != 2 || covMatrix.RowCount() != 2)
                throw new ArgumentException("Covariance matrix must have the same dimensions, and the dimension length must be 2!");

            return Fit(covMatrix[0, 0], covMatrix[0, 1], covMatrix[1, 1], center);
        }

        public static Ellipse Fit(double a, double b, double c, PointF center, out bool success)
        {  
            var acDiff = a - c;
            var acSum = a + c;

            //A * X = lambda * X => solve quadratic equation => lambda1/2 = [a + c +/- sqrt((a-c)^2 + 4b^2)]  / 2
            var sqrtDiscriminant = System.Math.Sqrt(acDiff * acDiff + 4 * b * b);

            var eigVal1 = (acSum + sqrtDiscriminant) / 2;
            var eigVal2 = (acSum - sqrtDiscriminant) / 2;

            //A * X = lambda * X => y / x = b / (lambda - c); where lambda is the first eigen-value
            var angle = System.Math.Atan2(2 * b, (acDiff + sqrtDiscriminant));

            success = eigVal1 > 0 && eigVal2 > 0;
            return new Ellipse
            {
                Center = center,
                Size = new SizeF 
                {
                    Width = (float)System.Math.Sqrt(eigVal1) * 4,
                    Height =(float)System.Math.Sqrt(eigVal2) * 4
                },
                Angle =  (float)Accord.Extensions.Math.Geometry.Angle.ToDegrees(angle) 
            };
        }

        public static Ellipse Fit(double a, double b, double c, PointF center = default(PointF))
        {
            bool success;
            return Fit(a, b, c, center, out success);
        }
    }
}
