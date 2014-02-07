using Accord.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ObjectAnnotater.Components
{
    public class ImageAnnotations
    {
        public string ImageName { get; set; }
        public string Comment { get; set; }
        public Rectangle[] Annotations { get; set; }
    }

    public class XmlDatabase
    {
        XDocument xDoc;

        private XmlDatabase()
        {
        }

        public static XmlDatabase Load(string fileName)
        {
            var database = new XmlDatabase();

            database.xDoc = XDocument.Load(fileName);

            return database;
        }

        public void AddOrUpdate(ImageAnnotations imgAnnotations)
        {
            var serializedObj = imgAnnotations.ToXElement();

            var node = (from imgAnns in xDoc.Elements()
                       where imgAnns.Attribute("ImageName").Value == imgAnnotations.ImageName
                       select imgAnns)
                       .FirstOrDefault();

            if (String.IsNullOrWhiteSpace(node.Value) == false)
                node.ReplaceWith(serializedObj);
            else
                xDoc.Add(serializedObj);
        }

        private bool tryFind(ImageAnnotations imgAnnotations, Expression<Func<T>> keySelector)
        { 

        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
    {
        MemberExpression body = (MemberExpression) expression.Body;
        return body.Member.Name;
    }
    }
}
