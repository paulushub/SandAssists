using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateDescription : BuildObject<TemplateDescription>
    {
        #region Private Fields

        private const string TagName = "TemplateDescription";

        private string       _previewImageId;
        private string       _previewImagePath;
        private BuildList<string> _descriptions;

        #endregion

        #region Constructors and Destructor

        public TemplateDescription()
        {
            _previewImageId   = String.Empty;
            _previewImagePath = String.Empty;
            _descriptions     = new BuildList<string>();
        }

        public TemplateDescription(TemplateDescription source)
            : base(source)
        {
            _descriptions     = source._descriptions;
            _previewImageId   = source._previewImageId;
            _previewImagePath = source._previewImagePath;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (_descriptions == null || _descriptions.Count == 0);
            }
        }

        public string PreviewImageId
        {
            get
            {
                return _previewImageId;
            }
            set
            {
                if (value == null)
                {
                    _previewImageId = String.Empty;
                }
                else
                {
                    _previewImageId = value;
                }
            }
        }

        public string PreviewImagePath
        {
            get
            {
                return _previewImagePath;
            }
            set
            {
                if (value == null)
                {
                    _previewImagePath = String.Empty;
                }
                else
                {
                    _previewImagePath = value;
                }
            }
        }

        public IList<string> Descriptions
        {
            get
            {
                return _descriptions;
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
            if (reader.IsEmptyElement)
            {
                return;
            }

            _previewImageId   = String.Empty;
            _previewImagePath = String.Empty;
            if (_descriptions == null || _descriptions.Count != 0)
            {
                _descriptions = new BuildList<string>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "Description":
                            string tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _descriptions.Add(tempText);
                            }
                            break;
                        case "PreviewImage":
                            _previewImageId = reader.GetAttribute("Id");
                            _previewImagePath = reader.ReadString();
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

            writer.WriteStartElement(TagName);  // start - TemplateDescription 

            writer.WriteStartElement("PreviewImage");            // start - PreviewImage
            writer.WriteAttributeString("Id", _previewImageId);
            writer.WriteString(_previewImagePath);
            writer.WriteEndElement();                            // end - PreviewImage

            if (_descriptions != null && _descriptions.Count != 0)
            {
                for (int i = 0; i < _descriptions.Count; i++)
                {
                    writer.WriteStartElement("Description");  // start - Description
                    writer.WriteString(_descriptions[i]);
                    writer.WriteEndElement();                 // end - Description
                }
            }

            writer.WriteEndElement();           // end - TemplateDescription
        }

        #endregion

        #region ICloneable Members

        public override TemplateDescription Clone()
        {
            TemplateDescription description = new TemplateDescription(this);
            if (_previewImageId != null)
            {
                description._previewImageId = String.Copy(_previewImageId);
            }
            if (_previewImagePath != null)
            {
                description._previewImagePath = String.Copy(_previewImagePath);
            }
            if (_descriptions != null)
            {
                description._descriptions = _descriptions.Clone();
            }

            return description;
        }

        #endregion
    }
}
