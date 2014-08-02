#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using RangeF = AForge.Range;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using LineSegment2DF = AForge.Math.Geometry.LineSegment;

namespace Accord.Extensions.Math.Geometry //TODO: update Vector2D class according to Vector3D
{
    /// <summary>
    /// Represents 2D Vector structure.
    /// </summary>
    public class Vector2D
    {
        /// <summary>
        /// Gets an empty 2D vector.
        /// </summary>
        public static readonly Vector2D Empty = new Vector2D(0f, 0f); 

        /// <summary>
        /// Creates new <see cref="Vector2D"/> structure.
        /// </summary>
        /// <param name="directionX">Horizontal component.</param>
        /// <param name="directionY">Vertical component.</param>
        public Vector2D(float directionX, float directionY)
        {
            this.X = directionX;
            this.Y = directionY;

            this.Length = System.Math.Sqrt(System.Math.Pow(this.X, 2) + System.Math.Pow(this.Y, 2));
        }

        /// <summary>
        /// Creates new <see cref="Vector2D"/> structure.
        /// </summary>
        /// <param name="startPoint">Start point.</param>
        /// <param name="endPoint">End point.</param>
        public Vector2D(PointF startPoint, PointF endPoint)
            : this(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y)
        { }

        /// <summary>
        /// Creates new <see cref="Vector2D"/> structure.
        /// </summary>
        /// <param name="angleRad">Vector direction angle in radians.</param>
        /// <param name="scale">Vector scale.</param>
        public Vector2D(double angleRad, float scale = 1)
        {
            this.X = (float)System.Math.Cos(angleRad) * scale;
            this.Y = (float)System.Math.Sin(angleRad) * scale;
        }

        /// <summary>
        /// Gets the horizontal vector component.
        /// </summary>
        public float X { get; private set; }
        /// <summary>
        /// Gets the vertical vector component.
        /// </summary>
        public float Y { get; private set; }

        /// <summary>
        /// Gets the vector magnitude.
        /// </summary>
        public double Length { get; private set; }

        /// <summary>
        /// Changes vector direction.
        /// </summary>
        /// <returns>Negated vector.</returns>
        public Vector2D Negate()
        {
            return new Vector2D(-X, -Y);
        }

        /// <summary>
        /// Normalizes the vector with its length and produces a new vector.
        /// </summary>
        /// <returns>Normalized vector.</returns>
        public Vector2D Normalize()
        {
            var norm = (float)this.Length;
            var v = new Vector2D(this.X / norm, this.Y / norm);
            return v;
        }

        /// <summary>
        /// Calculates angle between vectors (in radians).
        /// <seealso cref="Angle"/>
        /// </summary>
        /// <param name="v1">First vector.</param>
        /// <param name="v2">Second vector.</param>
        /// <returns>Angle between vectors in radians.</returns>
        public static double Angle(Vector2D v1, Vector2D v2)
        {
            double cosAngle = DotProduct(v1, v2);
            double angleRad = System.Math.Acos(cosAngle);
            return angleRad;
        }

        /// <summary>
        /// Calculates vectors dot product.
        /// </summary>
        /// <param name="v1">First vector.</param>
        /// <param name="v2">Second vector.</param>
        /// <returns>Dot product magnitude.</returns>
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

        /// <summary>
        /// Gets whether two vectors have opposite directions or not.
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>True if two vectors have opposite directions, otherwise false.</returns>
        public static bool AreOpositeDirection(Vector2D v1, Vector2D v2)
        {
            return v1.X == -v2.X && v1.Y == -v2.Y;
        }

        /// <summary>
        /// Converts line segment into vector.
        /// </summary>
        /// <param name="line">Line segment.</param>
        /// <returns>New <see cref="Vector2D"/>.</returns>
        public static explicit operator Vector2D(LineSegment2DF line)
        {
            return new Vector2D(line.Start, line.End);
        }

        /// <summary>
        /// Converts direction represented as point into vector.
        /// </summary>
        /// <param name="direction">Vector direction.</param>
        /// <returns>New <see cref="Vector2D"/>.</returns>
        public static explicit operator Vector2D(PointF direction)
        {
            return new Vector2D(direction.X, direction.Y);
        }

        //TODO: this code belongs to point structure
        #region Add point functions 

        /// <summary>
        /// Adds point and <see cref="Vector2D"/>.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <returns> Translated point.</returns>
        public PointF Add(PointF point)
        {
            return Add(point, this);
        }

        /// <summary>
        /// Adds point and <see cref="Vector2D"/>.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <param name="vector">Vector.</param>
        /// <returns> Translated point. </returns>
        public static PointF Add(PointF point, Vector2D vector)
        {
            return new PointF 
            {
                X = point.X + vector.X,
                Y = point.Y + vector.Y
            };
        }

        /// <summary>
        /// Adds point and <see cref="Vector2D"/>.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <param name="vector">Vector.</param>
        /// <returns> Translated point. </returns>
        public static PointF operator +(PointF point, Vector2D vector)
        {
            return Add(point, vector);
        }

        #endregion

        #region Multiply

        /// <summary>
        /// Multiplies vector and scale.
        /// </summary>
        /// <param name="scale">Scale</param>
        /// <returns>New scaled <see cref="Vector2D"/>.</returns>
        public Vector2D Multiply(float scale)
        {
            return Multiply(this, scale);
        }

        /// <summary>
        /// Multiplies vector and scale.
        /// </summary>
        /// <param name="vector"><see cref="Vector2D"/>.</param>
        /// <param name="scale">Scale</param>
        /// <returns>New scaled <see cref="Vector2D"/>.</returns>
        public static Vector2D Multiply(Vector2D vector, float scale)
        {
            return new Vector2D(vector.X * scale, vector.Y * scale);
        }

        /// <summary>
        /// Multiplies vector and scale.
        /// </summary>
        /// <param name="vector"><see cref="Vector2D"/>.</param>
        /// <param name="scale">Scale</param>
        /// <returns>New scaled <see cref="Vector2D"/>.</returns>
        public static Vector2D operator *(Vector2D vector, float scale)
        {
            return Multiply(vector, scale);
        }

        #endregion

        /// <summary>
        /// Gets whether the specified object has the same data as this object.
        /// </summary>
        /// <param name="obj">Specified object.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            var v = obj as Vector2D;
            if (v == null) return false;

            return this.X == v.X && this.Y == v.Y;
        }

        /// <summary>
        /// Gets object hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Converts <see cref="Vector2D"/> to point that represents direction.
        /// </summary>
        /// <param name="vector"><see cref="Vector2D"/> structure.</param>
        /// <returns>Direction as point.</returns>
        public static explicit operator PointF(Vector2D vector)
        {
            return new PointF(vector.X, vector.Y);
        }
    }
}
