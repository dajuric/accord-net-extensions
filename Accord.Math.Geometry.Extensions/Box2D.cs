using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace Accord.Math.Geometry
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Box2D
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

        /// <summary>
        /// Creates new structure from area and angle.
        /// </summary>
        /// <param name="rect">Area.</param>
        /// <param name="angle">Angle in degrees.</param>
        public Box2D(Rectangle rect, float angle)
        {
            this.Center = rect.Center();
            this.Size = rect.Size;
            this.Angle = angle;
        }

        /// <summary>
        /// Gets empty structure.
        /// </summary>
        public static Box2D Empty
        {
            get { return new Box2D(); }
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
        /// <param name="useScreenCoordinateSystem">During vertex rotation wheather to use standard Cartesian space or screen coordinate space (y-inverted).</param>
        /// <returns>Vertices.</returns>
        public PointF[] GetVertices(bool useScreenCoordinateSystem = false)
        {
            PointF center = this.Center;
            float angleRad = (float)Accord.Math.Geometry.Angle.ToRadians(this.Angle);
          
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
    }
}
