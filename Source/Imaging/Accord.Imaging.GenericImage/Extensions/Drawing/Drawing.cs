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
            correctValueMapping<TColor>(ref colorArr);
            
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

        private static void correctValueMapping<TColor>(ref int[] colorArr)
             where TColor : IColor
        {
            if (ColorInfo.GetInfo<TColor, double>().ConversionCodename == "BGR") //TODO (priority: lowest): other way to do that (without harcoding) - converters ?
            {
                var temp = colorArr[0];
                colorArr[0] = colorArr[2];
                colorArr[2] = temp;
            }

        }

        #region Rectangle

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

        #endregion

        #region Text

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

        #endregion

        #region Box

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

        #endregion


        #region Line

        /// <summary>
        /// Draws line segment.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="line">Line</param>
        /// <param name="width">Line thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, LineSegment line, TColor color, float width)
            where TColor : IColor3
        {
            Draw(image, new LineSegment[] { line }, color, width);
        }

        /// <summary>
        /// Draws line segments.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="lines">Lines</param>
        /// <param name="width">Line thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<LineSegment> lines, TColor color, float width, bool connectLines = true)
            where TColor : IColor3
        {
            var pointPairs = new List<AForge.Point>();

            if (connectLines)
            {
                foreach (var line in lines)
                {
                    pointPairs.Add(line.Start);
                    pointPairs.Add(line.End);
                }

                Draw(image, pointPairs.Select(x => new PointF(x.X, x.Y)), color, width);
            }
            else
            {
                var bgr = new Bgr(getColor(color));
                Draw(image, lines, width, (_) => bgr);
            }
        }

        /// <summary>
        /// Draws lines in various colors regarding their angle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="lines">Line segments (treated as vectors)</param>
        /// <param name="width">Line thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<LineSegment> lines, float width)
            where TColor : IColor3
        {
            Func<LineSegment, Bgr> colorFunc = (segment) => 
            {
                /************** calculate angle ************/
                var diff = segment.End - segment.Start;
                var angle = System.Math.Atan2(diff.Y, diff.X);
                angle = angle * 180 / System.Math.PI; //to degrees
                angle = (angle < 0) ? angle + 360 : angle;
                /************** calculate angle ************/

                var rgbColor = new HSL((int)angle, 0.5f, 0.5f).ToRGB();
                return new Bgr(rgbColor.Color);
            };

            Draw(image, lines, width, colorFunc);
        }

        /// <summary>
        /// Draws lines in various colors regarding user specified function.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="lines">Line segments (treated as vectors)</param>
        /// <param name="width">Line thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<LineSegment> lines, float width, Func<LineSegment, Bgr> colorFunc)
            where TColor : IColor3
        {
            var bmp = image.ToBitmap(false, true);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (var line in lines)
                {
                    var color = getColor(colorFunc(line));
                    Pen pen = new Pen(color, width);

                    g.DrawLine(pen, line.Start.X, line.Start.Y,
                                    line.End.X, line.End.Y);
                }
            }
        }

        #endregion

        #region Contour

        /// <summary>
        /// Draws contour.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="contour">Contour points.</param>
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
                g.DrawCurve(pen, contour.ToArray());
            }
        }

        /// <summary>
        /// Draws lines in various colors regarding their angle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="contour">Line segments (treated as vectors)</param>
        /// <param name="width">Contours thickness.</param>
        /// <param name="connectPoints">Connect points and draw contour or draw points as circles.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<PointF> contour, TColor color, float width, bool connectPoints = true)
            where TColor : IColor3
        {
            var contourArr = contour.ToArray();
            if (contourArr.Length < 2)
                return;

            Color drawingColor = getColor(color);
            Pen pen = new Pen(drawingColor, width);

            var bmp = image.ToBitmap(false, true);

            if (connectPoints)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawCurve(pen, contour.ToArray());
                }
            }
            else
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    foreach (var p in contour)
                    {
                        g.DrawEllipse(pen, p.X - width, p.Y - width, width * 2, width * 2);
                    }
                }
            }
        }

        #endregion

        #region Circle

        /// <summary>
        /// Draws circle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="circle">Circle</param>
        /// <param name="width">Contours thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, CircleF circle, TColor color, float width)
            where TColor : IColor3
        {
            Color drawingColor = getColor(color);
            Pen pen = new Pen(drawingColor, width);

            var bmp = image.ToBitmap(false, true);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawEllipse(pen, circle.X, circle.Y, circle.Radius * 2, circle.Radius * 2);
            }
        }

        /// <summary>
        /// Draws circles in various colors.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="circles">Circles</param>
        /// <param name="width">Contours thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<CircleF> circles, TColor color, float width)
            where TColor : IColor3
        {
            Color drawingColor = getColor(color);
            Pen pen = new Pen(drawingColor, width);

            var bmp = image.ToBitmap(false, true);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (var c in circles)
	            {
                    g.DrawEllipse(pen, c.X - c.Radius, c.Y - c.Radius, c.Radius * 2, c.Radius * 2);
                }
            }
        }

        #endregion
    }
}
