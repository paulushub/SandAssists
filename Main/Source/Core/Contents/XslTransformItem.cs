using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class XslTransformItem : BuildItem<XslTransformItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "xslTransformItem";

        #endregion

        #region Private Fields

        private string _tag;
        private string _name;
        private string _description;
        private BuildFilePath _transformFile;

        #endregion

        #region Constructors and Destructor

        public XslTransformItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public XslTransformItem(string name, string transformFile)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _tag           = String.Empty;
            _name          = name;
            if (!String.IsNullOrEmpty(transformFile))
            {
                _transformFile = new BuildFilePath(transformFile);
            }
        }

        public XslTransformItem(string name, BuildFilePath transformFile)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _tag           = String.Empty;
            _name          = name;
            _transformFile = transformFile;
        }

        public XslTransformItem(XslTransformItem source)
            : base(source)
        {
            _tag           = source._tag;
            _name          = source._name;
            _description   = source._description;
            _transformFile = source._transformFile;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name) ||
                    (_transformFile == null && !_transformFile.Exists))
                {
                    return true;
                }

                return false;
            }
        }

        public string Name
        {
            get
            {
                return _name;
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
                _description = value;
            }
        }

        public BuildFilePath TransformFile
        {
            get
            {
                return _transformFile;
            }
            set
            {
                _transformFile = value;
            }
        }

        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="XslTransformItem"/> class instance, this 
        /// property is <see cref="XslTransformItem.TagName"/>.
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

        public override bool Equals(XslTransformItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }
            if (!String.Equals(this._transformFile, other._transformFile))
            {
                return (this._tag == other._tag);
            }
            if (!String.Equals(this._description, other._description))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            XslTransformItem other = obj as XslTransformItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 29;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            if (_transformFile != null)
            {
                hashCode ^= _transformFile.GetHashCode();
            }
            if (_tag != null)
            {
                hashCode ^= _tag.GetHashCode();
            }
            if (_description != null)
            {
                hashCode ^= _description.GetHashCode();
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

            _name = reader.GetAttribute("name");

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower())
                    {
                        case "description":
                            _description = reader.ReadString();
                            break;
                        case "tag":
                            _tag = reader.ReadString();
                            break;
                        case "transformFile":
                            _transformFile = BuildFilePath.ReadLocation(reader);
                            break;
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

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", _name);
            writer.WriteTextElement("description", _description);
            writer.WriteTextElement("tag", _tag);

            BuildFilePath.WriteLocation(_transformFile, 
                "transformFile", writer);

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override XslTransformItem Clone()
        {
            XslTransformItem item = new XslTransformItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_transformFile != null)
            {
                item._transformFile = _transformFile.Clone();
            }
            if (_tag != null)
            {
                item._tag = String.Copy(_tag);
            }
            if (_description != null)
            {
                item._description = String.Copy(_description);
            }

            return item;
        }

        #endregion
    }
}
