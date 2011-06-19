using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Mono.Cecil;
using Sandcastle.Contents;

namespace Sandcastle.References
{
    public class ReferenceFilterConfigurator : BuildConfigurator
    {
        #region Public Constant Fields

        public const string KeywordNamer           = "referenceNamer";
        public const string KeywordAddins          = "referenceAddins";
        public const string KeywordOptions         = "referenceOptions";
        public const string KeywordPlatform        = "referencePlatform";
        public const string KeywordResolver        = "referenceResolver";
        public const string KeywordApiFilter       = "referenceApiFilter";
        public const string KeywordAttributeFilter = "referenceAttributeFilter";

        public const string DefaultApiFilterFile   = "MRefApiFilter.xml";

        #endregion

        #region Private Fields

        private string _sourceId;
        private string _sourceFile;
        private string _destFile;
        private string _defaultAttrFile;

        private BuildSettings   _settings;
        private BuildContext    _context;
        private ReferenceGroup  _group;

        private ReferenceRootFilter     _compilerRootFilter;

        private ConfiguratorContent     _configContent;
        [NonSerialized]
        private ReferenceEngineSettings _engineSettings;

        #endregion

        #region Constructors and Destructor

        /// <summary>  
        /// Initializes a new instance of the <see cref="ReferenceFilterConfigurator"/> class.
        /// </summary>
        public ReferenceFilterConfigurator()
        {
            _configContent = new ConfiguratorContent();
        }

        public ReferenceFilterConfigurator(string sourceId)
            : this()
        {
            _sourceId = sourceId;
        }

        #endregion

        #region Public Properties

