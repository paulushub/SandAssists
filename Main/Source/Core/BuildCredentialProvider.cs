using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildCredentialProvider : BuildObject<BuildCredentialProvider>
    {
        #region Public Fields

        public const string TagName = "credentials";

        #endregion

        #region Private Fields

        private bool _isLoaded;
        private bool _isModified;
        private BuildKeyedList<BuildCredential> _credentials;

        #endregion

        #region Constructors and Destrucctor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildCredentialProvider"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildCredentialProvider"/> class
        /// to the default values.
        /// </summary>
        protected BuildCredentialProvider()
        {
            _credentials = new BuildKeyedList<BuildCredential>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildCredentialProvider"/> class
        /// with properties from the specified source, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildCredentialProvider"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildCredentialProvider(BuildCredentialProvider source)
            : base(source)
        {
            _isLoaded    = source._isLoaded;
            _isModified  = source._isModified;
            _credentials = source._credentials;
        }

        #endregion

        #region Public Properties

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
            protected set
            {
                _isLoaded = value;
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return (_credentials == null || _credentials.Count == 0);
            }
        }

        public bool Modified
        {
            get
            {
                return _isModified;
            }
            set
            {
                _isModified = value;
            }
        }

        public BuildCredential this[int index]
        {
            get
            {
                return _credentials[index];
            }
        }

        public BuildCredential this[string id]
        {
            get
            {
                return _credentials[id];
            }
        }

        public int Count
        {
            get
            {
                return _credentials.Count;
            }
        }

        public IEnumerable<BuildCredential> Credentials
        {
            get
            {
                return _credentials;
            }
        }

        #endregion

        #region Public Method

        public abstract void Load();
        public abstract void Save();

        public virtual void CredentialModified(BuildCredential credential)
        {   
            if (credential != null)
            {
                _isModified = true;
            }
        }

        public static BuildCredentialProvider Create()
        {
            return new SystemCredentialProvider();
        }

        public static BuildCredentialProvider Create(string directory)
        {
            BuildExceptions.PathMustExist(directory, "directory");

            return new SystemCredentialProvider(directory);
        }

        public virtual void Add(BuildCredential credential)
        {
            BuildExceptions.NotNull(credential, "credential");

            if (credential.Provider != this)
            {
                credential.Provider = this;
            }

            _credentials.Add(credential);

            if (this.IsLoaded)
            {
                _isModified = true;
            }
        }

        public virtual void Add(IEnumerable<BuildCredential> credentials)
        {
            BuildExceptions.NotNull(credentials, "credentials");

            foreach (BuildCredential credential in credentials)
            {
                this.Add(credential);
            }
        }

        public virtual void Add(ICollection<BuildCredential> credentials)
        {
            BuildExceptions.NotNull(credentials, "credentials");

            foreach (BuildCredential credential in credentials)
            {
                this.Add(credential);
            }
        }

        public virtual void Add(IList<BuildCredential> credentials)
        {
            BuildExceptions.NotNull(credentials, "credentials");

            int itemCount = credentials.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Add(credentials[i]);
            }
        }

        public virtual void Insert(int index, BuildCredential credential)
        {
            BuildExceptions.NotNull(credential, "credential");

            if (credential.Provider != this)
            {
                credential.Provider = this;
            }

            _credentials.Insert(index, credential);

            if (this.IsLoaded)
            {
                _isModified = true;
            }
        }

        public virtual void Remove(int index)
        {
            if (_credentials.Count == 0)
            {
                return;
            }

            _credentials.RemoveAt(index);

            if (this.IsLoaded)
            {
                _isModified = true;
            }
        }

        public virtual bool Remove(BuildCredential credential)
        {
            BuildExceptions.NotNull(credential, "credential");

            if (_credentials.Count == 0)
            {
                return false;
            }

            bool isRemoved = _credentials.Remove(credential);

            if (isRemoved && this.IsLoaded)
            {
                _isModified = true;
            }

            return isRemoved;
        }

        public virtual bool Contains(BuildCredential credential)
        {
            if (credential == null || _credentials.Count == 0)
            {
                return false;
            }

            return _credentials.Contains(credential);
        }

        public virtual void Clear()
        {
            if (_credentials.Count == 0)
            {
                return;
            }

            _credentials.Clear();

            if (this.IsLoaded)
            {
                _isModified = true;
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

            if (reader.IsEmptyElement)
            {
                return;
            }

            this.Clear();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, BuildCredential.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildCredential credential = new BuildCredential();
                        credential.Provider = this;
                        credential.ReadXml(reader);

                        if (!credential.IsEmpty)
                        {
                            this.Add(credential);
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

            writer.WriteStartElement(TagName);

            int itemCount = this.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region SystemCredentialProvider

        private sealed class SystemCredentialProvider : BuildCredentialProvider
        {
            #region Private Fields

            private const string SourceFile = "Credentials.xml";

            private string _credentialSource;

            #endregion

            #region Constructors and Destructor

            public SystemCredentialProvider()
            {
                string assemblyPath = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location);

                _credentialSource = Path.Combine(assemblyPath, SourceFile);
            }

            public SystemCredentialProvider(string directory)
            {
                BuildExceptions.PathMustExist(directory, "directory");

                _credentialSource = Path.Combine(directory, SourceFile);
            }

            public SystemCredentialProvider(SystemCredentialProvider source)
                : base(source)
            {
                _credentialSource = source._credentialSource;
            }

            #endregion

            #region Public Method

            public override void Load()
            {
                if (String.IsNullOrEmpty(_credentialSource) ||
                    !File.Exists(_credentialSource))
                {
                    return;
                }

                XmlReader reader = null;

                try
                {
                    // Decrypt the file...
                    File.Decrypt(_credentialSource);

                    XmlReaderSettings settings = new XmlReaderSettings();

                    settings.IgnoreComments = true;
                    settings.IgnoreWhitespace = true;
                    settings.IgnoreProcessingInstructions = true;

                    reader = XmlReader.Create(_credentialSource, settings);

                    reader.MoveToContent();

                    this.ReadXml(reader);

                    this.IsLoaded = true;
                    this.Modified = false;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }

                    // Encrypt the file
                    File.Encrypt(_credentialSource);
                    // Hide the file, it is not meant for general access...
                    File.SetAttributes(_credentialSource,
                        FileAttributes.Encrypted | FileAttributes.Hidden);
                }
            }

            public override void Save()
            {
                if (String.IsNullOrEmpty(_credentialSource))
                {
                    return;
                }

                // If this is not yet located, and the contents is empty, we
                // will simply not continue from here...
                if (!this.IsLoaded && base.IsEmpty)
                {
                    return;
                }
                // If loaded but not modified, there is no need to save it...
                if (this.IsLoaded && !this.Modified)
                {
                    return;
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = new string(' ', 4);
                settings.Encoding = Encoding.UTF8;
                settings.OmitXmlDeclaration = false;

                XmlWriter writer = null;
                try
                {
                    writer = XmlWriter.Create(_credentialSource, settings);

                    writer.WriteStartDocument();

                    this.WriteXml(writer);

                    writer.WriteEndDocument();

                    // The file content is now same as the memory, so it can be
                    // considered loaded...
                    this.IsLoaded = true;
                    this.Modified = false;
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                        writer = null;
                    }

                    // Encrypt the file
                    File.Encrypt(_credentialSource);
                    // Hide the file, it is not meant for general access...
                    File.SetAttributes(_credentialSource,
                        FileAttributes.Encrypted | FileAttributes.Hidden);
                }
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
            public override BuildCredentialProvider Clone()
            {
                SystemCredentialProvider provider = 
                    new SystemCredentialProvider(this);

                if (_credentialSource != null)
                {
                    provider._credentialSource = String.Copy(_credentialSource);
                }

                int itemCount = this.Count;
                if (itemCount > 0)
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        provider.Add(this[i].Clone());
                    }
                }

                return provider;
            }

            #endregion
        }

        #endregion
    }
}
