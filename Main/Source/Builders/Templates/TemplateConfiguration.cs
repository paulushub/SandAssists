using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateConfiguration : BuildObject<TemplateConfiguration>
    {
        #region Private Fields

        private const string TagName = "TemplateConfiguration";

        private bool   _requiresProject;

        private string _id;

        private string _iconId;
        private string _iconPath;

        private string _title;
        private string _category;
        private string _subCategory;
        private string _defaultFileName;
        private string _projectType;
        private string _projectSubType;

        #endregion

        #region Constructors and Destructor

        public TemplateConfiguration()
        {
            _id              = Guid.NewGuid().ToString();
            _iconId          = String.Empty;
            _iconPath        = String.Empty;
            _title           = String.Empty;
            _category        = String.Empty;
            _subCategory     = String.Empty;
            _defaultFileName = String.Empty;
            _projectType     = String.Empty;
            _projectSubType  = String.Empty;
        }

        public TemplateConfiguration(TemplateConfiguration source)
            : base(source)
        {
            _id              = source._id;
            _iconId          = source._iconId;
            _iconPath        = source._iconPath;
            _title           = source._title;
            _category        = source._category;
            _subCategory     = source._subCategory;
            _defaultFileName = source._defaultFileName;
            _projectType     = source._projectType;
            _projectSubType  = source._projectSubType;
            _requiresProject = source._requiresProject;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get              
            {
                // TODO: Is the category required?
                return String.IsNullOrEmpty(_id) || String.IsNullOrEmpty(_category);
            }
        }

        public bool RequiresProject
        {
            get
            {
                return _requiresProject;
            }
            set
            {
                _requiresProject = value;
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public string IconId
        {
            get
            {
                return _iconId;
            }
            set
            {
                _iconId = value;
            }
        }

        public string IconPath
        {
            get
            {
                return _iconPath;
            }
            set
            {
                _iconPath = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        }

        public string SubCategory
        {
            get
            {
                return _subCategory;
            }
            set
            {
                _subCategory = value;
            }
        }

        public string DefaultFileName
        {
            get
            {
                return _defaultFileName;
            }
            set
            {
                _defaultFileName = value;
            }
        }

        public string ProjectType
        {
            get
            {
                return _projectType;
            }
            set
            {
                _projectType = value;
            }
        }

        public string ProjectSubType
        {
            get
            {
                return _projectSubType;
            }
            set
            {
                _projectSubType = value;
            }
        }

        #endregion

        #region Private Methods

        private static void WriteConfiguration(XmlWriter writer,
            string attributeValue, string elementValue)
        {
            writer.WriteStartElement("Configuration");
            writer.WriteAttributeString("Name", 
                attributeValue == null ? String.Empty : attributeValue);
            writer.WriteString(elementValue == null ? String.Empty : elementValue);
            writer.WriteEndElement();
        }

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

            _iconId          = String.Empty;
            _iconPath        = String.Empty;
            _title           = String.Empty;
            _category        = String.Empty;
            _subCategory     = String.Empty;
            _defaultFileName = String.Empty;
            _projectType     = String.Empty;
            _projectSubType  = String.Empty;

            string tempText = reader.GetAttribute("Id");
            if (!String.IsNullOrEmpty(tempText))
            {
                _id = tempText;
            }
            tempText = reader.GetAttribute("RequiresProject");
            if (!String.IsNullOrEmpty(tempText))
            {
                _requiresProject = Convert.ToBoolean(tempText);
            }
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "Configuration":
                            switch (reader.GetAttribute("Name"))
                            {
                                case "Title":
                                    _title = reader.ReadString();
                                    break;
                                case "Category":
                                    _category = reader.ReadString();
                                    break;
                                case "SubCategory":
                                    _subCategory = reader.ReadString();
                                    break;
                                case "DefaultFileName":
                                    _defaultFileName = reader.ReadString();
                                    break;
                                case "ProjectType":
                                    _projectType = reader.ReadString();
                                    break;
                                case "ProjectSubType":
                                    _projectSubType = reader.ReadString();
                                    break;
                            }
                            break;
                        case "Icon":
                            _iconId   = reader.GetAttribute("Id");
                            _iconPath = reader.ReadString();
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

            writer.WriteStartElement(TagName);  // start - TemplateConfiguration 
            writer.WriteAttributeString("Id", _id);
            if (!String.IsNullOrEmpty(_projectType))
            {
                // If the project type is not specified, the "RequiresProject" is meaningless...
                writer.WriteAttributeString("RequiresProject", _requiresProject.ToString());
            }

            writer.WriteStartElement("Icon");            // start - Icon
            writer.WriteAttributeString("Id", _iconId);
            writer.WriteString(_iconPath);
            writer.WriteEndElement();                    // end - Icon

            WriteConfiguration(writer, "Title",           _title);
            if (!String.IsNullOrEmpty(_category))
            {
                WriteConfiguration(writer, "Category", _category);
                // If there is no category, the sub-category is meaningless...
                if (!String.IsNullOrEmpty(_subCategory))
                {
                    WriteConfiguration(writer, "SubCategory", _subCategory);
                }
            }
            WriteConfiguration(writer, "DefaultFileName", _defaultFileName);
            if (!String.IsNullOrEmpty(_projectType))
            {
                WriteConfiguration(writer, "ProjectType", _projectType);
                // If there is no project, the sub-project is meaningless...
                if (!String.IsNullOrEmpty(_projectSubType))
                {
                    WriteConfiguration(writer, "ProjectSubType", _projectSubType);
                }
            }

            writer.WriteEndElement();           // end - TemplateConfiguration
        }

        #endregion

        #region ICloneable Members

        public override TemplateConfiguration Clone()
        {
            TemplateConfiguration configuration = new TemplateConfiguration(this);
            if (_iconId != null)
            {
                configuration._iconId = String.Copy(_iconId);
            }
            if (_iconPath != null)
            {
                configuration._iconPath = String.Copy(_iconPath);
            }
            if (_title != null)
            {
                configuration._title = String.Copy(_title);
            }
            if (_category != null)
            {
                configuration._category = String.Copy(_category);
            }
            if (_subCategory != null)
            {
                configuration._subCategory = String.Copy(_subCategory);
            }
            if (_defaultFileName != null)
            {
                configuration._defaultFileName = String.Copy(_defaultFileName);
            }
            if (_projectType != null)
            {
                configuration._projectType = String.Copy(_projectType);
            }
            if (_projectSubType != null)
            {
                configuration._projectSubType = String.Copy(_projectSubType);
            }

            return configuration;
        }

        #endregion
    }
}
