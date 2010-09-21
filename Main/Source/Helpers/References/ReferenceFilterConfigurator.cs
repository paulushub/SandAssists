using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Sandcastle.Configurators;

namespace Sandcastle.References
{
    public class ReferenceFilterConfigurator : BuildConfigurator
    {
        #region Public Constant Fields

        public const string KeywordNamer           = "referenceNamer";
        public const string KeywordAddins          = "referenceAddins";
        public const string KeywordPlatform        = "referencePlatform";
        public const string KeywordResolver        = "referenceResolver";
        public const string KeywordApiFilter       = "referenceApiFilter";
        public const string KeywordAttributeFilter = "referenceAttributeFilter";

        #endregion

        #region Private Fields

        private string _sourceFile;
        private string _destFile;
        private string _defaultAttrFile;

        private BuildSettings   _settings;
        private BuildContext    _context;
        private ReferenceGroup  _group;

        private ConfiguratorContent _configContent;

        #endregion

        #region Constructors and Destructor

        /// <summary>  
        /// Initializes a new instance of the <see cref="ReferenceFilterConfigurator"/> class.
        /// </summary>
        public ReferenceFilterConfigurator()
        {
            _configContent = new ConfiguratorContent();
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

            _settings        = settings;
            _context         = context;
            _defaultAttrFile = defaultAttrFile;

            // 1. The reference ...
            this.RegisterItem(KeywordNamer,
                new ConfigurationItemHandler(OnNamerItem));
            // 2. The reference ...
            this.RegisterItem(KeywordAddins,
                new ConfigurationItemHandler(OnAddinsItem));
            // 3. The reference ...
            this.RegisterItem(KeywordPlatform,
                new ConfigurationItemHandler(OnPlatformItem));
            // 4. The reference ...
            this.RegisterItem(KeywordResolver,
                new ConfigurationItemHandler(OnResolverItem));
            // 5. The reference ...
            this.RegisterItem(KeywordApiFilter,
                new ConfigurationItemHandler(OnApiFilterItem));
            // 6. The reference ...
            this.RegisterItem(KeywordAttributeFilter,
                new ConfigurationItemHandler(OnAttributeFilterItem));

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

        public void RegisterItem(string keyword, ConfigurationItemHandler handler)
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

        private void OnNamerItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;
            ReferenceOptions options = _group.Options;

            string sandcastleDir = _context.SandcastleDirectory;

            if (options.Namer == ReferenceNamer.Whidbey)
            {   
                XmlWriter xmlWriter = navigator.InsertAfter();

                //...
                // <namer type="Microsoft.Ddue.Tools.Reflection.OrcasNamer" 
                //   assembly="Microsoft.Ddue.Tools.Reflection.dll" />
                xmlWriter.WriteStartElement("namer");
                xmlWriter.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.Reflection.WhidbeyNamer");
                xmlWriter.WriteAttributeString("assembly", Path.Combine(sandcastleDir,
                    @"ProductionTools\MRefBuilder.exe"));
                xmlWriter.WriteEndElement();

                xmlWriter.Close();
            }

            navigator.DeleteSelf();
        }

        #endregion

        #region OnAddinsItem Method

        private void OnAddinsItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

            //XmlWriter xmlWriter = navigator.InsertAfter();

            //...
            //<addins>
            //  <addin type="Microsoft.Ddue.Tools.XamlAttachedMembersAddIn" 
            //       assembly="MRefBuilder.exe" />
            //</addins>

            //xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnPlatformItem Method

        private void OnPlatformItem(object sender, ConfigurationItemEventArgs args)
        {
            BuildFramework framework = _settings.Framework;
            XPathNavigator navigator = args.Navigator;

            XmlWriter xmlWriter = navigator.InsertAfter();

            // For now, write the default...
            // <platform version="2.0" 
            //   path="%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\" />
            xmlWriter.WriteStartElement("platform");
            xmlWriter.WriteAttributeString("version", framework.Version.ToString(2));
            xmlWriter.WriteAttributeString("path", String.Format(
                @"%SystemRoot%\Microsoft.NET\Framework\{0}\", framework.Folder));
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnResolverItem Method

        private void OnResolverItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

            string sandcastleDir = _context.SandcastleDirectory;

            XmlWriter xmlWriter = navigator.InsertAfter();

            // For now, write the default...
            // <resolver type="Microsoft.Ddue.Tools.Reflection.AssemblyResolver" 
            //   assembly="%DXROOT%\ProductionTools\MRefBuilder.exe" use-gac="false" />
            xmlWriter.WriteStartElement("resolver");
            xmlWriter.WriteAttributeString("type", 
                "Microsoft.Ddue.Tools.Reflection.AssemblyResolver");
            xmlWriter.WriteAttributeString("assembly", Path.Combine(sandcastleDir,
                @"ProductionTools\MRefBuilder.exe"));
            xmlWriter.WriteAttributeString("use-gac", "false");
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnApiFilterItem Method

        private void OnApiFilterItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

            XmlWriter xmlWriter = navigator.InsertAfter();

            // <apiFilter expose="true">
            //   <namespace name="System" expose="true">
            //     <type name="Object" expose="false">
            //       <member name="ToString" expose="true" />
            //     </type>
            //   </namespace>
            // </apiFilter>
            xmlWriter.WriteStartElement("apiFilter");
            xmlWriter.WriteAttributeString("expose", "true");
            ReferenceRootFilter typeFilters = _group.TypeFilters;

            if (typeFilters != null && typeFilters.Count != 0)
            {
                int itemNamespaces = typeFilters.Count;
                for (int i = 0; i < itemNamespaces; i++)
                {
                    ReferenceNamespaceFilter namespaceFilter = typeFilters[i];
                    if (namespaceFilter == null)
                    {
                        continue;
                    }
                    namespaceFilter.WriteXml(xmlWriter);
                }
            }

            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnAttributeFilterItem Method

        private void OnAttributeFilterItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

            ReferenceRootFilter attributeFilters = _group.AttributeFilters;

            // If there is customization of the attribute filters, we use it...
            if (attributeFilters != null && attributeFilters.Count != 0)
            {
                XmlWriter xmlWriter = navigator.InsertAfter();

                //<attributeFilter expose="true">
                //  <namespace name="System.Diagnostics" expose="false">
                //    <type name="ConditionalAttribute" expose="true" />
                //  </namespace>
                //  <namespace name="System.Xml.Serialization" expose="false" />
                //</attributeFilter>
                xmlWriter.WriteStartElement("attributeFilter");
                xmlWriter.WriteAttributeString("expose", "true");

                int itemNamespaces = attributeFilters.Count;
                for (int i = 0; i < itemNamespaces; i++)
                {
                    ReferenceNamespaceFilter namespaceFilter = attributeFilters[i];
                    if (namespaceFilter == null)
                    {
                        continue;
                    }
                    namespaceFilter.WriteXml(xmlWriter);
                }

                xmlWriter.WriteEndElement();

                xmlWriter.Close();
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
