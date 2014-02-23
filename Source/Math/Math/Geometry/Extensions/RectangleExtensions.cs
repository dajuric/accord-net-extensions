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
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides extension methods for <see cref="Recatngle"/>.
    /// </summary>
    public static class RectangleExtennsions
    {
        /// <summary>
        /// Gets intersection percent of two rectangles.
        /// </summary>
        /// <param name="rect1">First rectangle.</param>
        /// <param name="rect2">Second rectangle.</param>
        /// <returns>Intersection percent (1 - full intersection, 0 - no intersection).</returns>
        public static float IntersectionPercent(this Rectangle rect1, Rectangle rect2)
        {
            return RectangleFExtensions.IntersectionPercent(rect1, rect2);
        }

        /// <summary>
        /// Infaltes the rectangle by specified width and height (can be negative) and automaticaly clamps rectangle coordinates.
        /// </summary>
        /// <param name="rect">Rectangle to inflate.</param>
        /// <param name="width">Horizontal amount.</param>
        /// <param name="height">Vecrtical amount.</param>
        /// <param name="constrainedArea">If specified rectangle region will be clamped.</param>
        /// <returns>Inflated rectangle.</returns>
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

        /// <summary>
        /// Infaltes the rectangle by specified width and height (can be negative) and automaticaly clamps rectangle coordinates.
        /// </summary>
        /// <param name="rect">Rectangle to inflate.</param>
        /// <param name="width">Horizontal scale.</param>
        /// <param name="height">Vecrtical scale.</param>
        /// <param name="constrainedArea">If specified rectangle region will be clamped.</param>
        /// <returns>Inflated rectangle.</returns>
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

        /// <summary>
        /// Gets the rectangle area.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <returns>Area of the rectangle.</returns>
        public static int Area(this Rectangle rect)
        {
            return rect.Width * rect.Height;
        }

        /// <summary>
        /// Gets rectangle center.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <returns>Center of the rectangle.</returns>
        public static PointF Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        /// <summary>
        /// Gets rectangle vertices in clock-wise order staring from left-upper corner.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <returns>Vertices.</returns>
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

        /// <summary>
        /// Gets whether the rectangle has an empty area. It is different than <see cref="Rectangle.Empty"/> property.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <returns>True if the rectangle has an empty area.</returns>
        public static bool IsEmptyArea(this Rectangle rect)
        {
            return rect.Width == 0 || rect.Height == 0;
        }
    }

    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides extension methods for <see cref="Recatngle"/>.
    /// </summary>
    public static class RectangleFExtensions
    {
        /// <summary>
        /// Caclulates intersected rectangle from specified area (transformed into rectangle with location (0,0)).
        /// </summary>
        /// <param name="rect">Recatangle to intersect.</param>
        /// <param name="area">Maximum bounding box represented as size.</param>
        /// <returns>Intersected rectangle.</returns>
        public static Rectangle Intersect(this Rectangle rect, Size area)
        {
            Rectangle newRect = rect;

            if (area.IsEmpty == false)
                newRect.Intersect(new Rectangle(new Point(), area));

            return newRect;
        }

        /// <summary>
        /// Gets intersection percent of two rectangles.
        /// </summary>
        /// <param name="rect1">First rectangle.</param>
        /// <param name="rect2">Second rectangle.</param>
        /// <returns>Intersection percent (1 - full intersection, 0 - no intersection).</returns>
        public static float IntersectionPercent(this RectangleF rect1, RectangleF rect2)
        {
            float rect1Area = rect1.Width * rect1.Height;
            float rect2Area = rect2.Width * rect2.Height;

            RectangleF interesectRect = RectangleF.Intersect(rect1, rect2);
            float intersectRectArea = interesectRect.Width * interesectRect.Height;

            float minRectArea = System.Math.Min(rect1Area, rect2Area);

            return (float)intersectRectArea / minRectArea;
        }

        /// <summary>
        /// Gets the rectangle area.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <returns>Area of the rectangle.</returns>
        public static float Area(this RectangleF rect)
        {
            return rect.Width * rect.Height;
        }

        /// <summary>
        /// Gets rectangle center.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <returns>Center of the rectangle.</returns>
        public static PointF Center(this RectangleF rect)
        {
            return new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        /// <summary>
        /// Gets rectangle vertices in clock-wise order staring from left-upper corner.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <returns>Vertices.</returns>
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

        /// <summary>
        /// Gets whether the rectangle has an empty area. It is different than <see cref="Rectangle.Empty"/> property.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <returns>True if the rectangle has an empty area.</returns>
        public static bool IsEmptyArea(this RectangleF rect)
        {
            return rect.Width == 0 || rect.Height == 0;
        }

        /// <summary>
        /// Transforms rectangle to the lower pyramid level.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <param name="level">Specifies how many levels to take.</param>
        /// <param name="factor">Specifies the pyramid scale factor.</param>
        /// <returns>Transformed rectangle.</returns>
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

        /// <summary>
        /// Transforms rectangle to the higher pyramid level.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <param name="level">Specifies how many levels to take.</param>
        /// <param name="factor">Specifies the pyramid scale factor.</param>
        /// <returns>Transformed rectangle.</returns>
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
