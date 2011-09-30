using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateProperty : BuildObject<TemplateProperty>
    {
        #region Private Fields

        private const string TagName = "TemplateProperty";

        private string _id;   
        private string _name; 
        private string _type; 
        private string _category;
        private string _description;
        private string _defaultValue;

        #endregion

        #region Constructors and Destructor

        public TemplateProperty()
        {
            _id           = String.Empty;
            _name         = String.Empty;
            _type         = String.Empty;
            _category     = String.Empty;
            _description  = String.Empty;
            _defaultValue = String.Empty;
        }

        public TemplateProperty(TemplateProperty source)
            : base(source)
        {
            _id           = source._id;
            _name         = source._name;
            _type         = source._type;
            _category     = source._category;
            _description  = source._description;
            _defaultValue = source._defaultValue;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_id) || String.IsNullOrEmpty(_name) 
                    || String.IsNullOrEmpty(_type);
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value == null)
                {
                    _id = String.Empty;
                }
                else
                {
                    _id = value;
                }
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
                if (value == null)
                {
                    _name = String.Empty;
                }
                else
                {
                    _name = value;
                }
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value == null)
                {
                    _type = String.Empty;
                }
                else
                {
                    _type = value;
                }
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
                if (value == null)
                {
                    _category = String.Empty;
                }
                else
                {
                    _category = value;
                }
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value == null)
                {
                    _description = String.Empty;
                }
                else
                {
                    _description = value;
                }
            }
        }

        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                if (value == null)
                {
                    _defaultValue = String.Empty;
                }
                else
                {
                    _defaultValue = value;
                }
            }
        }

        #endregion

        #region Private Methods

        private static void WriteProperty(XmlWriter writer,
            string attributeValue, string elementValue)
        {
            writer.WriteStartElement("Property");
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

            _id           = String.Empty;
            _name         = String.Empty;
            _type         = String.Empty;
            _category     = String.Empty;
            _description  = String.Empty;
            _defaultValue = String.Empty;

            string tempText = reader.GetAttribute("Id");
            if (!String.IsNullOrEmpty(tempText))
            {
                _id = tempText;
            }
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "Property", StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("Name"))
                        {
                            case "Name":
                                _name = reader.ReadString();
                                break;
                            case "Type":
                                _type = reader.ReadString();
                                break;
                            case "DefaultValue":
                                _defaultValue = reader.ReadString();
                                break;
                            case "Category":
                                _category = reader.ReadString();
                                break;
                            case "Description":
                                _description = reader.ReadString();
                                break;
                        }
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

            writer.WriteStartElement(TagName);  // start - TemplateProperty 
            writer.WriteAttributeString("Id", _id);

            WriteProperty(writer, "Name",         _name);
            WriteProperty(writer, "Type",         _type);
            WriteProperty(writer, "DefaultValue", _defaultValue);
            WriteProperty(writer, "Category",     _category);
            WriteProperty(writer, "Description",  _description);

            writer.WriteEndElement();           // end - TemplateProperty
        }

        #endregion

        #region ICloneable Members

        public override TemplateProperty Clone()
        {
            TemplateProperty property = new TemplateProperty(this);
            if (_id != null)
            {
                property._id = String.Copy(_id);
            }
            if (_name != null)
            {
                property._name = String.Copy(_name);
            }
            if (_type != null)
            {
                property._type = String.Copy(_type);
            }
            if (_category != null)
            {
                property._category = String.Copy(_category);
            }
            if (_description != null)
            {
                property._description = String.Copy(_description);
            }
            if (_defaultValue != null)
            {
                property._defaultValue = String.Copy(_defaultValue);
            }

            return property;
        }

        #endregion
    }
}
