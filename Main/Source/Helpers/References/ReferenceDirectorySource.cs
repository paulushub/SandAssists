using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.References
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class ReferenceDirectorySource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceDirectorySource";

        #endregion

        #region Private Fields

        private bool               _isRecursive;
        private string             _searchPattern;

        private BuildFrameworkType _frameworkType;

        private HashSet<string>    _excludeSet;
        private BuildDirectoryPath _sourcePath;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceDirectorySource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDirectorySource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceDirectorySource()
        {
            _isRecursive   = false;
            _searchPattern = "*.dll";
            _frameworkType = BuildFrameworkType.Null;
            _excludeSet    = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDirectorySource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceDirectorySource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceDirectorySource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceDirectorySource(ReferenceDirectorySource source)
            : base(source)
        {
            _excludeSet    = source._excludeSet;
            _sourcePath    = source._sourcePath;
            _isRecursive   = source._isRecursive;
            _searchPattern = source._searchPattern;
            _frameworkType = source._frameworkType;
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceDirectorySource.SourceName;
            }
        }

        public override bool IsValid
        {
            get
            {
                if (_sourcePath != null && _sourcePath.Exists)
                {
                    return !DirectoryUtils.IsDirectoryEmpty(_sourcePath.Path);
                }

                return false;
            }
        }

        public bool SearchRecursive
        {
            get
            {
                return _isRecursive;
            }
            set
            {
                _isRecursive = value;
            }
        }


        public string SearchPattern
        {
            get
            {
                return _searchPattern;
            }
            set
            {
                _searchPattern = value;
            }
        }

        public BuildDirectoryPath SourcePath
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

        public IEnumerable<string> Excludes
        {
            get
            {
                return _excludeSet;
            }
        }

        public BuildFrameworkType FrameworkType
        {
            get
            {
                return _frameworkType;
            }
            set
            {
                _frameworkType = value;
            }
        }

        #endregion

        #region Public Methods

        public override ReferenceContent Create(BuildGroupContext groupContext)
        {
            BuildExceptions.NotNull(groupContext, "groupContext");

            BuildContext context = groupContext.Context;
            BuildLogger logger = null;
            if (context != null)
            {
                logger = context.Logger;
            }

            if (!this.IsInitialized)
            {
                throw new BuildException(String.Format(
                    "The content source '{0}' is not yet initialized.", this.Title));
            }
            if (!this.IsValid)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The content group source '{0}' is invalid.", this.Title),
                        BuildLoggerLevel.Warn);
                }

                return null;
            }

            string searchPattern = _searchPattern;
            if (String.IsNullOrEmpty(searchPattern))
            {
                searchPattern = "*.dll";
            }
            string[] fullPaths = Directory.GetFiles(_sourcePath.Path,
                searchPattern, _isRecursive ? SearchOption.AllDirectories :
                SearchOption.TopDirectoryOnly);

            if (fullPaths == null || fullPaths.Length == 0)
            {
                return null;
            }

            HashSet<string> dependencyDirs = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> assemblies =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (string fullPath in fullPaths)
            {
                string fileName = Path.GetFileName(fullPath);
                if (assemblies.ContainsKey(fileName) || 
                    (_excludeSet.Count != 0 && _excludeSet.Contains(fileName)))
                {
                    continue;
                }

                // We will not use the assemblies without comments as
                // dependencies, since we cannot verify these are .NET DLLs.
                string commentFile = Path.ChangeExtension(fullPath, ".xml");
                if (!File.Exists(commentFile))
                {
                    continue;
                }

                string dependencyDir = Path.GetDirectoryName(fullPath);
                if (!dependencyDir.EndsWith("\\"))
                {
                    dependencyDir += "\\";
                }
                dependencyDirs.Add(dependencyDir);

                assemblies[fileName] = fullPath;
            }

            if (assemblies.Count == 0)
            {
                return null;
            }

            ReferenceContent content = new ReferenceContent();

            // Set the framework version...
            if (_frameworkType == BuildFrameworkType.Null ||
                _frameworkType == BuildFrameworkType.None)
            {
                BuildFramework framework = BuildFrameworks.LatestFramework;

                if (framework == null)
                {
                    // If not successful, use the default...
                    framework = BuildFrameworks.DefaultFramework;
                }

                content.FrameworkType = framework.FrameworkType;
            }
            else
            {
                content.FrameworkType = _frameworkType;
            }

            foreach (KeyValuePair<string, string> pair in assemblies)
            {
                string assemblyFile = pair.Value;
                string commentFile  = Path.ChangeExtension(assemblyFile, ".xml");

                content.AddItem(commentFile, assemblyFile);
            }

            // Provide the dependency information for the content...
            DependencyContent depContents = content.Dependencies;
            foreach (string dependencyDir in dependencyDirs)
            {
                depContents.Paths.Add(new BuildDirectoryPath(dependencyDir));
            }

            // Provide other user-supplied information to the content...
            content.Comments         = this.Comments;
            content.HierarchicalToc  = this.HierarchicalToc;
            content.TypeFilters      = this.TypeFilters;
            content.AttributeFilters = this.AttributeFilters;

            return content;
        }

        #region Exclude Methods

        public void AddExclude(string assemblyName)
        {
            if (String.IsNullOrEmpty(assemblyName))
            {
                return;
            }
            if (Path.IsPathRooted(assemblyName))
            {
                assemblyName = Path.GetFileName(assemblyName);
            }
            _excludeSet.Add(assemblyName);
        }

        public bool IsExcluded(string assemblyName)
        {
            if (String.IsNullOrEmpty(assemblyName) ||
                _excludeSet == null || _excludeSet.Count == 0)
            {
                return false;
            }

            return _excludeSet.Contains(assemblyName);
        }

        public bool RemoveExclude(string assemblyName)
        {
            if (String.IsNullOrEmpty(assemblyName))
            {
                return false;
            }

            return _excludeSet.Remove(assemblyName);
        }

        public void ClearExcludes()
        {
            if (_excludeSet != null && _excludeSet.Count != 0)
            {
                _excludeSet.Clear();
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region ReadPropertyGroup Method

        private void ReadPropertyGroup(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "title":
                                this.Title = reader.ReadString();
                                break;
                            case "searchrecursive":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _isRecursive = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "searchpattern":
                                _searchPattern = reader.ReadString();
                                break;
                            case "frameworktype":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _frameworkType = BuildFrameworkType.Parse(tempText);
                                }
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadExcludes Method

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
        private void ReadExcludes(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "excludes"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_excludeSet == null || _excludeSet.Count != 0)
            {
                _excludeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "exclude",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = reader.ReadString();
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            this.AddExclude(tempText);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
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

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadPropertyGroup(reader);
                    }
                    else if (String.Equals(reader.Name, "location",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _sourcePath = BuildDirectoryPath.ReadLocation(reader);
                    }
                    else if (String.Equals(reader.Name, "excludes",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadExcludes(reader);
                    }
                    else if (String.Equals(reader.Name, "contents",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadContents(reader);
                    }
                    else if (String.Equals(reader.Name, "filters",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadFilters(reader);
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

            if (!this.IsValid)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", this.Name);

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Title",           this.Title);
            writer.WritePropertyElement("SearchRecursive", _isRecursive);
            writer.WritePropertyElement("SearchPattern",   _searchPattern);
            writer.WritePropertyElement("FrameworkType",   _frameworkType.ToString());
            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteStartElement("location");  // start - location            
            _sourcePath.WriteXml(writer);
            writer.WriteEndElement();              // end - location

            writer.WriteStartElement("excludes");  // start - excludes   
            if (_excludeSet != null)
            {
                foreach (string exclude in _excludeSet)
                {
                    writer.WriteStartElement("exclude");  // start - exclude
                    writer.WriteAttributeString("name", exclude);
                    writer.WriteEndElement();             // end - exclude
                }
            }
            writer.WriteEndElement();              // end - excludes

            // Write the user-defined contents...
            this.WriteContents(writer);

            // Write the filters...
            this.WriteFilters(writer);

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override ReferenceSource Clone()
        {
            ReferenceDirectorySource source = new ReferenceDirectorySource(this);

            this.Clone(source);

            if (_searchPattern != null)
            {
                source._searchPattern = String.Copy(_searchPattern);
            }
            if (_sourcePath != null)
            {
                source._sourcePath = _sourcePath.Clone();
            }
            if (_excludeSet != null)
            {
                source._excludeSet = new HashSet<string>(_excludeSet);
            }

            return source;
        }

        #endregion
    }
}
