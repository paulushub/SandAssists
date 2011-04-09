using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sandcastle
{
    public interface IBuildItem : ICloneable, IXmlSerializable
    {
        IBuildContent Content
        {
            get;
            set;
        }
    }
}
