using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class FileTemplate : BuildObject<FileTemplate>
    {
        #region Public Fields

        public const string TagName   = "FileTemplate";

        #endregion

        #region Private Fields

        private bool                        _isLoaded;
        private Version                     _version;
        private string                      _sourceFile;

        private TemplateAuthoring           _authoring;
        private TemplateAssistant           _assistant;
        private TemplateDescription         _description;
        private TemplateConfiguration       _configuration;
        private BuildList<TemplateFile>     _files;
        private BuildList<TemplateProperty> _properties;

        #endregion

        #region Constructors and Destructor

        public FileTemplate()
        {
            _version       = new Version(1, 0, 0, 0);
            _files         = new BuildList<TemplateFile>();
            _properties    = new BuildList<TemplateProperty>();
        }

        public FileTemplate(string sourceFile)
            : this()
        {
            _sourceFile    = sourceFile;
        }

        public FileTemplate(FileTemplate source)
            : base(source)
        {
            _files         = source._files;
            _version       = source._version;
            _isLoaded      = source._isLoaded;
            _sourceFile    = source._sourceFile;
            _authoring     = source._authoring;
            _assistant     = source._assistant;
            _properties    = source._properties;
            _description   = source._description;
            _configuration = source._configuration;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (_configuration == null || _configuration.IsEmpty);
            }
        }

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }

        public string SourceFile
        {
            get
            {
                return _sourceFile;
            }
            set
            {
                _sourceFile = value;
            }
        }

        public TemplateAuthoring Authoring
        {
            get
            {
                return _authoring;
            }
            set
            {
                _authoring = value;
            }
        }

        public TemplateAssistant Assistant
        {
            get
            {
                return _assistant;
            }
            set
            {
                _assistant = value;
            }
        }

        public TemplateDescription Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public TemplateConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                _configuration = value;
            }
        }

        public IList<TemplateFile> Files
        {
            get
            {
                return _files;
            }
        }

        public IList<TemplateProperty> Properties
        {
            get
            {
                return _properties;
            }
        }

        #endregion

        #region Public Method

        #region Load Method

        public void Load()
        {
            if (_isLoaded)
            {
                return;
            }

            if (String.IsNullOrEmpty(_sourceFile) || !File.Exists(_sourceFile))
            {
                return;
            }

            XmlReader reader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments               = true;
                settings.IgnoreWhitespace             = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(_sourceFile, settings);

                // The ReadXml expect start from the root or FileTemplate element...
                reader.MoveToContent();

                this.ReadXml(reader);

                _isLoaded = true;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        public void Reload()
        {
            _isLoaded = false;

            this.Load();
        }

        #endregion

        #region Save Method

        public void Save()
        {
            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.Indent             = true;
            settings.IndentChars        = new string(' ', 4);
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(_sourceFile, settings);

                writer.WriteStartDocument();

                this.WriteXml(writer);

                writer.WriteEndDocument();

                // The file content is now same as the memory, so it can be
                // considered loaded...
                _isLoaded = true;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

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

            string tempText = reader.GetAttribute("Version");
            if (!String.IsNullOrEmpty(tempText))
            {
                _version = new Version(tempText);
            }
            if (reader.IsEmptyElement)
            {
                return;
            }

            string sourceDir = String.IsNullOrEmpty(_sourceFile) ? String.Empty : 
                Path.GetDirectoryName(_sourceFile);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "TemplateAuthoring":
                            if (_authoring == null)
                            {
                                _authoring = new TemplateAuthoring();
                            }
                            _authoring.ReadXml(reader);
                            break;
                        case "TemplateConfiguration":
                            if (_configuration == null)
                            {
                                _configuration = new TemplateConfiguration();
                            }
                            _configuration.ReadXml(reader);
                            break;
                        case "TemplateDescription":
                            if (_description == null)
                            {
                                _description = new TemplateDescription();
                            }
                            _description.ReadXml(reader);
                            break;
                        case "TemplateProperty":
                            if (_properties == null)
                            {
                                _properties = new BuildList<TemplateProperty>();
                            }
                            TemplateProperty property = new TemplateProperty();
                            property.ReadXml(reader);
                            if (!property.IsEmpty)
                            {
                                _properties.Add(property);
                            }
                            break;
                        case "TemplateAssistant":
                            if (_assistant == null)
                            {
                                _assistant = new TemplateAssistant();
                            }
                            _assistant.ReadXml(reader);
                            break;
                        case "TemplateFile":
                            if (_files == null)
                            {
                                _files = new BuildList<TemplateFile>();
                            }
                            TemplateFile file = new TemplateFile();
                            if (!String.IsNullOrEmpty(sourceDir))
                            {
                                file.SourceDir = sourceDir;
                            }
                            file.ReadXml(reader);
                            if (!file.IsEmpty)
                            {
                                _files.Add(file);
                            }
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName, TemplateConstants.Namespace);  // start - FileTemplate 
            writer.WriteAttributeString("Version", _version.ToString(2));

            // For the TemplateAuthoring object
            if (_authoring != null)
            {
                _authoring.WriteXml(writer);
            }
            // For the TemplateConfiguration object
            if (_configuration != null)
            {
                _configuration.WriteXml(writer);
            }
            // For the TemplateDescription object
            if (_description != null)
            {
                _description.WriteXml(writer);
            }
            // For the TemplateProperties/TemplateProperty objects
            if (_properties != null && _properties.Count != 0)
            {
                writer.WriteStartElement("TemplateProperties"); // start - TemplateProperties
                for (int i = 0; i < _properties.Count; i++)
                {
                    _properties[i].WriteXml(writer);
                }                                              
                writer.WriteEndElement();                       // end - TemplateProperties
            }
            // For the TemplateAssistant object
            if (_assistant != null)
            {
                _assistant.WriteXml(writer);
            }
            // For the TemplateFiles/TemplateFile objects
            if (_files != null && _files.Count != 0)
            {
                writer.WriteStartElement("TemplateFiles"); // start - TemplateFiles
                for (int i = 0; i < _files.Count; i++)
                {
                    _files[i].WriteXml(writer);
                }
                writer.WriteEndElement();                  // end - TemplateFiles
            }

            writer.WriteEndElement();           // end - FileTemplate
        }

        #endregion

        #region ICloneable Members

        public override FileTemplate Clone()
        {
            FileTemplate template = new FileTemplate(this);
            if (_sourceFile != null)
            {
                template._sourceFile = String.Copy(_sourceFile);
            }
            if (_version != null)
            {
                template._version = (Version)_version.Clone();
            }
            if (_authoring != null)
            {
                template._authoring = _authoring.Clone();
            }
            if (_configuration != null)
            {
                template._configuration = _configuration.Clone();
            }
            if (_description != null)
            {
                template._description = _description.Clone();
            }
            if (_properties != null)
            {
                template._properties = _properties.Clone();
            }
            if (_assistant != null)
            {
                template._assistant = _assistant.Clone();
            }
            if (_files != null)
            {
                template._files = _files.Clone();
            }

            return template;
        }

        #endregion
    }
}
