using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;
using Sandcastle.ReflectionData;

namespace Sandcastle.Conceptual
{
    using ReferenceEngine = Sandcastle.References.ReferenceEngine;

    [Serializable]
    public sealed class ConceptualReferenceLinkConfiguration : ConceptualComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.Conceptual.ConceptualReferenceLinkConfiguration";

        #endregion

        #region Private Fields

        [NonSerialized]
        private BuildFormat   _format;
        [NonSerialized]
        private BuildContext _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualReferenceLinkConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualReferenceLinkConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ConceptualReferenceLinkConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualReferenceLinkConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualReferenceLinkConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualReferenceLinkConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualReferenceLinkConfiguration(
            ConceptualReferenceLinkConfiguration source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the category of options.
        /// </summary>
        /// <value>
        /// <para>
        /// A <see cref="System.String"/> specifying the name of this category of options.
        /// </para>
        /// <para>
        /// The value is <see cref="ConceptualReferenceLinkConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ConceptualReferenceLinkConfiguration.ConfigurationName;
            }
        }

        /// <summary>
        /// Gets the source of the build component supported by this configuration.
        /// </summary>
        /// <value>
        /// An enumeration of the type, <see cref="BuildComponentType"/>,
        /// specifying the source of this build component.
        /// </value>
        public override BuildComponentType ComponentType
        {
            get
            {
                return BuildComponentType.SandcastleAssist;
            }
        }

        /// <summary>
        /// Gets the unique name of the build component supported by this configuration. 
        /// </summary>
        /// <value>
        /// A string containing the unique name of the build component, this 
        /// should normally include the namespace.
        /// </value>
        public override string ComponentName
        {
            get
            {
                return "Sandcastle.Components.ReferenceLinkComponent";
            }
        }

        /// <summary>
        /// Gets the path of the build component supported by this configuration.
        /// </summary>
        /// <value>
        /// A string containing the path to the assembly in which the build
        /// component is defined.
        /// </value>
        public override string ComponentPath
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value specifying whether this configuration is displayed or 
        /// visible to the user.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this configuration is visible
        /// and accessible to the user; otherwise it is <see langword="false"/>.
        /// </value>
        public override bool IsBrowsable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a copyright and license notification for the component targeted 
        /// by this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the copyright and license of the component.
        /// </value>
        public override string Copyright
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the description of the component targeted by this configuration.
        /// </summary>
        /// <value>
        /// A string providing a description of the component.
        /// </value>
        /// <remarks>
        /// This must be a plain text, brief and informative.
        /// </remarks>
        public override string Description
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the file name of the documentation explaining the features and
        /// how to use the component.
        /// </summary>
        /// <value>
        /// A string containing the file name of the documentation.
        /// </value>
        /// <remarks>
        /// <para>
        /// This should be either a file name (with file extension, but without
        /// the path) or include a path relative to the assembly containing this
        /// object implementation.
        /// </para>
        /// <para>
        /// The expected file format is HTML, PDF, XPS, CHM and plain text.
        /// </para>
        /// </remarks>
        public override string HelpFileName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the version of the target component.
        /// </summary>
        /// <value>
        /// An instance of <see cref="System.Version"/> specifying the version
        /// of the target component.
        /// </value>
        public override Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            if (this.IsInitialized)
            {
                if (_format == null)
                {
                    this.IsInitialized = false;
                    return;
                }

                _context = context;
                Debug.Assert(context.Settings != null);
                if (context.Settings == null)
                {
                    this.IsInitialized = false;
                    return;
                }
            }
        }

        public void Initialize(BuildContext context, BuildFormat format)
        {
            BuildExceptions.NotNull(format, "format");

            _format = format;

            this.Initialize(context);
        }

        public override void Uninitialize()
        {
            _format  = null;
            _context = null;

            base.Uninitialize();
        }

        /// <summary>
        /// The creates the configuration information or settings required by the
        /// target component for the build process.
        /// </summary>
        /// <param name="group">
        /// A build group, <see cref="BuildGroup"/>, representing the documentation
        /// target for configuration.
        /// </param>
        /// <param name="writer">
        /// An <see cref="XmlWriter"/> object used to create one or more new 
        /// child nodes at the end of the list of child nodes of the current node. 
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> for a successful configuration;
        /// otherwise, it returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The <see cref="XmlWriter"/> writer passed to this configuration object
        /// is created as a new child specifically for this object, and will not
        /// be passed onto other configuration objects.
        /// </remarks>
        public override bool Configure(BuildGroup group, XmlWriter writer)
        {
            BuildExceptions.NotNull(group,  "group");
            BuildExceptions.NotNull(writer, "writer");

            if (!this.Enabled || !this.IsInitialized)
            {
                return false;
            }

            BuildSettings settings = _context.Settings;

            writer.WriteStartElement("options");   // start - options
            writer.WriteAttributeString("locale", 
                settings.CultureInfo.Name.ToLower());
            writer.WriteAttributeString("linkTarget",
                "_" + _format.ExternalLinkTarget.ToString().ToLower());
            writer.WriteEndElement();              // end - options

            List<DataSource> dataSources = new List<DataSource>();

            // For now, lets simply write the default...
            //writer.WriteStartElement("targets");    // start - targets
            //writer.WriteAttributeString("base", @"%DXROOT%\Data\Reflection\");
            //writer.WriteAttributeString("recurse", "true");
            //writer.WriteAttributeString("files", "*.xml");
            //writer.WriteAttributeString("type",
            //    _format.ExternalLinkType.ToString().ToLower());
            //writer.WriteEndElement();               // end - targets
            string dotNetDataDir = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(ReferenceEngine.ReflectionDirectory));

            writer.WriteStartElement("targets");  // start - targets
            writer.WriteAttributeString("base", dotNetDataDir);
            writer.WriteAttributeString("recurse", "true");
            writer.WriteAttributeString("system", "true");
            writer.WriteAttributeString("files", "*.xml");
            writer.WriteAttributeString("type",
                _format.ExternalLinkType.ToString().ToLower());

            // Write the data source...
            this.WriteDataSource(writer, DataSourceType.Framework,
                dotNetDataDir, ReferenceEngine.ReflectionVersion, true,
                false, dataSources);

            writer.WriteEndElement();             // end - targets

            // Write Silverlight framework too...
            Version latestVersion = BuildFrameworks.LatestSilverlightVersion;
            string reflectionDir  = _context.ReflectionDataDirectory;
            if (latestVersion != null && (!String.IsNullOrEmpty(reflectionDir) &&
                Directory.Exists(reflectionDir)))
            {
                string silverlightDir = Path.Combine(reflectionDir,
                    @"Silverlight\v" + latestVersion.ToString(2));

                // If it exits and not empty, we assume the reflection data
                // is already created..
                if (Directory.Exists(silverlightDir) &&
                    !DirectoryUtils.IsDirectoryEmpty(silverlightDir))
                {
                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base", silverlightDir);
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("system",  "true");
                    writer.WriteAttributeString("files",   "*.xml");
                    writer.WriteAttributeString("type",
                        _format.ExternalLinkType.ToString().ToLower());

                    // Write the data source...
                    this.WriteDataSource(writer, DataSourceType.Silverlight,
                        silverlightDir, latestVersion, true, true, dataSources);

                    writer.WriteEndElement();
                }
            }
            latestVersion = null;
            bool isSilverlight = false;

            // Write Blend SDK...
            BuildSpecialSdk latestBlendSdk = null;
            BuildSpecialSdk silverlightBlendSdk = BuildSpecialSdks.LatestBlendSilverlightSdk;
            BuildSpecialSdk wpfBlendSdk = BuildSpecialSdks.LatestBlendWpfSdk;
            if (silverlightBlendSdk != null && wpfBlendSdk != null)
            {
                // In this case we use the latest version, most likely to
                // have improved API...
                latestBlendSdk = wpfBlendSdk;     
                if (silverlightBlendSdk.Version > wpfBlendSdk.Version)
                {
                    isSilverlight  = true;
                    latestBlendSdk = silverlightBlendSdk;
                }
            }
            else if (silverlightBlendSdk != null)
            {
                isSilverlight  = true;
                latestBlendSdk = silverlightBlendSdk;
            }
            else if (wpfBlendSdk != null)
            {
                latestBlendSdk = wpfBlendSdk;
            }

            latestVersion = (latestBlendSdk == null) ?
                null : latestBlendSdk.Version;
            
            if (latestVersion != null && (!String.IsNullOrEmpty(reflectionDir) &&
                Directory.Exists(reflectionDir)))
            {
                string blendDir = null;
                if (isSilverlight)
                {
                    blendDir = Path.Combine(reflectionDir,
                        @"Blend\Silverlight\v" + latestVersion.ToString(2));
                }
                else
                {
                    blendDir = Path.Combine(reflectionDir,
                        @"Blend\Wpf\v" + latestVersion.ToString(2));
                }

                // If it exits and not empty, we assume the reflection data
                // is already created..
                if (Directory.Exists(blendDir) &&
                    !DirectoryUtils.IsDirectoryEmpty(blendDir))
                {
                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base", blendDir);
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("system",  "true");
                    writer.WriteAttributeString("files",   "*.xml");
                    writer.WriteAttributeString("type",
                        _format.ExternalLinkType.ToString().ToLower());

                    // Write the data source...
                    this.WriteDataSource(writer, DataSourceType.Blend,
                        blendDir, latestVersion, true, isSilverlight, dataSources);

                    writer.WriteEndElement();
                }
            }

            // Provide the information for the MSDN link resolvers... 
            writer.WriteStartElement("linkResolver"); // start - linkResolver
            writer.WriteAttributeString("storage", "database");
            writer.WriteAttributeString("cache",   "false");
            if (dataSources != null && dataSources.Count != 0)
            {
                for (int i = 0; i < dataSources.Count; i++)
                {
                    DataSource dataSource = dataSources[i];

                    this.WriteDataSource(writer, dataSource.SourceType,
                        dataSource.InputDir, dataSource.Version,
                        dataSource.IsDatabase, dataSource.IsSilverlight, true);
                }
            }
            writer.WriteEndElement();                 // end - linkResolver

            BuildLinkType linkType = _format.LinkType;
            string linkTypeText = linkType.ToString().ToLower();

            IBuildNamedList<BuildGroupContext> groupContexts = _context.GroupContexts;
            if (groupContexts != null && groupContexts.Count != 0)
            {
                for (int i = 0; i < groupContexts.Count; i++)
                {
                    BuildGroupContext groupContext = groupContexts[i];

                    if (groupContext.GroupType != BuildGroupType.Reference)
                    {
                        continue;
                    }

                    string linkFile = groupContext["$ReflectionFile"];
                    if (!String.IsNullOrEmpty(linkFile))
                    {
                        writer.WriteStartElement("targets");

                        writer.WriteAttributeString("base", @".\");
                        writer.WriteAttributeString("recurse", "false");
                        writer.WriteAttributeString("files",
                            @".\" + groupContext["$ReflectionFile"]);
                        writer.WriteAttributeString("type", linkTypeText);

                        writer.WriteEndElement();
                    }
                }
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void WriteDataSource(XmlWriter writer, DataSourceType sourceType,
            string baseInput, Version version, bool useDatabase,
            bool isSilverlight, IList<DataSource> dataSources)
        {
            this.WriteDataSource(writer, sourceType, baseInput, version,
                useDatabase, isSilverlight, false);

            if (dataSources != null)
            {
                DataSource dataSource = new DataSource(false, true,
                    useDatabase, isSilverlight, sourceType);

                dataSource.Version = version;
                dataSource.InputDir = baseInput;
                dataSources.Add(dataSource);
            }
        }

        private void WriteDataSource(XmlWriter writer, DataSourceType sourceType,
            string baseInput, Version version, bool useDatabase,
            bool isSilverlight, bool isLinks)
        {
            if (baseInput == null)
            {
                baseInput = String.Empty;
            }

            writer.WriteStartElement("source");    // start: source
            writer.WriteAttributeString("system", "true");
            writer.WriteAttributeString("name", sourceType.ToString());
            writer.WriteAttributeString("platform", isSilverlight ?
                "Silverlight" : "Framework");
            writer.WriteAttributeString("version", version != null ?
                version.ToString(2) : "");
            writer.WriteAttributeString("lang", "");
            writer.WriteAttributeString("storage",
                useDatabase ? "database" : "memory");

            writer.WriteStartElement("paths"); // start: paths
            writer.WriteAttributeString("baseInput", Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(baseInput)));
            writer.WriteAttributeString("baseOutput", isLinks ?
                _context.LinksDataDirectory : _context.TargetDataDirectory);
            writer.WriteEndElement();          // end: paths 

            writer.WriteEndElement();              // end: source
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

            string tempText = reader.GetAttribute("name");
            if (String.IsNullOrEmpty(tempText) || !String.Equals(tempText,
                ConfigurationName, StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXml: The current name '{0}' does not match the expected name '{1}'.",
                    tempText, ConfigurationName));
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) &&
                    String.Equals(reader.Name, "property",
                    StringComparison.OrdinalIgnoreCase))
                {
                    switch (reader.GetAttribute("name").ToLower())
                    {
                        case "enabled":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                this.Enabled = Convert.ToBoolean(tempText);
                            }
                            break;
                        default:
                            // Should normally not reach here...
                            throw new NotImplementedException(reader.GetAttribute("name"));
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
            writer.WriteAttributeString("name", ConfigurationName);

            // Write the general properties
            writer.WriteStartElement("propertyGroup"); // start - propertyGroup;
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Enabled", this.Enabled);
            writer.WriteEndElement();                  // end - propertyGroup

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override BuildComponentConfiguration Clone()
        {
            ConceptualReferenceLinkConfiguration options = new ConceptualReferenceLinkConfiguration(this);

            return options;
        }

        #endregion
    }
}
