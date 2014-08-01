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
        /// Creates new empty annotation.
        /// </summary>
        public Annotation()
        {
            this.Label = String.Empty;
            this.Polygon = new Point[0];
            this.Tag = null;
        }

        /// <summary>
        /// Gets or sets the annotation label
        /// </summary>
        [XmlAttribute]
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the object contour.
        /// <para>See rectangle to vertices extension to transform rectangle into set of points.</para>
        /// </summary>
        public Point[] Polygon { get; set; }
        /// <summary>
        /// Additional annotation data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Clones annotation. Tag is cloned only is implements <see cref="System.ICloneable"/> interface.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Annotation 
            {
                Label = this.Label,
                Polygon = (Point[])this.Polygon.Clone(),
                Tag = this.Tag is ICloneable ? ((ICloneable)this.Tag).Clone() : this.Tag
            };
        }
    }
}
