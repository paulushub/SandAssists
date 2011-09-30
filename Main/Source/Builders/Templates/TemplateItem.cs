using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateItem : BuildObject<TemplateItem>
    {
        #region Public Fields

        public const string TagName = "TemplateItem";

        private string _itemType;
        private string _include;

        private BuildProperties _metadata;

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public TemplateItem()
        {
            _metadata = new BuildProperties();
        }

        public TemplateItem(TemplateItem source)
            : base(source)
        {
            _include  = source._include;
            _itemType = source._itemType;
            _metadata = source._metadata;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_itemType) || String.IsNullOrEmpty(_include);
            }
        }

        public string ItemType
        {
            get
            {
                return _itemType;
            }
            set
            {
                _itemType = value;
            }
        }

        public string Include
        {
            get
            {
                return _include;
            }
            set
            {
                _include = value;
            }
        }

        public BuildProperties Metadata
        {
            get
            {
                return _metadata;
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

            _itemType = reader.GetAttribute("ItemType");
            _include  = reader.GetAttribute("Include");

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_metadata == null || _metadata.Count != 0)
            {
                _metadata = new BuildProperties();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string tempText = reader.ReadString();
                    if (!String.IsNullOrEmpty(tempText))
                    {
                        _metadata[reader.Name] = tempText;
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

            writer.WriteStartElement(TagName);  // start - TemplateItem
            writer.WriteAttributeString("ItemType", _itemType);
            writer.WriteAttributeString("Include",  _include);
            if (_metadata != null && _metadata.Count != 0)
            {
                foreach (KeyValuePair<string, string> pair in _metadata)
                {
                    if (!String.IsNullOrEmpty(pair.Key) && !String.IsNullOrEmpty(pair.Value))
                    {
                        writer.WriteTextElement(pair.Key, pair.Value);
                    }
                }
            }
            writer.WriteEndElement();           // end - TemplateItem
        }

        #endregion

        #region ICloneable Members

        public override TemplateItem Clone()
        {
            TemplateItem item = new TemplateItem(this);
            if (_itemType != null)
            {
                item._itemType = String.Copy(_itemType);
            }
            if (_include != null)
            {
                item._include = String.Copy(_include);
            }
            if (_metadata != null)
            {
                item._metadata = _metadata.Clone();
            }

            return item;
        }

        #endregion
    }
}
