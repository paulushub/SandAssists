using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceVsNetItem : BuildItem<ReferenceVsNetItem>
    {
        #region Public Fields

        public const string TagName = "item";

        #endregion

        #region Private Fields

        private bool            _isEnabled;
        private bool            _xamlSyntax;
        private BuildFilePath   _sourcePath;
        private HashSet<string> _includeSet;

        #endregion

        #region Constructors and Destructor

        public ReferenceVsNetItem()
        {
            _isEnabled  = true;
            _includeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public ReferenceVsNetItem(BuildFilePath sourcePath)
            : this()
        {
            _sourcePath = sourcePath;
        }

        public ReferenceVsNetItem(ReferenceVsNetItem source)
            : base(source)
        {
            _isEnabled  = source._isEnabled;
            _xamlSyntax = source._xamlSyntax;
            _sourcePath = source._sourcePath;
            _includeSet = source._includeSet;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (_sourcePath == null)
                {
                    return true;
                }

                return !_sourcePath.Exists;
            }
        }

        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
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

        public BuildFilePath SourcePath
        {
            get
            {
                return _sourcePath;
            }
            set
            {
                _sourcePath = value;
            }
        }

        public IEnumerable<string> Includes
        {
            get
            {
                return _includeSet;
            }
        }

        #endregion

        #region Public Method

        public void AddInclude(string projectGuid)
        {
            if (!String.IsNullOrEmpty(projectGuid))
            {             
                try
                {
                    // We create a guid to verify the validity of the project guid.
                    Guid guid = new Guid(projectGuid);

                    _includeSet.Add(guid.ToString("B").ToUpper());
                }
                catch
                {                    	
                }    
            }
        }

        public bool IsIncluded(string projectGuid)
        {
            if (String.IsNullOrEmpty(projectGuid) ||
                _includeSet == null || _includeSet.Count == 0)
            {
                return false;
            }

            return _includeSet.Contains(projectGuid);
        }

        public bool RemoveInclude(string projectGuid)
        {
            if (String.IsNullOrEmpty(projectGuid))
            {
                return false;
            }

            return _includeSet.Remove(projectGuid);
        }

        public void ClearIncludes()
        {
            if (_includeSet != null && _includeSet.Count != 0)
            {
                _includeSet.Clear();
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ReferenceVsNetItem other)
        {
            if (other == null)
            {
                return false;
            }
            //if (!String.Equals(this._name, other._name))
            //{
            //    return false;
            //}
            //if (!String.Equals(this._value, other._value))
            //{
            //    return false;
            //}

            return true;
        }

        public override bool Equals(object obj)
        {
            ReferenceVsNetItem other = obj as ReferenceVsNetItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 41;
            if (_sourcePath != null)
            {
                hashCode ^= _sourcePath.GetHashCode();
            }
            if (_includeSet != null)
            {
                hashCode ^= _includeSet.GetHashCode();
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
                return;
            }
            string tempText = reader.GetAttribute("enabled");
            if (!String.IsNullOrEmpty(tempText))
            {
                _isEnabled = Convert.ToBoolean(tempText);
            }
            tempText = reader.GetAttribute("xamlSyntax");
            if (!String.IsNullOrEmpty(tempText))
            {
                _xamlSyntax = Convert.ToBoolean(tempText);
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_includeSet == null || _includeSet.Count != 0)
            {
                _includeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "location",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _sourcePath = BuildFilePath.ReadLocation(reader);
                    }
                    else if (String.Equals(reader.Name, "include",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        tempText = reader.GetAttribute("name");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            this.AddInclude(tempText);
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

            writer.WriteStartElement(TagName);  // start - item
            writer.WriteAttributeString("enabled",    _isEnabled.ToString());
            writer.WriteAttributeString("xamlSyntax", _xamlSyntax.ToString());

            BuildFilePath.WriteLocation(_sourcePath, "location", writer);

            writer.WriteStartElement("includes");  // start - includes
            if (_includeSet != null && _includeSet.Count != 0)
            {
                foreach (string include in _includeSet)
                {
                    writer.WriteStartElement("include");  // start - include
                    writer.WriteAttributeString("name", include);
                    writer.WriteEndElement();           // end - include
                }
            }
            writer.WriteEndElement();             // end - includes

            writer.WriteEndElement();           // end - item
        }

        #endregion

        #region ICloneable Members

        public override ReferenceVsNetItem Clone()
        {
            ReferenceVsNetItem vsItem = new ReferenceVsNetItem(this);
            if (_sourcePath != null)
            {
                vsItem._sourcePath = _sourcePath.Clone();
            }
            if (_includeSet != null)
            {
                vsItem._includeSet = new HashSet<string>(_includeSet);
            }

            return vsItem;
        }

        #endregion
    }
}
