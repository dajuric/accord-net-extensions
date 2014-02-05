using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Annotation = Accord.Extensions.Rectangle;
using Point = AForge.IntPoint;
using System.Linq;
using System.Collections;

namespace ObjectAnnotater
{
    public partial class ObjectAnnotater : Form
    {
        IStreamableSource capture = null;
        Stream annotationStream = null;

        int maxAnnotationIdx = -1;
        List<Annotation> frameAnnotations = new List<Annotation>();

        public ObjectAnnotater()
        {
            InitializeComponent();
            clearActionHistory();
        }

        public ObjectAnnotater(IStreamableSource capture, Stream annotationStream)
        {
            InitializeComponent();

            this.capture = capture;
            this.annotationStream = annotationStream;

            capture.Open();
            getFrame();
        }

        Image<Bgr, byte> frame = null;
        Image<Bgr, byte> annotationImage = null;

        #region Commands

        private void getFrame(int offset = 0)
        {
            clearActionHistory();

            capture.Seek(offset);
            frame = capture.ReadAs<Bgr, byte>();
            annotationImage = frame.Clone();

            this.pictureBox.Image = annotationImage.ToBitmap();
            this.lblFrameIndex.Text = capture.Position.ToString();

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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            { 
                case Keys.Left:
                    getFrame(-1);
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

            addAnnotation(roi);
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

            foreach (var ann in getAllAnnotations())
            {
                annotationImage.Draw(ann, Bgr8.Red, 3);
            }

            if (!roi.IsEmpty)
            {
                annotationImage.Draw(roi, Bgr8.Red, 3);
            }
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
        Dictionary<string, List<Rectangle>> annotations;

        private void readAnnotations()
        {
            annotations = new Dictionary<string, List<Annotation>>();

            annotationStream.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(annotationStream))
            {
                var data = deserializeData(reader.ReadLine());
                annotations.Add(data.Key, data.Value);
            }
        }

        private void writeAnnotation(string imageName, Annotation annotation)
        {
            var serializedAnn = serializeAnnotation(annotation);

            if (annotations.ContainsKey(imageName))
                annotations[imageName].Add(annotation);
            else
            { 
                var list = new List<Annotation>();
                list.Add(annotation);
                annotations.Add(imageName, list);
            }



            annotationWriter.WriteLine(line);
            annotationWriter.Flush();
        }

        private static int findLine(Stream stream, string id, string data)
        {
            stream.

            using (var reader = new StreamReader(annotationStream))
            {
                var data = deserializeData(reader.ReadLine());
                annotations.Add(data.Key, data.Value);
            }
        }

        private static string serializeAnnotation(Rectangle rect)
        {
            return String.Format("{0} {1} {2} {3}", rect.X, rect.Y, rect.Width, rect.Height);
        }

        const char SEPARATOR = ',';
        private static string serializeFrameData(string imageName, List<Annotation> annotations)
        {
            string line = imageName + SEPARATOR;
            foreach (var ann in annotations)
            {
                line += serializeAnnotation(ann) + SEPARATOR;
            }

            line.Remove(line.Length - 1);
            return line;
        }

        private static Annotation deserializeAnnotation(string serializedRect)
        {
            var parts = serializedRect.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return new Annotation 
            {
                X =      Int32.Parse(parts[0]),
                Y =      Int32.Parse(parts[1]),
                Width =  Int32.Parse(parts[2]),
                Height = Int32.Parse(parts[3])
            };
        }

        private static KeyValuePair<string, List<Annotation>> deserializeData(string data)
        {
            var parts = data.Split(new char[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);

            var fileName = parts[0];
            var annotations = parts.Select(x => deserializeAnnotation(x)).ToList();

            return new KeyValuePair<string, List<Annotation>>(fileName, annotations);
        }

        #endregion

        private void ObjectAnnotater_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (capture != null)
                capture.Close();

            if (annotationWriter != null)
                annotationWriter.Dispose();
        }
    }
}