        public ConfiguratorContent Handlers
        {
            get
            {
                return _configContent;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the current settings of the build process.
        /// </summary>
        /// <value>
        /// A <see cref="BuildSettings"/> specifying the current settings of the 
        /// build process. This is <see langword="null"/> if the
        /// configuration process is not initiated.
        /// </value>
        protected BuildSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(BuildContext context, string defaultAttrFile)
        {
            BuildExceptions.NotNull(context, "context");

            base.Initialize(context.Logger);

            BuildSettings settings = context.Settings;
            if (settings == null)
            {
                this.IsInitialized = false;
                return;
            }
            _engineSettings = (ReferenceEngineSettings)settings.EngineSettings[
                BuildEngineType.Reference];
            Debug.Assert(_engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (_engineSettings == null)
            {
                return;
            }

            _settings        = settings;
            _context         = context;
            _defaultAttrFile = defaultAttrFile;

            if (!String.IsNullOrEmpty(_defaultAttrFile) && 
                File.Exists(_defaultAttrFile))
            {
                string configDir = Path.GetDirectoryName(_defaultAttrFile);
                string apiFilterFile = Path.Combine(configDir, DefaultApiFilterFile);
                if (File.Exists(apiFilterFile))
                {
                    _compilerRootFilter = new ReferenceRootFilter();
                    _compilerRootFilter.Load(apiFilterFile);
                }
            }

            // 1. The reference ...
            this.RegisterItem(KeywordNamer,
                new Action<string, XPathNavigator>(OnNamerItem));
            // 2. The reference ...
            this.RegisterItem(KeywordAddins,
                new Action<string, XPathNavigator>(OnAddinsItem));
            // 3. The reference ...
            this.RegisterItem(KeywordPlatform,
                new Action<string, XPathNavigator>(OnPlatformItem));
            // 4. The reference ...
            this.RegisterItem(KeywordResolver,
                new Action<string, XPathNavigator>(OnResolverItem));
            // 5. The reference ...
            this.RegisterItem(KeywordOptions,
                new Action<string, XPathNavigator>(OnOptionsItem));
            // 6. The reference ...
            this.RegisterItem(KeywordApiFilter,
                new Action<string, XPathNavigator>(OnApiFilterItem));
            // 7. The reference ...
            this.RegisterItem(KeywordAttributeFilter,
                new Action<string, XPathNavigator>(OnAttributeFilterItem));

            this.IsInitialized = true;
        }

        public void Configure(ReferenceGroup group, string sourceFile, 
            string destFile)
        {
            BuildExceptions.NotNull(group, "group");
            BuildExceptions.PathMustExist(sourceFile, "sourceFile");
            BuildExceptions.NotNullNotEmpty(destFile, "destFile");

            if (this.IsInitialized == false)
            {
                throw new BuildException(
                    "The reference filter configurator is not initialized.");
            }

            _group      = group;
            _sourceFile = sourceFile;
            _destFile   = destFile;

            string destDir = Path.GetDirectoryName(destFile);
            if (Directory.Exists(destDir) == false)
            {
                Directory.CreateDirectory(destDir);
            }

            if (this.IsInitialized == false || String.IsNullOrEmpty(_sourceFile) ||
                String.IsNullOrEmpty(_destFile))
            {
                return;
            }

            XmlDocument document = new XmlDocument();
            document.Load(_sourceFile);

            XPathNavigator navigator = document.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select("//SandcastleItem");

            int nodeCount = iterator.Count;

            XPathNavigator[] nodeNavigators = new XPathNavigator[iterator.Count];
            for (int i = 0; i < nodeNavigators.Length; i++)
            {
                iterator.MoveNext();
                nodeNavigators[i] = iterator.Current.Clone();
            }

            string configKeyword = null;
            ConfiguratorItem configItem = null;
            for (int i = 0; i < nodeCount; i++)
            {
                XPathNavigator nodeNavigator = nodeNavigators[i];
                configKeyword = nodeNavigator.GetAttribute("name", String.Empty);
                if (String.IsNullOrEmpty(configKeyword))
                {
                    continue;
                }

                configItem = _configContent[configKeyword];
                if (configItem != null)
                {
                    configItem.Execute(nodeNavigator);
                }
            }

            this.ApplyContents(document);

            document.Save(_destFile);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public ConfiguratorItem GetItem(string keyword)
        {
            if (_configContent == null || String.IsNullOrEmpty(keyword))
            {
                return null;
            }

            return _configContent[keyword];
        }

        public void RegisterItem(string keyword, 
            Action<string, XPathNavigator> handler)
        {
            if (_configContent == null || String.IsNullOrEmpty(keyword) ||
                handler == null)
            {
                return;
            }

            _configContent.Add(new ConfiguratorItem(keyword, handler));
        }

        #endregion

        #region Private Methods

        #region OnNamerItem Method

        private void OnNamerItem(string keyword, XPathNavigator navigator)
        {
            Debug.Assert(_engineSettings != null);
            if (_engineSettings == null)
            {
                return;
            }

            string sandcastleDir = _context.SandcastleDirectory;

            if (_engineSettings.Namer == ReferenceNamer.Whidbey)
            {   
                XmlWriter writer = navigator.InsertAfter();

                //...
                // <namer type="Microsoft.Ddue.Tools.Reflection.OrcasNamer" 
                //   assembly="Microsoft.Ddue.Tools.Reflection.dll" />
                writer.WriteStartElement("namer");
                writer.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.Reflection.WhidbeyNamer");
                writer.WriteAttributeString("assembly", Path.Combine(sandcastleDir,
                    @"ProductionTools\MRefBuilder.exe"));
                writer.WriteEndElement();

                writer.Close();
            }

            navigator.DeleteSelf();
        }

        #endregion

        #region OnAddinsItem Method

        private void OnAddinsItem(string keyword, XPathNavigator navigator)
        {
            //...
            //<addins>
            //  <addin type="Microsoft.Ddue.Tools.XamlAttachedMembersAddIn" 
            //       assembly="MRefBuilder.exe" />
            //</addins>

            string sandcastleDir = _context.SandcastleDirectory;

            XmlWriter writer = navigator.InsertAfter();

            writer.WriteStartElement("addins");  // start: addins

            // For the extension method addin...
            writer.WriteStartElement("addin");   // start: addin
            writer.WriteAttributeString("type", "Microsoft.Ddue.Tools.ExtensionMethodAddIn");
            writer.WriteAttributeString("assembly", Path.Combine(sandcastleDir,
                @"ProductionTools\MRefBuilder.exe"));
            writer.WriteEndElement();            // end: addin

            // For the XAML attached members addin...
            writer.WriteStartElement("addin");   // start: addin
            writer.WriteAttributeString("type", "Microsoft.Ddue.Tools.XamlAttachedMembersAddIn");
            writer.WriteAttributeString("assembly", Path.Combine(sandcastleDir,
                @"ProductionTools\MRefBuilder.exe"));
            writer.WriteEndElement();            // end: addin

            writer.WriteEndElement();            // end: addins

            writer.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnPlatformItem Method

        private void OnPlatformItem(string keyword, XPathNavigator navigator)
        {
            ReferenceGroupContext groupContext = 
                _context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ReferenceGroupContext sourceContext = groupContext;
            if (!String.IsNullOrEmpty(_sourceId))
            {
                sourceContext = groupContext.Contexts[_sourceId] as ReferenceGroupContext;
            }
            if (sourceContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            BuildFramework framework = sourceContext.Framework;
            if (framework == null)
            {
                throw new BuildException("No valid framework is specified.");
            }

            // For Silver, we use the equivalent .NET Framework version, since
            // the current reflection tool does not directly support Silverlight.
            if (framework.FrameworkType.IsSilverlight)
            {
                int major = framework.Version.Major;

                BuildFramework netFramework = null;
                switch (major)
                {
                    case 4:
                        netFramework = BuildFrameworks.GetFramework(
                            major, false);
                        if (netFramework == null)
                        {
                            major = major - 1;
                            goto case 3;
                        }
                        break;
                    case 3:
                        netFramework = BuildFrameworks.GetFramework(
                            major, 5, false);
                        if (netFramework == null)
                        {
                            netFramework = BuildFrameworks.GetFramework(
                                major, false);
                        }
                        if (netFramework == null)
                        {
                            major = major - 1;
                            goto case 2;
                        }
                        break;
                    case 2:
                        netFramework = BuildFrameworks.GetFramework(
                            major, false);
                        break;
                    default:
                        throw new BuildException(
                            "The specified Silverlight version is not supported.");
                }

                if (netFramework == null)
                {
                    throw new BuildException(
                        "The equivalent .NET Framework for the specified Silverlight version cannot be found.");
                }   

                framework = netFramework;
            }

            Version version = framework.Version;

            XmlWriter writer = navigator.InsertAfter();

            // For now, write the default...
            // <platform version="2.0" 
            //   path="%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\" />
            writer.WriteStartElement("platform");
            if (version.Major > 2)
            {
                writer.WriteAttributeString("version", "2.0");
                if (version.Major == 3)
                {
                    Version version2 = BuildFrameworks.GetVersion(2, -1, false);

                    writer.WriteAttributeString("path", String.Format(
                        @"%SystemRoot%\Microsoft.NET\Framework\v{0}\", version2.ToString(3)));
                }
                else
                {
                    writer.WriteAttributeString("path", String.Format(
                        @"%SystemRoot%\Microsoft.NET\Framework\{0}\", framework.Folder));
                }
            }
            else
            {
                writer.WriteAttributeString("version", version.ToString(2));
                writer.WriteAttributeString("path", String.Format(
                    @"%SystemRoot%\Microsoft.NET\Framework\{0}\", framework.Folder));
            }
            writer.WriteEndElement();

            writer.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnResolverItem Method

        private void OnResolverItem(string keyword, XPathNavigator navigator)
        {
            ReferenceGroupContext groupContext =
                _context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ReferenceGroupContext sourceContext = groupContext;
            if (!String.IsNullOrEmpty(_sourceId))
            {
                sourceContext = groupContext.Contexts[_sourceId] as ReferenceGroupContext;
            }
            if (sourceContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string sandcastleDir = _context.SandcastleDirectory;

            string assistComponents = null;
            string sandcastleAssist = _settings.SandAssistDirectory;
            if (!String.IsNullOrEmpty(sandcastleAssist) &&
                Directory.Exists(sandcastleAssist))
            {
                // If the Sandcastle Assist component assembly is in the same 
                // directory as the Sandcastle Helpers...
                string tempText = Path.Combine(sandcastleAssist,
                    "Sandcastle.Components.dll");
                if (File.Exists(tempText))
                {
                    assistComponents = tempText;
                }
            }

            XmlWriter writer = navigator.InsertAfter();

            // For now, write the default...
            // <resolver type="Microsoft.Ddue.Tools.Reflection.AssemblyResolver" 
            //   assembly="%DXROOT%\ProductionTools\MRefBuilder.exe" use-gac="false" />
            writer.WriteStartElement("resolver");  

            if (!String.IsNullOrEmpty(assistComponents) &&
                File.Exists(assistComponents))
            {
                writer.WriteAttributeString("type",
                    "Sandcastle.Reflections.RedirectAssemblyResolver");
                writer.WriteAttributeString("assembly", assistComponents);
                writer.WriteAttributeString("use-gac", "false");

                IList<DependencyItem> items = sourceContext.BindingRedirects;

                if (items != null && items.Count != 0)
                {
                    writer.WriteStartElement("bindingRedirects");  
                    for (int i = 0; i < items.Count; i++)
                    {
                        DependencyItem item = items[i];
                        if (item.IsRedirected)
                        {
                            writer.WriteStartElement("bindingRedirect");

                            string strongName = item.StrongName;
                            if (String.IsNullOrEmpty(strongName))
                            {
                                AssemblyDefinition itemAssembly = AssemblyDefinition.ReadAssembly(
                                   item.Location);

                                strongName = itemAssembly.FullName;
                            }
                            writer.WriteAttributeString("from", item.RedirectStrongName);
                            writer.WriteAttributeString("to", strongName);

                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.Reflection.AssemblyResolver");
                writer.WriteAttributeString("assembly", Path.Combine(sandcastleDir,
                    @"ProductionTools\MRefBuilder.exe"));
                writer.WriteAttributeString("use-gac", "false");
            }
            
            writer.WriteEndElement();

            writer.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnOptionsItem Method

        private void OnOptionsItem(string keyword, XPathNavigator navigator)
        {
            //<!-- Whether to include protected sealed members -->
            //<protectedSealed expose="false" /> 
            //<!-- Whether to include "no-PIA" COM types, aka types marked 
            //     with TypeIdentifierAttribute and CompilerGeneratedAttribute -->
            //<noPIA expose="false" />  

            string sandcastleDir = _context.SandcastleDirectory;

            XmlWriter writer = navigator.InsertAfter();

            // For the extension method addin...
            writer.WriteComment(" Whether to include protected sealed members ");
            writer.WriteStartElement("protectedSealed");   // start: protectedSealed
            writer.WriteAttributeString("expose", "true");
            writer.WriteEndElement();                      // end: protectedSealed

            // For the XAML attached members addin...
            writer.WriteComment(
                " Whether to include \"no-PIA\" COM types, aka types marked with TypeIdentifierAttribute and CompilerGeneratedAttribute ");
            writer.WriteStartElement("noPIA");   // start: noPIA
            writer.WriteAttributeString("expose", "false");
            writer.WriteEndElement();            // end: noPIA

            writer.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnApiFilterItem Method

        private void OnApiFilterItem(string keyword, XPathNavigator navigator)
        {
            XmlWriter writer = navigator.InsertAfter();

            // <apiFilter expose="true">
            //   <namespace name="System" expose="true">
            //     <type name="Object" expose="false">
            //       <member name="ToString" expose="true" />
            //     </type>
            //   </namespace>
            // </apiFilter>
            writer.WriteStartElement("apiFilter");
            writer.WriteAttributeString("expose", "true");

            // Handle compiler and Generator outputs
            if (_compilerRootFilter != null && _compilerRootFilter.Count != 0)
            {
                int itemNamespaces = _compilerRootFilter.Count;
                for (int i = 0; i < itemNamespaces; i++)
                {
                    ReferenceNamespaceFilter namespaceFilter = _compilerRootFilter[i];
                    namespaceFilter.WriteXml(writer);
                }
            }

            ReferenceRootFilter typeFilters = _group.TypeFilters;

            if (typeFilters != null && typeFilters.Count != 0)
            {
                int itemNamespaces = typeFilters.Count;
                for (int i = 0; i < itemNamespaces; i++)
                {
                    ReferenceNamespaceFilter namespaceFilter = typeFilters[i];
                    namespaceFilter.WriteXml(writer);
                }
            }

            writer.WriteEndElement();

            writer.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnAttributeFilterItem Method

        private void OnAttributeFilterItem(string keyword, XPathNavigator navigator)
        {
            ReferenceRootFilter attributeFilters = _group.AttributeFilters;

            // If there is customization of the attribute filters, we use it...
            if (attributeFilters != null && attributeFilters.Count != 0)
            {
                XmlWriter writer = navigator.InsertAfter();

                //<attributeFilter expose="true">
                //  <namespace name="System.Diagnostics" expose="false">
                //    <type name="ConditionalAttribute" expose="true" />
                //  </namespace>
                //  <namespace name="System.Xml.Serialization" expose="false" />
                //</attributeFilter>
                writer.WriteStartElement("attributeFilter");
                writer.WriteAttributeString("expose", "true");

                int itemNamespaces = attributeFilters.Count;
                for (int i = 0; i < itemNamespaces; i++)
                {
                    ReferenceNamespaceFilter namespaceFilter = attributeFilters[i];
                    if (namespaceFilter == null)
                    {
                        continue;
                    }
                    namespaceFilter.WriteXml(writer);
                }

                writer.WriteEndElement();

                writer.Close();
            }
            else  //...otherwise, we put in the default atttribute filters...
            {   
                // Write the defaults...
                if (String.IsNullOrEmpty(_defaultAttrFile) == false &&
                    File.Exists(_defaultAttrFile))
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreWhitespace  = true;
                    settings.IgnoreComments    = false;
                    settings.CloseInput        = true;

                    using (XmlReader xmlReader = XmlReader.Create(
                        _defaultAttrFile, settings))
                    {
                        if (xmlReader.IsStartElement("attributeFilter"))
                        {
                            navigator.InsertAfter(xmlReader);
                        }
                    }
                }
            }

            navigator.DeleteSelf();
        }

        #endregion

        #endregion
    }
}
