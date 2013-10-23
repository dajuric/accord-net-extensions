using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Math.Geometry
{
    public static class RectangleExtensions
    {
        public static float IntersectionPercent(this RectangleF rect1, RectangleF rect2)
        {
            float rect1Area = rect1.Width * rect1.Height;
            float rect2Area = rect2.Width * rect2.Height;

            RectangleF interesectRect = RectangleF.Intersect(rect1, rect2);
            float intersectRectArea = interesectRect.Width * interesectRect.Height;

            float minRectArea = System.Math.Min(rect1Area, rect2Area);

            return (float)intersectRectArea / minRectArea;
        }

        public static Rectangle Inflate(this Rectangle rect, double widthScale, double heightScale, Size constrainedArea = default(Size))
        {
            Rectangle newRect = new Rectangle
            {
                X = (int)(rect.X - rect.Width * widthScale),
                Y = (int)(rect.Y - rect.Width * heightScale),
                Width = (int)(rect.Width + 2 * rect.Width * widthScale),
                Height = (int)(rect.Height + 2 * rect.Height * heightScale)
            };

            if (constrainedArea.IsEmpty == false)
                newRect.Intersect(new Rectangle(Point.Empty, constrainedArea));

            return newRect;
        }

        public static int Area(this Rectangle rect)
        {
            return rect.Width * rect.Height;
        }

        public static PointF Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static Point[] Vertices(this Rectangle rect)
        {
            return new Point[] 
            { 
                new Point(rect.X, rect.Y), //left-upper
                new Point(rect.Right, rect.Y), //right-upper
                new Point(rect.X, rect.Bottom), //left-bottom
                new Point(rect.Right, rect.Bottom) //right-bottom
            };
        }

    }
}
