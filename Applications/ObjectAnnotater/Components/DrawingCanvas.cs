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

namespace ObjectAnnotater.Components
{
    public partial class DrawingCanvas : UserControl
    {
        public DrawingCanvas()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                         ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            this.TranslateImageModifierKey = Keys.ShiftKey;
            this.ResetTransformOnLoad = false;
        }

        #region Point transformation

        public PointF ToPictureBoxCoordinate(PointF point)
        {
            var scale = (float)imageBounds.Width / image.Width;

            return new PointF
            {
                X = point.X * scale + imageBounds.X,
                Y = point.Y * scale + imageBounds.Y
            };
        }

        public PointF ToImageCoordinate(PointF point)
        {
            var scale = (float)imageBounds.Width / image.Width;

            return new PointF
            {
                X = (point.X - imageBounds.X) / scale,
                Y = (point.Y - imageBounds.Y) / scale
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
                    if(this.ResetTransformOnLoad || this.imageBounds == default(Rectangle))
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

        Rectangle imageBounds = default(Rectangle);
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

        Keys pressedKey = Keys.None;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            pressedKey = e.KeyCode;
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            pressedKey = Keys.None;
            base.OnKeyUp(e);
        }

        Point startDragLocation = default(Point);
        protected override void OnMouseDown(MouseEventArgs e)
        {
            bool shouldTranslate = pressedKey == TranslateImageModifierKey &&
                                    e.Button == MouseButtons.Left;

            if (shouldTranslate)
                startDragLocation = e.Location;
            else
                base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool shouldTranslate =  pressedKey == TranslateImageModifierKey && 
                                    e.Button == MouseButtons.Left;

            if (shouldTranslate)
            {
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
            var shouldScroll = pressedKey == TranslateImageModifierKey;

            if (shouldScroll)
            {
                var zoomCenter = new Point(e.X - imageBounds.X, e.Y - imageBounds.Y);
                var zoomIn = e.Delta > 0;

                updateImageBounds(ref imageBounds, image.Size, zoomIn, zoomCenter);

                if (imageBounds.Width < image.Width && imageBounds.Height < image.Height && !zoomIn)
                    imageBounds = fitAndCenterImage(image.Size, this.ClientSize);
            }

            base.OnMouseWheel(e);

            if (shouldScroll)
                this.Invalidate();
        }

        private static Rectangle fitAndCenterImage(Size imageSize, Size clientSize)
        {
            var scale = Math.Min((float)clientSize.Width / imageSize.Width, (float)clientSize.Height / imageSize.Height);
            var offsetX = (float)(clientSize.Width - imageSize.Width * scale) / 2;
            var offsetY = (float)(clientSize.Height - imageSize.Height * scale) / 2;

            return new Rectangle((int)offsetX, (int)offsetY, (int)(imageSize.Width * scale), (int)(imageSize.Height * scale));
        }

        private static void updateImageBounds(ref Rectangle imageBounds, Size imageSize, bool zoomIn, Point zoomCenter)
        {
            var previousZoom = (float)imageBounds.Width / imageSize.Width;
            var currentZoom = getZoomFactor(imageBounds, imageSize, zoomIn);

            if (Math.Abs(previousZoom - currentZoom) < 1E-2) return;
            var zoomRatio = currentZoom / previousZoom;
           
            //update bounds size
            imageBounds.Width = (int)(imageBounds.Width * zoomRatio);
            imageBounds.Height = (int)(imageBounds.Height * zoomRatio);

            //return the zoom center to the previous position
            imageBounds.X += (int)((1 - zoomRatio) * zoomCenter.X);
            imageBounds.Y += (int)((1 - zoomRatio) * zoomCenter.Y);
        }

        private static float getZoomFactor(Rectangle imageBounds, Size imageSize, bool zoomIn)
        {
            var previousZoom = (float)imageBounds.Width / imageSize.Width;
            return zoomIn ? (previousZoom + 0.1f) : (previousZoom - 0.1f);
        }
    }
}
