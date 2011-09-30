using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;
using Sandcastle.Conceptual;
using Sandcastle.References;

namespace Sandcastle.Sources
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class ImportGroupSource : BuildGroupSource
    {
        #region Public Static Fields

        public const string SourceName = "Sandcastle.Sources.ImportGroupSource";

        #endregion

        #region Private Fields

        private BuildList<BuildFilePath> _listImports;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ImportGroupSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportGroupSource"/> class
        /// with the default parameters.
        /// </summary>
        public ImportGroupSource()
        {
            _listImports = new BuildList<BuildFilePath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportGroupSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ImportGroupSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ImportGroupSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ImportGroupSource(ImportGroupSource source)
            : base(source)
        {
            _listImports = source._listImports;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique identifier of this group source.
        /// </summary>
        /// <value>
        /// A string containing the unique name of this group source. 
        /// The value of this is <see cref="ImportGroupSource.SourceName"/>.
        /// </value>
        public override string Name
        {
            get
            {
                return ImportGroupSource.SourceName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this source is valid and can
        /// generate the documentation content.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this content source is
        /// valid and can create documentation content or group; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsValid
        {
            get
            {
                if (_listImports == null || _listImports.Count == 0)
                {
                    return false;
                }
                for (int i = 0; i < _listImports.Count; i++)
                {
                    if (_listImports[i].Exists)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public IList<BuildFilePath> Imports
        {
            get
            {
                return _listImports;
            }
        }

        #endregion

        #region Public Methods

        public override IList<BuildGroup> Create(BuildSettings settings,
            BuildContext context)
        {
            bool includesConceptuals = this.IncludesConceptuals;
            bool includesReferences  = this.IncludesReferences;

            BuildLogger logger = null;
            if (context != null && context.IsInitialized)
            {
                logger = context.Logger;

                if (!settings.BuildConceptual)
                {
                    includesConceptuals = false;
                }
                if (!settings.BuildReferences)
                {
                    includesReferences = false;
                }
            }

            // 1. This source must be usable...
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
                        "The import group source '{0}' is invalid.", this.Title), 
                        BuildLoggerLevel.Warn);
                }

                return null;
            }

            // 2. At least one group must be allowed...
            if (!includesConceptuals && !includesReferences)
            {
                return null;
            }

            // 3. Go over the list and create the requested groups...
            List<BuildGroup> groups = new List<BuildGroup>();
            for (int i = 0; i < _listImports.Count; i++)
            {
                BuildGroup group = null;

                BuildFilePath importFile = _listImports[i];
                if (importFile == null || !importFile.Exists)
                {
                    if (logger != null)
                    {
                        logger.WriteLine(String.Format(
                            "The import file '{0}' for '{1}' does not exist.", 
                            importFile.Path, this.Title), BuildLoggerLevel.Warn);
                    }

                    continue;
                }

                string importExt = importFile.Extension;
                if (!String.IsNullOrEmpty(importExt))
                {
                    if (logger != null)
                    {
                        logger.WriteLine(String.Format(
                            "The import file '{0}' for '{1}' does not have a valid file extension.", 
                            importFile.Path, this.Title), BuildLoggerLevel.Warn);
                    }

                    continue;
                }
                if (includesConceptuals && includesReferences)
                {
                    if (importExt.Equals(BuildFileExts.ConceptualGroupExt, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        group = new ConceptualGroup();
                    }
                    else if (importExt.Equals(BuildFileExts.ReferenceGroupExt,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        group = new ReferenceGroup();
                    }
                }
                else if (includesConceptuals)
                {
                    if (importExt.Equals(BuildFileExts.ConceptualGroupExt,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        group = new ConceptualGroup();
                    }
                }
                else if (includesReferences)
                {
                    if (importExt.Equals(BuildFileExts.ReferenceGroupExt,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        group = new ReferenceGroup();
                    }
                }

                // Try loading the group, if created...
                if (group != null)
                {
                    group.Load(importFile);
                }

                // Add the group if valid...
                if (group != null && !group.IsEmpty)
                {
                    groups.Add(group);
                }
            }

            return groups;
        }

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
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    this.Title = tempText;
                                }
                                break;
                            case "id":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    this.Id = tempText;
                                }
                                break;
                            case "includessettings":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    this.IncludesSettings = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includesconceptuals":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    this.IncludesConceptuals = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includesreferences":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    this.IncludesReferences = Convert.ToBoolean(tempText);
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

        #region ReadImportSources Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void ReadImportSources(XmlReader reader)
        {
            string startElement = reader.Name; 
            Debug.Assert(String.Equals(startElement, "importSources"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), this.Name));

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_listImports == null || _listImports.Count != 0)
            {
                _listImports = new BuildList<BuildFilePath>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "importSource",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildFilePath filePath = BuildFilePath.ReadLocation(reader);
                        if (filePath != null && filePath.IsValid)
                        {
                            _listImports.Add(filePath);
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
                    else if (String.Equals(reader.Name, "importSources",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadImportSources(reader);
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
            writer.WritePropertyElement("Id",                  this.Id);
            writer.WritePropertyElement("Title",               this.Title);
            writer.WritePropertyElement("Enabled",             this.Enabled);
            writer.WritePropertyElement("IncludesSettings",    this.IncludesSettings);
            writer.WritePropertyElement("IncludesConceptuals", this.IncludesConceptuals);
            writer.WritePropertyElement("IncludesReferences",  this.IncludesReferences);
            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteStartElement("importSources");  // start - importSources
            if (_listImports != null && _listImports.Count != 0)
            {
                for (int i = 0; i < _listImports.Count; i++)
                {
                    BuildFilePath.WriteLocation(_listImports[i],
                        "importSource", writer);
                }
            }
            writer.WriteEndElement();                   // end - importSources

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override BuildGroupSource Clone()
        {
            ImportGroupSource source = new ImportGroupSource(this);

            this.Clone(source);

            if (_listImports != null)
            {
                source._listImports = _listImports.Clone();
            }

            return source;
        }

        #endregion
    }
}
