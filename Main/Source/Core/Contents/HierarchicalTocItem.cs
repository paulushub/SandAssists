using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class HierarchicalTocItem : BuildItem<HierarchicalTocItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "hierarchicalTocItem";

        #endregion

        #region Private Fields

        private string       _fileName;
        private string       _projectName;
        private string       _namespaceName;

        [NonSerialized]
        private List<string> _namespaceParts;

        #endregion

        #region Constructors and Destructor

        public HierarchicalTocItem()
        {
            _namespaceName = Guid.NewGuid().ToString();
            _fileName      = String.Empty;
            _projectName   = String.Empty;
            _namespaceName = String.Empty;
        }

        public HierarchicalTocItem(string namespaceName, 
            string projectName, string fileName)
        {
            BuildExceptions.NotNullNotEmpty(namespaceName, "namespaceName");

            _namespaceName  = namespaceName.Trim();
            _fileName       = fileName;
            _projectName    = projectName;

            if (_fileName == null)
            {
                _fileName = String.Empty;
            }
            if (_projectName == null)
            {
                _projectName = String.Empty;
            }

            this.SplitName();
        }

        public HierarchicalTocItem(HierarchicalTocItem source)
            : base(source)
        {
            _namespaceName = source._namespaceName;
            _fileName      = source._fileName;
            _projectName   = source._projectName;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_namespaceName) ||
                    String.IsNullOrEmpty(_fileName);
            }
        }

        public string this[int index]
        {
            get
            {
                if (index >= 0 && index < _namespaceParts.Count)
                {
                    return _namespaceParts[index];
                }

                return String.Empty;
            }
        }

        public int Count
        {
            get
            {
                if (_namespaceParts == null)
                {
                    if (!String.IsNullOrEmpty(_namespaceName))
                    {
                        this.SplitName();
                    }
                }

                if (_namespaceParts != null)
                {
                    return _namespaceParts.Count;
                }

                return 0;
            }
        }

        public string Name
        {
            get
            {
                return _namespaceName;
            }
            private set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _namespaceName = value.Trim();

                this.SplitName();
            }
        }

        public string FileName
        {
            get 
            { 
                return _fileName; 
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _fileName = value.Trim();
            }
        }

        public string ProjectName
        {
            get 
            { 
                return _projectName; 
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _projectName = value.Trim();
            }
        }

        public IList<string> Parts
        {
            get
            {
                if (_namespaceParts == null)
                {
                    if (!String.IsNullOrEmpty(_namespaceName))
                    {
                        this.SplitName();
                    }
                }

                return _namespaceParts;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="HierarchicalTocItem"/> class instance, this property is 
        /// <see cref="HierarchicalTocItem.TagName"/>.
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

        #region Private Methods

        private void SplitName()
        {
            if (String.IsNullOrEmpty(_namespaceName))
            {
                return;
            }

            _namespaceParts = new List<string>();

            string[] items = _namespaceName.Split('.');

            if (items != null && items.Length != 0)
            {
                _namespaceParts = new List<string>(items);
            }
            else
            {
                _namespaceParts = new List<string>();
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(HierarchicalTocItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._namespaceName, other._namespaceName))
            {
                return false;
            }
            if (!String.Equals(this._fileName, other._fileName))
            {
                return false;
            }
            if (!String.Equals(this._projectName, other._projectName))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            HierarchicalTocItem other = obj as HierarchicalTocItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 53;
            if (_namespaceName != null)
            {
                hashCode ^= _namespaceName.GetHashCode();
            }
            if (_fileName != null)
            {
                hashCode ^= _fileName.GetHashCode();
            }
            if (_projectName != null)
            {
                hashCode ^= _projectName.GetHashCode();
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

            if (String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                _namespaceName = reader.GetAttribute("id");
                _projectName   = reader.GetAttribute("project");
                _fileName      = reader.GetAttribute("file");
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
            writer.WriteAttributeString("id",      _namespaceName);
            writer.WriteAttributeString("project", _projectName);
            writer.WriteAttributeString("file",    _fileName);
            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override HierarchicalTocItem Clone()
        {
            HierarchicalTocItem item = new HierarchicalTocItem(this);
            if (_namespaceName != null)
            {
                item._namespaceName = String.Copy(_namespaceName);
            }
            if (_fileName != null)
            {
                item._fileName = String.Copy(_fileName);
            }
            if (_projectName != null)
            {
                item._projectName = String.Copy(_projectName);
            }

            return item;
        }

        #endregion
    }
}
