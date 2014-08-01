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

namespace ObjectAnnotater.Components
{
    public class PointAnnotation : DrawingAnnotation
    {
        public override void Initialize(PictureBox element)
        {
            base.Initialize(element);
        }

        public override Rectangle BoundingRectangle
        {
            get 
            { 
                var rect = Element.ToPictureBoxCoordinate(Annotation.Polygon.BoundingRect());
                rect.Inflate(RECT_SIZE / 2, RECT_SIZE / 2);
                return rect;
            }
        }

        public override bool BelongsTo(IList<Point> polygon)
        {
            return polygon.Count == 1;
        }

        const int RECT_SIZE = 10;
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (Annotation.Polygon.Length == 0) return;

            var pt = this.Annotation.Polygon
                         .Select(x => Element.ToPictureBoxCoordinate(x))
                         .Select(x => x.Round())
                         .First();

            g.DrawRectangle(Pen, new System.Drawing.Rectangle(pt.X - RECT_SIZE / 2, pt.Y - RECT_SIZE / 2, RECT_SIZE, RECT_SIZE));
            g.DrawLine(Pen, pt.X - RECT_SIZE / 2, pt.Y - RECT_SIZE / 2, pt.X + RECT_SIZE / 2, pt.Y + RECT_SIZE / 2); // \
            g.DrawLine(Pen, pt.X + RECT_SIZE / 2, pt.Y - RECT_SIZE / 2, pt.X - RECT_SIZE / 2, pt.Y + RECT_SIZE / 2); // /
        }

        Point pt;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !this.IsSelected || isDrawn)
                return;

            pt = Element.ToImageCoordinate(e.Location.ToPt()).Round();

            var imageSize = Element.Image.Size.ToSize(); 
            this.Annotation.Polygon = new Point[] { pt.Clamp(imageSize) };
            isDrawn = true;
        }

        public override void OnMouseUp(object sender, MouseEventArgs e)
        {}

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {}
    }
}