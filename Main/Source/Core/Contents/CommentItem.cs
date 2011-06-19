using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class CommentItem : BuildItem<CommentItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "member";

        #endregion

        #region Private Fields

        private string _name;
        private CommentItemType _itemType;
        private BuildList<CommentPart> _listParts;

        #endregion

        #region Constructors and Destructor

        public CommentItem()
            : this(Guid.NewGuid().ToString(), CommentItemType.None)
        {
        }

        public CommentItem(string name, CommentItemType itemType)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name = name;
            _itemType = itemType;
            _listParts = new BuildList<CommentPart>();
        }

        public CommentItem(CommentItem source)
            : base(source)
        {
            _name = source._name;
            _itemType = source._itemType;
            _listParts = source._listParts;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name) || _listParts == null 
                    || _listParts.Count == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public CommentItemType ItemType
        {
            get
            {
                return _itemType;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public IList<CommentPart> Value
        {
            get
            {
                return _listParts;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(CommentItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }
            //if (!String.Equals(this._listParts, other._listParts))
            //{
            //    return false;
            //}

            return true;
        }

        public override bool Equals(object obj)
        {
            CommentItem other = obj as CommentItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 43;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            if (_listParts != null)
            {
                hashCode ^= _listParts.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a XML format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the XML attributes of this object are accessed.
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

            _name = reader.GetAttribute("name");

            if (_listParts == null)
            {
                _listParts = new BuildList<CommentPart>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    CommentPartType partType = CommentPartType.None;
                    switch (reader.Name)
                    {
                        case "overloads":
                            partType = CommentPartType.Overloads;
                            break;
                        case "summary":
                            partType = CommentPartType.Summary;
                            break;
                        case "remarks":
                            partType = CommentPartType.Remarks;
                            break;
                        case "exception":
                            partType = CommentPartType.Exception;
                            break;
                        case "param":
                            partType = CommentPartType.Parameter;
                            break;
                        case "typeparam":
                            partType = CommentPartType.TypeParameter;
                            break;
                        case "returns":
                            partType = CommentPartType.Returns;
                            break;
                        case "value":
                            partType = CommentPartType.Value;
                            break;
                        case "example":
                            partType = CommentPartType.Example;
                            break;
                    }
                    if (partType == CommentPartType.None)
                    {
                        throw new InvalidOperationException();
                    }

                    CommentPart part = new CommentPart(String.Empty, partType);

                    part.ReadXml(reader);

                    _listParts.Add(part);
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, CommentItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the XML format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The XML writer with which the XML format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);  // start - member
            writer.WriteAttributeString("name", _name);
            if (_listParts != null && _listParts.Count != 0)
            {
                for (int i = 0; i < _listParts.Count; i++)
                {
                    _listParts[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();           // end - member
        }

        #endregion

        #region ICloneable Members

        public override CommentItem Clone()
        {
            CommentItem attrItem = new CommentItem(this);
            if (_name != null)
            {
                attrItem._name = String.Copy(_name);
            }
            if (_listParts != null)
            {
                attrItem._listParts = _listParts.Clone();
            }

            return attrItem;
        }

        #endregion
    }
}
