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

namespace ObjectAnnotater.Components
{
    public class RectangleAnnotation: DrawingAnnotation
    {
        public override void Initialize(PictureBox element)
        {
            base.Initialize(element);
        }

        public override Rectangle BoundingRectangle
        {
            get { return Element.ToPictureBoxCoordinate(Annotation.Polygon.BoundingRect()); }
        }

        public override bool BelongsTo(IList<Point> polygon)
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
                                 .Select(x => Element.ToPictureBoxCoordinate(x))
                                 .Select(x => x.ToPt())
                                 .ToArray();

            g.DrawPolygon(Pen, pictureBoxPoly);
        }


        Rectangle roi = new Rectangle(0, 0, MIN_RECT_SIZE, MIN_RECT_SIZE);
        Point ptFirst;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !this.IsSelected || isDrawn)
                return;

            ptFirst = Element.ToImageCoordinate(e.Location.ToPt()).Round();
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

            var ptSecond = Element.ToImageCoordinate(e.Location.ToPt()).Round();

            roi = new Rectangle
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
