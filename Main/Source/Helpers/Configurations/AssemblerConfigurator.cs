using System;
using System.IO;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using System.Text;
using System.Text.RegularExpressions;

using Sandcastle.Contents;

namespace Sandcastle.Configurations
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for configuration handlers used in
    /// editing or modifying the build assembler tool configuration files.
    /// </summary>
    public abstract class AssemblerConfigurator : BuildConfigurator
    {
        #region Private Fields

        private string _sourceFile;
        private string _destFile;
        private ConfigurationContent _configContent;
        private Dictionary<string, string> _dicConfigMap;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblerConfigurator"/> class.
        /// </summary>
        protected AssemblerConfigurator()
        {
            // 1. Create the dictionary for mapping the handlers...
            _dicConfigMap = new Dictionary<string, string>(
                StringComparer.CurrentCultureIgnoreCase);

            _configContent = new ConfigurationContent();

            // 2. Create the default component handlers...
            //Keyword: "$(SandcastleComponent)";
            string sandcastlePath = Environment.ExpandEnvironmentVariables("%DXROOT%");
            if (String.IsNullOrEmpty(sandcastlePath) == false ||
                Directory.Exists(sandcastlePath))
            {
                string sandcastleComponents =
                    @"%DXROOT%\ProductionTools\BuildComponents.dll";
                _dicConfigMap.Add("SandcastleComponent", sandcastleComponents);
            }
        }

        #endregion

        #region Public Properties

        public ConfigurationContent Handlers
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
        protected abstract BuildSettings Settings
        {
            get;
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

            string sandcastlePath = "%DXROOT%";
            string fullPath = Environment.ExpandEnvironmentVariables(sandcastlePath);
            if (String.IsNullOrEmpty(fullPath) ||
                Directory.Exists(fullPath) == false)
            {
                sandcastlePath = settings.SandcastleDirectory;
            }
            // Make sure the default component handlers are added...
            //Keyword: "$(SandcastleComponent)";
            if (_dicConfigMap.ContainsKey("SandcastleComponent") == false)
            {
                if (String.IsNullOrEmpty(sandcastlePath) == false ||
                    Directory.Exists(sandcastlePath))
                {
                    string sandcastleComponents = Path.Combine(sandcastlePath,
                        @"ProductionTools\BuildComponents.dll");
                    _dicConfigMap.Add("SandcastleComponent", sandcastleComponents);
                }
            }

            //Keyword: "$(SandAssistComponent)";
            if (_dicConfigMap.ContainsKey("SandAssistComponent") == false)
            {
                string sandcastleAssist = settings.SandAssistDirectory;
                if (String.IsNullOrEmpty(sandcastleAssist) == false ||
                    Directory.Exists(sandcastleAssist))
                {
                    string assistComponents = Path.Combine(sandcastleAssist,
                        "Sandcastle.Components.dll");
                    if (File.Exists(assistComponents))
                    {
                        _dicConfigMap.Add("SandAssistComponent", assistComponents);
                    }
                    else
                    {
                        assistComponents = Path.Combine(sandcastleAssist,
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

        public virtual ConfigurationItem GetConfigurationItem(string keyword)
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

            string assemblyFile = null;

            if (_dicConfigMap.TryGetValue(keyword, out assemblyFile))
            {
                return assemblyFile;
            }

            return null;
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
            ConfigurationItemHandler handler)
        {
            if (_configContent == null || String.IsNullOrEmpty(keyword) ||
                handler == null)
            {
                return;
            }

            _configContent.Add(new ConfigurationItem(keyword, handler));
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

        protected abstract void OnComponentInclude(object sender,
            ConfigurationItemEventArgs args);

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
            ProcessComponentIncludes(configDoc);

            // Process the various components...
            ProcessComponents(configDoc);

            // Process the component items...
            ProcessConfigurationItems(configDoc);

            this.ApplyContents(configDoc);

            configDoc.Save(_destFile);
        }

        #endregion

        #region ProcessComponentIncludes Method

        protected virtual void ProcessComponentIncludes(XmlDocument configDoc)
        {
            XPathNavigator navigator = configDoc.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select("//SandcastleInclude");

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

                ConfigurationItemEventArgs args = new ConfigurationItemEventArgs(configItem,
                    nodeNavigator);

                this.OnComponentInclude(this, args);
            }
        }

        #endregion

        #region ProcessComponents Method

        protected virtual void ProcessComponents(XmlDocument configDoc)
        {
            Regex reIsConfigValue = new Regex(@"^\$\(([^\$\(\)]*)\)$");

            XmlNodeList nodeList = configDoc.SelectNodes("//component");
            //XmlNodeList nodeList = configDoc.SelectNodes("//components/component | //then/component");

            int nodeCount = nodeList.Count;

            string configKey   = null;
            string configValue = null;
            for (int i = 0; i < nodeCount; i++)
            {
                XmlNode nodeItem = nodeList[i];
                XmlAttribute attrAssembly = nodeItem.Attributes["assembly"];
                if (attrAssembly != null)
                {
                    string attrValue = attrAssembly.Value;
                    if (reIsConfigValue.IsMatch(attrValue))
                    {
                        configKey = attrValue.Substring(2, attrValue.Length - 3);
                        if (_dicConfigMap.TryGetValue(configKey, out configValue))
                        {
                            attrAssembly.Value = configValue;
                        }
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
            ConfigurationItem configItem = null;
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
            BuildSettings settings = this.Settings;
            if (settings == null)
            {
                return;
            }

            BuildSyntaxType syntaxType = settings.SyntaxType;
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
            BuildSettings settings = this.Settings;
            if (settings == null)
            {
                return;
            }

            BuildSyntaxType syntaxType = settings.SyntaxType;
            if (syntaxType == BuildSyntaxType.None)
            {
                return;
            }
            string sandcastleDir = settings.SandcastleDirectory;
            if (String.IsNullOrEmpty(sandcastleDir))
            {
                sandcastleDir = "%DXROOT%";
            }
            string syntaxComponents = Path.Combine(sandcastleDir,
                @"ProductionTools\SyntaxComponents.dll");

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
                xmlWriter.WriteAttributeString("files", Path.Combine(sandcastleDir,
                    @"Presentation\Shared\configuration\xamlSyntax.config"));
                xmlWriter.WriteEndElement();             // end - filter

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

        protected static IList<SnippetContent> GetSnippetContents(BuildGroup group)
        {
            if (group == null)
            {
                return null;
            }

            IList<SnippetContent> listSnippets = group.SnippetContents;

            if (listSnippets == null || listSnippets.Count == 0)
            {
                return null;
            }

            int itemCount = listSnippets.Count;
            List<SnippetContent> validSnippets = new List<SnippetContent>(
                itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                SnippetContent content = listSnippets[i];
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
