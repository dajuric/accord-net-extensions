using System;
using System.IO;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Vision;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;

namespace ObjectAnnotater
{
    public partial class Wizard : Form
    {
        private string imageDirPath = null;

        public Wizard()
        {
            InitializeComponent();
            boxImageFormatSelection.SelectedIndex = 0;
        }

        private void btnImageSeq_Click(object sender, EventArgs e)
        {
            var ext = (string)boxImageFormatSelection.SelectedItem;

            using (var diag = new FolderBrowserDialog())
            {
                diag.ShowNewFolderButton = false;

                var result = diag.ShowDialog();
                if (result == DialogResult.OK)
                {
                    CaptureObj = new ImageDirectoryReader(diag.SelectedPath, ext);

                    imageDirPath = diag.SelectedPath;
                    btnSaveAnnotations.Enabled = true;
                }
            }
        }

        private void isAnnotationFileValid(string annFilePath)
        {
            if (btnSelectVideo.Checked)
                return;

            //if images...
            if (imageDirPath.IsSubfolder(new FileInfo(annFilePath).DirectoryName) == false)
            {
                MessageBox.Show("Cannot find relative path of the selected image directory regarding the database path! \n" +
                                "The database location must be in the same or in parent folder regarding selected image directory.",

                                "Incorrect database path selection",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
        }

        private void btnVideo_Click(object sender, EventArgs e)
        {
            using (var diag = new OpenFileDialog())
            {
                diag.Filter = "Video files (*.avi, *.mp4, *.wmv, *.mxf)|*.avi;*.mp4;*.wmv;*.mxf|All Files (*.*)|*.*";

                var result = diag.ShowDialog();
                if (result == DialogResult.OK)
                {
                    CaptureObj = new FileCapture(diag.FileName);
                    btnSaveAnnotations.Enabled = true;
                }
            }
        }

        private void btnSaveAnnotations_Click(object sender, EventArgs e)
        {
            using (var diag = new SaveFileDialog())
            {
                diag.Filter = "(*.xml)|*.xml";
                diag.OverwritePrompt = false;

                var result = diag.ShowDialog();
                if (result == DialogResult.OK)
                {
                    isAnnotationFileValid(diag.FileName);

                    this.DatabaseFileName = diag.FileName;
                    this.lblAnnFile.Text = "Annotation file:" + "\n" + new FileInfo(diag.FileName).Name;
                }
            }
        }

        public ImageStreamReader CaptureObj { get; private set; }
        public string DatabaseFileName { get; private set; }

        private void btnSelectImages_Click(object sender, EventArgs e)
        {
            gpBoxImageSequence.Enabled = true;
            gpBoxVideo.Enabled = false;
        }

        private void btnSelectVideo_Click(object sender, EventArgs e)
        {
            gpBoxImageSequence.Enabled = false;
            gpBoxVideo.Enabled = true;
        }
    }
}
