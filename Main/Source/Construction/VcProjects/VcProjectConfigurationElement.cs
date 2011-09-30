using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;
using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectConfigurationElement : VcProjectContainerElement
    {
        #region Public Fields

        public const string TagName = "Configuration";

        #endregion

        #region Private Fields

        private string _tagName; 
        private ProjectProperties _properties;

        #endregion

        #region Constructors and Destructor

        internal VcProjectConfigurationElement()
            : this(null, null)
        {
        }

        internal VcProjectConfigurationElement(VcProjectContainerElement parent,
            VcProjectRootElement root)
            : this(parent, root, TagName)
        {
        }

        internal VcProjectConfigurationElement(VcProjectContainerElement parent,
            VcProjectRootElement root, string tagName)
            : base(parent, root)
        {
            ProjectExceptions.NotNullNotEmpty(tagName, "tagName");

            _tagName    = tagName;
            _properties = new ProjectProperties();
        }

        #endregion

        #region Public Properties

        public override VcProjectElementType ElementType
        {
            get
            {
                return VcProjectElementType.Configuration;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                if (_properties.Count == 0)
                {
                    return true;
                }

                return base.IsEmpty;
            }
        }

        public string Name
        {
            get
            {
                return _properties["Name"];
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

        public string OutputDirectory
        {
            get
            {
                return _properties["OutputDirectory"];
            }
        }

        public string IntermediateDirectory
        {
            get
            {
                return _properties["IntermediateDirectory"];
            }
        }

        public string ConfigurationType
        {
            get
            {
                return _properties["ConfigurationType"];
            }
        }

        public string CharacterSet
        {
            get
            {
                return _properties["CharacterSet"];
            }
        }

        public string ManagedExtensions
        {
            get
            {
                return _properties["ManagedExtensions"];
            }
        }

        public string WholeProgramOptimization
        {
            get
            {
                return _properties["WholeProgramOptimization"];
            }
        }

        public int AttributeCount
        {
            get
            {
                return _properties.Count;
            }
        }

        public ICollection<VcProjectToolElement> Tools
        {
            get
            {
                return new ReadOnlyCollection<VcProjectToolElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectToolElement>(
                        this.Children, VcProjectElementType.Tool));
            }
        }

        public ICollection<VcProjectDeploymentToolElement> DeploymentTools
        {
            get
            {
                return new ReadOnlyCollection<VcProjectDeploymentToolElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectDeploymentToolElement>(
                        this.Children, VcProjectElementType.DeploymentTool));
            }
        }

        public ICollection<VcProjectDebuggerToolElement> DebuggerTools
        {
            get
            {
                return new ReadOnlyCollection<VcProjectDebuggerToolElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectDebuggerToolElement>(
                        this.Children, VcProjectElementType.DebuggerTool));
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
                    case VcProjectToolElement.TagName:
                        return true;
                    case VcProjectDeploymentToolElement.TagName:
                        return true;
                    case VcProjectDebuggerToolElement.TagName:
                        return true;
                }
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.Tool:
                    return true;
                case VcProjectElementType.DeploymentTool:
                    return true;
                case VcProjectElementType.DebuggerTool:
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
                    case VcProjectToolElement.TagName:
                        return new VcProjectToolElement(this, this.Root);
                    case VcProjectDeploymentToolElement.TagName:
                        return new VcProjectDeploymentToolElement(this, this.Root);
                    case VcProjectDebuggerToolElement.TagName:
                        return new VcProjectDebuggerToolElement(this, this.Root);
                }
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.Tool:
                    return new VcProjectToolElement(this, this.Root);
                case VcProjectElementType.DeploymentTool:
                    return new VcProjectDeploymentToolElement(this, this.Root);
                case VcProjectElementType.DebuggerTool:
                    return new VcProjectDebuggerToolElement(this, this.Root);
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

            if (!String.Equals(reader.Name, _tagName,
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
                            _tagName));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, _tagName,
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

            writer.WriteStartElement(_tagName);  // start - configuration

            foreach (KeyValuePair<string, string> item in _properties)
            {
                writer.WriteAttributeString(item.Key, item.Value);
            }

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();            // end - the configuration
        }

        #endregion
    }
}
