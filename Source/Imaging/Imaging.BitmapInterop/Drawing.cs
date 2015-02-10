#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Accord.Extensions.Math.Geometry;
using AForge.Math.Geometry;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Generic image drawing extensions.
    /// </summary>
    public static class DrawingExtensions
    {
       
        #region Rectangle

        /// <summary>
        /// Draws rectangle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="rect">Rectangle.</param>
        /// <param name="color">Object's color.</param>
        /// <param name="thickness">Border thickness. If less than zero strcuture will be filled.</param>
        /// <param name="opacity">Opacity for color. If color is 4 channel color, parameter value is discarded.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, RectangleF rect, TColor color, float thickness, byte opacity = Byte.MaxValue)
            where TColor: IColor3
        {
            if (float.IsNaN(rect.X) || float.IsNaN(rect.Y))
                return;

            var drawingColor = color.ToColor(opacity);
            var pen = new System.Drawing.Pen(drawingColor, thickness);

            var bmp = image.ToBitmap(false, true);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                if (thickness > 0)
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                else
                    g.FillRectangle(new System.Drawing.SolidBrush(drawingColor), rect.ToRect());
            }
        }

        #endregion

        #region Text

        /// <summary>
        /// Draws text on the provided image.
        /// </summary>
        /// <typeparam name="TColor">Image color type.</typeparam>
        /// <param name="image">Input image.</param>
        /// <param name="text">User text.</param>
        /// <param name="font">Font.</param>
        /// <param name="leftUpperPoint">Upper-left point.</param>
        /// <param name="color">Text color.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, string text, Font font, PointF leftUpperPoint, TColor color)
            where TColor : IColor3
        {
            var region = new RectangleF(leftUpperPoint, SizeF.Empty);
            Draw(image, text, font, region, color);
        }

        /// <summary>
        /// Draws text on the provided image.
        /// </summary>
        /// <typeparam name="TColor">Image color type.</typeparam>
        /// <param name="image">Input image.</param>
        /// <param name="text">User text.</param>
        /// <param name="font">Font.</param>
        /// <param name="region">Area in which the provided string will be drawn.</param>
        /// <param name="color">Text color.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, string text, Font font, RectangleF region, TColor color)
           where TColor : IColor3
        {
            var drawingColor = color.ToColor();
            var brush = new System.Drawing.SolidBrush(drawingColor);

            var bmp = image.ToBitmap(false, true);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.DrawString(text, font, brush, region.ToRect());
            }
        }

        #endregion

        #region Box & Ellipse

        /// <summary>
        /// Draws Box2D.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="box">Box 2D.</param>
        /// <param name="color">Object's color.</param>
        /// <param name="thickness">Border thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, Box2D box, TColor color, float thickness)
            where TColor : IColor3
        {
            var drawingColor = color.ToColor();
            var pen = new System.Drawing.Pen(drawingColor, thickness);

            var vertices = box.GetVertices().Select(x => x.ToPt()).ToArray();

            var bmp = image.ToBitmap(false, true);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    int idx2 = (i + 1) % vertices.Length;

                    g.DrawLine(pen, vertices[i], vertices[idx2]);
                }
            }
        }

        /// <summary>
        /// Draws ellipse.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="ellipse">Ellipse.</param>
        /// <param name="color">Object's color.</param>
        /// <param name="thickness">Border thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, Ellipse ellipse, TColor color, float thickness)
            where TColor : IColor3
        {
            var drawingColor = color.ToColor();
            var pen = new System.Drawing.Pen(drawingColor, thickness);

            var bmp = image.ToBitmap(false, true);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.ResetTransform();                             
                g.TranslateTransform(ellipse.Center.X, ellipse.Center.Y);
                g.RotateTransform(ellipse.Angle);  
                g.DrawEllipse(pen,
                              -ellipse.Size.Width / 2,
                              -ellipse.Size.Height / 2,
                              ellipse.Size.Width, 
                              ellipse.Size.Height);
            }
        }

        #endregion

        #region Line

        /// <summary>
        /// Draws line segment.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="line">Line</param>
        /// <param name="color">Line color.</param>
        /// <param name="thickness">Line thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, LineSegment line, TColor color, float thickness)
            where TColor : IColor3
        {
            Draw(image, new LineSegment[] { line }, color, thickness);
        }

        /// <summary>
        /// Draws line segments.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="lines">Lines</param>
        /// <param name="color">Line color.</param>
        /// <param name="thickness">Line thickness.</param>
        /// <param name="connectLines">True if the lines must form closed shape, false otherwise.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<LineSegment> lines, TColor color, float thickness, bool connectLines = true)
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

                Draw(image, pointPairs.Select(x => new PointF(x.X, x.Y)), color, thickness);
            }
            else
            {
                var bgr = color.ToColor().ToBgr();
                Draw(image, lines, thickness, (_) => bgr);
            }
        }

        /// <summary>
        /// Draws lines in various colors regarding their angle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="lines">Line segments (treated as vectors)</param>
        /// <param name="thickness">Line thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<LineSegment> lines, float thickness)
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

                Hsv8 hsv = new Hsv8 { H = (byte)(angle / 2), S = Byte.MaxValue / 2, V = Byte.MaxValue / 2 };
                return hsv.ToBgr();
            };

            Draw(image, lines, thickness, colorFunc);
        }

        /// <summary>
        /// Draws lines in various colors regarding user specified function.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="lines">Line segments (treated as vectors)</param>
        /// <param name="thickness">Line thickness.</param>
        /// <param name="colorFunc">Function that returns color for each line segment.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<LineSegment> lines, float thickness, Func<LineSegment, Bgr> colorFunc)
            where TColor : IColor3
        {
            var bmp = image.ToBitmap(false, true);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                foreach (var line in lines)
                {
                    var color = colorFunc(line).ToColor();
                    var pen = new System.Drawing.Pen(color, thickness);

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
        /// <param name="color">Contour color.</param>
        /// <param name="thickness">Contours thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<Point> contour, TColor color, float thickness)
            where TColor : IColor3
        {
            var contourArr = contour.Select(x => x.ToPt()).ToArray();
            if (contourArr.Length < 2)
                return;

            var drawingColor = color.ToColor();
            var pen = new System.Drawing.Pen(drawingColor, thickness);

            var bmp = image.ToBitmap(false, true);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.DrawCurve(pen, contourArr);
            }
        }

        /// <summary>
        /// Draws lines in various colors regarding their angle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="contour">Line segments (treated as vectors)</param>
        /// <param name="color">Contour color.</param>
        /// <param name="thickness">Contours thickness.</param>
        /// <param name="connectPoints">Connect points and draw contour or draw points as circles.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<PointF> contour, TColor color, float thickness, bool connectPoints = true)
            where TColor : IColor3
        {
            var contourArr = contour.Select(x => x.ToPt()).ToArray();
            if (contourArr.Length < 2)
                return;

            var drawingColor = color.ToColor();
            var pen = new Pen(drawingColor, thickness);

            var bmp = image.ToBitmap(false, true);

            if (connectPoints)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawCurve(pen, contourArr);
                }
            }
            else
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    foreach (var p in contour)
                    {
                        g.DrawEllipse(pen, p.X - thickness, p.Y - thickness, thickness * 2, thickness * 2);
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
        /// <param name="color">Circle color.</param>
        /// <param name="thickness">Contours thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, CircleF circle, TColor color, float thickness)
            where TColor : IColor3
        {
            Color drawingColor = color.ToColor();
            Pen pen = new Pen(drawingColor, thickness);

            var bmp = image.ToBitmap(false, true);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawEllipse(pen, circle.X - circle.Radius, circle.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2);
            }
        }

        /// <summary>
        /// Draws circles in various colors.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="circles">Circles</param>
        /// <param name="color">Circle color.</param>
        /// <param name="thickness">Contours thickness.</param>
        public static void Draw<TColor>(this Image<TColor, byte> image, IEnumerable<CircleF> circles, TColor color, float thickness)
            where TColor : IColor3
        {
            Color drawingColor = color.ToColor();
            Pen pen = new Pen(drawingColor, thickness);

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

        #region Annotations

        /// <summary>
        /// Draws rectangle annotation.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="rect">User specified area to annotate.</param>
        /// <param name="text">Label.</param>
        /// <param name="annotationWidth">Width of annotation rectangle.</param>
        /// <param name="color">Color for rectangle. Label area is filled. Default color is yellow-green.</param>
        /// <param name="textColor">Label color. Default color is black.</param>
        /// <param name="font">Font to use. Default is "Arial" of size 10, style: Bold.</param>
        /// <param name="thickness">Rectangle thickness.</param>
        public static void DrawAnnotation(this Image<Bgr, byte> image, Rectangle rect, string text, 
                                          int annotationWidth = 100, Bgr color = default(Bgr), Bgr textColor = default(Bgr), Font font = null,
                                          int thickness = 1)
        {
            color = color.Equals(default(Bgr)) ? Color.YellowGreen.ToBgr() : color;
            textColor = textColor.Equals(default(Bgr)) ? Color.Black.ToBgr() : color;
            font = font ?? new Font("Arial", 8, System.Drawing.FontStyle.Bold);

            var nLines = text.ToCharArray().Where(x => x.Equals('\n')).Count() + 1;
            var annotationHeight = (int)(3 + (font.SizeInPoints + 3) * nLines + 3);
            var xOffset = (annotationWidth - rect.Width) / 2;
            var annotationRect = new Rectangle(rect.X - xOffset, rect.Y - annotationHeight, annotationWidth, annotationHeight);

            image.Draw(annotationRect, color, thickness);
            image.Draw(rect, color, thickness);
            image.Draw(annotationRect, color, -1, 80);

            image.Draw(text, font, annotationRect, textColor);
        }

        #endregion
    }
}
