using System;
using System.IO;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using System.Text;
using System.Text.RegularExpressions;

using Sandcastle.Contents;

namespace Sandcastle.Configurators
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for configuration handlers used in
    /// editing or modifying the build assembler tool configuration files.
    /// </summary>
    public abstract class AssemblerConfigurator : BuildConfigurator
    {
        #region Private Fields

        private Regex _reIsConfigValue;

        private string _sourceFile;
        private string _destFile;
        private string _sandcastleDir;
        private string _sandcastleAssistDir;

        private BuildGroup _group;

        private BuildContext _context;

        private BuildProperties     _dicConfigMap;
        private ConfiguratorContent _configContent;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblerConfigurator"/> class.
        /// </summary>
        protected AssemblerConfigurator()
        {
            _reIsConfigValue = new Regex(@"^\$\(([^\$\(\)]*)\)$", 
                RegexOptions.Compiled);
            // Create the dictionary for mapping the handlers...
            _dicConfigMap  = new BuildProperties();
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
                if (_context != null)
                {
                    return _context.Settings;
                }

                return null;
            }
        }

        protected BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            base.Initialize(context.Logger);

            BuildSettings settings = context.Settings;
            if (settings == null)
            {
                this.IsInitialized = false;
                return;
            }

            _context = context;

            _sandcastleDir       = context.SandcastleDirectory;
            _sandcastleAssistDir = settings.SandAssistDirectory;

            // Make sure the default component handlers are added...
            //Keyword: "$(SandcastleComponent)";
            if (!_dicConfigMap.ContainsKey("SandcastleComponent"))
            {
                string sandcastleComponents = null;
                if (context.IsDirectSandcastle)
                {
                    if (!String.IsNullOrEmpty(_sandcastleAssistDir) ||
                        Directory.Exists(_sandcastleAssistDir))
                    {
                        sandcastleComponents = Path.Combine(_sandcastleAssistDir,
                            "Sandcastle.BuildAssembler.dll");
                    }    
                }
                else
                {
                    if (!String.IsNullOrEmpty(_sandcastleDir) ||
                        Directory.Exists(_sandcastleDir))
                    {
                        sandcastleComponents = Path.Combine(_sandcastleDir,
                            @"ProductionTools\BuildComponents.dll");
                    }
                }
                if (!String.IsNullOrEmpty(sandcastleComponents) &&
                    File.Exists(sandcastleComponents))
                {
                    _dicConfigMap.Add("SandcastleComponent", sandcastleComponents);
                }
            }

            //Keyword: "$(SandAssistComponent)";
            if (_dicConfigMap.ContainsKey("SandAssistComponent") == false)
            {
                if (String.IsNullOrEmpty(_sandcastleAssistDir) == false ||
                    Directory.Exists(_sandcastleAssistDir))
                {
                    // If the Sandcastle Assist component assembly is in the same 
                    // directory as the Sandcastle Helpers...
                    string assistComponents = Path.Combine(_sandcastleAssistDir,
                        "Sandcastle.Components.dll");
                    if (File.Exists(assistComponents))
                    {
                        _dicConfigMap.Add("SandAssistComponent", assistComponents);
                    }
                    else
                    {
                        // Otherwise, if in the "Components" sub-directory...
                        assistComponents = Path.Combine(_sandcastleAssistDir,
                            @"Components\Sandcastle.Components.dll");
                        if (File.Exists(assistComponents))
                        {
                            _dicConfigMap.Add("SandAssistComponent", assistComponents);
                        }
                    }
                }
            }

            this.IsInitialized = true;
        }

        public virtual void Configure(BuildGroup buildGroup, 
            string sourceFile, string destFile)
        {
            BuildExceptions.NotNull(buildGroup, "buildGroup");
            BuildExceptions.PathMustExist(sourceFile, "sourceFile");
            BuildExceptions.NotNullNotEmpty(destFile, "destFile");

            if (this.IsInitialized == false)
            {
                throw new BuildException("The configurator is not initialized.");
            }
            if (_dicConfigMap == null || _configContent == null)
            {
                throw new BuildException("There is not initialization.");
            }

            _group      = buildGroup;
            _sourceFile = sourceFile;
            _destFile   = destFile;
            string destDir = Path.GetDirectoryName(destFile);
            if (Directory.Exists(destDir) == false)
            {
                Directory.CreateDirectory(destDir);
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public virtual ConfiguratorItem GetConfigurationItem(string keyword)
        {
            if (_configContent == null || String.IsNullOrEmpty(keyword))
            {
                return null;
            }

            return _configContent[keyword];
        }

        public virtual string GetComponents(string keyword)
        {
            if (_dicConfigMap == null || String.IsNullOrEmpty(keyword))
            {
                return null;
            }

            return _dicConfigMap[keyword];
        }

        public virtual bool ContainsComponents(string keyword)
        {
            if (_dicConfigMap == null || _dicConfigMap.Count == 0 
                || String.IsNullOrEmpty(keyword))
            {
                return false;
            }

            return _dicConfigMap.ContainsKey(keyword);
        }

        public virtual void RegisterConfigurationItem(string keyword,
            Action<string, XPathNavigator> handler)
        {
            if (_configContent == null || String.IsNullOrEmpty(keyword) ||
                handler == null)
            {
                return;
            }

            _configContent.Add(new ConfiguratorItem(keyword, handler));
        }

        public virtual void RegisterComponents(string keyword, string assemblyFile)
        {
            if (_dicConfigMap == null || String.IsNullOrEmpty(keyword) ||
                String.IsNullOrEmpty(assemblyFile))
            {
                return;
            }

            _dicConfigMap[keyword] = assemblyFile;
        }

        #endregion

        #region Protected Methods

        protected abstract void OnComponentInclude(
            string keyword, XPathNavigator navigator);

        #region Configure Method

        protected virtual void Configure()
        {
            if (this.IsInitialized == false || String.IsNullOrEmpty(_sourceFile) ||
                String.IsNullOrEmpty(_destFile))
            {
                return;
            }

            XmlDocument configDoc = new XmlDocument();
            configDoc.Load(_sourceFile);

            // Process the various component includes...
            ProcessComponents(configDoc);

            // Process the various components...
            ProcessComponentPaths(configDoc);

            // Process the component items...
            ProcessConfigurationItems(configDoc);

            this.ApplyContents(configDoc);

            configDoc.Save(_destFile);
        }

        #endregion

        #region ProcessComponents Method

        protected virtual void ProcessComponents(XmlDocument configDoc)
        {
            XPathNavigator navigator = configDoc.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select("//SandcastleInclude");

            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            int nodeCount = iterator.Count;

            XPathNavigator[] nodeNavigators = new XPathNavigator[iterator.Count];
            for (int i = 0; i < nodeNavigators.Length; i++)
            {
                iterator.MoveNext();
                nodeNavigators[i] = iterator.Current.Clone();
            }

            for (int i = 0; i < nodeCount; i++)
            {
                XPathNavigator nodeNavigator = nodeNavigators[i];
                string configItem = nodeNavigator.GetAttribute("component", String.Empty);
                if (String.IsNullOrEmpty(configItem))
                {
                    continue;
                }

                this.OnComponentInclude(configItem, nodeNavigator);
            }
        }

        #endregion

        #region ProcessComponentPaths Method

        protected virtual void ProcessComponentPaths(XmlDocument configDoc)
        {
            XmlNodeList nodeList = configDoc.SelectNodes("//component");
            //XmlNodeList nodeList = configDoc.SelectNodes("//components/component | //then/component");

            int nodeCount = nodeList.Count;

            for (int i = 0; i < nodeCount; i++)
            {
                XmlNode nodeItem = nodeList[i];
                XmlAttribute attrAssembly = nodeItem.Attributes["assembly"];
                if (attrAssembly != null)
                {
                    string attrValue = attrAssembly.Value;
                    if (_reIsConfigValue.IsMatch(attrValue))
                    {
                        string configKey = attrValue.Substring(2, attrValue.Length - 3);
                        attrAssembly.Value = _dicConfigMap[configKey];
                    }
                }
            }
        }

        #endregion

        #region ProcessConfigurationItems Method

        protected virtual void ProcessConfigurationItems(XmlDocument configDoc)
        {
            XPathNavigator navigator = configDoc.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select("//SandcastleItem");
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

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
        }

        #endregion

        #region WriteSyntaxTypes Method

        protected virtual void WriteSyntaxTypes(XmlWriter xmlWriter, bool includeUsage)
        {
            BuildSettings settings = _context.Settings;
            if (settings == null)
            {
                return;
            }

            BuildSyntaxType syntaxType = _group.SyntaxType;
            if (syntaxType == BuildSyntaxType.None)
            {
                return;
            }

            // <language label="VisualBasic" name="VisualBasic" style="vb" />
            // <language label="CSharp" name="CSharp" style="cs" />
            // <language label="ManagedCPlusPlus" name="ManagedCPlusPlus" style="cpp" />
            // <language label="JSharp" name="JSharp" style="cs" />
            // <language label="JScript" name="JScript" style="cs" />
            if ((syntaxType & BuildSyntaxType.CSharp) != 0)
            {
                xmlWriter.WriteStartElement("language");    // start - language
                xmlWriter.WriteAttributeString("label", "CSharp");
                xmlWriter.WriteAttributeString("name", "CSharp");
                xmlWriter.WriteAttributeString("style", "cs");
                xmlWriter.WriteEndElement();                // end - language
            }
            if ((syntaxType & BuildSyntaxType.VisualBasic) != 0)
            {
                xmlWriter.WriteStartElement("language");    // start - language
                xmlWriter.WriteAttributeString("label", "VisualBasic");
                xmlWriter.WriteAttributeString("name", "VisualBasic");
                xmlWriter.WriteAttributeString("style", "vb");
                xmlWriter.WriteEndElement();                // end - language
            }
            if (includeUsage)
            {   
                xmlWriter.WriteStartElement("language");    // start - language
                xmlWriter.WriteAttributeString("label", "VisualBasicUsage");
                xmlWriter.WriteAttributeString("name", "VisualBasicUsage");
                xmlWriter.WriteAttributeString("style", "vb");
                xmlWriter.WriteEndElement();                // end - language
            }
            if ((syntaxType & BuildSyntaxType.CPlusPlus) != 0)
            {
                xmlWriter.WriteStartElement("language");    // start - language
                xmlWriter.WriteAttributeString("label", "ManagedCPlusPlus");
                xmlWriter.WriteAttributeString("name", "ManagedCPlusPlus");
                xmlWriter.WriteAttributeString("style", "cpp");
                xmlWriter.WriteEndElement();                // end - language
            }
            if ((syntaxType & BuildSyntaxType.JSharp) != 0)
            {
                xmlWriter.WriteStartElement("language");    // start - language
                xmlWriter.WriteAttributeString("label", "JSharp");
                xmlWriter.WriteAttributeString("name", "JSharp");
                xmlWriter.WriteAttributeString("style", "cs");
                xmlWriter.WriteEndElement();                // end - language
            }
            if ((syntaxType & BuildSyntaxType.JScript) != 0)
            {
                xmlWriter.WriteStartElement("language");    // start - language
                xmlWriter.WriteAttributeString("label", "JScript");
                xmlWriter.WriteAttributeString("name", "JScript");
                xmlWriter.WriteAttributeString("style", "cs");
                xmlWriter.WriteEndElement();                // end - language
            }
            if ((syntaxType & BuildSyntaxType.JavaScript) != 0)
            {
                xmlWriter.WriteStartElement("language");    // start - language
                xmlWriter.WriteAttributeString("label", "JavaScript");
                xmlWriter.WriteAttributeString("name", "JavaScript");
                xmlWriter.WriteAttributeString("style", "cs");
                xmlWriter.WriteEndElement();                // end - language
            }
            if ((syntaxType & BuildSyntaxType.Xaml) != 0)
            {
                xmlWriter.WriteStartElement("language");    // start - language
                xmlWriter.WriteAttributeString("label", "XAML");
                xmlWriter.WriteAttributeString("name", "XAML");
                xmlWriter.WriteAttributeString("style", "cs");
                xmlWriter.WriteEndElement();                // end - language
            }
        }

        #endregion

        #region WriteSyntaxGenerators Method

        protected virtual void WriteSyntaxGenerators(XmlWriter xmlWriter, bool includeUsage)
        {
            BuildSettings settings = _context.Settings;
            if (settings == null)
            {
                return;
            }

            BuildSyntaxType syntaxType = _group.SyntaxType;
            if (syntaxType == BuildSyntaxType.None)
            {
                return;
            }

            string syntaxComponents = null;
            if (_context.IsDirectSandcastle &&
                (!String.IsNullOrEmpty(_sandcastleAssistDir) &&
                Directory.Exists(_sandcastleAssistDir)))
            {
                syntaxComponents = Path.Combine(_sandcastleAssistDir,
                    "Sandcastle.BuildAssembler.dll");
            }
            else
            {
                syntaxComponents = Path.Combine(_sandcastleDir,
                    @"ProductionTools\SyntaxComponents.dll");
            }

            //<generator type="Microsoft.Ddue.Tools.VisualBasicDeclarationSyntaxGenerator" 
            //   assembly="%DXROOT%\ProductionTools\SyntaxComponents.dll" />
            //<generator type="Microsoft.Ddue.Tools.CSharpDeclarationSyntaxGenerator" 
            //   assembly="%DXROOT%\ProductionTools\SyntaxComponents.dll" />
            //<generator type="Microsoft.Ddue.Tools.CPlusPlusDeclarationSyntaxGenerator" 
            //   assembly="%DXROOT%\ProductionTools\SyntaxComponents.dll" />
            //<generator type="Microsoft.Ddue.Tools.JSharpDeclarationSyntaxGenerator" 
            //   assembly="%DXROOT%\ProductionTools\SyntaxComponents.dll" />
            //<generator type="Microsoft.Ddue.Tools.ScriptSharpDeclarationSyntaxGenerator" 
            //   assembly="%DXROOT%\ProductionTools\SyntaxComponents.dll" />
            //<generator type="Microsoft.Ddue.Tools.XamlUsageSyntaxGenerator" 
            //   assembly="%DXROOT%\ProductionTools\SyntaxComponents.dll">
            //     <filter files="%DXROOT%\Presentation\Shared\configuration\xamlSyntax.config" />
            // </generator>
            if ((syntaxType & BuildSyntaxType.CSharp) != 0)
            {
                xmlWriter.WriteStartElement("generator");    // start - generator
                xmlWriter.WriteAttributeString("type", 
                    "Microsoft.Ddue.Tools.CSharpDeclarationSyntaxGenerator");
                xmlWriter.WriteAttributeString("assembly", syntaxComponents);
                xmlWriter.WriteEndElement();                // end - generator
            }
            if ((syntaxType & BuildSyntaxType.VisualBasic) != 0)
            {
                xmlWriter.WriteStartElement("generator");    // start - generator
                xmlWriter.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.VisualBasicDeclarationSyntaxGenerator");
                xmlWriter.WriteAttributeString("assembly", syntaxComponents);
                xmlWriter.WriteEndElement();                // end - generator
            }
            if (includeUsage)
            {
                xmlWriter.WriteStartElement("generator");    // start - generator
                xmlWriter.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.VisualBasicUsageSyntaxGenerator");
                xmlWriter.WriteAttributeString("assembly", syntaxComponents);
                xmlWriter.WriteEndElement();                // end - generator
            }
            if ((syntaxType & BuildSyntaxType.CPlusPlus) != 0)
            {
                xmlWriter.WriteStartElement("generator");    // start - generator
                xmlWriter.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.CPlusPlusDeclarationSyntaxGenerator");
                xmlWriter.WriteAttributeString("assembly", syntaxComponents);
                xmlWriter.WriteEndElement();                // end - generator
            }
            if ((syntaxType & BuildSyntaxType.JSharp) != 0)
            {
                xmlWriter.WriteStartElement("generator");    // start - generator
                xmlWriter.WriteAttributeString("type", 
                    "Microsoft.Ddue.Tools.JSharpDeclarationSyntaxGenerator");
                xmlWriter.WriteAttributeString("assembly", syntaxComponents);
                xmlWriter.WriteEndElement();                // end - generator
            }
            if ((syntaxType & BuildSyntaxType.JScript) != 0)
            {
                xmlWriter.WriteStartElement("generator");    // start - generator
                xmlWriter.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.JScriptDeclarationSyntaxGenerator");
                xmlWriter.WriteAttributeString("assembly", syntaxComponents);
                xmlWriter.WriteEndElement();                // end - generator
            }
            if ((syntaxType & BuildSyntaxType.JavaScript) != 0)
            {
                xmlWriter.WriteStartElement("generator");    // start - generator
                xmlWriter.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.ScriptSharpDeclarationSyntaxGenerator");
                xmlWriter.WriteAttributeString("assembly", syntaxComponents);
                xmlWriter.WriteEndElement();                // end - generator
            }
            if ((syntaxType & BuildSyntaxType.Xaml) != 0)
            {
                xmlWriter.WriteStartElement("generator");    // start - generator
                xmlWriter.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.XamlUsageSyntaxGenerator");
                xmlWriter.WriteAttributeString("assembly", syntaxComponents);

                xmlWriter.WriteStartElement("filter");   // start - filter
                xmlWriter.WriteAttributeString("files", Path.Combine(_sandcastleDir,
                    @"Presentation\Shared\configuration\xamlSyntax.config"));
                xmlWriter.WriteEndElement();             // end - filter

                // If the group XAML configuration is created, we use it...
                BuildGroupContext groupContext = _context.GroupContexts[_group.Id];
                if (groupContext == null)
                {
                    throw new BuildException(
                        "The group context is not provided, and it is required by the build system.");
                }
                string xamlConfig = groupContext["$XamlSyntaxFile"];

                if (!String.IsNullOrEmpty(xamlConfig))
                {
                    if (!Path.IsPathRooted(xamlConfig))
                    {
                        xamlConfig = Path.Combine(_context.WorkingDirectory,
                            xamlConfig);
                    }

                    if (File.Exists(xamlConfig))
                    {   
                        xmlWriter.WriteStartElement("filter");   // start - filter
                        xmlWriter.WriteAttributeString("files", xamlConfig);
                        xmlWriter.WriteEndElement();             // end - filter
                    }
                }   

                xmlWriter.WriteEndElement();                // end - generator
            }
        }

        #endregion

        #endregion

        #region Protected Static Methods

        protected static void WriteColorPattern(XmlWriter xmlWriter,
            string pattern, string attribute)
        {
            xmlWriter.WriteStartElement("color");   // start - color
            xmlWriter.WriteAttributeString("pattern", pattern);
            xmlWriter.WriteAttributeString("class", attribute);
            xmlWriter.WriteEndElement();            // end - color
        }

        protected static IList<CodeSnippetContent> GetSnippetContents(BuildGroup group)
        {
            if (group == null)
            {
                return null;
            }

            IList<CodeSnippetContent> listSnippets = group.SnippetContents;

            if (listSnippets == null || listSnippets.Count == 0)
            {
                return null;
            }

            int itemCount = listSnippets.Count;
            List<CodeSnippetContent> validSnippets = new List<CodeSnippetContent>(
                itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                CodeSnippetContent content = listSnippets[i];
                if (content != null && content.IsEmpty == false)
                {
                    if (content.Count == 0)
                    {
                        validSnippets.Add(content);
                    }
                    else
                    {
                        //TODO: Grab the items and serialize them to a file...
                    }
                }
            }

            return validSnippets;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
