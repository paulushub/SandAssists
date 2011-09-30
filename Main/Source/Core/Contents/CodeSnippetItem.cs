using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class CodeSnippetItem : BuildItem<CodeSnippetItem>
    {
        #region Public Fields

        public const string TagName = "item";

        #endregion

        #region Private Fields

        private string _exampleId;
        private string _snippetId;
        private string _snippetText;
        private CodeSnippetLanguage _snippetLang;

        #endregion

        #region Constructors and Destructor

        public CodeSnippetItem()
            : this(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
        {
        }


        public CodeSnippetItem(string exampleId, string snippetId)
        {
            BuildExceptions.NotNullNotEmpty(exampleId, "exampleId");
            BuildExceptions.NotNullNotEmpty(snippetId, "snippetId");

            _snippetLang = CodeSnippetLanguage.None;
            _snippetText = String.Empty;

            _exampleId = exampleId;
            _snippetId = snippetId;
        }

        public CodeSnippetItem(string exampleId, string snippetId,
            CodeSnippetLanguage snippetLang, string snippetText) 
            : this(exampleId, snippetId)
        {
            BuildExceptions.NotNull(snippetLang, "snippetLang");
            BuildExceptions.NotNull(snippetText, "snippetText");

            _snippetLang = snippetLang;
            _snippetText = snippetText;
        }

        public CodeSnippetItem(CodeSnippetItem source)
            : base(source)
        {
            _exampleId   = source._exampleId;
            _snippetId   = source._snippetId;
            _snippetLang = source._snippetLang;
            _snippetText = source._snippetText;
       }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return !this.IsValid;
            }
        }

        public bool IsValid
        {
            get
            {
                if (_snippetLang == CodeSnippetLanguage.None)
                {
                    return false;
                }
                if (String.IsNullOrEmpty(_exampleId))
                {
                    return false;
                }
                if (String.IsNullOrEmpty(_snippetId))
                {
                    return false;
                }
                if (String.IsNullOrEmpty(_snippetText))
                {
                    return false;
                }

                return true;
            }
        }

        public string ExampleId
        {
            get
            {
                return _exampleId;
            }
        }

        public string SnippetId
        {
            get
            {
                return _snippetId;
            }
        }

        public CodeSnippetLanguage Language
        {
            get
            {
                return _snippetLang;
            }
            set
            {
                _snippetLang = value;
            }
        }

        public string Text
        {
            get
            {
                return _snippetText;
            }
            set
            {
                if (value != null)
                {
                    _snippetText = value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="CodeSnippetItem"/> class instance, this property is 
        /// <see cref="CodeSnippetItem.TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(CodeSnippetItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._exampleId, other._exampleId))
            {
                return false;
            }
            if (!String.Equals(this._snippetId, other._snippetId))
            {
                return false;
            }
            if (!String.Equals(this._snippetLang, other._snippetLang))
            {
                return false;
            }
            if (!String.Equals(this._snippetText, other._snippetText))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            CodeSnippetItem other = obj as CodeSnippetItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 13;
            if (_exampleId != null)
            {
                hashCode ^= _exampleId.GetHashCode();
            }
            if (_snippetId != null)
            {
                hashCode ^= _snippetId.GetHashCode();
            }
            hashCode ^= _snippetLang.GetHashCode();
            if (_snippetText != null)
            {
                hashCode ^= _snippetText.GetHashCode();
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

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, "The tag name does not match.");
                return;
            }

            string nodeText = reader.GetAttribute("id");
            if (String.IsNullOrEmpty(nodeText))
            {
                Debug.Assert(false, "The tag has no id attribute.");
                return;
            }
            int hashIndex = nodeText.IndexOf('#');
            if (hashIndex < 0)
            {
                Debug.Assert(false, "The tag has valid id attribute.");
                return;
            }
            _exampleId = nodeText.Substring(0, hashIndex);
            _snippetId = nodeText.Substring(hashIndex + 1);

            if (reader.IsEmptyElement)
            {
                return;
            }

            Type langType = typeof(CodeSnippetLanguage);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "sampleCode", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        nodeText = reader.GetAttribute("language");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _snippetLang = (CodeSnippetLanguage)Enum.Parse(
                                langType, nodeText, true);
                            _snippetText = reader.ReadString();
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
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

            writer.WriteStartElement(TagName);  // start - attribute
            writer.WriteAttributeString("id", String.Format("{0}#{1}",
                _exampleId, _snippetId));

            writer.WriteStartElement("sampleCode"); // start - sampleCode
            writer.WriteAttributeString("language", _snippetLang.ToString());
            writer.WriteCData(_snippetText);
            writer.WriteEndElement();               // end - sampleCode

            writer.WriteEndElement();           // end - attribute
        }

        #endregion

        #region ICloneable Members

        public override CodeSnippetItem Clone()
        {
            CodeSnippetItem item = new CodeSnippetItem(this);

            if (_exampleId != null)
            {
                item._exampleId = String.Copy(_exampleId);
            }
            if (_snippetId != null)
            {
                item._snippetId = String.Copy(_snippetId);
            }
            if (_snippetText != null)
            {
                item._snippetText = String.Copy(_snippetText);
            }

            return item;
        }

        #endregion

        #region IBuildNamedItem Members

        //string IBuildNamedItem.Name
        //{
        //    get 
        //    {
        //        return _exampleId; 
        //    }
        //}

        #endregion
    }
}
