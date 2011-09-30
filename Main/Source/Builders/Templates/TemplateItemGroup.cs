using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateItemGroup : BuildObject<TemplateItemGroup>
    {
        #region Public Fields

        public const string TagName = "TemplateItemGroup";

        #endregion

        #region Private Fields

        private string                  _condition;
        private TemplateFrameworkType   _frameworkType;
        private BuildList<TemplateItem> _items;

        #endregion

        #region Constructors and Destructor

        public TemplateItemGroup()
        {
            _items         = new BuildList<TemplateItem>();
            _condition     = String.Empty;
            _frameworkType = TemplateFrameworkType.None;
        }

        public TemplateItemGroup(TemplateItemGroup source)
            : base(source)
        {
            _items         = source._items;
            _condition     = source._condition;
            _frameworkType = source._frameworkType;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (_items == null || _items.Count == 0);
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

        public IList<TemplateItem> Items
        {
            get
            {
                return _items;
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

            if (_items == null || _items.Count != 0)
            {
                _items = new BuildList<TemplateItem>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, TemplateItem.TagName, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        TemplateItem item = new TemplateItem();
                        item.ReadXml(reader);

                        if (!item.IsEmpty)
                        {
                            _items.Add(item);
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

            writer.WriteStartElement(TagName);  // start - TemplateItemGroup
            if (_frameworkType != TemplateFrameworkType.None)
            {
                writer.WriteAttributeString("FrameworkType", _frameworkType.ToString());
            }
            if (!String.IsNullOrEmpty(_condition))
            {
                writer.WriteAttributeString("Condition", _condition);
            }
            if (_items != null && _items.Count != 0)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    _items[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();           // end - TemplateItemGroup
        }

        #endregion

        #region ICloneable Members

        public override TemplateItemGroup Clone()
        {
            TemplateItemGroup itemGroup = new TemplateItemGroup(this);
            if (_condition != null)
            {
                itemGroup._condition = String.Copy(_condition);
            }
            if (_items != null)
            {
                itemGroup._items = _items.Clone();
            }

            return itemGroup;
        }

        #endregion
    }
}
