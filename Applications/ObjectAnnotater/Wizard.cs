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
using Accord.Extensions;
using System.IO;

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

        private void btnSaveAnnotations_Click(object sender, EventArgs e)
        {
            using (var diag = new SaveFileDialog())
            {
                diag.Filter = "(*.xml)|*.xml";
                diag.OverwritePrompt = false;

                var result = diag.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (imageDirPath.IsSubfolder(new FileInfo(diag.FileName).DirectoryName) == false)
                    {
                        MessageBox.Show("Cannot find relative path of the selected image directory regarding the database path! \n" +
                                        "The database location must be in the same or in parent folder regarding selected image directory.", 
                                        
                                        "Incorrect database path selection",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    try
                    {
                        DatabaseFileName = diag.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Database creation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    this.lblAnnFile.Text = "Annotation file:" + "\n" + new FileInfo(diag.FileName).Name;
                }
            }
        }

        public ImageDirectoryReader CaptureObj { get; private set; }
        public string DatabaseFileName { get; private set; }
    }
}
