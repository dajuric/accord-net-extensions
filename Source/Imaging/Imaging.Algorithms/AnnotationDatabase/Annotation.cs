using System;
using System.Xml.Serialization;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Object annotation.
    /// <para>Used during training process. See ObjectAnnotater application in samples.</para>
    /// </summary>
    [Serializable]
    public class Annotation: ICloneable
    {
        /// <summary>
        /// Gets or sets the annotation label
        /// </summary>
        [XmlAttribute]
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the object contour.
        /// <para>See <see cref="Recatngle.Vertices"/> to transform rectangle into set of points.</para>
        /// </summary>
        public Point[] Polygon { get; set; }
        /// <summary>
        /// Additional annotation data.
        /// </summary>
        public object Tag { get; set; }

        public object Clone()
        {
            return new Annotation 
            {
                Label = this.Label,
                Polygon = (Point[])this.Polygon.Clone(),
                Tag = this.Tag
            };
        }
    }
}
