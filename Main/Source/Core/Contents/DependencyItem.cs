using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class DependencyItem : BuildItem<DependencyItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "item";

        #endregion

        #region Private Fields

        private string        _name;
        private string        _strongName;
        private BuildFilePath _path;

        private bool          _isRedirected;

        private string        _redirectPublicKeyToken;
        private Version       _redirectVersion;
        private CultureInfo   _redirectCulture;

        #endregion

        #region Constructors and Destructor

        public DependencyItem()
        {
            _name       = Guid.NewGuid().ToString();
            _strongName = String.Empty;
        }

        public DependencyItem(string path)
            : this()
        {
            if (path != null)
            {
                _path = new BuildFilePath(path);
                if (_path.IsValid)
                {
                    _name = _path.Name;
                }
            }
        }

        public DependencyItem(BuildFilePath path)
            : this()
        {
            if (path != null)
            {
                _path = path;
                if (_path.IsValid)
                {
                    _name = _path.Name;
                }
            }
        }

        public DependencyItem(string name, string path)
            : this(path)
        {
            if (name != null)
            {
                name = name.Trim();
            }

            if (!String.IsNullOrEmpty(name))
            {
                _name = name;
            }
        }

        public DependencyItem(string name, BuildFilePath path)
            : this(path)
        {
            if (name != null)
            {
                name = name.Trim();
            }

            if (!String.IsNullOrEmpty(name))
            {
                _name = name;
            }
        }

        public DependencyItem(DependencyItem source)
            : base(source)
        {
            _name                   = source._name;
            _path                   = source._path;
            _strongName             = source._strongName;
            _isRedirected           = source._isRedirected;
            _redirectPublicKeyToken = source._redirectPublicKeyToken;
            _redirectVersion        = source._redirectVersion;
            _redirectCulture        = source._redirectCulture;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (_path == null || !_path.Exists);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string StrongName
        {
            get
            {
                return _strongName;
            }
            set
            {
                if (value != null)
                {
                    _strongName = value.Trim();
                }
                else
                {
                    _strongName = String.Empty;
                }
            }
        }

        public BuildFilePath Location
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                if (_path != null && _path.IsValid)
                {
                    _name = _path.Name;
                }
                else
                {
                    _name = Guid.NewGuid().ToString();
                }
            }
        }

        public bool IsRedirected
        {
            get
            {
                if (_isRedirected)
                {
                    if (String.IsNullOrEmpty(_redirectPublicKeyToken) ||
                        _redirectVersion == null)
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }
        }

        public string RedirectStrongName
        {
            get
            {
                if (!this.IsRedirected || String.IsNullOrEmpty(_name))
                {
                    return String.Empty;
                }

                const string sep = ", ";

                var builder = new StringBuilder();
                builder.Append(Path.GetFileNameWithoutExtension(_name));
                if (_redirectVersion != null)
                {
                    builder.Append(sep);
                    builder.Append("Version=");
                    builder.Append(_redirectVersion.ToString());
                }
                builder.Append(sep);
                builder.Append("Culture=");
                builder.Append((_redirectCulture == null || _redirectCulture.IsNeutralCulture) ? "neutral" : _redirectCulture.Name);
                builder.Append(sep);
                builder.Append("PublicKeyToken=");

                if (String.IsNullOrEmpty(_redirectPublicKeyToken))
                {
                    builder.Append("null");
                }
                else
                {
                    builder.Append(_redirectPublicKeyToken);
                }

                return builder.ToString();
            }
        }

        public bool Redirected
        {
            get
            {
                return _isRedirected;
            }
            set
            {
                _isRedirected = value;
            }
        }

        public string RedirectPublicKeyToken
        {
            get
            {
                return _redirectPublicKeyToken;
            }
            set
            {
                if (value != null)
                {
                    _redirectPublicKeyToken = value.Trim();
                }
                else
                {
                    _redirectPublicKeyToken = value;
                }
            }
        }

        public Version RedirectVersion
        {
            get
            {
                return _redirectVersion;
            }
            set
            {
                _redirectVersion = value;
            }
        }

        public CultureInfo RedirectCulture
        {
            get
            {
                return _redirectCulture;
            }
            set
            {
                _redirectCulture = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="DependencyItem"/> class instance, this property is 
        /// <see cref="DependencyItem.TagName"/>.
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

        #region Public Methods

        /// <summary>
        /// Gets the public key token, which is the last 8 bytes of the 
        /// SHA-1 hash of the public key under which the application or 
        /// assembly is signed.
        /// </summary>
        /// <returns>
        /// An array of type byte containing the public key token.
        /// </returns>
        public byte[] GetPublicKeyToken()
        {
            if (!String.IsNullOrEmpty(_redirectPublicKeyToken))
            {
                byte[] publicKeyToken = new byte[_redirectPublicKeyToken.Length / 2];
                for (int i = 0; i < publicKeyToken.Length; i++)
                {
                    publicKeyToken[i] = Byte.Parse(
                        _redirectPublicKeyToken.Substring(i * 2, 2), NumberStyles.HexNumber);
                }

                return publicKeyToken;
            }

            return null;
        }

        /// <summary>
        /// Sets the public key token, which is the last 8 bytes of the 
        /// SHA-1 hash of the public key under which the application or 
        /// assembly is signed.
        /// </summary>
        /// <param name="publicKeyToken">
        /// A byte array containing the public key token of the assembly.
        /// </param>
        public void SetPublicKeyToken(byte[] publicKeyToken)
        {   
            if (publicKeyToken == null || publicKeyToken.Length == 0)
            {
                _redirectPublicKeyToken = String.Empty;
                return;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < publicKeyToken.Length; i++)
            {
                builder.Append(publicKeyToken[i].ToString("x2"));
            }

            _redirectPublicKeyToken = builder.ToString();
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(DependencyItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._name, other._name))
            {
                return false;
            }
            if (this._path != null && other._path != null)
            {
                if (this._path != other._path)
                {
                    return false;
                }
            }
            else if (this._path != null || other._path != null)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            DependencyItem other = obj as DependencyItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 53;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            if (_path != null)
            {
                hashCode ^= _path.GetHashCode();
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

            _name  = reader.GetAttribute("name");
            //_strongName = reader.GetAttribute("strongName");

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "location",
                        StringComparison.OrdinalIgnoreCase) &&
                           !reader.IsEmptyElement)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, BuildDirectoryPath.TagName,
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    if (_path == null)
                                    {
                                        _path = new BuildFilePath();
                                    }
                                    _path.ReadXml(reader);
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "location",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(reader.Name, "redirection",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string nodeText = reader.GetAttribute("enabled");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _isRedirected = Convert.ToBoolean(nodeText);
                        }
                        
                        _redirectPublicKeyToken = reader.GetAttribute("publicKeyToken");
                        
                        nodeText = reader.GetAttribute("version");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _redirectVersion = new Version(nodeText);
                        }

                        nodeText = reader.GetAttribute("culture");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _redirectCulture = new CultureInfo(nodeText);
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
            writer.WriteAttributeString("name", _name);
            //writer.WriteAttributeString("strongName", _strongName);

            writer.WriteStartElement("location");     // start - location
            if (_path != null)
            {
                _path.WriteXml(writer);
            }
            writer.WriteEndElement();                 // end - location

            writer.WriteStartElement("redirection");  // start - redirection
            writer.WriteAttributeString("enabled", _isRedirected.ToString());
            if (!String.IsNullOrEmpty(_redirectPublicKeyToken))
            {
                writer.WriteAttributeString("publicKeyToken", _redirectPublicKeyToken);
            }
            if (_redirectVersion != null)
            {
                writer.WriteAttributeString("version", _redirectVersion.ToString());
            }
            if (_redirectCulture != null && !_redirectCulture.IsNeutralCulture)
            {
                writer.WriteAttributeString("culture", _redirectCulture.Name);
            }
            writer.WriteEndElement();                 // end - redirection

            writer.WriteEndElement();           // end - item
        }

        #endregion

        #region ICloneable Members

        public override DependencyItem Clone()
        {
            DependencyItem item = new DependencyItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_path != null)
            {
                item._path = _path.Clone();
            }
            if (_strongName != null)
            {
                item._strongName = String.Copy(_strongName);
            }
            if (_redirectPublicKeyToken != null)
            {
                item._redirectPublicKeyToken = String.Copy(_redirectPublicKeyToken);
            }
            if (_redirectVersion != null)
            {
                item._redirectVersion = (Version)_redirectVersion.Clone();
            }
            if (_redirectCulture != null)
            {
                item._redirectCulture = (CultureInfo)_redirectCulture.Clone();
            }

            return item;
        }

        #endregion
    }
}
