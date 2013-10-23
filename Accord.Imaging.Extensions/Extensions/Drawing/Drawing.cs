using Accord.Math.Geometry;
using Accord.Imaging.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Math.Geometry;
using AForge.Imaging;
using Accord.Core;

namespace Accord.Imaging
{
    public static class DrawingExtensions
    {
        private static Color getColor<TColor>(TColor color)
            where TColor : IColor
        {
            int[] colorArr = HelperMethods.ColorToArray<TColor, int>(color);

            switch (colorArr.Length)
            { 
                case 1:
                    return Color.FromArgb(0, 0, colorArr[0]);
                case 2:
                    return Color.FromArgb(0, colorArr[0], colorArr[1]);
                case 3:
                    return Color.FromArgb(colorArr[0], colorArr[1], colorArr[2]);
                case 4:
                    return Color.FromArgb(colorArr[0], colorArr[1], colorArr[2], colorArr[3]);
            }

            throw new Exception("Unknown color model!");
        }

        /// <summary>
        /// Draws rectangle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="rect">Rectangle.</param>
        /// <param name="color">Object's color.</param>
        /// <param name="width">Border thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, RectangleF rect, TColor color, float width)
            where TColor: IColor3
        {
            if (float.IsNaN(rect.X) || float.IsNaN(rect.Y))
                return;

            Color drawingColor = getColor(color);
            Pen pen = new Pen(drawingColor, width);

            var bmp = image.ToBitmap(false, true);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="text">User text.</param>
        /// <param name="font">Font.</param>
        /// <param name="leftUpperPoint">Upper-left point.</param>
        /// <param name="color">Text's color.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, string text, Font font, PointF leftUpperPoint, TColor color)
            where TColor : IColor3
        {
            try
            {
                Color drawingColor = getColor(color);
                Brush brush = new SolidBrush(drawingColor);

                var bmp = image.ToBitmap(false, true);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawString(text, font, brush, leftUpperPoint);
                }
            }
            catch (Exception) 
            {
                Console.WriteLine("String drawing exception!");
            }
        }

        /// <summary>
        /// Draws Box2D.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="box">Box 2D.</param>
        /// <param name="color">Object's color.</param>
        /// <param name="width">Border thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, Box2D box, TColor color, float width)
            where TColor : IColor3
        {
            Color drawingColor = getColor(color);
            Pen pen = new Pen(drawingColor, width);

            PointF[] vertices = box.GetVertices();

            var bmp = image.ToBitmap(false, true);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    int idx2 = (i + 1) % vertices.Length;

                    g.DrawLine(pen, vertices[i], vertices[idx2]);
                }
            }
        }

        /// <summary>
        /// Draws lines in various colors regarding their angle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="lines">Line segments (treated as vectors)</param>
        /// <param name="width">Line thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, List<LineSegment> lines, float width)
            where TColor : IColor3
        {
            var bmp = image.ToBitmap(false, true);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    /************** calculate angle ************/
                    var diff = lines[i].End - lines[i].Start;
                    var angle = System.Math.Atan2(diff.Y, diff.X);
                    angle = angle * 180 / System.Math.PI; //to degrees
                    angle = (angle < 0) ? angle + 360 : angle;
                    /************** calculate angle ************/

                    var rgbColor = new HSL((int)angle, 0.5f, 0.5f).ToRGB();
                    Pen pen = new Pen(rgbColor.Color, width);

                    g.DrawLine(pen, lines[i].Start.X, lines[i].Start.Y,
                                    lines[i].End.X, lines[i].End.Y);
                }
            }
        }

        /// <summary>
        /// Draws lines in various colors regarding their angle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="contour">Line segments (treated as vectors)</param>
        /// <param name="width">Contours thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<Point> contour, TColor color, float width)
            where TColor : IColor3
        {
            var contourArr = contour.ToArray();
            if (contourArr.Length < 2)
                return;

            Color drawingColor = getColor(color);
            Pen pen = new Pen(drawingColor, width);

            var bmp = image.ToBitmap(false, true);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < contour.Count(); i++)
                {
                    g.DrawLines(pen, contour.ToArray());
                }
            }
        }

        /// <summary>
        /// Draws lines in various colors regarding their angle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="contour">Line segments (treated as vectors)</param>
        /// <param name="width">Contours thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<PointF> contour, TColor color, float width)
            where TColor : IColor3
        {
            var contourArr = contour.ToArray();
            if (contourArr.Length < 2)
                return;

            Color drawingColor = getColor(color);
            Pen pen = new Pen(drawingColor, width);

            var bmp = image.ToBitmap(false, true);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < contour.Count(); i++)
                {
                    g.DrawLines(pen, contour.ToArray());
                }
            }
        }

    }
}
