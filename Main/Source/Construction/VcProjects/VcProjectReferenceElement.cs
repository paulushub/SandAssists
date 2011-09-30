using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;
using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public abstract class VcProjectReferenceElement : VcProjectContainerElement
    {
        #region Private Fields

        private ProjectProperties _properties;

        #endregion

        #region Constructors and Destructor

        protected VcProjectReferenceElement()
            : this(null, null)
        {
        }

        protected VcProjectReferenceElement(VcProjectContainerElement parent,
            VcProjectRootElement root)
            : base(parent, root)
        {
            _properties = new ProjectProperties();
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (_properties.Count == 0)
                {
                    return base.IsEmpty;
                }

                return false;
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

        public ICollection<VcProjectConfigurationElement> ReferenceConfigurations
        {
            get
            {
                return new ReadOnlyCollection<VcProjectConfigurationElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectConfigurationElement>(
                        this.Children, VcProjectElementType.Configuration));
            }
        }

        #endregion

        #region Protected Properties

        protected abstract string XmlTagName
        {
            get;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override bool IsChildElement(string elementName)
        {
            if (!String.IsNullOrEmpty(elementName))
            {
                return String.Equals(elementName, "ReferenceConfiguration",
                    StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            return (elementType == VcProjectElementType.Configuration);
        }

        protected override VcProjectElement CreateChildElement(string elementName)
        {
            if (this.IsChildElement(elementName))
            {
                return new VcProjectConfigurationElement(this, this.Root, "ReferenceConfiguration");
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            if (elementType == VcProjectElementType.Configuration)
            {
                return new VcProjectConfigurationElement(this, this.Root, "ReferenceConfiguration");
            }

            return null;
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

            if (!String.Equals(reader.Name, this.XmlTagName,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Keep this, after reading the attributes, the reader will be
            // on the last attribute, and this property will return false...
            bool isEmptyElement = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                _properties.Add(reader.Name, reader.Value);
            }

            if (isEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (this.IsChildElement(reader.Name))
                    {
                        VcProjectElement element = this.CreateChildElement(reader.Name);
                        element.ReadXml(reader);

                        this.AppendChild(element);
                    }
                    else
                    {
                        throw new InvalidOperationException(String.Format(
                            "The element '{0}' is not a child element of {1}", reader.Name,
                            this.XmlTagName));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, this.XmlTagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            ProjectExceptions.NotNull(writer, "writer");

            //if (this.IsEmpty)
            //{
            //    return;
            //}

            writer.WriteStartElement(this.XmlTagName);  // start - reference

            foreach (KeyValuePair<string, string> item in _properties)
            {
                writer.WriteAttributeString(item.Key, item.Value);
            }

            if (base.IsEmpty)
            {   
                writer.WriteEndElement();               // end - reference
            }
            else
            {   
                for (int i = 0; i < this.Count; i++)
                {
                    this[i].WriteXml(writer);
                }

                writer.WriteFullEndElement();           // end - reference
            }
        }

        #endregion
    }
}
