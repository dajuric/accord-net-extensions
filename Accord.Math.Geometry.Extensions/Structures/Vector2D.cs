using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RangeF = AForge.Range;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using LineSegment2DF = AForge.Math.Geometry.LineSegment;

namespace Accord.Math.Geometry
{
    public class Vector2D
    {
        public Vector2D(float directionX, float directionY)
        {
            this.X = directionX;
            this.Y = directionY;

            this.Length = System.Math.Sqrt(System.Math.Pow(this.X, 2) + System.Math.Pow(this.Y, 2));
        }

        public Vector2D(PointF startPoint, PointF endPoint)
            : this(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y)
        { }

        public float X { get; private set; }
        public float Y { get; private set; }

        public double Length { get; private set; }

        public Vector2D Negate()
        {
            return new Vector2D(-X, -Y);
        }

        public static int Angle(Vector2D v1, Vector2D v2)
        {
            double cosAngle = DotProduct(v1, v2);

            double angleRad = System.Math.Acos(cosAngle);
            int angle = (int)(angleRad * 180 / System.Math.PI);

            return angle;
        }

        public static double DotProduct(Vector2D v1, Vector2D v2)
        {
            double dotProduct = v1.X * v2.X + v1.Y * v2.Y;
            dotProduct /= v1.Length;
            dotProduct /= v2.Length;

            dotProduct = System.Math.Min(dotProduct, 1);

            return dotProduct;
        }

        /// <summary>
        /// Vector obtained by cross product in 2D is facing toward Z direction (other coordinates are zero)
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>Vector signed magnitude in Z direction.</returns>
        public static double CrossProduct(Vector2D v1, Vector2D v2)
        {
            return (v1.X * v2.Y) - (v1.Y * v2.X);
        }

        public static bool AreOpositeDirection(Vector2D v1, Vector2D v2)
        {
            return v1.X == -v2.X && v1.Y == -v2.Y;
        }

        public static explicit operator Vector2D(LineSegment2DF line)
        {
            return new Vector2D(line.Start, line.End);
        }

        public override bool Equals(object obj)
        {
            var v = obj as Vector2D;
            if (v == null) return false;

            return this.X == v.X && this.Y == v.Y;
        }
    }
}
