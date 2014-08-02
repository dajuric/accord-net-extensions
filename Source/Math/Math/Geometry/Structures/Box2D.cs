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

using System.Linq;
using System.Runtime.InteropServices;
using PointF = AForge.Point;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Box2D equivalent of OpenCV's Box2D Class.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Box2D
    {
         /// <summary>
        /// Gets empty structure.
        /// </summary>
        public static readonly Box2D Empty = new Box2D();

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

        /// <summary>
        /// Creates new structure from area and angle.
        /// </summary>
        /// <param name="rect">Area.</param>
        /// <param name="angle">Angle in degrees.</param>
        public Box2D(RectangleF rect, float angle)
            :this(rect.Center(), rect.Size, angle)
        {}

        /// <summary>
        /// Creates new structure from area and angle.
        /// </summary>
        /// <param name="center">Box2D center.</param>
        /// <param name="size">Box 2D size.</param>
        /// <param name="angle">Angle in degrees.</param>
        public Box2D(PointF center, SizeF size, float angle)
        {
            this.Center = new PointF(center.X, center.Y);
            this.Size = size;
            this.Angle = angle;
        }

        /// <summary>
        /// Returns true if the structure is  empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Equals(Empty); }
        }

        /// <summary>
        /// Gets the minimum enclosing rectangle for this box.
        /// </summary>
        public RectangleF GetMinArea()
        { 
            var vertices = this.GetVertices();

            float minX = vertices.Min(x => x.X);
            float maxX = vertices.Max(x => x.X);

            float minY = vertices.Min(x => x.Y);
            float maxY = vertices.Max(x => x.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Gets vertices.
        /// </summary>
        /// <param name="useScreenCoordinateSystem">During vertex rotation whether to use standard Cartesian space or screen coordinate space (y-inverted).</param>
        /// <returns>Vertices.</returns>
        public PointF[] GetVertices(bool useScreenCoordinateSystem = false)
        {
            PointF center = this.Center;
            float angleRad = (float)Accord.Extensions.Math.Geometry.Angle.ToRadians(this.Angle);
          
            PointF[] nonRotatedVertices = getNonRotatedVertices();
            PointF[] rotatedVertices = nonRotatedVertices.Select(x=> new PointF(x.X - center.X, x.Y - center.Y)) //translate to (0,0)
                                                         .Select(x => x.Rotate(angleRad, useScreenCoordinateSystem)) //rotate
                                                         .Select(x => new PointF(x.X + center.X, x.Y + center.Y)) //translate back
                                                         .ToArray();

            return rotatedVertices;
        }

        private PointF[] getNonRotatedVertices()
        {
            float offsetX = this.Size.Width / 2;
            float offsetY = this.Size.Height / 2;

            return new PointF[] 
            {
                new PointF(this.Center.X - offsetX, this.Center.Y - offsetY), //left-upper
                new PointF(this.Center.X + offsetX, this.Center.Y - offsetY), //right-upper
                new PointF(this.Center.X + offsetX, this.Center.Y + offsetY), //right-bottom
                new PointF(this.Center.X - offsetX, this.Center.Y + offsetY) //left-bottom
            };
        }

        /// <summary>
        /// Converts Rectangle to the Box2D representation (angle is zero).
        /// </summary>
        /// <param name="rect">Rectangle to convert.</param>
        /// <returns>Box2D representation.</returns>
        public static implicit operator Box2D(Rectangle rect)
        {
            return new Box2D(rect, 0);
        }

        /// <summary>
        /// Converts RectangleF to the Box2D representation (angle is zero).
        /// </summary>
        /// <param name="rect">Rectangle to convert.</param>
        /// <returns>Box2D representation.</returns>
        public static implicit operator Box2D(RectangleF rect)
        {
            return new Box2D(rect, 0);
        }

        /// <summary>
        /// Determines whether two objects are equal.
        /// </summary>
        /// <param name="obj">Object to test.</param>
        /// <returns>True if two objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Box2D == false) return false;

            var b = (Box2D)obj;
            return b.Center.Equals(this.Center) && b.Angle.Equals(this.Angle);
        }

        /// <summary>
        /// Gets hash-code for the structure.
        /// </summary>
        /// <returns>Hash-code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
