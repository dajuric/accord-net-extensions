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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Accord.Extensions.Imaging;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// Image template which contains object mask. It can be created only from a black-white image.
    /// </summary>
    public class ImageTemplateWithMask: ImageTemplate
    {
        /// <summary>
        /// Object's binary mask.
        /// </summary>
        public Gray<byte>[,] BinaryMask
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
        public override void Initialize(Gray<byte>[,] sourceImage, int minFeatureStrength, int maxNumberOfFeatures, string classLabel)
        {
            base.Initialize(sourceImage, minFeatureStrength, maxNumberOfFeatures, classLabel);

            this.BinaryMask = sourceImage.Clone(BoundingRect);

            if (this.BinaryMask[0, 0].Intensity != 0) //background should be black
                BinaryMask = this.BinaryMask.Not();

            this.BinaryMask = this.BinaryMask.ThresholdToZero((byte)(255 * 0.75), (byte)255); //if Gauss kernel was applied...
        }

        /// <summary>
        /// This kind of template can not be created from color images. This function will throw an exception <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="sourceImage">Input image.</param>
        /// <param name="minFeatureStrength">Minimum gradient value for the feature.</param>
        /// <param name="maxNumberOfFeatures">Maximum number of features per template. The features will be extracted so that their locations are semi-uniformly spread.</param>
        /// <param name="classLabel">Template class label.</param>
        public override void Initialize(Bgr<byte>[,] sourceImage, int minFeatureStrength, int maxNumberOfFeatures, string classLabel)
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
                this.BinaryMask = (Gray<byte>[,])(new XmlSerializer(typeof(Image<Gray<byte>>))).Deserialize(new StringReader(binaryMaskData));
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
            (new XmlSerializer(typeof(Gray<byte>[,]))).Serialize(new StringWriter(sb), this.BinaryMask);

            xElem.Add(XElement.Parse(sb.ToString()) /*sb.ToString()*/);
            xElem.WriteTo(writer);
        }

        #endregion   
    }
}
