using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Imaging
{
    public static class DrawingStructureConversions
    {
        public static Point ToPt(this System.Drawing.Point point)
        {
            return new Point(point.X, point.Y);
        }

        public static PointF ToPt(this System.Drawing.PointF point)
        {
            return new PointF(point.X, point.Y);
        }

        public static System.Drawing.Point ToPt(this Point point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }

        public static System.Drawing.PointF ToPt(this PointF point)
        {
            return new System.Drawing.PointF(point.X, point.Y);
        }




        public static Rectangle ToRect(this System.Drawing.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static RectangleF ToRect(this System.Drawing.RectangleF rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Size ToSize(this System.Drawing.Size size)
        {
            return new Size(size.Width, size.Height);
        }

        public static SizeF ToSize(this System.Drawing.SizeF size)
        {
            return new SizeF(size.Width, size.Height);
        }


        public static System.Drawing.Rectangle ToRect(this Rectangle rect)
        {
            return new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static System.Drawing.RectangleF ToRect(this RectangleF rect)
        {
            return new System.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static System.Drawing.Size ToSize(this Size size)
        {
            return new System.Drawing.Size(size.Width, size.Height);
        }

        public static System.Drawing.SizeF ToSize(this SizeF size)
        {
            return new System.Drawing.SizeF(size.Width, size.Height);
        }


        public static Bgr32 ToBgr(this System.Drawing.Color color)
        {
            return new Bgr32 { B = color.B, G = color.G, R = color.R };
        }
    }
}
