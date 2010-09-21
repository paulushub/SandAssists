using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

namespace Sandcastle.Configurators
{
    public class GenericConfigurator : BuildConfigurator
    {
        #region Private Fields

        private string _sourceFile;
        private string _destFile;

        private BuildContext  _context;
        private BuildSettings _settings;

        private ConfiguratorContent _configContent;

        #endregion

        #region Constructors and Destructor

        /// <summary>  
        /// Initializes a new instance of the <see cref="GenericConfigurator"/> class.
        /// </summary>
        public GenericConfigurator()
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

            _context  = context;
            _settings = settings;

            this.IsInitialized = true;
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

            XmlDocument configDoc = new XmlDocument();
            configDoc.Load(_sourceFile);

            if (_configContent != null && _configContent.IsEmpty == false)
            {
                XPathNavigator docNavigator = configDoc.CreateNavigator();
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

            this.ApplyContents(configDoc);

            configDoc.Save(_destFile);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
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

        #endregion
    }
}
