using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Construction.Utils;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectGlobalElement : VcProjectElement
    {
        #region Public Fields

        public const string TagName = "Global";

        #endregion

        #region Private Fields

        private string _name;
        private string _value;

        #endregion

        #region Constructors and Destructor

        internal VcProjectGlobalElement()
            : this(null, null)
        {
        }

        internal VcProjectGlobalElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.Global;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return (String.IsNullOrEmpty(_name) ||
                    String.IsNullOrEmpty(_value));
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

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
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

            _name  = reader.GetAttribute("Name");
            _value = reader.GetAttribute("Value");
        }

        public override void WriteXml(XmlWriter writer)
        {
            ProjectExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - Global 
            writer.WriteAttributeString("Name",  _name);
            writer.WriteAttributeString("Value", _value);         
            writer.WriteEndElement();           // end - Global
        }

        #endregion
    }
}
