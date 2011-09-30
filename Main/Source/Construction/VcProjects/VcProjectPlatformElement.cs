using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Construction.Utils;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectPlatformElement : VcProjectElement
    {
        #region Public Fields

        public const string TagName = "Platform";

        #endregion

        #region Private Fields

        private string _name;

        #endregion

        #region Constructors and Destructor

        internal VcProjectPlatformElement()
            : this(null, null)
        {
        }

        internal VcProjectPlatformElement(VcProjectContainerElement parent,
            VcProjectRootElement root)
            : base(parent, root)
        {
        }

        #endregion

        #region Public Properties

        public override VcProjectElementType ElementType
        {
            get
            {
                return VcProjectElementType.Platform;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_name);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            ProjectExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _name = reader.GetAttribute("Name");
        }

        public override void WriteXml(XmlWriter writer)
        {
            ProjectExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - Platform     
            writer.WriteAttributeString("Name", _name);                 
            writer.WriteEndElement();           // end - Platform
        }

        #endregion
    }
}
