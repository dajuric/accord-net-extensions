using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Accord.Extensions.Imaging;

namespace Accord.Extensions.Imaging.Algorithms.LNE2D
{
    /// <summary>
    /// Image template which contains object mask. It can be created only from a black-white image.
    /// </summary>
    public class ImageTemplateWithMask: ImageTemplate
    {
        /// <summary>
        /// Object's binary mask.
        /// </summary>
        public Image<Gray, byte> BinaryMask
        {
            get;
            private set;
        }

        /// <summary>
        /// True if the template contains binary mask, false otherwise.
        /// </summary>
        public bool HasBinaryMask { get { return BinaryMask != null; } }

        /// <summary>
        /// Creates template from the input image by using provided parameters.
        /// </summary>
        /// <param name="sourceImage">Input image.</param>
        /// <param name="minFeatureStrength">Minimum gradient value for the feature.</param>
        /// <param name="maxNumberOfFeatures">Maximum number of features per template. The features will be extracted so that their locations are semi-uniformly spread.</param>
        /// <param name="classLabel">Template class label.</param>
        public override void Initialize(Image<Gray, byte> sourceImage, int minFeatureStrength, int maxNumberOfFeatures, string classLabel)
        {
            base.Initialize(sourceImage, minFeatureStrength, maxNumberOfFeatures, classLabel);

            this.BinaryMask = sourceImage.GetSubRect(BoundingRect).Clone();

            if (this.BinaryMask[0, 0].Intensity != 0) //background should be black
                BinaryMask = this.BinaryMask.Not();

            this.BinaryMask = this.BinaryMask.ThresholdToZero(255 * 0.75, 255); //if Gauss kernel was applied...
        }

        /// <summary>
        /// This kind of template can not be created from color images. This function will throw an exception <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="sourceImage">Input image.</param>
        /// <param name="minFeatureStrength">Minimum gradient value for the feature.</param>
        /// <param name="maxNumberOfFeatures">Maximum number of features per template. The features will be extracted so that their locations are semi-uniformly spread.</param>
        /// <param name="classLabel">Template class label.</param>
        public override void Initialize(Image<Bgr, byte> sourceImage, int minFeatureStrength, int maxNumberOfFeatures, string classLabel)
        {
            throw new Exception("Binary mask can not be saved from non black-white image!");
        }

        #region ISerializable

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">Reader's stream.</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            var element = XElement.Load(reader);
            //var binaryMaskData = element.Descendants("BinaryMask").First().Value; //comp old-ver
            var binaryMaskData = element.Descendants("BinaryMask").Elements().First().ToString(); //get BinaryMask element

            if(String.IsNullOrWhiteSpace(binaryMaskData) == false)
                this.BinaryMask = (Image<Gray, Byte>)(new XmlSerializer(typeof(Image<Gray, Byte>))).Deserialize(new StringReader(binaryMaskData));
        }

        /// <summary>
        /// Generates XML representation for the object.
        /// </summary>
        /// <param name="writer">Writers stream.</param>
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
