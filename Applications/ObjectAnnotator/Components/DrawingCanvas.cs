using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using System.Drawing.Drawing2D;

namespace ObjectAnnotator.Components
{
    public partial class DrawingCanvas : UserControl
    {
        public DrawingCanvas()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                         ControlStyles.DoubleBuffer, true);

            this.TranslateImageModifierKey = Keys.ShiftKey;
            this.ResetTransformOnLoad = false;
        }

        #region Point transformation

        public PointF ToPictureBoxCoordinate(PointF point)
        {
            var scaleX = (float)imageBounds.Width / image.Width;
            var scaleY = (float)imageBounds.Height / image.Height;

            return new PointF
            {
                X = point.X * scaleX + imageBounds.X,
                Y = point.Y * scaleY + imageBounds.Y
            };
        }

        public PointF ToImageCoordinate(PointF point)
        {
            var scaleX = (float)imageBounds.Width / image.Width;
            var scaleY = (float)imageBounds.Height / image.Height;

            return new PointF
            {
                X = (point.X - imageBounds.X) / scaleX,
                Y = (point.Y - imageBounds.Y) / scaleY
            };
        }

        public RectangleF ToPictureBoxCoordinate(RectangleF imageRect)
        {
            var upperLeft = ToPictureBoxCoordinate(imageRect.Location);
            var bottomRight = ToPictureBoxCoordinate(new PointF(imageRect.Right, imageRect.Bottom));

            return RectangleF.FromLTRB(upperLeft.X, upperLeft.Y, bottomRight.X, bottomRight.Y);
        }

        public RectangleF ToImageCoordinate(RectangleF pictureBoxRect)
        {
            var upperLeft = ToPictureBoxCoordinate(pictureBoxRect.Location);
            var bottomRight = ToPictureBoxCoordinate(new PointF(pictureBoxRect.Right, pictureBoxRect.Bottom));

            return RectangleF.FromLTRB(upperLeft.X, upperLeft.Y, bottomRight.X, bottomRight.Y);
        }

        #endregion

        private Bitmap image;
        public Bitmap Image 
        {
            get { return image; }
            set 
            {
                this.image = value;

                if (image != null)
                {
                    if(this.ResetTransformOnLoad || this.imageBounds == default(RectangleF))
                        this.imageBounds = fitAndCenterImage(image.Size, this.ClientSize);
                }

                this.Invalidate();
            }
        }

        public bool ResetTransformOnLoad { get; set; }

        public Keys TranslateImageModifierKey 
        {
            get; 
            set; 
        }

        RectangleF imageBounds = default(RectangleF);
        protected override void OnPaint(PaintEventArgs e)
        {
            if(Image != null)
            {
                e.Graphics.DrawImage(Image, imageBounds);
            }

            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (image != null)
            {
                if (imageBounds.Width < image.Width && imageBounds.Height < image.Height)
                    imageBounds = fitAndCenterImage(image.Size, this.ClientSize);
            }

            base.OnResize(e);
            this.Invalidate();
        }

        #region Image mouse translate

        Cursor cursor = Cursors.Default;
        Keys pressedKey = Keys.None;
        bool keyHeld = false; //keyDown event fires multiple times. Why ?
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (keyHeld) return;
            keyHeld = true;

            pressedKey = e.KeyCode;
            if ((pressedKey & TranslateImageModifierKey) != Keys.None)
            {
                cursor = this.Cursor;
                this.Cursor = Cursors.Hand;
            }

            base.OnKeyDown(e);
        }

        Point startDragLocation = default(Point);
        protected override void OnKeyUp(KeyEventArgs e)
        {
            keyHeld = false;
            pressedKey = Keys.None;
            this.Cursor = cursor;
            startDragLocation = default(Point);
            base.OnKeyUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool shouldTranslate = (pressedKey & TranslateImageModifierKey) != Keys.None;
            
            if (shouldTranslate)
            {
                if (startDragLocation.IsEmpty) //on start drag
                    startDragLocation = e.Location;

                imageBounds.X += e.X - startDragLocation.X;
                imageBounds.Y += e.Y - startDragLocation.Y;
                startDragLocation = e.Location;

                this.Invalidate();
            }
            else
                base.OnMouseMove(e);           
        }

        #endregion

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var shouldScroll = (pressedKey & TranslateImageModifierKey) != Keys.None;

            if (shouldScroll)
            {
                var zoomCenter = new PointF(e.X - imageBounds.X, e.Y - imageBounds.Y);
                var zoomIn = e.Delta > 0;

                updateImageBounds(ref imageBounds, image.Size, zoomIn, zoomCenter);

                if (imageBounds.Width < image.Width && imageBounds.Height < image.Height && !zoomIn)
                    imageBounds = fitAndCenterImage(image.Size, this.ClientSize);
            }

            base.OnMouseWheel(e);

            if (shouldScroll)
                this.Invalidate();
        }

        private static RectangleF fitAndCenterImage(Size imageSize, Size clientSize)
        {
            var scale = Math.Min((float)clientSize.Width / imageSize.Width, (float)clientSize.Height / imageSize.Height);
            var offsetX = (float)(clientSize.Width - imageSize.Width * scale) / 2;
            var offsetY = (float)(clientSize.Height - imageSize.Height * scale) / 2;

            return new RectangleF(offsetX, offsetY, imageSize.Width * scale, imageSize.Height * scale);
        }

        private static void updateImageBounds(ref RectangleF imageBounds, Size imageSize, bool zoomIn, PointF zoomCenter)
        {
            var previousZoom = (float)imageBounds.Width / imageSize.Width;
            var currentZoom = getZoomFactor(imageBounds, imageSize, zoomIn);

            if (Math.Abs(previousZoom - currentZoom) < 1E-2) return;
            var zoomRatio = currentZoom / previousZoom;
           
            //update bounds size
            imageBounds.Width = imageBounds.Width * zoomRatio;
            imageBounds.Height = imageBounds.Height * zoomRatio;

            //return the zoom center to the previous position
            imageBounds.X += (1 - zoomRatio) * zoomCenter.X;
            imageBounds.Y += (1 - zoomRatio) * zoomCenter.Y;
        }

        private static float getZoomFactor(RectangleF imageBounds, Size imageSize, bool zoomIn)
        {
            var previousZoom = (float)imageBounds.Width / imageSize.Width;
            return zoomIn ? (previousZoom + 0.2f) : (previousZoom - 0.2f);
        }
    }
}
