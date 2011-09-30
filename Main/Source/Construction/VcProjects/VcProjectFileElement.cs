using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;
using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectFileElement : VcProjectContainerElement
    {
        #region Public Fields

        public const string TagName = "File";

        #endregion

        #region Private Fields

        private ProjectProperties _properties;

        #endregion

        #region Constructors and Destructor

        internal VcProjectFileElement()
            : this(null, null)
        {
        }

        internal VcProjectFileElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.File;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return (_properties.Count == 0);
            }
        }

        public string RelativePath
        {
            get
            {
                return _properties["RelativePath"];
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

        public ICollection<VcProjectFileElement> Files
        {
            get
            {
                return new ReadOnlyCollection<VcProjectFileElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectFileElement>(
                        this.Children, VcProjectElementType.File));
            }
        }

        public ICollection<VcProjectConfigurationElement> FileConfigurations
        {
            get
            {
                return new ReadOnlyCollection<VcProjectConfigurationElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectConfigurationElement>(
                        this.Children, VcProjectElementType.Configuration));
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override bool IsChildElement(string elementName)
        {
            if (!String.IsNullOrEmpty(elementName))
            {
                switch (elementName)
                {
                    case VcProjectFileElement.TagName:
                        return true;
                    case "FileConfiguration":
                        return true;
                }
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.File:
                    return true;
                case VcProjectElementType.Configuration:
                    return true;
            }

            return false;
        }

        protected override VcProjectElement CreateChildElement(string elementName)
        {
            if (!String.IsNullOrEmpty(elementName))
            {
                switch (elementName)
                {
                    case VcProjectFileElement.TagName:
                        return new VcProjectFileElement(this, this.Root);
                    case "FileConfiguration":
                        return new VcProjectConfigurationElement(this, this.Root, "FileConfiguration");
                }
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.File:
                    return new VcProjectFileElement(this, this.Root);
                case VcProjectElementType.Configuration:
                    return new VcProjectConfigurationElement(this, this.Root, "FileConfiguration");
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

            if (!String.Equals(reader.Name, TagName,
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
                            TagName));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - file

            foreach (KeyValuePair<string, string> item in _properties)
            {
                writer.WriteAttributeString(item.Key, item.Value);
            }

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteFullEndElement();       // end - file
        }

        #endregion
    }
}
