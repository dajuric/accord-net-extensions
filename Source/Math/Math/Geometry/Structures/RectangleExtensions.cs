using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Math.Geometry
{
    public static class RectangleExtensions
    {
        public static Rectangle Intersect(this Rectangle rect, Size area)
        {
            Rectangle newRect = rect;

            if (area.IsEmpty == false)
                newRect.Intersect(new Rectangle(new Point(), area));

            return newRect;
        }

        public static float IntersectionPercent(this RectangleF rect1, RectangleF rect2)
        {
            float rect1Area = rect1.Width * rect1.Height;
            float rect2Area = rect2.Width * rect2.Height;

            RectangleF interesectRect = RectangleF.Intersect(rect1, rect2);
            float intersectRectArea = interesectRect.Width * interesectRect.Height;

            float minRectArea = System.Math.Min(rect1Area, rect2Area);

            return (float)intersectRectArea / minRectArea;
        }

        public static float IntersectionPercent(this Rectangle rect1, Rectangle rect2)
        {
            float rect1Area = rect1.Width * rect1.Height;
            float rect2Area = rect2.Width * rect2.Height;

            Rectangle interesectRect = Rectangle.Intersect(rect1, rect2);
            float intersectRectArea = interesectRect.Width * interesectRect.Height;

            float minRectArea = System.Math.Min(rect1Area, rect2Area);

            return (float)intersectRectArea / minRectArea;
        }

        public static Rectangle Inflate(this Rectangle rect, int width, int height, Size constrainedArea = default(Size))
        {
            Rectangle newRect = new Rectangle
            {
                X = rect.X - width,
                Y = rect.Y - height,
                Width = rect.Width + 2 * width,
                Height = rect.Height + 2 * height
            };

            if (constrainedArea.IsEmpty == false)
                newRect.Intersect(new Rectangle(new Point(), constrainedArea));

            return newRect;
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
                newRect.Intersect(new Rectangle(new Point(), constrainedArea));

            return newRect;
        }

        public static int Area(this Rectangle rect)
        {
            return rect.Width * rect.Height;
        }

        public static float Area(this RectangleF rect)
        {
            return rect.Width * rect.Height;
        }

        public static PointF Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static PointF Center(this RectangleF rect)
        {
            return new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
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

        public static PointF[] Vertices(this RectangleF rect)
        {
            return new PointF[] 
            { 
                new PointF(rect.X, rect.Y), //left-upper
                new PointF(rect.Right, rect.Y), //right-upper
                new PointF(rect.X, rect.Bottom), //left-bottom
                new PointF(rect.Right, rect.Bottom) //right-bottom
            };
        }

        public static bool IsEmptyArea(this Rectangle rect)
        {
            return rect.Width == 0 || rect.Height == 0;
        }

        public static bool IsEmptyArea(this RectangleF rect)
        {
            return rect.Width == 0 || rect.Height == 0;
        }

        public static RectangleF UpScale(this RectangleF rect, int level = 1, float factor = 2)
        {
            float pyrScale = (float)System.Math.Pow(factor, level);

            return new RectangleF
            {
                X = rect.X * pyrScale,
                Y = rect.Y * pyrScale,
                Width = rect.Width * pyrScale,
                Height = rect.Height * pyrScale
            };
        }

        public static RectangleF DownScale(this RectangleF rect, int level = 1, float factor = 2)
        {
            float pyrScale = 1 / (float)System.Math.Pow(factor, level);

            return new RectangleF
            {
                X = rect.X * pyrScale,
                Y = rect.Y * pyrScale,
                Width = rect.Width * pyrScale,
                Height = rect.Height * pyrScale
            };
        }
    }
}
