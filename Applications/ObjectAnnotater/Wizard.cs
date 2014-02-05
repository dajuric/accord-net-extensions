using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using System.IO;

namespace ObjectAnnotater
{
    public partial class Wizard : Form
    {
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
                    CaptureObj = new ImageDirectoryCapture(diag.SelectedPath, ext,
                                                   (path) => System.Drawing.Bitmap.FromFile(path).ToImage(),
                                                    1);
                }
            }
        }

        private void btnSaveAnnotations_Click(object sender, EventArgs e)
        {
            using (var diag = new SaveFileDialog())
            {
                diag.Filter = "(*.txt)|.txt";
             
                var result = diag.ShowDialog();
                if (result == DialogResult.OK)
                {
                    AnnotationWriter = new StreamWriter(File.Open(diag.FileName, FileMode.Append));
                    this.lblAnnFile.Text = diag.FileName;
                }
            }
        }

        public ImageDirectoryCapture CaptureObj { get; private set; }
        public StreamWriter AnnotationWriter { get; private set; }
    }
}
