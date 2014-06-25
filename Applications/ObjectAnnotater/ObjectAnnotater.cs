using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using Point = AForge.IntPoint;
using RangeF = AForge.Range;

namespace ObjectAnnotater
{
    public partial class ObjectAnnotater : Form
    {
        StreamableSource capture = null;
        string databaseFileName = null;

        Annotation selectedAnnotation = null;
        List<Annotation> frameAnnotations = null;

        public Database Database { get; private set; }

        public ObjectAnnotater()
        {
            InitializeComponent();
        }

        public ObjectAnnotater(StreamableSource capture, string databaseFileName)
        {
            InitializeComponent();

            this.capture = capture;

            this.databaseFileName = databaseFileName;
            Database = new Database();
            Database.Load(databaseFileName);

            this.frameAnnotations = new List<Annotation>();

            loadCurrentImageAnnotations();
            showCurrentInfo();

            /*foreach (var key in Database.Keys)
            { 
                capture.Seek(Int32.Parse(key), SeekOrigin.Begin);
                capture.ReadAs<Bgr, byte>().Save(String.Format(@"S:\Svjetla - baza podataka - Boris\Stražnja\1\{0}.png", key));
            }*/

            /*
            foreach (var key in Database.Keys)
            {
                foreach (var imgAnn in Database[key])
                {
                    var rect = imgAnn.Polygon.BoundingRect();

                    if (Math.Abs((float)rect.Width / rect.Height - 2.2) > 1.5E-1)
                    {
                        throw new ArgumentException("Region size width-height ratio must be equal to WindowWidthMultiplier! (tolerance +/- 0.1)");
                    }

                    if (rect.X < 0 || rect.Y < 0 || rect.Right > 1280 || rect.Bottom > 720)
                        throw new Exception();
                }
            }
            */
        }

        Image<Bgr, byte> frame = null;

        #region Commands

        private void loadCurrentImageAnnotations()
        {
            if (capture.Position == capture.Length)
                return;

            frame = capture.ReadAs<Bgr, byte>(); //the order is relevant (position is automatically increased)

            var imageKey = getCurrentImageKey();

            if (Database.ContainsKey(imageKey) == false)
                frameAnnotations = new List<Annotation>();
            else
                frameAnnotations = new List<Annotation>(Database[imageKey]);
        }

        private void showCurrentInfo()
        {
            /*if (capture.Position == capture.Length)
                return;*/

            drawAnnotations();
           
            this.Text = getCurrentImageKey() + " -> " + new FileInfo(databaseFileName).Name;

            if (selectedAnnotation != null)
            {
                this.txtAnnotationLabel.Text = selectedAnnotation.Label;
                this.txtAnnotationLabel.SelectionStart = this.txtAnnotationLabel.SelectionLength = 0;
            }

            showFrameInfo();
        }

        private void showFrameInfo()
        {
            this.slider.Value = (int)Math.Max(0, this.capture.Position - 1);
            this.slider.Maximum = (int)(capture.Length - 1);

            this.lblCurrentFrame.Text = this.slider.Value.ToString();
            this.lblTotalFrames.Text = this.slider.Maximum.ToString();
        }

        private void saveCurrentImageAnnotations()
        {
            if (capture.Position == capture.Length)
                return;

            var imageKey = getCurrentImageKey();

            if (frameAnnotations.Count == 0 && !Database.ContainsKey(imageKey))
                return; //do not save images that does not have annotations

            Database[imageKey] = frameAnnotations.ToList();
        }

        private void getFrame(long offset)
        {
            this.selectedAnnotation = null;
            saveCurrentImageAnnotations();

            if(offset != capture.Position) //faster when reading video
                capture.Seek(offset, SeekOrigin.Begin);

            loadCurrentImageAnnotations();

            showCurrentInfo();
            GC.Collect();
        }

