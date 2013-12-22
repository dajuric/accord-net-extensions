using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Threading.Tasks;

namespace LINE2D.TemplateMatching
{
    public class TemplateSerializer
    {
        private static XElement SerializeTemplateFeature(Template.Feature feature)
        {
            XElement xElem = new XElement("Feature");
            xElem.SetAttributeValue("X", feature.X);
            xElem.SetAttributeValue("Y", feature.Y);
            xElem.SetAttributeValue("AngleLabel", feature.AngleLabel);
            xElem.SetAttributeValue("Magnitude", feature.GradientMagnitude);

            return xElem;
        }

        private static XElement SerializeTemplate(Template t, int pyrLevel)
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


            XElement xElemAditionalData = new XElement("AditionalData");
            t.SerializeAditionalData(xElemAditionalData);
          
            if (string.IsNullOrEmpty(xElemAditionalData.Value) ==  false)
            {
                xElem.Add(xElemAditionalData);
            }

            return xElem;
        }

        private static XElement SerializeTemplatePyramid(TemplatePyramid templatePyramid)
        {
            XElement xElem = new XElement("TemplatePyramid");

            for (int i = 0; i < templatePyramid.Templates.Length; i++)
            {
                xElem.Add(SerializeTemplate(templatePyramid.Templates[i], i));
            }

            return xElem;
        }

        public static XElement SerializeTemplatePyramidClass(IEnumerable<TemplatePyramid> c)
        {
            XElement xElem = new XElement("TemplatePyramidClass");
            xElem.SetAttributeValue("classLabel", c.First().Templates.First().ClassLabel);
            xElem.SetAttributeValue("numOfTemplatePyrs", c.Count());

            foreach (TemplatePyramid templatePyr in c)
            {
                xElem.Add(SerializeTemplatePyramid(templatePyr));
            }

            return xElem;
        }

        public static void Save(IEnumerable<TemplatePyramid> cluster, string fileName)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.CloseOutput = true;

            XmlWriter xmlWriter = XmlWriter.Create(new StreamWriter(fileName, false), settings);

            SerializeTemplatePyramidClass(cluster).Save(xmlWriter);

            xmlWriter.Flush();
            xmlWriter.Close();
        }


        public static IEnumerable<TemplatePyramid> Load(string fileName)
        { 
            XDocument xDoc = XDocument.Load(new StreamReader(fileName));

            IEnumerable<XElement> templatePyrClusters = from pyrCluster in xDoc.Descendants()
                                                        where pyrCluster.Name == "TemplatePyramidClass"
                                                        select pyrCluster;

            return DeserializeTemplatePyramidClass(templatePyrClusters.First());
        }

        private static IEnumerable<TemplatePyramid> DeserializeTemplatePyramidClass(XElement templatePyrClassNode)
        {
            string templateClass = templatePyrClassNode.Attribute("classLabel").Value;

            IEnumerable<XElement> templatePyrsNodes = from templatePyr in templatePyrClassNode.Descendants()
                                                      where templatePyr.Name == "TemplatePyramid"
                                                      select templatePyr;

            object syncObj = new object();
            List<TemplatePyramid> templPyrs = new List<TemplatePyramid>();

            Parallel.ForEach(templatePyrsNodes, delegate(XElement templatePyrNode) 
            {
                lock (syncObj)
                {
                    templPyrs.Add(DeserializeTemplatePyramid(templatePyrNode, templateClass));
                }
            });

            return templPyrs;
        }

        private static TemplatePyramid DeserializeTemplatePyramid(XElement templatePyrNode, string templateClass)
        {
            IEnumerable<XElement> templateNodes = from templateNode in templatePyrNode.Descendants()
                                                  where templateNode.Name == "Template"
                                                  select templateNode;

            List<Template> templates = new List<Template>();
            foreach (XElement templateNode in templateNodes)
            {           
                templates.Add(DeserializeTemplate(templateNode, templateClass));
            }

            return new TemplatePyramid(templates.ToArray());
        }

        private static Template DeserializeTemplate(XElement templateNode, string templateClass)
        {
            int width = (int)templateNode.Attribute("width");
            int height = (int)templateNode.Attribute("height");

            IEnumerable<XElement> featureNodes = from featureNode in templateNode.Descendants()
                                                  where featureNode.Name == "Feature"
                                                  select featureNode;
            
            List<Template.Feature> features = new List<Template.Feature>();
            foreach (XElement featureNode in featureNodes)
            {
                features.Add(DeserializeFeature(featureNode));
            }

            XElement aditionalDataNode = templateNode.Descendants("AditionalData").FirstOrDefault();

            Template t = TemplatePyramid.CreateNew(templateClass);
            t.Initialize(features.ToArray(), new System.Drawing.Size(width, height), templateClass);
            t.DeserializeAditionalData(aditionalDataNode);
            return t;
        }

        private static Template.Feature DeserializeFeature(XElement featureNode)
        {
            int x = (int)featureNode.Attribute("X");
            int y = (int)featureNode.Attribute("Y");
            byte angleLabel = (byte)(int)featureNode.Attribute("AngleLabel");
            int magnitude = (int)featureNode.Attribute("Magnitude");

            return new Template.Feature { X = x, Y = y, AngleBinaryRepresentation = Template.Feature.CalcAngleBinRepresentation(angleLabel), GradientMagnitude = magnitude };
        }

    }
}
