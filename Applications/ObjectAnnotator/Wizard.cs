#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.IO;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Vision;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;

namespace ObjectAnnotator
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
            var searchPatterns = ((string)boxImageFormatSelection.SelectedItem).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            using (var diag = new FolderBrowserDialog())
            {
                diag.ShowNewFolderButton = false;

                var result = diag.ShowDialog();
                if (result == DialogResult.OK)
                {
                    CaptureObj = new ImageDirectoryReader(diag.SelectedPath, searchPatterns, useNaturalSorting: true, recursive: chkRecursive.Checked);

                    imageDirPath = diag.SelectedPath;
                    btnSaveAnnotations.Enabled = true;
                }
            }
        }

        private bool isAnnotationFileValid(string annFilePath)
        {
            //if images...
            if (imageDirPath.IsSubfolder(new FileInfo(annFilePath).DirectoryName) == false)
            {
                MessageBox.Show("Cannot find relative path of the selected image directory regarding the database path! \n" +
                                "The database location must be in the same or in parent folder regarding selected image directory.",

                                "Incorrect database path selection",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
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
                    bool isPathValid = isAnnotationFileValid(diag.FileName);
                    if (!isPathValid)
                        return;

                    this.DatabaseFileName = diag.FileName;
                    this.lblAnnFile.Text = "Annotation file:" + "\n" + new FileInfo(diag.FileName).Name;
                }
            }
        }

        public ImageStreamReader CaptureObj { get; private set; }
        public string DatabaseFileName { get; private set; }
    }
}
