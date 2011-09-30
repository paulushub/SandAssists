using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class ProjectTemplate : BuildObject<ProjectTemplate>
    {
        #region Public Fields

        public const string TagName   = "ProjectTemplate";

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

        private BuildList<TemplateAction>          _actions;
        private BuildList<TemplateItemGroup>       _itemGroups;
        private BuildList<TemplatePropertyGroup>   _propertyGroups;
        private BuildList<TemplateTargetFramework> _targetFrameworks;

        #endregion

        #region Constructors and Destructor

        public ProjectTemplate()
        {
            _version          = new Version(1, 0, 0, 0);
            _files            = new BuildList<TemplateFile>();
            _properties       = new BuildList<TemplateProperty>();     
            _actions          = new BuildList<TemplateAction>();
            _itemGroups       = new BuildList<TemplateItemGroup>();
            _propertyGroups   = new BuildList<TemplatePropertyGroup>();
            _targetFrameworks = new BuildList<TemplateTargetFramework>();
        }

        public ProjectTemplate(string sourceFile)
            : this()
        {
            _sourceFile       = sourceFile;
        }

        public ProjectTemplate(ProjectTemplate source)
            : base(source)
        {
            _files            = source._files;
            _version          = source._version;
            _isLoaded         = source._isLoaded;
            _sourceFile       = source._sourceFile;
            _authoring        = source._authoring;
            _assistant        = source._assistant;
            _properties       = source._properties;
            _description      = source._description;
            _configuration    = source._configuration;
            _actions          = source._actions;
            _itemGroups       = source._itemGroups;
            _propertyGroups   = source._propertyGroups;
            _targetFrameworks = source._targetFrameworks;
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

        public IList<TemplateAction> Actions
        {
            get
            {
                return _actions;
            }
        }

        public IList<TemplateItemGroup> ItemGroups
        {
            get
            {
                return _itemGroups;
            }
        }

        public IList<TemplatePropertyGroup> PropertyGroups
        {
            get
            {
                return _propertyGroups;
            }
        }

        public IList<TemplateTargetFramework> TargetFrameworks
        {
            get
            {
                return _targetFrameworks;
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

                // The ReadXml expect start from the root or ProjectTemplate element...
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
                        case "TemplateAction":
                            if (_actions == null)
                            {
                                _actions = new BuildList<TemplateAction>();
                            }
                            TemplateAction action = new TemplateAction();
                            action.ReadXml(reader);
                            if (!action.IsEmpty)
                            {
                                _actions.Add(action);
                            }
                            break;
                        case "TemplateTargetFramework":
                            if (_targetFrameworks == null)
                            {
                                _targetFrameworks = new BuildList<TemplateTargetFramework>();
                            }
                            TemplateTargetFramework targetFramework = new TemplateTargetFramework();
                            targetFramework.ReadXml(reader);
                            if (!targetFramework.IsEmpty)
                            {
                                _targetFrameworks.Add(targetFramework);
                            }
                            break;
                        case "TemplatePropertyGroup":
                            if (_propertyGroups == null)
                            {
                                _propertyGroups = new BuildList<TemplatePropertyGroup>();
                            }
                            TemplatePropertyGroup propertyGroup = new TemplatePropertyGroup();
                            propertyGroup.ReadXml(reader);
                            if (!propertyGroup.IsEmpty)
                            {
                                _propertyGroups.Add(propertyGroup);
                            }
                            break;
                        case "TemplateItemGroup":
                            if (_itemGroups == null)
                            {
                                _itemGroups = new BuildList<TemplateItemGroup>();
                            }
                            TemplateItemGroup itemGroup = new TemplateItemGroup();
                            itemGroup.ReadXml(reader);
                            if (!itemGroup.IsEmpty)
                            {
                                _itemGroups.Add(itemGroup);
                            }
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

            writer.WriteStartElement(TagName, TemplateConstants.Namespace);  // start - ProjectTemplate 
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

            // For the TemplateActions/TemplateAction objects
            if (_actions != null && _actions.Count != 0)
            {
                writer.WriteStartElement("TemplateActions"); // start - TemplateActions
                for (int i = 0; i < _actions.Count; i++)
                {
                    _actions[i].WriteXml(writer);
                }
                writer.WriteEndElement();                    // end - TemplateActions
            }
            // For the TemplateTargetFrameworks/TemplateTargetFramework objects
            if (_targetFrameworks != null && _targetFrameworks.Count != 0)
            {
                writer.WriteStartElement("TemplateTargetFrameworks"); // start - TemplateTargetFrameworks
                for (int i = 0; i < _targetFrameworks.Count; i++)
                {
                    _targetFrameworks[i].WriteXml(writer);
                }
                writer.WriteEndElement();                             // end - TemplateTargetFrameworks
            }
            // For the TemplatePropertyGroups/TemplatePropertyGroup objects
            if (_propertyGroups != null && _propertyGroups.Count != 0)
            {
                writer.WriteStartElement("TemplatePropertyGroups"); // start - TemplatePropertyGroups
                for (int i = 0; i < _propertyGroups.Count; i++)
                {
                    _propertyGroups[i].WriteXml(writer);
                }
                writer.WriteEndElement();                           // end - TemplatePropertyGroups
            }
            // For the TemplateItemGroups/TemplateItemGroup objects
            if (_itemGroups != null && _itemGroups.Count != 0)
            {
                writer.WriteStartElement("TemplateItemGroups"); // start - TemplateItemGroups
                for (int i = 0; i < _itemGroups.Count; i++)
                {
                    _itemGroups[i].WriteXml(writer);
                }
                writer.WriteEndElement();                       // end - TemplateItemGroups
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

            writer.WriteEndElement();           // end - ProjectTemplate
        }

        #endregion

        #region ICloneable Members

        public override ProjectTemplate Clone()
        {
            ProjectTemplate template = new ProjectTemplate(this);
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
            if (_actions != null)
            {
                template._actions = _actions.Clone();
            }
            if (_itemGroups != null)
            {
                template._itemGroups = _itemGroups.Clone();
            }
            if (_propertyGroups != null)
            {
                template._propertyGroups = _propertyGroups.Clone();
            }
            if (_targetFrameworks != null)
            {
                template._targetFrameworks = _targetFrameworks.Clone();
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
