using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using Annotation = Accord.Extensions.Rectangle;
using System.IO;

namespace ObjectAnnotater
{
    public partial class ObjectAnnotater : Form
    {
        CaptureBase capture = null;
        StreamWriter annotationWriter = null;

        int maxAnnotationIdx = -1;
        List<Annotation> frameAnnotations = new List<Annotation>();

        public ObjectAnnotater()
        {
            InitializeComponent();
            clearActionHistory();
        }

        public ObjectAnnotater(CaptureBase capture, StreamWriter annotationWriter)
        {
            InitializeComponent();

            this.capture = capture;
            this.annotationWriter = annotationWriter;

            capture.Start();
            showNextFrame();
        }

        Image<Bgr, byte> frame = null;
        Image<Bgr, byte> annotationImage = null;

        #region Commands

        int frameIdx = 0;
        private void showNextFrame()
        {
            if (frame != null)
            {
                frame.Dispose();
                annotationImage.Dispose();
            }

            bool hasNewFrame = capture.WaitForNewFrame(); 
            if (!hasNewFrame)
                return;

            clearActionHistory();

            frame = capture.QueryFrame();
            annotationImage = frame.Clone();

            this.pictureBox.Image = frame.ToBitmap();
            this.lblFrameIdx.Text = frameIdx.ToString();
            frameIdx++;

            GC.Collect();
        }

        private void undo()
        {
            undoAction();
            pictureBox.Invalidate();
        }

        private void redo()
        {
            redoAction();
            pictureBox.Invalidate();
        }

        private void ObjectAnnotater_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'n')
                showNextFrame();

            if (e.KeyChar == 'u')
                undo();

            if (e.KeyChar == 'r')
                redo();
        }

        #endregion

        #region BoundingBox selection

        Rectangle roi = Rectangle.Empty;
        Point ptFirst;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            ptFirst = TranslateZoomMousePosition(this.pictureBox, e.Location.ToPt());
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (frame == null) return;

            roi.Intersect(new Rectangle(new Point(), frame.Size));

            addAnnotation(roi);
            roi = Rectangle.Empty;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
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

            if (annotationImage != null) annotationImage.Dispose();
            annotationImage = frame.Clone();

            foreach (var ann in getAllAnnotations())
            {
                annotationImage.Draw(ann, Bgr8.Red, 3);
            }

            if (!roi.IsEmpty)
            {
                annotationImage.Draw(roi, Bgr8.Red, 3);
            }

            pictureBox.Image = annotationImage.ToBitmap();
        }

        #endregion

        #region History handling

        private void addAnnotation(Annotation annotation)
        {
            maxAnnotationIdx++;

            if (maxAnnotationIdx == frameAnnotations.Count)
            {
                frameAnnotations.Add(annotation);
            }
            else //if undo, then add
            {
                frameAnnotations[maxAnnotationIdx] = annotation;
            }
        }

        private IEnumerable<Annotation> getAllAnnotations()
        {
            for (int i = 0; i <= maxAnnotationIdx; i++)
            {
                yield return frameAnnotations[i];
            }
        }

        private void clearActionHistory()
        {
            frameAnnotations.Clear();
            maxAnnotationIdx = -1;
        }

        private void undoAction()
        {
            maxAnnotationIdx = Math.Max(-1, maxAnnotationIdx - 1);
        }

        private void redoAction()
        {
            maxAnnotationIdx = Math.Min(frameAnnotations.Count - 1, maxAnnotationIdx + 1);
        }

        #endregion

        #region File handling

        private void writeToFile()
        {
            string line = "";
            foreach (var ann in getAllAnnotations())
            {
                line += serializeRect(ann) + ";";
            }

            line.Remove(line.Length - 1);

            annotationWriter.WriteLine(line);
            annotationWriter.Flush();
        }

        private static string serializeRect(Rectangle rect)
        {
            return String.Format("{0} {1} {2} {3}", rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion

        private void ObjectAnnotater_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (capture != null)
                capture.Stop();

            if (annotationWriter != null)
                annotationWriter.Dispose();
        }
    }
}
