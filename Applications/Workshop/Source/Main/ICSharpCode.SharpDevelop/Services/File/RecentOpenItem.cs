using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ICSharpCode.SharpDevelop
{
    /// <summary>
    /// This represents an item in the recent file and project list.
    /// </summary>
    public sealed class RecentOpenItem : IEquatable<RecentOpenItem>,
        IXmlSerializable
    {
        #region Private Fields

        public const string XmlTagName = "recentItem";

        private bool   _isPinned;
        private string _fullPath;

        #endregion

        #region Constructors and Destructor

        public RecentOpenItem()
        {
            _fullPath = String.Empty;
        }

        public RecentOpenItem(bool isPinned, string fullPath)
        {
            _isPinned = isPinned;
            _fullPath = fullPath;

            if (_fullPath == null)
            {
                _fullPath = String.Empty;
            }
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_fullPath);
            }
        }

        public bool Pinned
        {
            get
            {
                return _isPinned;
            }
            set
            {
                _isPinned = value;
            }
        }

        public string FullPath
        {
            get
            {
                return _fullPath;
            }
        }

        #endregion

        #region IEquatable<RecentOpenItem> Members

        public override bool Equals(object obj)
        {
            return this.Equals(obj as RecentOpenItem);
        }

        public bool Equals(RecentOpenItem other)
        {
            if (other == null)
            {
                return false;
            }

            return String.Equals(this._fullPath, other._fullPath, 
                StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Methods

        public override int GetHashCode()
        {
            if (!String.IsNullOrEmpty(_fullPath))
            {
                return _fullPath.ToLowerInvariant().GetHashCode();
            }

            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (_fullPath != null)
            {
                return _fullPath;
            }

            return String.Empty;
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null || !String.Equals(reader.Name, XmlTagName))
            {
                return;
            }

            string tempText = reader.GetAttribute("pinned");
            if (!String.IsNullOrEmpty(tempText))
            {
                _isPinned = String.Equals(tempText, "true",
                    StringComparison.OrdinalIgnoreCase);
            }
            _fullPath = reader.GetAttribute("path");
            if (_fullPath == null)
            {
                _fullPath = String.Empty;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }
            if (_fullPath == null)
            {
                _fullPath = String.Empty;
            }
            writer.WriteStartElement(XmlTagName);
            writer.WriteAttributeString("pinned", _isPinned.ToString());
            writer.WriteAttributeString("path",   _fullPath);
            writer.WriteEndElement();
        }

        #endregion
    }
}
