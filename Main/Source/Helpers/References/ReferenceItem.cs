using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceItem : BuildItem<ReferenceItem>
    {
        #region Public Fields

        public const string TagName = "referenceItem";

        #endregion

        #region Private Fields

        private bool          _xamlSyntax;
        private BuildFilePath _comments;
        private BuildFilePath _assembly;
        private BuildFilePath _source;

        #endregion

        #region Constructors and Destructor

        public ReferenceItem()
        {
        }

        public ReferenceItem(string comments, string assembly)
        {
            if (!String.IsNullOrEmpty(comments))
            {
                _comments = new BuildFilePath(comments);
            }
            if (!String.IsNullOrEmpty(assembly))
            {
                _assembly = new BuildFilePath(assembly);
            }
        }

        public ReferenceItem(BuildFilePath comments, BuildFilePath assembly)
        {
            _comments = comments;
            _assembly = assembly;
        }

        public ReferenceItem(ReferenceItem source)
            : base(source)
        {
            _comments   = source._comments;
            _assembly   = source._assembly;
            _source     = source._source;
            _xamlSyntax = source._xamlSyntax;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (_comments == null && _assembly == null && _source == null)
                {
                    return true;
                }
                else if (_comments != null)
                {
                    return (_comments.Exists == false);
                }
                else if (_assembly != null)
                {
                    return (_assembly.Exists == false);
                }
                else if (_source != null)
                {
                    return (_source.Exists == false);
                }

                return false;
            }
        }

        public bool IsCommentOnly
        {
            get
            {     
                // Valid project/solution will yield at least an assembly...
                if (_source != null && _source.IsValid)
                {
                    return false;
                }

                return (_comments != null && _assembly == null);
            }
        }

        public bool XamlSyntax
        {
            get
            {
                return _xamlSyntax;
            }
            set
            {
                _xamlSyntax = value;
            }
        }

        public BuildFilePath Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
            }
        }

        public BuildFilePath Assembly
        {
            get
            {
                return _assembly;
            }
            set
            {
                _assembly = value;
            }
        }

        public BuildFilePath Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ReferenceItem"/> class instance, this property is 
        /// <see cref="ReferenceItem.TagName"/>.
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

        public override bool Equals(ReferenceItem other)
        {
            if (other == null)
            {
                return false;
            }

            if (this._source != null && other._source != null)
            {
                return (this._source == other._source);
            } 
            
            if (this._comments != null && other._comments != null)
            {
                if (this._comments != other._comments)
                {
                    return false;
                }   
            }
            else if (this._comments != null || other._comments != null)
            {
                return false;
            }

            if (this._assembly != null && other._assembly != null)
            {
                if (this._assembly != other._assembly)
                {
                    return false;
                }
            }
            else if (this._assembly != null || other._assembly != null)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            ReferenceItem other = obj as ReferenceItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 19;
            if (_comments != null)
            {
                hashCode ^= _comments.GetHashCode();
            }
            if (_assembly != null)
            {
                hashCode ^= _assembly.GetHashCode();
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
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            string nodeText = reader.GetAttribute("xamlSyntax");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _xamlSyntax = Convert.ToBoolean(nodeText);
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "assembly",
                        StringComparison.OrdinalIgnoreCase) && !reader.IsEmptyElement)
                    {
                        _assembly = BuildFilePath.ReadLocation(reader);
                    }
                    else if (String.Equals(reader.Name, "comments",
                        StringComparison.OrdinalIgnoreCase) && !reader.IsEmptyElement)
                    {
                        _comments = BuildFilePath.ReadLocation(reader);
                    }
                    else if (String.Equals(reader.Name, "source",
                        StringComparison.OrdinalIgnoreCase) && !reader.IsEmptyElement)
                    {
                        _source = BuildFilePath.ReadLocation(reader);
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

            writer.WriteStartElement(TagName);  // start - topic
            writer.WriteAttributeString("xamlSyntax", _xamlSyntax.ToString());

            writer.WriteStartElement("assembly"); // start - assembly
            if (_assembly != null)
            {
                _assembly.WriteXml(writer);
            }
            writer.WriteEndElement();             // end - assembly

            writer.WriteStartElement("comments"); // start - comments
            if (_comments != null)
            {
                _comments.WriteXml(writer);
            }
            writer.WriteEndElement();             // end - comments

            writer.WriteStartElement("source"); // start - source
            if (_source != null)
            {
                _source.WriteXml(writer);
            }
            writer.WriteEndElement();           // end - source

            writer.WriteEndElement();           // end - topic
        }

        #endregion

        #region ICloneable Members

        public override ReferenceItem Clone()
        {
            ReferenceItem reference = new ReferenceItem(this);
            if (_comments != null)
            {
                reference._comments = _comments.Clone();
            }
            if (_assembly != null)
            {
                reference._assembly = _assembly.Clone();
            }
            if (_source != null)
            {
                reference._source = _source.Clone();
            }

            return reference;
        }

        #endregion
    }
}
