using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MoreLinq;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using Point = AForge.IntPoint;
using RangeF = AForge.Range;

namespace ObjectAnnotater
{
    public partial class ObjectAnnotater : Form
    {
        ImageDirectoryReader capture = null;
        Database database = null;
        string databaseFileName = null;

        Annotation selectedAnnotation = null;
        List<Annotation> frameAnnotations = null;

        public ObjectAnnotater()
        {
            InitializeComponent();
        }

        public ObjectAnnotater(ImageDirectoryReader capture, string databaseFileName)
        {
            InitializeComponent();

            this.capture = capture;

            this.databaseFileName = databaseFileName;
            this.database = new Database();
            database.Load(databaseFileName);

            this.frameAnnotations = new List<Annotation>();

            loadCurrentImageAnnotations();
            showCurrentInfo();

            var nAnns = database.NumberOfAnnotations(x => x.Contains("Car"));
            Console.WriteLine(nAnns);

            /*var proceesedSamples = database.ProcessSamples(2.15f, 
                                                           new Pair<RangeF>(new RangeF(-0.05f, +0.05f), new RangeF(-0.05f, +0.05f)),
                                                           new Pair<RangeF>(new RangeF(0.9f, 1.1f), new RangeF(0.9f, 1.1f)), 
                                                           5, 
                                                           0.1f);

            proceesedSamples.Save("S:\\procsssedImagesAnnotations.xml");
            database = proceesedSamples;*/
        }

        Image<Bgr, byte> frame = null;

        #region Commands

        private void loadCurrentImageAnnotations()
        {
            if (capture.Position == capture.Length)
                return;

            var imageKey = capture.CurrentImageName.GetRelativeFilePath(new FileInfo(databaseFileName).Directory);

            if (database.ContainsKey(imageKey) == false)
                frameAnnotations = new List<Annotation>();
            else
                frameAnnotations = new List<Annotation>(database[imageKey]);

            frame = capture.ReadAs<Bgr, byte>();
            capture.Seek(-1); //do not move position
        }

        private void showCurrentInfo()
        {
            if (capture.Position == capture.Length)
                return;

            drawAnnotations();
            this.lblFrameIndex.Text = (this.capture.Position + 1).ToString();
            this.Text = capture.CurrentImageName.GetRelativeFilePath(new FileInfo(databaseFileName).Directory) + " -> " + new FileInfo(databaseFileName).Name;

            if (selectedAnnotation != null)
            {
                this.txtLabel.Text = selectedAnnotation.Label;
                this.txtLabel.SelectionStart = this.txtLabel.SelectionLength = 0;
            }
        }

        private void saveCurrentImageAnnotations()
        {
            if (capture.Position == capture.Length)
                return;

            var imageKey = capture.CurrentImageName.GetRelativeFilePath(new FileInfo(databaseFileName).Directory);

            if (frameAnnotations.Count == 0 && !database.ContainsKey(imageKey))
                return; //do not save images that does not have annotations

            database[imageKey] = frameAnnotations.ToList();
            //database.Save(databaseFileName); //slows to much at fast viewing (only at app closing)
        }

        private void getFrame(long offset)
        {
            this.selectedAnnotation = null;
            saveCurrentImageAnnotations(); 

            capture.Seek(offset);
            loadCurrentImageAnnotations();

            showCurrentInfo();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            { 
                case Keys.Left:
                    getFrame(-1);
                    break;
                case Keys.Right:
                    getFrame(+1);
                    break;
                case Keys.Delete:
                    if (selectedAnnotation != null)
                        frameAnnotations.Remove(selectedAnnotation);
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Object selection

        Rectangle roi = Rectangle.Empty;
        bool isSelecting = false;
        Point ptFirst;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
      
            ptFirst = translateZoomMousePosition(this.pictureBox, e.Location.ToPt());
            isSelecting = true;
        }

        const int MIN_RECT_SIZE = 5;
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (frame == null || !isSelecting) return;

            roi = roi.Intersect(frame.Size);

            if (roi.Width >= MIN_RECT_SIZE && roi.Height > MIN_RECT_SIZE)
            {
                selectedAnnotation = new Annotation { Label = this.txtLabel.Text, Polygon = roi.Vertices() };
                frameAnnotations.Add(selectedAnnotation);
            }

            roi = Rectangle.Empty;
            isSelecting = false;

            drawAnnotations();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !isSelecting)
                return;

            var ptSecond = translateZoomMousePosition(this.pictureBox, e.Location.ToPt());

            roi = new Rectangle
            {
                X = System.Math.Min(ptFirst.X, ptSecond.X),
                Y = System.Math.Min(ptFirst.Y, ptSecond.Y),
                Width = System.Math.Abs(ptFirst.X - ptSecond.X),
                Height = System.Math.Abs(ptFirst.Y - ptSecond.Y)
            };

            drawAnnotations();
        }

        //taken from: http://www.codeproject.com/Articles/20923/Mouse-Position-over-Image-in-a-PictureBox
        private static Point translateZoomMousePosition(PictureBox control, Point coordinates)
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

        private void drawAnnotations()
        {
            if (frame == null) return;

            var annotationImage = frame.Clone();
            var drawingFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);

            foreach (var ann in frameAnnotations)
            {
                var annLabelWidth = TextRenderer.MeasureText(ann.Label, drawingFont).Width;

                Rectangle rect = ann.Polygon.BoundingRect();

                if(ann == selectedAnnotation)
                    annotationImage.DrawAnnotation(rect, ann.Label, annLabelWidth, Bgr8.Red, Bgr8.Black, drawingFont);
                else
                    annotationImage.DrawAnnotation(rect, ann.Label, annLabelWidth, Bgr8.Black, Bgr8.Black, drawingFont);
            }

            if (!roi.IsEmpty)
            {
                annotationImage.Draw(roi, Bgr8.Red, 3);
            }

            this.pictureBox.Image = annotationImage.ToBitmap();
        }

        #endregion

        private void ObjectAnnotater_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveCurrentImageAnnotations();
            database.Save(databaseFileName);
        }

        private void txtLabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control || e.Alt || selectedAnnotation == null)
                e.Handled = true;
        }

        private void txtLabel_KeyUp(object sender, KeyEventArgs e)
        {
            if (selectedAnnotation == null)
                return;

            selectedAnnotation.Label = txtLabel.Text;
            drawAnnotations();
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var imagePt = translateZoomMousePosition(this.pictureBox, e.Location.ToPt());
            selectedAnnotation = frameAnnotations.Where(x => 
            {
                Rectangle rect = x.Polygon.BoundingRect();
                return rect.Contains(imagePt);
            })
            .FirstOrDefault();

            if (selectedAnnotation != null)
                this.txtLabel.Text = selectedAnnotation.Label;
        }
    }
}
