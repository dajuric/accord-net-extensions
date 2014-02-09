using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Annotation = Accord.Extensions.Rectangle;
using Point = AForge.IntPoint;

namespace ObjectAnnotater
{
    public partial class ObjectAnnotater : Form
    {
        ImageDirectoryReader capture = null;
        AnnotationDatabase database = null;

        CommandHistory<Annotation> frameAnnotations = null;

        public ObjectAnnotater()
        {
            InitializeComponent();
        }

        public ObjectAnnotater(ImageDirectoryReader capture, AnnotationDatabase database)
        {
            InitializeComponent();

            this.capture = capture;
            this.database = database;
            this.frameAnnotations = new CommandHistory<Annotation>();

            capture.Open();
            getFrame();
        }

        Image<Bgr, byte> frame = null;
        Image<Bgr, byte> annotationImage = null;

        #region Commands

        private void getFrame(int offset = 0)
        {
            //save current annotations
            if (offset == 0)
            {
                var currentAnnotations = frameAnnotations.GetValid();
                if (currentAnnotations.Count() > 0)
                {
                    database.AddOrUpdate(capture.CurrentImageName, currentAnnotations);
                    database.Commit();
                }
            }

            capture.Seek(offset);

            var annotations = database.Find(capture.CurrentImageName);
            frameAnnotations = new CommandHistory<Annotation>(annotations);

            frame = capture.ReadAs<Bgr, byte>();
            annotationImage = frame.Clone();

            this.pictureBox.Image = annotationImage.ToBitmap();
            this.lblFrameIndex.Text = capture.Position.ToString();

            GC.Collect();
        }

        private void undo()
        {
            frameAnnotations.Undo();
            pictureBox.Invalidate();
        }

        private void redo()
        {
            frameAnnotations.Redo();
            pictureBox.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            { 
                case Keys.Left:
                    getFrame(-1 + -1);
                    break;
                case Keys.Right:
                    getFrame();
                    break;
                case Keys.U:
                    undo();
                    break;
                case Keys.R:
                    redo();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region BoundingBox selection

        Rectangle roi = Rectangle.Empty;
        bool isSelecting = false;
        Point ptFirst;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            ptFirst = TranslateZoomMousePosition(this.pictureBox, e.Location.ToPt());
            isSelecting = true;
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (frame == null || !isSelecting) return;

            roi.Intersect(new Rectangle(new Point(), frame.Size));

            frameAnnotations.AddOrUpdate(roi);
            roi = Rectangle.Empty;
            isSelecting = false;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !isSelecting)
                return;

            var ptSecond = TranslateZoomMousePosition(this.pictureBox, e.Location.ToPt());

            roi = new Rectangle
            {
                X = System.Math.Min(ptFirst.X, ptSecond.X),
                Y = System.Math.Min(ptFirst.Y, ptSecond.Y),
                Width = System.Math.Abs(ptFirst.X - ptSecond.X),
                Height = System.Math.Abs(ptFirst.Y - ptSecond.Y)
            };

            pictureBox.Invalidate();
        }

        //taken from: http://www.codeproject.com/Articles/20923/Mouse-Position-over-Image-in-a-PictureBox
        private static Point TranslateZoomMousePosition(PictureBox control, Point coordinates)
        {
            var image = control.Image;

            // test to make sure our image is not null
            if (image == null) return coordinates;
            // Make sure our control width and height are not 0 and our 
            // image width and height are not 0
            if (control.Width == 0 || control.Height == 0 || control.Image.Width == 0 || control.Image.Height == 0) return coordinates;
            // This is the one that gets a little tricky. Essentially, need to check 
            // the aspect ratio of the image to the aspect ratio of the control
            // to determine how it is being rendered
            float imageAspect = (float)control.Image.Width / control.Image.Height;
            float controlAspect = (float)control.Width / control.Height;
            float newX = coordinates.X;
            float newY = coordinates.Y;
            if (imageAspect > controlAspect)
            {
                // This means that we are limited by width, 
                // meaning the image fills up the entire control from left to right
                float ratioWidth = (float)control.Image.Width / control.Width;
                newX *= ratioWidth;
                float scale = (float)control.Width / control.Image.Width;
                float displayHeight = scale * control.Image.Height;
                float diffHeight = control.Height - displayHeight;
                diffHeight /= 2;
                newY -= diffHeight;
                newY /= scale;
            }
            else
            {
                // This means that we are limited by height, 
                // meaning the image fills up the entire control from top to bottom
                float ratioHeight = (float)control.Image.Height / control.Height;
                newY *= ratioHeight;
                float scale = (float)control.Height / control.Image.Height;
                float displayWidth = scale * control.Image.Width;
                float diffWidth = control.Width - displayWidth;
                diffWidth /= 2;
                newX -= diffWidth;
                newX /= scale;
            }
            return new Point
            {
                X = (int)Math.Round(newX),
                Y = (int)Math.Round(newY)
            };
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (frame == null) return;

            this.annotationImage.SetValue(frame);

            foreach (var ann in frameAnnotations.GetValid())
            {
                annotationImage.Draw(ann, Bgr8.Red, 3);
            }

            if (!roi.IsEmpty)
            {
                annotationImage.Draw(roi, Bgr8.Red, 3);
            }
        }

        #endregion

        #region File handling
        #endregion

        private void ObjectAnnotater_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (capture != null)
                capture.Close();
        }
    }
}
