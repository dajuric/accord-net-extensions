#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// <para>Methods of this class can be used as extensions.</para>
    /// Provides extension methods for the annotation database structure:
    /// <para>    Dictionary(imageKey, list of annotations).</para>
    /// </summary>
    public static class AnnotationXmlDatabaseExtensions
    {
        /// <summary>
        /// This class encapsulates all annotations in an image. 
        /// It is used internally, but it is public due to serialization / deserialization request.
        /// </summary>
        /// <typeparam name="TAnnotation">Type of annotation.</typeparam>
        [Serializable]
        public class ImageAnnotations<TAnnotation>
        {
            /// <summary>
            /// Image key.
            /// </summary>
            [XmlAttribute]
            public string ImageKey { get; set; }
            /// <summary>
            /// List of annotations for the image specified by the key.
            /// </summary>
            public List<TAnnotation> Annotations { get; set; }
        }

        /// <summary>
        /// Gets the root element name of the serialized database.
        /// </summary>
        public static readonly string ROOT_ELEMENT = "Annotations";

        /// <summary>
        /// Load the annotation database. Existing data will not be overwritten.
        /// </summary>
        /// <param name="data">Empty database.</param>
        /// <param name="fileName">Database file.</param>
        public static void Load(this Database data, string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("The file with the specified file name does not exist!");

            XDocument doc = XDocument.Load(fileName);
            var elems = doc.Element(ROOT_ELEMENT).Elements().Select(x => x.FromXElement<ImageAnnotations<Annotation>>());

            foreach (var imageAnnotations in elems)
            {
                if (!data.ContainsKey(imageAnnotations.ImageKey))
                    data[imageAnnotations.ImageKey] = new List<Annotation>();
             
                data[imageAnnotations.ImageKey].AddRange(imageAnnotations.Annotations);
            }
        }

        /// <summary>
        /// Saves database to a xml file.
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="fileName">File name for the serialized database.</param>
        public static void Save(this Database data, string fileName)
        {
            XDocument doc = new XDocument();

            var imagesAnnotations = data.Select(x => new ImageAnnotations<Annotation> 
                                            { 
                                                ImageKey = x.Key, 
                                                Annotations = x.Value
                                            });

            var root = new XElement(ROOT_ELEMENT);
            foreach (var imageAnnotations in imagesAnnotations)
            {
                root.Add(imageAnnotations.ToXElement(writeEmptyNamespace: true));
            }

            doc.Add(root);
            doc.Save(fileName);
        }

        /// <summary>
        /// Merges specified annotation databases into one database and saves it into specified file.
        /// <para>All image paths will be modified so the root of their relatives paths is the output database path.</para>
        /// </summary>
        /// <param name="dbFileNames">Database file names.</param>
        /// <param name="outputDbFileName">Output database file name.</param>
        public static void MergeDatabases(this IEnumerable<string> dbFileNames, string outputDbFileName)
        {
            var outputFolder = Path.GetDirectoryName(outputDbFileName);
            Database outputDb = new Database();

            foreach (var dbFileName in dbFileNames)
            {
                var pathPrefix = Path.DirectorySeparatorChar +
                                 Path.GetDirectoryName(dbFileName).GetRelativeFilePath(new DirectoryInfo(outputFolder))
                                 .Trim(Path.DirectorySeparatorChar);

                Database db = new Database();
                db.Load(dbFileName);

                foreach (var imageKey in db.Keys)
                {
                    var newKey = pathPrefix + imageKey;

                    if (!outputDb.ContainsKey(imageKey))
                        outputDb.Add(newKey, db[imageKey]);
                }
            }

            outputDb.Save(outputDbFileName);
        }

        /// <summary>
        /// Saves annotations of type rectangle to the specified text file as: &lt;image relative path&gt; ;   &lt;bounding box&gt; ; &lt;label&gt;
        /// <para>Each annotation is written in a new line in case of multiple bounding boxes per image.</para>
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="fileName">Text file where to export the object bounding boxes.</param>
        /// <param name="labelSelector">Labels selector.</param>
        public static void ExportBoundingBoxes(this Database data, string fileName, Func<string, bool> labelSelector)
        {
            var txtWriter = File.CreateText(fileName);

            foreach (var d in data)
            {
                var imgKey = d.Key;
                foreach (var ann in d.Value.Where(ann => ann.Type == AnnotationType.Rectangle &&
                                    labelSelector(ann.Label)))
                {
                    var boundingRect = ann.BoundingRectangle;
                    var textLine = String.Format("{0}; {1}; {2} {3} {4} {5}", imgKey, ann.Label, boundingRect.X, boundingRect.Y, boundingRect.Width, boundingRect.Height);
                    txtWriter.WriteLine(textLine);
                }
            }

            txtWriter.Close();
        }

        /// <summary>
        /// Saves annotations of type rectangle to the specified text file as: &lt;image relative path&gt; ;   &lt;bounding box&gt; ; &lt;label&gt;
        /// <para>Each annotation is written in a new line in case of multiple bounding boxes per image.</para>
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="fileName">Text file where to export the object bounding boxes.</param>
        public static void ExportBoundingBoxes(this Database data, string fileName)
        {
            ExportBoundingBoxes(data, fileName, (annLabel) => true);
        }
    }
}
