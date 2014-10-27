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
using Accord.Extensions.Imaging;
using Accord.Extensions.Vision;
using Point = AForge.IntPoint;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using System.Collections.ObjectModel;

namespace ObjectAnnotator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ImageStreamReader capture = null;
            string databaseFileName = null;

            using (var wizard = new Wizard())
            {
                wizard.ShowDialog();

                capture = wizard.CaptureObj;
                databaseFileName = wizard.DatabaseFileName;
            }//*/

            //capture = new ImageDirectoryReader(@"S:\Svjetla - baza podataka\prepared\prednja+straznja-svjetla-4\", "*.jpg");
            //databaseFileName = @"S:\Svjetla - baza podataka\prepared\prednja+straznja-svjetla-4.xml";//

            if (capture == null && databaseFileName == null) //a user clicked "X" without data selection
            {
                return;
            }
            else if (databaseFileName == null)
            {
                MessageBox.Show("Capture or database file name is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            capture.Open();

            if (capture.Length == 0)
            {
                MessageBox.Show("The directory does not contain images!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (capture != null && databaseFileName != null)
            {
                AnnotaterForm form = null;
                try
                {
                    form = new AnnotaterForm(capture, databaseFileName);
                    Application.Run(form);
                }
                catch (Exception)
                {
                    var fInfo = new FileInfo(databaseFileName);
                    var autosaveName = fInfo.Name.Replace(fInfo.Extension, String.Empty) + "-autosave" + fInfo.Extension;

                    autosaveName = Path.Combine(fInfo.DirectoryName, autosaveName);
                    form.Database.Save(autosaveName);

                    var msg = "Unfortunately not your fault :/" + "\r\n" +
                              "However your work is successfully saved to:" + "\r\n" +
                              autosaveName;

                    MessageBox.Show(msg, "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }//*/
            }

            capture.Close();
        }
    }
}