        private string getCurrentImageKey()
        {
            string imageKey = null;

            if (capture is ImageDirectoryReader)
            {
                capture.Seek(-1);
                imageKey = (capture as ImageDirectoryReader).CurrentImageName;
                imageKey = new FileInfo(imageKey).Name;
                capture.Seek(+1);
            }
            else if (capture is FileCapture)
            {
                imageKey = Math.Max(0, capture.Position - 1).ToString();
            }
            else
                throw new NotSupportedException("Unsupported image stream reader!");

            return imageKey;
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
                selectedAnnotation = new Annotation { Label = this.txtAnnotationLabel.Text, Polygon = roi.Vertices() };
                frameAnnotations.Add(selectedAnnotation);
                enableSave();
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
            //check if it is null
            if (control.Image == null) return coordinates;
            // try get size
            Size imageSize = control.Image.Size.ToSize();

            // Make sure our control width and height are not 0 and our 
            // image width and height are not 0
            if (control.Width == 0 || control.Height == 0 || imageSize.Width == 0 || imageSize.Height == 0) return coordinates;
            // This is the one that gets a little tricky. Essentially, need to check 
            // the aspect ratio of the image to the aspect ratio of the control
            // to determine how it is being rendered
            float imageAspect = (float)imageSize.Width / imageSize.Height;
            float controlAspect = (float)control.Width / control.Height;
            float newX = coordinates.X;
            float newY = coordinates.Y;
            if (imageAspect > controlAspect)
            {
                // This means that we are limited by width, 
                // meaning the image fills up the entire control from left to right
                float ratioWidth = (float)imageSize.Width / control.Width;
                newX *= ratioWidth;
                float scale = (float)control.Width / control.Image.Width;
                float displayHeight = scale * imageSize.Height;
                float diffHeight = control.Height - displayHeight;
                diffHeight /= 2;
                newY -= diffHeight;
                newY /= scale;
            }
            else
            {
                // This means that we are limited by height, 
                // meaning the image fills up the entire control from top to bottom
                float ratioHeight = (float)imageSize.Height / control.Height;
                newY *= ratioHeight;
                float scale = (float)control.Height / imageSize.Height;
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
            bool showAnnLabels = btnToggleLabels.Checked;

            var annotationImage = frame.Clone();
            var drawingFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);

            foreach (var ann in frameAnnotations)
            {
                Rectangle rect = ann.Polygon.BoundingRect();

                var annLabel = showAnnLabels ? ann.Label : "";
                var annLabelWidth = showAnnLabels ? TextRenderer.MeasureText(ann.Label, drawingFont).Width : 0;

                if(ann == selectedAnnotation)
                    annotationImage.DrawAnnotation(rect, annLabel, annLabelWidth, Bgr8.Red, Bgr8.Black, drawingFont);
                else
                    annotationImage.DrawAnnotation(rect, annLabel, annLabelWidth, Bgr8.Black, Bgr8.Black, drawingFont);
            }

            if (!roi.IsEmpty)
            {
                annotationImage.Draw(roi, Bgr8.Red, 3);
            }

            this.pictureBox.Image = annotationImage.ToBitmap(copyAlways: true);
        }

        #endregion

        private void ObjectAnnotater_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isModified)
            {
                var result = MessageBox.Show("Do you want to save current changes ?", "Save annotations", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == System.Windows.Forms.DialogResult.Yes)
                    saveToFile();
            }
        }

        private void txtLabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control || e.Alt || e.KeyData == Keys.Delete || selectedAnnotation == null)
                e.Handled = true;
        }

        private void txtLabel_KeyUp(object sender, KeyEventArgs e)
        {
            if (selectedAnnotation == null)
                return;

            selectedAnnotation.Label = txtAnnotationLabel.Text;
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
                this.txtAnnotationLabel.Text = selectedAnnotation.Label;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Delete:
                    if (selectedAnnotation != null)
                    {
                        frameAnnotations.Remove(selectedAnnotation);
                        drawAnnotations();
                        enableSave();
                    }
                    break;

                case Keys.Control | Keys.A:
                    this.btnToggleLabels.Checked = !btnToggleLabels.Checked;
                    drawAnnotations();
                    break;

                case Keys.Control | Keys.S:
                    saveToFile();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnToggleLabels_CheckedChanged(object sender, EventArgs e)
        {
            drawAnnotations();
        }

        private void enableSave()
        {
            isModified = true;
            this.btnSave.Enabled = true;
        }

        bool isModified = false;
        private void saveToFile()
        {
            saveCurrentImageAnnotations();
            Database.Save(databaseFileName);

            isModified = false;
            this.btnSave.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveToFile();          
        }

        private void slider_ValueChanged(object sender, EventArgs e)
        {
            getFrame(slider.Value);
        }

        private void btnPrepareSamples_Click(object sender, EventArgs e)
        {
            var prepareSamplesForm = new SampleGeneration.SamplePreparation(this.Database);
            prepareSamplesForm.ShowDialog(this);
        }
    }
}
