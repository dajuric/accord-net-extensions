using Accord.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Point = AForge.IntPoint;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ObjectAnnotater.Annotation>>;

namespace ObjectAnnotater
{
    [Serializable]
    public class ImageAnnotations<TAnnotation>
    {
        [XmlAttribute]
        public string ImageKey { get; set; }
        public List<TAnnotation> Annotations { get; set; }
    }

    [Serializable]
    public class Annotation
    {
        [XmlAttribute]
        public string Label { get; set; }
        public Point[] Polygon { get; set; }
        public object Tag { get; set; }
    }

    public static class AnnotationDatabaseExtensions
    {
        public static readonly string ROOT_ELEMENT = "Annotations";

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
