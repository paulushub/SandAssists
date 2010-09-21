using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Sandcastle.Contents;

namespace Sandcastle.Configurators
{
    public class SharedContentConfigurator : BuildConfigurator
    {
        #region Private Fields

        private string _sourceFile;
        private string _destFile;

        private RuleContent   _ruleContent;
        private SharedContent _sharedContent;

        private BuildContext  _context;
        private BuildStyle    _style;
        private BuildSettings _settings;
        private BuildEngineType _engineType;
        private ConfiguratorContent _configContent;

        private XmlNamespaceManager _nsmgr;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedContentConfigurator"/> class.
        /// </summary>
        public SharedContentConfigurator()
        {
            _ruleContent   = new RuleContent();
            _sharedContent = new SharedContent();
            _configContent = new ConfiguratorContent();
        }

        #endregion

        #region Public Properties

        public override string Prefix
        {
            get
            {
                return "MSHelp";
            }
        }

        public override string Namespace
        {
            get
            {
                return "http://msdn.microsoft.com/mshelp";
            }
        }

        public RuleContent Rules
        {
            get
            {
                return _ruleContent;
            }
        }

        public SharedContent Contents
        {
            get
            {
                return _sharedContent;
            }
        }

        public ConfiguratorContent Handlers
        {
            get
            {
                return _configContent;
            }
        }

