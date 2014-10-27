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
using PointF = AForge.Point;
using Rectangle = Accord.Extensions.Rectangle;
using RectangleF = Accord.Extensions.RectangleF;

namespace ObjectAnnotator.Components
{
    public class PointAnnotation : DrawingAnnotation
    {
        public override void Initialize(DrawingCanvas element)
        {
            base.Initialize(element);
        }

        public override Rectangle BoundingRectangle
        {
            get 
            { 
                var rect = Element.ToPictureBoxCoordinate(Annotation.Polygon.BoundingRect().ToRect()).ToRect();
                rect.Inflate(RECT_SIZE / 2, RECT_SIZE / 2);
                return Rectangle.Round(rect);
            }
        }

        public override bool BelongsTo(IList<PointF> polygon)
        {
            return polygon.Count == 1;
        }

        const int RECT_SIZE = 10;
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (Annotation.Polygon.Length == 0) return;

            var pt = this.Annotation.Polygon
                         .Select(x => Element.ToPictureBoxCoordinate(x.ToPt()).ToPt())
                         .Select(x => x.Round())
                         .First();

            //pt = Element.ToPictureBoxCoordinate(new Point(475, 306).ToPt()).ToPt().Round();

            g.DrawRectangle(Pen, new System.Drawing.Rectangle(pt.X - RECT_SIZE / 2, pt.Y - RECT_SIZE / 2, RECT_SIZE, RECT_SIZE));
            g.DrawLine(Pen, pt.X - RECT_SIZE / 2, pt.Y - RECT_SIZE / 2, pt.X + RECT_SIZE / 2, pt.Y + RECT_SIZE / 2); // \
            g.DrawLine(Pen, pt.X + RECT_SIZE / 2, pt.Y - RECT_SIZE / 2, pt.X - RECT_SIZE / 2, pt.Y + RECT_SIZE / 2); // /
        }

        PointF pt;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !this.IsSelected || isDrawn)
                return;

            pt = Element.ToImageCoordinate(e.Location).ToPt();

            var imageSize = Element.Image.Size.ToSize(); 
            this.Annotation.Polygon = new PointF[] { pt.Clamp(imageSize) };
            isDrawn = true;
        }

        public override void OnMouseUp(object sender, MouseEventArgs e)
        {}

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {}
    }
}