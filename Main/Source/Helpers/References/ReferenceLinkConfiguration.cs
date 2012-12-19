using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;
using Sandcastle.ReflectionData;

namespace Sandcastle.References
{
    using ConceptualEngineSettings    = Sandcastle.Conceptual.ConceptualEngineSettings;
    using ConceptualLinkConfiguration = Sandcastle.Conceptual.ConceptualLinkConfiguration;

    [Serializable]
    public sealed class ReferenceLinkConfiguration : ReferenceComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceLinkConfiguration";

        #endregion

        #region Private Fields

        private bool _cacheLinks;
        private BuildCacheStorageType _linkStorage;

        [NonSerialized]
        private BuildFormat   _format;
        [NonSerialized]
        private BuildSettings _settings;
        [NonSerialized]
        private BuildContext _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceLinkConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLinkConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceLinkConfiguration()
        {
            _cacheLinks  = true;
            _linkStorage = BuildCacheStorageType.Database;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLinkConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceLinkConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceLinkConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceLinkConfiguration(ReferenceLinkConfiguration source)
            : base(source)
        {
            _cacheLinks  = source._cacheLinks;
            _linkStorage = source._linkStorage;
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
        /// The value is <see cref="ReferenceLinkConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceLinkConfiguration.ConfigurationName;
            }
        }

