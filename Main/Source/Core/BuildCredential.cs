using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildCredential : BuildObject<BuildCredential>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "credential";

        #endregion

        #region Private Fields

        private string _id;
        private string _name;
        private string _password;
        private string _domain;

        private BuildCredentialProvider _provider;

        #endregion

        #region Constructors and Destrucctor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildCredential"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildCredential"/> class
        /// to the default values.
        /// </summary>
        public BuildCredential()
        {
            _id       = Guid.NewGuid().ToString("B");
            _name     = String.Empty;
            _domain   = String.Empty;
            _password = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildCredential"/> class
        /// with properties from the specified source, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildCredential"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildCredential(BuildCredential source)
            : base(source)
        {
            _id       = source._id;
            _name     = source._name;
            _domain   = source._domain;
            _password = source._password;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name) ||
                    String.IsNullOrEmpty(_password))
                {
                    return true;
                }

                return false;
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == null)
                {
                    _name = String.Empty;
                }
                else
                {
                    _name = value.Trim();
                }

                if (_provider != null && _provider.IsLoaded)
                {
                    _provider.CredentialModified(this);
                }
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value == null)
                {
                    _password = String.Empty;
                }
                else
                {
                    _password = value.Trim();
                }

                if (_provider != null && _provider.IsLoaded)
                {
                    _provider.CredentialModified(this);
                }
            }
        }

        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                if (value == null)
                {
                    _domain = String.Empty;
                }
                else
                {
                    _domain = value.Trim();
                }

                if (_provider != null && _provider.IsLoaded)
                {
                    _provider.CredentialModified(this);
                }
            }
        }

        public BuildCredentialProvider Provider
        {
            get
            {
                return _provider;
            }
            internal set
            {
                _provider = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BuildCredential"/> class instance, this property is 
        /// <see cref="BuildCredential.TagName"/>.
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

            _id = reader.GetAttribute("id");
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "name",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _name = reader.ReadString();
                    }
                    else if (String.Equals(reader.Name, "password",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _password = reader.ReadString();
                    }
                    else if (String.Equals(reader.Name, "domain",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _domain = reader.ReadString();
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

            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("id", _id);
            writer.WriteTextElement("name",     _name);
            writer.WriteTextElement("password", _password);
            writer.WriteTextElement("domain",   _domain);
            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new credential object that is a deep copy of the 
        /// current instance.
        /// </summary>
        /// <returns>
        /// A new credential object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this credential object. 
        /// If you need just a copy, use the copy constructor to create a new 
        /// instance.
        /// </remarks>
        public override BuildCredential Clone()
        {
            BuildCredential credential = new BuildCredential(this);

            if (_name != null)
            {
                credential._name = String.Copy(_name);
            }
            if (_password != null)
            {
                credential._password = String.Copy(_password);
            }
            if (_domain != null)
            {
                credential._domain = String.Copy(_domain);
            }

            return credential;
        }

        #endregion
    }
}
