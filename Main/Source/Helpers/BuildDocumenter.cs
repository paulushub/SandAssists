using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Conceptual;
using Sandcastle.References;

namespace Sandcastle
{
    /// <summary>
    /// This is a generic or base documenter. A documenter builds or generates a
    /// documentation using contents defined by various build groups.
    /// </summary>
    /// <remarks>
    /// The generic documenter is independent of any content or project file format, 
    /// and will build contents or content locations loaded into memory.
    /// </remarks>
    [Serializable]
    public class BuildDocumenter : BuildObject<BuildDocumenter>
    {
        #region Public Fields

        public const string TagName = "documentation";

        #endregion

        #region Private Fields

        private bool _isLoaded;
        private string _documentId;

        private Version _version;
        private BuildFilePath _documentFile;

        private BuildSettings              _settings;
        private BuildKeyedList<BuildGroup> _listGroups;
        private BuildKeyedList<BuildGroupSource> _listSources;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class
        /// to the default properties or values.
        /// </summary>
        public BuildDocumenter()
        {
            _documentId  = Guid.NewGuid().ToString();
            _version     = new Version(1, 0, 0, 0);
            _settings    = new BuildSettings();
            _listGroups  = new BuildKeyedList<BuildGroup>();
            _listSources = new BuildKeyedList<BuildGroupSource>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class with
        /// the specified list of build groups to be initially added to 
        /// this documenter.
        /// </summary>
        /// <param name="groups">
        /// A list, <see cref="IList{T}"/>, specifying the build groups 
        /// <see cref="BuildGroup"/> to be initially added to this documenter.
        /// </param>
        public BuildDocumenter(IList<BuildGroup> groups)
            : this()
        {
            if (groups != null && groups.Count != 0)
            {
                _listGroups.Add(groups);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class
        /// with properties from the specified source, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildDocumenter"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildDocumenter(BuildDocumenter source)
            : base(source)
        {
            _isLoaded = source._isLoaded;
            _version = source._version;
            _documentFile = source._documentFile;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (_listGroups == null || _listGroups.Count == 0)
                {
                    return (_listSources == null || _listSources.Count == 0);
                }

                return false;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }

        public BuildSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public IList<BuildGroup> Groups
        {
            get
            {
                if (_listGroups != null)
                {
                    return _listGroups;
                }

                return null;
            }
        }

        public IList<BuildGroupSource> Sources
        {
            get
            {
                if (_listSources != null)
                {
                    return _listSources;
                }

                return null;
            }
        }

        public BuildFilePath DocumentFile
        {
            get
            {
                return _documentFile;
            }
            set
            {
                if (value != null)
                {
                    _documentFile = value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BuildDocumenter"/> class instance, 
        /// this property is <see cref="BuildDocumenter.TagName"/>.
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

        #region Load Method

        public void Load()
        {
            if (String.IsNullOrEmpty(_documentFile))
            {
                return;
            }

            BuildPathResolver resolver = BuildPathResolver.Create(
                Path.GetDirectoryName(_documentFile), _documentId);

            this.Load(resolver);
        }

        public void Load(BuildPathResolver resolver)
        {
            BuildExceptions.NotNull(resolver, "resolver");

            if (_isLoaded)
            {
                return;
            }

            if (String.IsNullOrEmpty(_documentFile) ||
                File.Exists(_documentFile) == false)
            {
                return;
            }

            XmlReader reader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments               = true;
                settings.IgnoreWhitespace             = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(_documentFile, settings);

                reader.MoveToContent();

                string resolverId = BuildPathResolver.Push(resolver);
                {
                    this.ReadXml(reader);

                    BuildPathResolver.Pop(resolverId);
                }   

                _isLoaded = true;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        public void Reload()
        {
            _isLoaded = false;

            this.Load();
        }

        #endregion

        #region Save Method

        public void Save()
        {
            if (String.IsNullOrEmpty(_documentFile))
            {
                return;
            }

            BuildPathResolver resolver = BuildPathResolver.Create(
                Path.GetDirectoryName(_documentFile), _documentId);

            this.Save(resolver);
        }

        public void Save(BuildPathResolver resolver)
        {
            BuildExceptions.NotNull(resolver, "resolver");

            // If this is not yet located, and the contents is empty, we
            // will simply not continue from here...
            if (_documentFile != null && _documentFile.Exists)
            {
                if (!this._isLoaded && this.IsEmpty)
                {
                    return;
                }
            }

            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.Indent             = true;
            settings.IndentChars        = new string(' ', 4);
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(_documentFile, settings);

                string resolverId = BuildPathResolver.Push(resolver);
                {
                    writer.WriteStartDocument();

                    this.WriteXml(writer);

                    writer.WriteEndDocument();

                    BuildPathResolver.Pop(resolverId);
                }

                // The file content is now same as the memory, so it can be
                // considered loaded...
                _isLoaded = true;
            }
            finally
            {  
            	if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
        }

        #endregion

        #region Group Methods

        public void AddGroup(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (_listGroups == null)
            {
                _listGroups = new BuildKeyedList<BuildGroup>();
            }

            _listGroups.Add(group);
        }

        public void AddGroups(IList<BuildGroup> groups)
        {
            BuildExceptions.NotNull(groups, "groups");

            for (int i = 0; i < groups.Count; i++)
            {
                this.AddGroup(groups[i]);
            }
        }

        public void RemoveGroup(int index)
        {
            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            _listGroups.RemoveAt(index);
        }

        public void RemoveGroup(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            _listGroups.Remove(group);
        }

        public bool ContainsGroup(BuildGroup group)
        {
            if (group == null || _listGroups == null || _listGroups.Count == 0)
            {
                return false;
            }

            return _listGroups.Contains(group);
        }

        public void ClearGroups()
        {
            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            _listGroups.Clear();
        }

        #endregion

        #region Source Methods

        public void AddSource(BuildGroupSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null)
            {
                _listSources = new BuildKeyedList<BuildGroupSource>();
            }

            _listSources.Add(source);
        }

        public void AddSources(IList<BuildGroupSource> sources)
        {
            BuildExceptions.NotNull(sources, "sources");

            for (int i = 0; i < sources.Count; i++)
            {
                this.AddSource(sources[i]);
            }
        }

        public void RemoveSource(int index)
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.RemoveAt(index);
        }

        public void RemoveSource(BuildGroupSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Remove(source);
        }

        public bool ContainsSource(BuildGroupSource source)
        {
            if (source == null || _listSources == null || _listSources.Count == 0)
            {
                return false;
            }

            return _listSources.Contains(source);
        }

        public void ClearSources()
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Clear();
        }

        #endregion

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

            // Read the version information of the file group...
            string tempText = reader.GetAttribute("version");
            if (!String.IsNullOrEmpty(tempText))
            {
                _version = new Version(tempText);
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_listGroups == null || _listGroups.Count != 0)
            {
                _listGroups = new BuildKeyedList<BuildGroup>();
            }
            if (_listSources == null || _listSources.Count != 0)
            {
                _listSources = new BuildKeyedList<BuildGroupSource>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, BuildSettings.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (_settings == null)
                        {
                            _settings = new BuildSettings();
                        }
                        _settings.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, BuildGroup.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {                          
                        BuildGroup group = null;

                        tempText = reader.GetAttribute("type");

                        if (String.Equals(tempText, "Conceptual",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            group = new ConceptualGroup();
                        }
                        else if (String.Equals(tempText, "Reference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            group = new ReferenceGroup();
                        }
                        else
                        {
                            throw new NotImplementedException(tempText);
                        }

                        if (reader.IsEmptyElement)
                        {
                            string sourceFile = reader.GetAttribute("source");
                            if (!String.IsNullOrEmpty(sourceFile))
                            {
                                group.ContentFile = new BuildFilePath(sourceFile);
                                group.Load();
                            }
                        }
                        else
                        {
                            group.ReadXml(reader);
                        }

                        _listGroups.Add(group);
                    }
                    else if (String.Equals(reader.Name, BuildGroupSource.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildGroupSource source = BuildGroupSource.CreateSource(
                            reader.GetAttribute("name"));

                        if (source == null)
                        {
                            throw new BuildException(String.Format(
                                "The creation of the group source '{0}' failed.",
                                reader.GetAttribute("name")));
                        }

                        source.ReadXml(reader);

                        _listSources.Add(source);
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

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("version", _version.ToString(2));

            //  1. Documentation: Settings of the documentation
            writer.WriteComment(" 1. Documentation: Settings of the documentation ");
            if (_settings != null)
            {
                _settings.WriteXml(writer);
            }

            // 2. Documentation: Group sources of the documentation
            writer.WriteComment(" 2. Documentation: Group sources of the documentation ");
            writer.WriteStartElement("documentSources"); // start - documentSources
            if (_listSources != null && _listSources.Count != 0)
            {
                for (int i = 0; i < _listSources.Count; i++)
                {
                    _listSources[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();                    // end - documentSources

            // 3. Documentation: Groups of the documentation 
            writer.WriteComment(" 3. Documentation: Groups of the documentation ");
            writer.WriteStartElement("documentGroups"); // start - documentGroups
            if (_listGroups != null && _listGroups.Count != 0)
            {            
                BuildPathResolver resolver = BuildPathResolver.Resolver;
                Debug.Assert(resolver != null && resolver.Id == _documentId);

                for (int i = 0; i < _listGroups.Count; i++)
                {
                    BuildGroup group = _listGroups[i];    

                    BuildFilePath filePath = group.ContentFile;
                    if (filePath != null && filePath.IsValid)
                    {
                        writer.WriteStartElement(BuildGroup.TagName);
                        writer.WriteAttributeString("type",   group.GroupType.ToString());
                        writer.WriteAttributeString("source", resolver.ResolveRelative(filePath));
                        writer.WriteEndElement();

                        group.Save();
                    }
                    else
                    {
                        group.WriteXml(writer);
                    }
                }
            }
            writer.WriteEndElement();                    // end - documentGroups

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new directory copier object that is a deep copy of the 
        /// current instance.
        /// </summary>
        /// <returns>
        /// A new directory copier object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this directory copier object. 
        /// If you need just a copy, use the copy constructor to create a new 
        /// instance.
        /// </remarks>
        public override BuildDocumenter Clone()
        {
            BuildDocumenter documenter = new BuildDocumenter(this);

            if (_settings != null)
            {
                documenter._settings = _settings.Clone();
            }
            if (_listGroups != null)
            {
                documenter._listGroups = _listGroups.Clone();
            }

            return documenter;
        }

        #endregion
    }
}
