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

using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using System;
using Accord.Extensions;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Point = AForge.IntPoint;
using PointF= AForge.Point;
using Rectangle = Accord.Extensions.Rectangle;
using RectangleF = Accord.Extensions.RectangleF;

namespace ObjectAnnotator.Components
{
    public class RectangleAnnotation: DrawingAnnotation
    {
        public override void Initialize(DrawingCanvas element)
        {
            base.Initialize(element);
        }

        public override Rectangle BoundingRectangle
        {
            get { return Rectangle.Round(Element.ToPictureBoxCoordinate(Annotation.Polygon.BoundingRect().ToRect()).ToRect()); }
        }

        public override bool BelongsTo(IList<PointF> polygon)
        {
            return polygon.IsRectangle();
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (Annotation.Polygon.Length == 0) return;

            var sortedPolyIndices = this.Annotation.Polygon.SortPointsClockwise();
            var poly = Annotation.Polygon.GetAt(sortedPolyIndices);
            var pictureBoxPoly = poly
                                 .Select(x => Element.ToPictureBoxCoordinate(x.ToPt()))
                                 .ToArray();

            g.DrawPolygon(Pen, pictureBoxPoly);
        }


        RectangleF roi = new RectangleF(0, 0, MIN_RECT_SIZE, MIN_RECT_SIZE);
        Point ptFirst;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !this.IsSelected || isDrawn)
                return;

            ptFirst = Element.ToImageCoordinate(e.Location).ToPt().Round();
            roi.Location = ptFirst; //if user draws MIN_RECT_SIZE, add it to click location
        }

        const int MIN_RECT_SIZE = 10;
        public override void OnMouseUp(object sender, MouseEventArgs e)
        {
           if (Element.Image == null || !this.IsSelected || isDrawn) 
               return;

           var vertices = roi.Vertices();
           this.Annotation.Polygon = vertices;
           isDrawn = true;
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !this.IsSelected || isDrawn)
                return;

            var ptSecond = Element.ToImageCoordinate(e.Location).ToPt().Round();

            roi = new RectangleF
            {
                X = System.Math.Min(ptFirst.X, ptSecond.X),
                Y = System.Math.Min(ptFirst.Y, ptSecond.Y),
                Width = System.Math.Abs(ptFirst.X - ptSecond.X),
                Height = System.Math.Abs(ptFirst.Y - ptSecond.Y)
            };

            roi.Width = Math.Max(MIN_RECT_SIZE, roi.Width);
            roi.Height = Math.Max(MIN_RECT_SIZE, roi.Height);

            var imageSize = Element.Image.Size.ToSize(); 
            this.Annotation.Polygon = roi.Vertices().Select(x => x.Clamp(imageSize)).ToArray();
        }
    }
}