        /// <summary>
        /// Gets a value specifying whether this options category is active, and should
        /// be process.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this options category enabled and userable 
        /// in the build process; otherwise, it is <see langword="false"/>.
        /// </value>
        public override bool IsActive
        {
            get
            {
                return base.IsActive;
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

        /// <summary>
        /// Gets or sets a value specifying whether to cache external
        /// reference links, which are links to the <c>MSDN</c> topics.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if there is external link caching;
        /// otherwise, it is <see langword="false"/>. The default is
        /// <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// This is only used if the storage type of the external links, 
        /// <see cref="ExternalLinkStorage"/>, is <see cref="BuildCacheStorageType.Database"/>.
        /// </remarks>
        public bool ExternalLinkCaching
        {
            get
            {
                return _cacheLinks;
            }
            set
            {
                _cacheLinks = value;
            }
        }

        /// <summary>
        /// Gets or sets the storage type of the external topic links, which 
        /// are links to the <c>MSDN</c> reference topics.
        /// </summary>
        /// <value>
        /// A structure specifying the storage type for the external links.
        /// The default value is <see cref="BuildCacheStorageType.Database"/>.
        /// </value>
        /// <remarks>
        /// This property supports only the <see cref="BuildCacheStorageType.Database"/>
        /// and <see cref="BuildCacheStorageType.Memory"/> values.
        /// </remarks>
        public BuildCacheStorageType ExternalLinkStorage
        {
            get
            {                 
                return _linkStorage;
            }
            set
            {
                if (value != BuildCacheStorageType.Memory &&
                    value != BuildCacheStorageType.Database)
                {
                    _linkStorage = value;
                }
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

                _settings = context.Settings;
                Debug.Assert(_settings != null);
                if (_settings == null)
                {
                    this.IsInitialized = false;
                    return;
                }

                _context = context;
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
            _format   = null;
            _settings = null;
            _context  = null;

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
        /// may be passed on to other configuration objects, so do not close or 
        /// dispose it.
        /// </remarks>
        public override bool Configure(BuildGroup group, XmlWriter writer)
        {
            BuildExceptions.NotNull(group, "group");
            BuildExceptions.NotNull(writer, "writer");

            IBuildNamedList<BuildGroupContext> groupContexts = _context.GroupContexts;
            if (groupContexts == null || groupContexts.Count == 0)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ReferenceGroupContext groupContext = groupContexts[group.Id] 
                as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            if (!this.Enabled || !this.IsInitialized)
            {
                return false;
            }

            BuildFramework framework = groupContext.Framework;
            BuildFrameworkKind kind  = framework.FrameworkType.Kind;

            ReferenceEngineSettings referenceSettings = _settings.EngineSettings[
                BuildEngineType.Reference] as ReferenceEngineSettings;
            BuildSpecialSdkType webSdkType = referenceSettings.WebMvcSdkType;

            writer.WriteStartElement("options");   // start - options
            writer.WriteAttributeString("locale", 
                _settings.CultureInfo.Name.ToLower());
            if (kind == BuildFrameworkKind.Silverlight)
            {
                writer.WriteAttributeString("version", "VS.95");
            }
            else if (kind == BuildFrameworkKind.Compact)
            {
                // The framework 3.5 is the last version of Windows CE...
                writer.WriteAttributeString("version", "VS.90");
            }
            if (webSdkType != BuildSpecialSdkType.Null &&
                webSdkType != BuildSpecialSdkType.None)
            {
                switch (webSdkType.Value)
                {
                    case 10:    // ASP.NET MVC 1.0: Supported in .NET 3.5
                        writer.WriteAttributeString("mvcVersion", "VS.90");
                        break;
                    case 20:    // ASP.NET MVC 2.0: Supported in .NET 3.5 SP1
                        // This is currently the default documentation for
                        // the ASP.NET MVC Framework...
                        writer.WriteAttributeString("mvcVersion", "");
                        break;
                    case 30:    // ASP.NET MVC 3.0: Supported in .NET 4.0
                        writer.WriteAttributeString("mvcVersion", "VS.98");
                        break;
                    case 40:    // ASP.NET MVC 4.0: Supported in .NET 4.5
                        writer.WriteAttributeString("mvcVersion", "VS.108");
                        break;
                }
            }
            writer.WriteAttributeString("linkTarget",
                "_" + _format.ExternalLinkTarget.ToString().ToLower());
            writer.WriteEndElement();              // end - options

            bool isEmbeddedGroup    = false;
            bool frameworkAvailable = false;
            bool isEmbeddedScript   = groupContext.IsEmbeddedGroup;
            string tempText = _context["$EmbeddedScriptSharp"];
            if (!String.IsNullOrEmpty(tempText))
            {
                isEmbeddedScript = Convert.ToBoolean(tempText);
            }

            List<DataSource> dataSources = new List<DataSource>();

            if (kind == BuildFrameworkKind.Silverlight)
            {
                string silverlightDir = groupContext["$SilverlightDataDir"];

                if (!String.IsNullOrEmpty(silverlightDir) &&
                    Directory.Exists(silverlightDir))
                {
                    frameworkAvailable = true;

                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base",    silverlightDir);
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("system",  "true");
                    writer.WriteAttributeString("files",   "*.xml");
                    writer.WriteAttributeString("type",
                        _format.ExternalLinkType.ToString().ToLower());

                    // Write the data source...
                    Version latestVersion = BuildFrameworks.LatestSilverlightVersion;
                    if (latestVersion == null)
                    {
                        latestVersion = framework.Version;
                    }
                    this.WriteDataSource(writer, DataSourceType.Silverlight,
                        silverlightDir, latestVersion, true, true, dataSources);

                    writer.WriteEndElement();
                }
            }    
            else if (kind == BuildFrameworkKind.Portable)
            {
                string portableDir = groupContext["$PortableDataDir"];

                if (!String.IsNullOrEmpty(portableDir) && Directory.Exists(portableDir))
                {
                    frameworkAvailable = true;

                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base",    portableDir);
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("system",  "true");
                    writer.WriteAttributeString("files",   "*.xml");
                    writer.WriteAttributeString("type",
                        _format.ExternalLinkType.ToString().ToLower());

                    // Write the data source...
                    Version latestVersion = BuildFrameworks.LatestPortableVersion;
                    if (latestVersion == null)
                    {
                        latestVersion = framework.Version;
                    }
                    this.WriteDataSource(writer, DataSourceType.Portable,
                        portableDir, latestVersion, true, false, dataSources);

                    writer.WriteEndElement();
                }
            }
            else if (kind == BuildFrameworkKind.ScriptSharp)
            {
                string scriptSharpDir = groupContext["$ScriptSharpDataDir"];

                if (!String.IsNullOrEmpty(scriptSharpDir) && Directory.Exists(scriptSharpDir))
                {
                    frameworkAvailable = true;

                    if (!isEmbeddedGroup)
                    {
                        writer.WriteStartElement("targets");
                        writer.WriteAttributeString("base", scriptSharpDir);
                        writer.WriteAttributeString("recurse", "false");
                        writer.WriteAttributeString("system", "true");
                        writer.WriteAttributeString("files", "*.xml");

                        if (isEmbeddedScript)
                        {
                            writer.WriteAttributeString("type",
                                BuildLinkType.Local.ToString().ToLower());
                        }
                        else
                        {
                            writer.WriteAttributeString("type",
                                BuildLinkType.None.ToString().ToLower());
                        }

                        // Write the data source...
                        Version latestVersion = BuildFrameworks.LatestScriptSharpVersion;
                        if (latestVersion == null)
                        {
                            latestVersion = framework.Version;
                        }
                        this.WriteDataSource(writer, DataSourceType.ScriptSharp,
                            scriptSharpDir, latestVersion, true, false, dataSources);

                        writer.WriteEndElement();
                    }
                }
            }

            // If not specialized framework, then write the default... 
            if (!frameworkAvailable || kind == BuildFrameworkKind.None ||
                kind == BuildFrameworkKind.DotNet || kind == BuildFrameworkKind.Compact)
            {
                string dotNetDataDir = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(ReferenceEngine.ReflectionDirectory));

                writer.WriteStartElement("targets");
                writer.WriteAttributeString("base", dotNetDataDir);
                writer.WriteAttributeString("recurse", "true");
                writer.WriteAttributeString("system",  "true");
                writer.WriteAttributeString("files",   "*.xml");
                writer.WriteAttributeString("type",
                    _format.ExternalLinkType.ToString().ToLower());
                
                // Write the data source...
                this.WriteDataSource(writer, DataSourceType.Framework,
                    dotNetDataDir, ReferenceEngine.ReflectionVersion, true, 
                    false, dataSources); 

                writer.WriteEndElement();
            } 

            // The Portable and ScriptSharp do not support Blend...
            if (kind != BuildFrameworkKind.Portable &&
                kind != BuildFrameworkKind.Compact  &&
                kind != BuildFrameworkKind.ScriptSharp)
            {
                string blendDir = groupContext["$BlendDataDir"];

                if (!String.IsNullOrEmpty(blendDir) && Directory.Exists(blendDir))
                {
                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base",    blendDir);
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("system",  "true");
                    writer.WriteAttributeString("files",   "*.xml");
                    writer.WriteAttributeString("type",
                        _format.ExternalLinkType.ToString().ToLower());

                    // Write the data source...
                    BuildSpecialSdk latestBlendSdk = null;
                    if (kind == BuildFrameworkKind.Silverlight)
                    {
                        latestBlendSdk = BuildSpecialSdks.LatestBlendSilverlightSdk;
                    }
                    else
                    {
                        latestBlendSdk = BuildSpecialSdks.LatestBlendWpfSdk;
                    }
                    Version latestVersion = (latestBlendSdk == null) ? 
                        null : latestBlendSdk.Version;
                    if (latestVersion == null)
                    {
                        latestVersion = framework.Version;
                    }
                    this.WriteDataSource(writer, DataSourceType.Blend, blendDir,
                        latestVersion, true, kind == BuildFrameworkKind.Silverlight, 
                        dataSources);

                    writer.WriteEndElement();
                }
            }

            IList<string> linkDirs = _context.GetValue(
                "$ReferenceLinkDirectories") as IList<string>;
            IList<ReferenceLinkSource> linkSources = _context.GetValue(
                "$ReferenceLinkSources") as IList<ReferenceLinkSource>;
            if ((linkDirs != null && linkDirs.Count != 0) &&
                (linkSources != null && linkSources.Count == linkDirs.Count))
            {
                for (int i = 0; i < linkDirs.Count; i++)
                {
                    ReferenceLinkSource linkSource = linkSources[i];
                    BuildLinkType sourceLinkType = linkSource.LinkType;

                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base",    linkDirs[i]);
                    writer.WriteAttributeString("recurse", "true");
                    writer.WriteAttributeString("system",  "false");
                    writer.WriteAttributeString("files",   "*.xml");
                    writer.WriteAttributeString("type",
                        sourceLinkType.ToString().ToLower());

                    writer.WriteEndElement();
                }
            }

            BuildLinkType linkType = _format.LinkType;
            string linkTypeText = linkType.ToString().ToLower();

            // For the embedded group, we will not link to the other groups...
            if (!isEmbeddedGroup)
            {
                for (int i = 0; i < groupContexts.Count; i++)
                {
                    ReferenceGroupContext aContext = groupContexts[i]
                        as ReferenceGroupContext;

                    if (aContext == null || aContext.IsLinkGroup)
                    {
                        continue;
                    }

                    if (aContext.GroupType != BuildGroupType.Reference ||
                        aContext == groupContext)
                    {
                        continue;
                    }

                    string linkFile = aContext["$ReflectionFile"];
                    if (!String.IsNullOrEmpty(linkFile))
                    {
                        writer.WriteStartElement("targets");

                        writer.WriteAttributeString("base", @".\");
                        writer.WriteAttributeString("recurse", "false");
                        writer.WriteAttributeString("system", "false");
                        writer.WriteAttributeString("files", @".\" + linkFile);
                        writer.WriteAttributeString("type", linkTypeText);
                        writer.WriteEndElement();
                    }
                }
            }

            //<targets base=".\" recurse="false"  
            //   files=".\reflection.xml" type="local" />        
            writer.WriteStartElement("targets");
            writer.WriteAttributeString("base",    @".\");
            writer.WriteAttributeString("recurse", "false");
            writer.WriteAttributeString("system",  "false");
            writer.WriteAttributeString("files", @".\" + groupContext["$ReflectionFile"]);

            if (isEmbeddedGroup)
            {
                writer.WriteAttributeString("type",
                    BuildLinkType.Local.ToString().ToLower());
            }
            else
            {
                writer.WriteAttributeString("type", linkTypeText);
            }
            
            writer.WriteEndElement();

            // Provide the information for the MSDN link resolvers... 
            writer.WriteStartElement("linkResolver"); // start - linkResolver
            writer.WriteAttributeString("storage", _linkStorage.ToString().ToLower()); 
            writer.WriteAttributeString("cache",   _cacheLinks ? "true" : "false");     
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

            // Finally, provide the information for the conceptual links
            // in reference documents, if any...
            bool hasConceptualContext = _settings.BuildConceptual;
            if (hasConceptualContext)
            {
                hasConceptualContext = false;

                for (int i = 0; i < groupContexts.Count; i++)
                {
                    BuildGroupContext aContext = groupContexts[i];
                    if (aContext.GroupType == BuildGroupType.Conceptual)
                    {
                        hasConceptualContext = true;
                        break;
                    }
                }
            }

            if (hasConceptualContext)
            {     
                ConceptualEngineSettings conceptualSettings = _settings.EngineSettings[
                    BuildEngineType.Conceptual] as ConceptualEngineSettings;
                Debug.Assert(conceptualSettings != null,
                    "The settings does not include the reference engine settings.");
                if (conceptualSettings == null)
                {
                    return false;
                }
                ConceptualLinkConfiguration linkConfig = 
                    conceptualSettings.ConceptualLinks;
                Debug.Assert(linkConfig != null,
                    "There is no conceptual link configuration available.");
                if (linkConfig == null)
                {
                    return false;
                }

                writer.WriteStartElement("conceptualLinks");  //start: conceptualLinks
                writer.WriteAttributeString("enabled", "true");
                writer.WriteAttributeString("showText",
                    linkConfig.ShowLinkText.ToString());
                writer.WriteAttributeString("showBrokenLinkText",
                    linkConfig.ShowBrokenLinkText.ToString());
                writer.WriteAttributeString("type", linkTypeText);

                for (int i = 0; i < groupContexts.Count; i++)
                {
                    BuildGroupContext aContext = groupContexts[i];
                    if (aContext.GroupType == BuildGroupType.Conceptual)
                    {
                        writer.WriteStartElement("conceptualTargets");  // start - conceptualTargets
                        writer.WriteAttributeString("base", String.Format(
                            @".\{0}", aContext["$DdueXmlCompDir"]));
                        writer.WriteAttributeString("type", linkTypeText);
                        writer.WriteEndElement();                       // end - conceptualTargets
                    }
                }
                writer.WriteEndElement();                     //end: conceptualLinks
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

                dataSource.Version  = version;
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
                        case "externallinkcaching":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _cacheLinks = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "externallinkstorage":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _linkStorage = BuildCacheStorageType.Parse(tempText);

                                if (_linkStorage != BuildCacheStorageType.Memory &&
                                    _linkStorage != BuildCacheStorageType.Database)
                                {
                                    // If not one of the valid values, reset it...
                                    _linkStorage = BuildCacheStorageType.Database;
                                }
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
            writer.WritePropertyElement("Enabled",             this.Enabled);
            writer.WritePropertyElement("ExternalLinkCaching", _cacheLinks);
            writer.WritePropertyElement("ExternalLinkStorage", _linkStorage.ToString());
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
            ReferenceLinkConfiguration options = new ReferenceLinkConfiguration(this);

            return options;
        }

        #endregion
    }
}
