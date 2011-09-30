using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class CommentPart : BuildObject<CommentPart>, IEquatable<CommentPart>
    {
        #region Private Fields

        private string          _partText;
        private string          _partName;
        private CommentPartType _partType;

        #endregion

        #region Constructors and Destructor

        public CommentPart()
        {
        }

        public CommentPart(string partText)
        {
            _partText = partText;
        }

        public CommentPart(string partText, CommentPartType partType)
        {
            _partText = partText;
            _partType = partType;
        }

        public CommentPart(string partText, string paraName, 
            CommentPartType partType)
        {
            _partText = partText;
            _partName = paraName;
            _partType = partType;
        }

        public CommentPart(CommentPart source)
            : base(source)
        {
            _partText = source._partText;
            _partName = source._partName;
            _partType = source._partType;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                // For the types that require the parameters, the parameter
                // must be available...
                switch (_partType)
                {
                    case CommentPartType.None:
                        return true;
                    case CommentPartType.Exception:
                    case CommentPartType.Parameter:
                    case CommentPartType.TypeParameter:
                        if (String.IsNullOrEmpty(_partName))
                        {
                            return true;
                        }
                        break;
                }

                return (String.IsNullOrEmpty(_partText));
            }
        }

        public CommentPartType PartType
        {
            get
            {
                return _partType;
            }
            set
            {
                _partType = value;
            }
        }

        public string Name
        {
            get
            {
                return _partName;
            }
            set
            {
                _partName = value;
            }
        }

        public string Text
        {
            get
            {
                return _partText;
            }
            set
            {
                _partText = value;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public bool Equals(CommentPart other)
        {
            if (other == null)
            {
                return false;
            }
            if (this._partType != other._partType)
            {
                return false;
            }
            if (!String.Equals(this._partText, other._partText))
            {
                return false;
            }
            if (!String.Equals(this._partName, other._partName))
            {
                return false;
            }

            return (this._partType == other._partType);
        }

        public override bool Equals(object obj)
        {
            CommentPart other = obj as CommentPart;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 41;
            hashCode    ^= _partType.GetHashCode();
            if (_partText != null)
            {
                hashCode ^= _partText.GetHashCode();
            }

            return hashCode;
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

            if (_partType == CommentPartType.None)
            {
                switch (reader.Name.ToLower())
                {
                    case "overloads":
                        _partType = CommentPartType.Overloads;
                        break;
                    case "summary":
                        _partType = CommentPartType.Summary;
                        break;
                    case "remarks":
                        _partType = CommentPartType.Remarks;
                        break;
                    case "exception":
                        _partType = CommentPartType.Exception;
                        break;
                    case "param":
                        _partType = CommentPartType.Parameter;
                        break;
                    case "typeparam":
                        _partType = CommentPartType.TypeParameter;
                        break;
                    case "returns":
                        _partType = CommentPartType.Returns;
                        break;
                    case "value":
                        _partType = CommentPartType.Value;
                        break;
                    case "example":
                        _partType = CommentPartType.Example;
                        break;
                }
                if (_partType == CommentPartType.None)
                {
                    throw new InvalidOperationException();
                }
            }

            if (reader.HasAttributes)
            {
                switch (_partType)
                {
                    case CommentPartType.Exception:
                        _partName = reader.GetAttribute("cref");
                        break;
                    case CommentPartType.Parameter:
                    case CommentPartType.TypeParameter:
                        _partName = reader.GetAttribute("name");
                        break;
                }
            }

            _partText = reader.ReadInnerXml();
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

            string tagName = null;
            switch (_partType)
            {
                case CommentPartType.Overloads:
                    tagName = "overloads";
                    break;
                case CommentPartType.Summary:
                    tagName = "summary";
                    break;
                case CommentPartType.Remarks:
                    tagName = "remarks";
                    break;
                case CommentPartType.Value:
                    tagName = "value";
                    break;
                case CommentPartType.Returns:
                    tagName = "returns";
                    break;
                case CommentPartType.Parameter:
                    tagName = "param";
                    break;
                case CommentPartType.TypeParameter:
                    tagName = "typeparam";
                    break;
                case CommentPartType.Example:
                    tagName = "example";
                    break;
                case CommentPartType.Exception:
                    tagName = "exception";
                    break;
                case CommentPartType.Enumeration:
                    tagName = "enumeration";
                    break;
            }

            if (String.IsNullOrEmpty(tagName))
            {
                return;
            }

            writer.WriteStartElement(tagName);  // start - tagName            
            switch (_partType)
            {
                case CommentPartType.Exception:
                    writer.WriteAttributeString("cref", _partName);
                    break;
                case CommentPartType.Parameter:
                case CommentPartType.TypeParameter:
                    writer.WriteAttributeString("name", _partName);
                    break;
            }
            writer.WriteRaw(_partText);
            writer.WriteEndElement();           // end - tagName
        }

        #endregion

        #region ICloneable Members

        public override CommentPart Clone()
        {
            CommentPart item = new CommentPart(this);
            if (_partText != null)
            {
                item._partText = String.Copy(_partText);
            }
            if (_partName != null)
            {
                item._partName = String.Copy(_partName);
            }

            return item;
        }

        #endregion
    }
}
