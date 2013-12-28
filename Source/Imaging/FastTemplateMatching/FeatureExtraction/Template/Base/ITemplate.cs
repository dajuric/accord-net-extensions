using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace LINE2D
{
    public interface ITemplate : IXmlSerializable
    {
        Feature[] Features { get; }
        Size Size { get; }
        string ClassLabel { get; }

        void Initialize(Feature[] features, Size size, string classLabel);
    }
}
