using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Accord.Extensions.Imaging;

namespace LINE2D
{
    public class ImageTemplateWithMask: ImageTemplate
    {
        public Image<Gray, byte> BinaryMask
        {
            get;
            private set;
        }

        public bool HasBinaryMask { get { return BinaryMask != null; } }

        public override void Initialize(Image<Gray, byte> sourceImage, int minFeatureStrength, int maxNumberOfFeatures, string classLabel)
        {
            base.Initialize(sourceImage, minFeatureStrength, maxNumberOfFeatures, classLabel);

            this.BinaryMask = sourceImage.GetSubRect(boundingRect).Clone();

            if (this.BinaryMask[0, 0].Intensity != 0) //background should be black
                BinaryMask = this.BinaryMask.Not();

            this.BinaryMask = this.BinaryMask.ThresholdToZero(255 * 0.75, 255); //if Gauss kernel was applied...
        }

        public override void Initialize(Image<Bgr, byte> sourceImage, int minNumOfFeatures, int maxNumberOfFeatures, string classLabel)
        {
            throw new Exception("Binary mask can not be saved from non black-white image!");
        }

        #region ISerializable

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            var element = XElement.Load(reader);
            //var binaryMaskData = element.Descendants("BinaryMask").First().Value; //comp old-ver
            var binaryMaskData = element.Descendants("BinaryMask").Elements().First().ToString(); //get BinaryMask element

            if(String.IsNullOrWhiteSpace(binaryMaskData) == false)
                this.BinaryMask = (Image<Gray, Byte>)(new XmlSerializer(typeof(Image<Gray, Byte>))).Deserialize(new StringReader(binaryMaskData));
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            XElement xElem = new XElement("BinaryMask");

            StringBuilder sb = new StringBuilder();
            (new XmlSerializer(typeof(Image<Gray, Byte>))).Serialize(new StringWriter(sb), this.BinaryMask);

            xElem.Add(XElement.Parse(sb.ToString()) /*sb.ToString()*/);
            xElem.WriteTo(writer);
        }

        #endregion   
    }
}
