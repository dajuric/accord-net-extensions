using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using Accord.Imaging;

namespace LINE2D.TemplateMatching
{
    public class TemplateWithObjectMask: Template
    {       
        internal override void SerializeAditionalData(XElement node)
        {
            base.SerializeAditionalData(node);

            XElement xElem = new XElement("BinaryMask");

            //this.BinaryMask.SerializationCompressionRatio = 9;

            StringBuilder sb = new StringBuilder();
            (new XmlSerializer(typeof(Image<Gray, Byte>))).Serialize(new StringWriter(sb), this.BinaryMask);

            xElem.SetValue(sb.ToString());

            node.Add(xElem);
        }

        internal override void DeserializeAditionalData(XElement metadataNode)
        {
            base.DeserializeAditionalData(metadataNode);

            if (metadataNode != null && !metadataNode.IsEmpty)
            {
                this.HasMetadata = true;

                XElement binaryMaskData = metadataNode.Descendants("BinaryMask").First();

                this.BinaryMask = (Image<Gray, Byte>)(new XmlSerializer(typeof(Image<Gray, Byte>))).Deserialize(new StringReader(binaryMaskData.Value));
            }
        }

        public override void Initialize(Image<Gray, byte> sourceImage, int maxNumberOfFeatures, string classLabel)
        {
            base.Initialize(sourceImage, maxNumberOfFeatures, classLabel);
            this.HasMetadata = true;

            this.BinaryMask = sourceImage.GetSubRect(this.boundingRect).Clone();

            if (this.BinaryMask[0, 0].Intensity != 0) //background should be black
                BinaryMask = this.BinaryMask.Not();

            this.BinaryMask = this.BinaryMask.ThresholdToZero(new Gray(255 * 0.75), new Gray(Byte.MaxValue)); //if Gauss kernel was applied...
        }

        public Image<Gray, byte> BinaryMask
        {
            get;
            private set;
        }

    }
}
