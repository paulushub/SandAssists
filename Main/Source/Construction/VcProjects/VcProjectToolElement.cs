using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;
using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectToolElement : VcProjectElement
    {
        #region Public Fields

        public const string TagName = "Tool";

        #endregion

        #region Private Fields

        private ProjectProperties _properties;

        #endregion

        #region Constructors and Destructor

        internal VcProjectToolElement()
            : this(null, null)
        {
        }

        internal VcProjectToolElement(VcProjectContainerElement parent,
            VcProjectRootElement root)
            : base(parent, root)
        {
            _properties = new ProjectProperties();
        }

        #endregion

        #region Public Properties

        public override VcProjectElementType ElementType
        {
            get
            {
                return VcProjectElementType.Tool;
            }
        }

        public string Name
        {
            get
            {
                return _properties["Name"];
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return (_properties.Count == 0);
            }
        }

        public string this[string attributeName]
        {
            get
            {
                return _properties[attributeName];
            }
            set
            {
                _properties[attributeName] = value;
            }
        }

        public int AttributeCount
        {
            get
            {
                return _properties.Count;
            }
        }

        #endregion

        #region Public Methods

        public void AddAttribute(string name, string value)
        {
            _properties.Add(name, value);
        }

        public void RemoveAttribute(string name)
        {
            _properties.Remove(name);
        }

        public bool ContainsAttribute(string name)
        {
            return _properties.ContainsKey(name);
        }

        public void ClearAttributes()
        {
            _properties.Clear();
        }

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

            while (reader.MoveToNextAttribute())
            {
                _properties.Add(reader.Name, reader.Value);
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            ProjectExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - Tool

            foreach (KeyValuePair<string, string> item in _properties)
            {
                writer.WriteAttributeString(item.Key, item.Value);
            }

            writer.WriteEndElement();           // end - Tool
        }

        #endregion
    }
}
