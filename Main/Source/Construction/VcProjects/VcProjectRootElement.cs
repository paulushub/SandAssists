using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;
using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectRootElement : VcProjectContainerElement
    {
        #region Public Fields

        public const string TagName = "VisualStudioProject";

        #endregion

        #region Private Fields

        private Encoding        _encoding;
        private ProjectProperties _properties;

        #endregion

        #region Constructors and Destructor

        private VcProjectRootElement()
            : base(null, null)
        {
            _encoding   = Encoding.UTF8;
            _properties = new ProjectProperties();
        }

        #endregion

        #region Public Properties

        public override VcProjectElementType ElementType
        {
            get
            {
                return VcProjectElementType.Root;
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

        public Encoding Encoding
        {
            get 
            {
                return _encoding; 
            }
            set
            {
                if (value != null)
                {
                    _encoding = value;
                }
            }
        }

        public string AssemblyOriginatorKeyFile
        {
            get
            {
                return _properties["AssemblyOriginatorKeyFile"];
            }
        }

        public string AssemblyReferenceSearchPaths
        {
            get
            {
                return _properties["AssemblyReferenceSearchPaths"];
            }
        }

        public bool? DelaySign
        {
            get
            {
                string propertyValue = _properties["DelaySign"];
                if (!String.IsNullOrEmpty(propertyValue))
                {
                    return Convert.ToBoolean(propertyValue);
                }

                return null;
            }
        }

        public string ExcludedPermissions
        {
            get
            {
                return _properties["ExcludedPermissions"];
            }
        }

        public bool? GenerateManifests
        {
            get
            {
                string propertyValue = _properties["GenerateManifests"];
                if (!String.IsNullOrEmpty(propertyValue))
                {
                    return Convert.ToBoolean(propertyValue);
                }

                return null;
            }
        }

        public string Keyword
        {
            get
            {
                return _properties["Keyword"];
            }
        }

        public string ManifestCertificateThumbprint
        {
            get
            {
                return _properties["ManifestCertificateThumbprint"];
            }
        }

        public string ManifestKeyFile
        {
            get
            {
                return _properties["ManifestKeyFile"];
            }
        }

        public string ManifestTimestampURL
        {
            get
            {
                return _properties["ManifestTimestampURL"];
            }
        }

        public string Name
        {
            get
            {
                return _properties["Name"];
            }
        }

        public string ProjectGUID
        {
            get
            {
                return _properties["ProjectGUID"];
            }
        }

        public string ProjectType
        {
            get
            {
                return _properties["ProjectType"];
            }
        }

        public string RootNamespace
        {
            get
            {
                return _properties["RootNamespace"];
            }
        }

        public bool? SignAssembly
        {
            get
            {
                string propertyValue = _properties["SignAssembly"];
                if (!String.IsNullOrEmpty(propertyValue))
                {
                    return Convert.ToBoolean(propertyValue);
                }

                return null;
            }
        }

        public bool? SignManifests
        {
            get
            {
                string propertyValue = _properties["SignManifests"];
                if (!String.IsNullOrEmpty(propertyValue))
                {
                    return Convert.ToBoolean(propertyValue);
                }

                return null;
            }
        }

        public string TargetFrameworkVersion
        {
            get
            {
                return _properties["TargetFrameworkVersion"];
            }
        }

        public string TargetZone
        {
            get
            {
                return _properties["TargetZone"];
            }
        }

        public string Version
        {
            get
            {
                return _properties["Version"];
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

        public ICollection<VcProjectPlatformsElement> Platforms
        {
            get
            {
                return new ReadOnlyCollection<VcProjectPlatformsElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectPlatformsElement>(
                        this.Children, VcProjectElementType.Platforms));
            }
        }

        public ICollection<VcProjectToolFilesElement> ToolFiles
        {
            get
            {
                return new ReadOnlyCollection<VcProjectToolFilesElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectToolFilesElement>(
                        this.Children, VcProjectElementType.ToolFiles));
            }
        }

        public ICollection<VcProjectPublishingDataElement> PublishingData
        {
            get
            {
                return new ReadOnlyCollection<VcProjectPublishingDataElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectPublishingDataElement>(
                        this.Children, VcProjectElementType.PublishingData));
            }
        }

        public ICollection<VcProjectConfigurationsElement> Configurations
        {
            get
            {
                return new ReadOnlyCollection<VcProjectConfigurationsElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectConfigurationsElement>(
                        this.Children, VcProjectElementType.Configurations));
            }
        }

        public ICollection<VcProjectReferencesElement> References
        {
            get
            {
                return new ReadOnlyCollection<VcProjectReferencesElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectReferencesElement>(
                        this.Children, VcProjectElementType.References));
            }
        }

        public ICollection<VcProjectFilesElement> Files
        {
            get
            {
                return new ReadOnlyCollection<VcProjectFilesElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectFilesElement>(
                        this.Children, VcProjectElementType.Files));
            }
        }

        public ICollection<VcProjectGlobalsElement> Globals
        {
            get
            {
                return new ReadOnlyCollection<VcProjectGlobalsElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectGlobalsElement>(
                        this.Children, VcProjectElementType.Globals));
            }
        }

        #endregion

        #region Public Methods

        public static VcProjectRootElement Open(string projectFile)
        {
            VcProjectRootElement rootElement = new VcProjectRootElement();

            rootElement.Load(projectFile);

            return rootElement;
        }

        public void Save(string path)
        {
            Save(path, Encoding);
        }


        public void Save(TextWriter writer, Encoding encoding)
        {
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
            {
                Encoding = this.Encoding,
                Indent = true,
                NewLineChars = "\r\n",
                OmitXmlDeclaration = false,
            }))
            {
                this.WriteXml(xmlWriter);
            }
        }

        public void Save(string path, Encoding encoding)
        {
            using (StreamWriter textWriter = new StreamWriter(path))
            {
                Save(textWriter, encoding);
            }
        }

        #endregion

        #region Protected Methods

        protected override bool IsChildElement(string elementName)
        {
            if (!String.IsNullOrEmpty(elementName))
            {
                switch (elementName)
                {
                    case VcProjectPlatformsElement.TagName:
                        return true;
                    case VcProjectToolFilesElement.TagName:
                        return true;
                    case VcProjectPublishingDataElement.TagName:
                        return true;
                    case VcProjectConfigurationsElement.TagName:
                        return true;
                    case VcProjectReferencesElement.TagName:
                        return true;
                    case VcProjectFilesElement.TagName:
                        return true;
                    case VcProjectGlobalsElement.TagName:
                        return true;
                }
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.Platforms:
                    return true;
                case VcProjectElementType.ToolFiles:
                    return true;
                case VcProjectElementType.PublishingData:
                    return true;
                case VcProjectElementType.Configurations:
                    return true;
                case VcProjectElementType.References:
                    return true;
                case VcProjectElementType.Files:
                    return true;
                case VcProjectElementType.Globals:
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
                    case VcProjectPlatformsElement.TagName:
                        return new VcProjectPlatformsElement(this, this.Root);
                    case VcProjectToolFilesElement.TagName:
                        return new VcProjectToolFilesElement(this, this.Root);
                    case VcProjectPublishingDataElement.TagName:
                        return new VcProjectPublishingDataElement(this, this.Root);
                    case VcProjectConfigurationsElement.TagName:
                        return new VcProjectConfigurationsElement(this, this.Root);
                    case VcProjectReferencesElement.TagName:
                        return new VcProjectReferencesElement(this, this.Root);
                    case VcProjectFilesElement.TagName:
                        return new VcProjectFilesElement(this, this.Root);
                    case VcProjectGlobalsElement.TagName:
                        return new VcProjectGlobalsElement(this, this.Root);
                }
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.Platforms:
                    return new VcProjectPlatformsElement(this, this.Root);
                case VcProjectElementType.ToolFiles:
                    return new VcProjectToolFilesElement(this, this.Root);
                case VcProjectElementType.PublishingData:
                    return new VcProjectPublishingDataElement(this, this.Root);
                case VcProjectElementType.Configurations:
                    return new VcProjectConfigurationsElement(this, this.Root);
                case VcProjectElementType.References:
                    return new VcProjectReferencesElement(this, this.Root);
                case VcProjectElementType.Files:
                    return new VcProjectFilesElement(this, this.Root);
                case VcProjectElementType.Globals:
                    return new VcProjectGlobalsElement(this, this.Root);
            }

            return null;
        }

        #endregion

        #region Private Methods

        private void Load(string projectFile)
        {
            using (StreamReader textReader = new StreamReader(projectFile))
            {
                this.Encoding = textReader.CurrentEncoding;

                using (XmlReader reader = XmlReader.Create(projectFile))
                {
                    reader.MoveToContent();

                    this.ReadXml(reader);
                }
            }
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

            writer.WriteStartElement(TagName);  // start - VisualStudioProject

            foreach (KeyValuePair<string, string> item in _properties)
            {
                writer.WriteAttributeString(item.Key, item.Value);
            }

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteFullEndElement();       // end - VisualStudioProject
        }

        #endregion
    }
}