        public override bool HasContents
        {
            get
            {
                if (_style == null)
                {
                    return base.HasContents;
                }

                if (_sharedContent != null && _sharedContent.Count > 0)
                {
                    return true;
                }

                SharedContent content = _style.GetSharedContent(
                    BuildStyle.SharedDefault);
                if (content != null && content.Count > 0)
                {
                    return true;
                }

                if (_engineType == BuildEngineType.Reference)
                {
                    content = _style.GetSharedContent(BuildStyle.SharedReferences);
                    if (content != null && content.Count > 0)
                    {
                        return true;
                    }
                }
                else if (_engineType == BuildEngineType.Conceptual)
                {
                    content = _style.GetSharedContent(BuildStyle.SharedConceptual);
                    if (content != null && content.Count > 0)
                    {
                        return true;
                    }
                }

                return false;
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

        public virtual void Initialize(BuildContext context, 
            BuildEngineType engineType)
        {
            BuildExceptions.NotNull(context, "context");
            base.Initialize(context.Logger);

            BuildSettings settings = context.Settings;
            if (settings == null)
            {
                this.IsInitialized = false;
                return;
            }

            _nsmgr      = null;
            _context    = context;
            _settings   = settings;
            _style      = Settings.Style;
            _engineType = engineType;

            this.IsInitialized    = true;
            this.WarnIfNotFound   = true;
            this.DeleteIfNotFound = true;
        }

        public virtual void Configure(string sourceFile, string destFile)
        {
            BuildExceptions.PathMustExist(sourceFile, "sourceFile");
            BuildExceptions.NotNullNotEmpty(destFile, "destFile");

            if (this.IsInitialized == false)
            {
                throw new BuildException("The configurator is not initialized.");
            }

            _sourceFile = sourceFile;
            _destFile   = destFile;

            string destDir = Path.GetDirectoryName(destFile);
            if (Directory.Exists(destDir) == false)
            {
                Directory.CreateDirectory(destDir);
            }

            XmlDocument document = new XmlDocument();            
            document.Load(_sourceFile);

            _nsmgr = this.GetNamespaceManager(document.NameTable);

            if (_ruleContent != null && _ruleContent.Count != 0)
            {
                this.ApplyRules(document);
            }

            if (_configContent != null && _configContent.IsEmpty == false)
            {   
                XPathNavigator docNavigator = document.CreateNavigator();
                XPathNodeIterator iterator = docNavigator.Select("//SandcastleItem");
                if (iterator != null && iterator.Count != 0)
                {
                    int nodeCount = iterator.Count;

                    XPathNavigator[] navigators = new XPathNavigator[iterator.Count];
                    for (int i = 0; i < navigators.Length; i++)
                    {
                        iterator.MoveNext();
                        navigators[i] = iterator.Current.Clone();
                    }

                    string configKeyword = null;
                    ConfiguratorItem configItem = null;
                    for (int i = 0; i < nodeCount; i++)
                    {
                        XPathNavigator navigator = navigators[i];
                        configKeyword = navigator.GetAttribute("name", String.Empty);
                        if (String.IsNullOrEmpty(configKeyword))
                        {
                            continue;
                        }

                        configItem = _configContent[configKeyword];
                        if (configItem != null)
                        {
                            configItem.Execute(navigator);
                        }
                    }
                }
            }

            this.ApplyContents(document);

            document.Save(_destFile);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            _style = null;
        }

        public virtual ConfiguratorItem GetItem(string keyword)
        {
            if (_configContent == null || String.IsNullOrEmpty(keyword))
            {
                return null;
            }

            return _configContent[keyword];
        }

        public virtual void RegisterItem(string keyword, 
            ConfigurationItemHandler handler)
        {
            if (_configContent == null || String.IsNullOrEmpty(keyword) ||
                handler == null)
            {
                return;
            }

            _configContent.Add(new ConfiguratorItem(keyword, handler));
        }

        public void AddRules(IList<RuleItem> listRules)
        {
            if (_ruleContent == null)
            {
                _ruleContent = new RuleContent();
            }

            if (listRules != null && listRules.Count != 0)
            {
                _ruleContent.Add(listRules);
            }
        }

        #endregion

        #region Protected Methods

        #region GetContent Method

        // look up shared content
        protected override string GetContent(string key, string[] parameters)
        {
            if (String.IsNullOrEmpty(key) || _style == null)
            {
                return base.GetContent(key, parameters);
            }

            bool isFound    = false;
            string value    = String.Empty;
            SharedItem item = null;

            if (_sharedContent != null && _sharedContent.Count > 0)
            {
                item = _sharedContent[key];
                if (item != null)
                {
                    isFound = true;
                    value = item.Value;
                }
            }

            // 2. Consider the shared contents from the groups...
            if (item == null)
            {
                if (_engineType == BuildEngineType.Reference)
                {
                    item = _style[BuildStyle.SharedReferences, key];
                    if (item != null)
                    {
                        isFound = true;
                        value = item.Value;
                    }
                }
                else if (_engineType == BuildEngineType.Conceptual)
                {
                    item = _style[BuildStyle.SharedConceptual, key];
                    if (item != null)
                    {
                        isFound = true;
                        value = item.Value;
                    }
                }
            }

            // 3. If not found, consider a common/default contents.... 
            if (item == null)
            {
                item = _style[key];
                if (item != null)
                {
                    isFound = true;
                    value = item.Value;
                }
            }

            if (isFound)
            {
                if (parameters != null && parameters.Length != 0)
                {
                    try
                    {
                        value = String.Format(value, parameters);
                    }
                    catch
                    {
                        LogMessage(BuildLoggerLevel.Error, String.Format(
                            "The shared content item '{0}' could not be formatted with {1} parameters.",
                            key, parameters.Length));
                    }
                }

                return value;
            }

            return base.GetContent(key, parameters);
        }

        #endregion

        #region ApplyRules Methods

        protected virtual void ApplyRules(XmlDocument document)
        {
            BuildExceptions.NotNull(document, "document");
            if (_ruleContent == null || _ruleContent.IsEmpty)
            {
                return;
            }

            XPathNavigator navigator = document.CreateNavigator();

            BuildLogger logger = _context.Logger;

            int itemCount = _ruleContent.Count;
            for (int i = 0; i < itemCount; i++)
            {
                RuleItem item = _ruleContent[i];
                if (item == null || item.IsEmpty)
                {
                    continue;
                }

                string pathText = String.Format("//ItemRule[@name='{0}']", item.Name);

                XPathNavigator itemNavigator = navigator.SelectSingleNode(
                    pathText, _nsmgr);
                if (itemNavigator == null)
                {      
                    if (logger != null)
                    {
                        logger.WriteLine(String.Format(
                            "The rule item '{0}' is not found.", 
                            item.Name), BuildLoggerLevel.Warn);
                    }
                    continue;
                }

                XmlWriter writer = itemNavigator.InsertAfter();
                string itemValue = item.Value;
                XPathNavigator matchNavigator = null;
                if (!String.IsNullOrEmpty(itemValue))
                {
                    pathText = String.Format(
                        "If[@value='{0}'] | ElseIf[@value='{0}']", itemValue);

                    matchNavigator = itemNavigator.SelectSingleNode(pathText, _nsmgr);
                }
                if (String.IsNullOrEmpty(itemValue) || matchNavigator == null)
                {                       
                    matchNavigator = itemNavigator.SelectSingleNode("Else", _nsmgr);
                }

                if (matchNavigator != null)
                {
                    XmlNodeType nodeType = XmlNodeType.None;
                    string nodeName = String.Empty;
                    // Obtain XmlReader for the current node and its child nodes
                    XmlReader reader = matchNavigator.ReadSubtree();
                    // The state would be initial, we call read to move the the
                    // current node of the matched-navigation.
                    if (reader.ReadState == ReadState.Initial || 
                        String.IsNullOrEmpty(reader.Name))
                    {
                        reader.Read();
                    }

                    int depth = reader.Depth;
                    if (depth == 0)
                    {
                        depth = 1;
                    }
                    string spaceText = Environment.NewLine + new string(' ', 4 * depth);
                    while (reader.Read())
                    {
                        nodeType = reader.NodeType;
                        if (nodeType == XmlNodeType.Element)
                        {
                            writer.WriteNode(reader, true);
                            writer.WriteRaw(spaceText);
                        }
                        else if (nodeType == XmlNodeType.Whitespace)
                        {
                        }
                        else if (nodeType == XmlNodeType.SignificantWhitespace)
                        {
                        }
                        else if (nodeType == XmlNodeType.Comment)
                        {
                        }
                        else if (nodeType == XmlNodeType.EndElement)
                        {
                            nodeName = reader.Name;
                            if (String.Equals(nodeName, "If")   ||
                                String.Equals(nodeName, "Else") ||
                                String.Equals(nodeName, "ElseIf"))
                            {
                                break;
                            }
                        }
                    }

                    reader.Close();
                }

                writer.Close();
                itemNavigator.DeleteSelf();
            }
        }

        #endregion

        #endregion
    }
}
