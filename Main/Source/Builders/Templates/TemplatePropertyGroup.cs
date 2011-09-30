using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplatePropertyGroup : BuildObject<TemplatePropertyGroup>
    {
        #region Public Fields

        public const string TagName = "TemplatePropertyGroup";

        #endregion

        #region Private Fields

        private string                _condition;
        private BuildProperties       _properties;
        private TemplateFrameworkType _frameworkType;

        #endregion

        #region Constructors and Destructor

        public TemplatePropertyGroup()
        {
            _condition     = String.Empty;
            _properties    = new BuildProperties();
            _frameworkType = TemplateFrameworkType.None;
        }

        public TemplatePropertyGroup(TemplatePropertyGroup source)
            : base(source)
        {
            _condition     = source._condition;
            _properties    = source._properties;
            _frameworkType = source._frameworkType;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (_properties == null || _properties.Count == 0);
            }
        }

        public string Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                if (value == null)
                {
                    _condition = String.Empty;
                }
                else
                {
                    _condition = value;
                }
            }
        }

        public TemplateFrameworkType FrameworkType
        {
            get
            {
                return _frameworkType;
            }
            set
            {
                _frameworkType = value;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                return _properties[propertyName];
            }
        }

        public BuildProperties Properties
        {
            get
            {
                return _properties;
            }
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

            string tempText = reader.GetAttribute("FrameworkType");
            if (!String.IsNullOrEmpty(tempText))
            {
                _frameworkType = (TemplateFrameworkType)Enum.Parse(
                    typeof(TemplateFrameworkType), tempText, true);
            }
            _condition = reader.GetAttribute("Condition");

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_properties == null || _properties.Count != 0)
            {
                _properties = new BuildProperties();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    tempText = reader.ReadString();
                    if (!String.IsNullOrEmpty(tempText))
                    {
                        _properties[reader.Name] = tempText;
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

            writer.WriteStartElement(TagName);  // start - TemplateItemGroup
            if (_frameworkType != TemplateFrameworkType.None)
            {
                writer.WriteAttributeString("FrameworkType", _frameworkType.ToString());
            }
            if (!String.IsNullOrEmpty(_condition))
            {
                writer.WriteAttributeString("Condition", _condition);
            }
            if (_properties != null && _properties.Count != 0)
            {
                foreach (KeyValuePair<string, string> pair in _properties)
                {
                    if (!String.IsNullOrEmpty(pair.Key) && !String.IsNullOrEmpty(pair.Value))
                    {
                        writer.WriteTextElement(pair.Key, pair.Value);
                    }
                }
            }
            writer.WriteEndElement();           // end - TemplateItemGroup
        }

        #endregion

        #region ICloneable Members

        public override TemplatePropertyGroup Clone()
        {
            TemplatePropertyGroup propertyGroup = new TemplatePropertyGroup(this);
            if (_condition != null)
            {
                propertyGroup._condition = String.Copy(_condition);
            }
            if (_properties != null)
            {
                propertyGroup._properties = _properties.Clone();
            }               

            return propertyGroup;
        }

        #endregion
    }
}
