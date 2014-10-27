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

using Accord.Extensions.Vision;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using Point = AForge.IntPoint;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Imaging;
using System.Collections.Generic;

namespace ObjectAnnotator
{
    public partial class SampleExtraction : Form
    {
        Database database = null;
        ImageStreamReader capture = null;
        Func<long, string> imageKeyFunc = null;

        //for designer
        public SampleExtraction()
        {
            InitializeComponent();
        }

        public SampleExtraction(Database database, ImageStreamReader capture, Func<long, string> imageKeyFunc)
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

            this.database = database;
            this.capture = capture;
            this.imageKeyFunc = imageKeyFunc;
        }

        /// <summary>
        /// Extracts annotated samples - their bounding boxes.
        /// </summary>
        /// <param name="labelMatch">Regular expression for valid label.</param>
        private void extractPositives(Regex labelMatch)
        {
            if (database == null || capture == null || imageKeyFunc == null)
                return; //form not initialized

            var currPos = capture.Position;

            for (int i = 0; i < capture.Length; i++)
            {
                var imgKey = imageKeyFunc(i);
                if (database.ContainsKey(imgKey))
                {
                    capture.Seek(i, SeekOrigin.Begin);
                    var imgName = (capture as ImageDirectoryReader).CurrentImageName;
                    var img = capture.Read();

                    var labelCounter = new Dictionary<string, int>();
                    foreach (var annotation in database[imgKey])
                    {
                        if (labelMatch.IsMatch(annotation.Label) == false)
                            continue;

                        //**************** get current label instance index ***************/
                        if (labelCounter.ContainsKey(annotation.Label) == false)
                            labelCounter.Add(annotation.Label, 0);

                        labelCounter[annotation.Label]++;

                        var label = annotation.Label;
                        if (labelCounter[annotation.Label] > 1)
                            label += " " + "(" + labelCounter[annotation.Label] + ")";
                        //**************** get current label instance index ***************/

                        var databaseDir = imgName.Replace(imgKey, String.Empty);
                        var samplePath = getOutputImageName(databaseDir, imgKey, label);
                        Directory.CreateDirectory(Path.GetDirectoryName(samplePath));

                        var sampleImg = img.GetSubRect(annotation.BoundingRectangle);
                                            //sampleImg = (sampleImg as Image<Bgr, byte>).SmoothGaussian(5);
                        sampleImg.ToBitmap().Save(samplePath, quality: 95);
                    }
                }
            }

            capture.Seek(currPos, SeekOrigin.Begin);
        }

        private static string getOutputImageName(string databasePath, string imgKey, string label)
        {
            string OUTPUT_DIR_EXTENSION = "_ExtractedSamples";

            var imgRelativePath = Path.GetDirectoryName(imgKey).Trim(Path.DirectorySeparatorChar);
            var imgName = Path.GetFileNameWithoutExtension(imgKey) + "_" + label + ".jpg";

            //<imagePath>__ExtractedSamples\<imgName>_<label>.jpg
            return Path.Combine(databasePath, imgRelativePath + OUTPUT_DIR_EXTENSION, imgName);
        }

        private void btnStart_Click(object sender, System.EventArgs e)
        {
            var labelSeachPattern = new Regex(this.txtPattern.Text);

            btnStart.Text = "Extracting...";
            extractPositives(labelSeachPattern);
            //MessageBox.Show("Done.", "Annotation extraction", MessageBoxButtons.OK);
            this.Close();
        }
    }
}
