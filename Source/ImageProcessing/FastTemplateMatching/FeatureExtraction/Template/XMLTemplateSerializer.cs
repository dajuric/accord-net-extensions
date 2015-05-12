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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Accord.Extensions;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// Serializes and de-serializes list of template pyramids into/from XML file.
    /// If a template implements <see cref="IXmlSerializable"/> interface additional data provided by a user may be serialized/deserialized as well.
    /// </summary>
    public class XMLTemplateSerializer<TTemplatePyramid, TTemplate>
        where TTemplatePyramid: ITemplatePyramid<TTemplate>, new()
        where TTemplate: ITemplate, new()
    {
        private static XElement SerializeTemplateFeature(Feature feature)
        {
            XElement xElem = new XElement("Feature");
            xElem.SetAttributeValue("X", feature.X);
            xElem.SetAttributeValue("Y", feature.Y);
            xElem.SetAttributeValue("AngleLabel", feature.AngleIndex);

            return xElem;
        }

        private static XElement SerializeTemplate(TTemplate t, int pyrLevel)
        {
            XElement xElem = new XElement("Template");

            xElem.SetAttributeValue("width", t.Size.Width);
            xElem.SetAttributeValue("height", t.Size.Height);
            xElem.SetAttributeValue("pyramidLevel", pyrLevel);
            xElem.SetAttributeValue("numOfFeatures", t.Features.Length);

            foreach (var feature in t.Features)
            {
                xElem.Add(SerializeTemplateFeature(feature));
            }

            if (t is IXmlSerializable)
            {
                XElement xElemAditionalData = new XElement("AditionalData");
                using (var v = xElemAditionalData.CreateWriter())
                {
                    ((IXmlSerializable)t).WriteXml(v);
                    v.Flush();
                }

                if (string.IsNullOrEmpty(xElemAditionalData.Value) == false)
                {
                    xElem.Add(xElemAditionalData);
                }
            }

            return xElem;
        }

        private static XElement SerializeTemplatePyramid(TTemplatePyramid templatePyramid)
        {
            XElement xElem = new XElement("TemplatePyramid");

            for (int i = 0; i < templatePyramid.Templates.Length; i++)
            {
                xElem.Add(SerializeTemplate((TTemplate)templatePyramid.Templates[i], i));
            }

            return xElem;
        }

        /// <summary>
        /// Serializes the collection of template pyramids with the same object class.
        /// </summary>
        /// <param name="c">The collection of template pyramids.</param>
        /// <returns>Serialized collection of template pyramids.</returns>
        public static XElement SerializeTemplatePyramidClass(IEnumerable<TTemplatePyramid> c)
        {
            XElement xElem = new XElement("TemplatePyramidClass");
            xElem.SetAttributeValue("classLabel", c.First().Templates.First().ClassLabel);
            xElem.SetAttributeValue("numOfTemplatePyrs", c.Count());

            foreach (var templatePyr in c)
            {
                xElem.Add(SerializeTemplatePyramid(templatePyr));
            }

            return xElem;
        }

        /// <summary>
        /// Serializes the collection of template pyramids and saves them to a file.
        /// </summary>
        /// <param name="cluster">The collection of template pyramids.</param>
        /// <param name="fileName">File name.</param>
        public static void Save(IEnumerable<TTemplatePyramid> cluster, string fileName)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.CloseOutput = true;

            using (var streamWriter = new StreamWriter(File.Create(fileName)))
            using (var xmlWriter = XmlWriter.Create(streamWriter, settings))
            {
                SerializeTemplatePyramidClass(cluster).Save(xmlWriter);
            }
        }

        /// <summary>
        /// De-serializes the collection of template pyramids.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>The collection of template pyramids.</returns>
        public static IEnumerable<TTemplatePyramid> Load(string fileName)
        { 
            XDocument xDoc = XDocument.Load(new StreamReader(File.OpenRead(fileName)));

            IEnumerable<XElement> templatePyrClusters = from pyrCluster in xDoc.Descendants()
                                                        where pyrCluster.Name == "TemplatePyramidClass"
                                                        select pyrCluster;

            return DeserializeTemplatePyramidClass(templatePyrClusters.First());
        }

        private static IEnumerable<TTemplatePyramid> DeserializeTemplatePyramidClass(XElement templatePyrClassNode)
        {
            string templateClass = templatePyrClassNode.Attribute("classLabel").Value;

            IEnumerable<XElement> templatePyrsNodes = from templatePyr in templatePyrClassNode.Descendants()
                                                      where templatePyr.Name == "TemplatePyramid"
                                                      select templatePyr;

            object syncObj = new object();
            var templPyrs = new List<TTemplatePyramid>();

            Parallel.ForEach(templatePyrsNodes, delegate(XElement templatePyrNode) 
            {
                lock (syncObj)
                {
                    templPyrs.Add(DeserializeTemplatePyramid(templatePyrNode, templateClass));
                }
            });

            return templPyrs;
        }

        private static TTemplatePyramid DeserializeTemplatePyramid(XElement templatePyrNode, string templateClass)
        {
            IEnumerable<XElement> templateNodes = from templateNode in templatePyrNode.Descendants()
                                                  where templateNode.Name == "Template"
                                                  select templateNode;

            var templates = new List<TTemplate>();
            foreach (XElement templateNode in templateNodes)
            {           
                templates.Add(DeserializeTemplate(templateNode, templateClass));
            }

            var pyr = new TTemplatePyramid(); pyr.Initialize(templates.ToArray());
            return pyr;
        }

        private static TTemplate DeserializeTemplate(XElement templateNode, string templateClass)
        {
            int width = (int)templateNode.Attribute("width");
            int height = (int)templateNode.Attribute("height");

            IEnumerable<XElement> featureNodes = from featureNode in templateNode.Descendants()
                                                  where featureNode.Name == "Feature"
                                                  select featureNode;
            
            List<Feature> features = new List<Feature>();
            foreach (XElement featureNode in featureNodes)
            {
                features.Add(DeserializeFeature(featureNode));
            }

            TTemplate t = new TTemplate();
            t.Initialize(features.ToArray(), new Size(width, height), templateClass);

            if (t is IXmlSerializable)
            {
                XElement aditionalDataNode = templateNode.Descendants("AditionalData").FirstOrDefault();
                if (aditionalDataNode != null)
                    ((IXmlSerializable)t).ReadXml(aditionalDataNode.CreateReader());
            }
            return t;
        }

        private static Feature DeserializeFeature(XElement featureNode)
        {
            int x = (int)featureNode.Attribute("X");
            int y = (int)featureNode.Attribute("Y");
            byte angleLabel = (byte)(int)featureNode.Attribute("AngleLabel");

            return new Feature(x, y, Feature.GetAngleBinaryForm(angleLabel));
        }

    }
}
