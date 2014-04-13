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
    /// <para>methods of this class can be used as extensions.</para>
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
            [XmlAttribute]
            public string ImageKey { get; set; }
            public List<TAnnotation> Annotations { get; set; }
        }

        /// <summary>
        /// Gets the root element name of the serialized database.
        /// </summary>
        public static readonly string ROOT_ELEMENT = "Annotations";

        /// <summary>
        /// Load the annotation database.
        /// </summary>
        /// <param name="data">Empty database.</param>
        /// <param name="fileName">Database file.</param>
        public static void Load(this Database data, string fileName)
        {
            if (!File.Exists(fileName))
                return;

            XDocument doc = XDocument.Load(fileName);

            var elems = doc.Element(ROOT_ELEMENT).Elements().Select(x => x.FromXElement<ImageAnnotations<Annotation>>());

            foreach (var imageAnnotations in elems)
            {
                data[imageAnnotations.ImageKey] = imageAnnotations.Annotations;
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
    }
}
