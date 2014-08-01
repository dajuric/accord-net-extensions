using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using System.Linq;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using Rectangle = Accord.Extensions.Rectangle;
using System.ComponentModel;

namespace ObjectAnnotater.Components
{
    public abstract class DrawingAnnotation
    {
        public static readonly Pen SelectedPen = new Pen(Color.Red, 3);
        public static readonly Pen DefaultPen = new Pen(Color.Green, 2);

        protected PictureBox Element = null;

        protected Pen Pen 
        {
            get { return this.IsSelected ? SelectedPen : DefaultPen; } 
        }

        public bool IsSelected
        {
            get;
            set;
        }
        public bool ShowLabel { get; set; }

        protected bool isDrawn = false;
        Annotation ann = new Annotation();
        /// <summary>
        /// Gets or sets the polygon in image coordinates.
        /// </summary>
        public virtual Annotation Annotation
        {
            get { return this.ann; }
            set 
            {
                if (this.BelongsTo(value.Polygon) == false)
                    throw new NotSupportedException();

                this.ann = value;
                this.isDrawn = true;
            }
        }

        /// <summary>
        /// Gets the bounding rectangle in pciture box coordinate.
        /// </summary>
        public abstract Rectangle BoundingRectangle
        {
            get;
        }

        public abstract bool BelongsTo(IList<Point> polygon);

        public virtual void Initialize(PictureBox element)
        {
            this.Element = element;
            this.ShowLabel = true;
            this.IsSelected = true;
        }

        System.Drawing.Font drawingFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
        public virtual void Draw(Graphics g)
        {
            var rect = BoundingRectangle;

            var annLabel = ShowLabel ? ann.Label : "";
            var labelSize = g.MeasureString(annLabel, drawingFont);

            g.DrawString(annLabel, drawingFont, new SolidBrush(Color.Black), 
                                  new System.Drawing.PointF 
                                  {
                                      X = rect.X,
                                      Y = rect.Y - labelSize.Height - 5
                                  });
        }

        public virtual void OnMouseDown(object sender, MouseEventArgs e) { }

        public virtual void OnMouseMove(object sender, MouseEventArgs e) { }

        public virtual void OnMouseUp(object sender, MouseEventArgs e) { }
    }
}
