using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Configurations
{
    public class GenericConfigurator : BuildConfigurator
    {
        #region Private Fields

        private bool _isInitialized;
        private string _sourceFile;
        private string _destFile;
        private Dictionary<string, ConfigurationItemHandler> _dicItemMap;

        private BuildEngine _engine;
        private BuildSettings _settings;

        #endregion

        #region Constructors and Destructor

        /// <summary>  
        /// Initializes a new instance of the <see cref="FormatConfigurator"/> class.
        /// </summary>
        public GenericConfigurator()
        {
            _dicItemMap = new Dictionary<string, ConfigurationItemHandler>(
                StringComparer.CurrentCultureIgnoreCase);
        }

        #endregion

        #region Public Properties

        public virtual bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the current help builder associated with this configuration handler.
        /// </summary>
        /// <value>
        /// A <see cref="BuildEngine"/> object instance specifying the
        /// current builder being configured. This is <see langword="null"/> if the
        /// configuration process is not initiated.
        /// </value>
        protected BuildEngine Engine
        {
            get
            {
                return _engine;
            }
        }

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

        public virtual void Initialize(BuildEngine engine)
        {
            BuildExceptions.NotNull(engine, "engine");

            base.Initialize(engine.Logger);

            BuildSettings settings = engine.Settings;
            if (settings == null)
            {
                return;
            }

            _engine   = engine;
            _settings = settings;

            _isInitialized = true;
        }

        public virtual void Configure(string sourceFile, string destFile)
        {
            BuildExceptions.PathMustExist(sourceFile, "sourceFile");
            BuildExceptions.NotNullNotEmpty(destFile, "destFile");

            if (_isInitialized == false)
            {
                throw new InvalidOperationException(
                    "The configurator is not initialized.");
            }
            if (_dicItemMap == null)
            {
                throw new InvalidOperationException("There is not initialization.");
            }

            _sourceFile = sourceFile;
            _destFile = destFile;

            string destDir = Path.GetDirectoryName(destFile);
            if (Directory.Exists(destDir) == false)
            {
                Directory.CreateDirectory(destDir);
            }

            XmlDocument configDoc = new XmlDocument();
            configDoc.Load(_sourceFile);

            if (this.ContentCount > 0)
            {
                this.ApplyContents(configDoc);
            }

            configDoc.Save(_destFile);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            _isInitialized = false;
        }

        public virtual ConfigurationItemHandler GetItem(string keyword)
        {
            if (_dicItemMap == null || String.IsNullOrEmpty(keyword))
            {
                return null;
            }

            ConfigurationItemHandler handler = null;
            if (_dicItemMap.TryGetValue(keyword, out handler))
            {
                return handler;
            }

            return null;
        }

        public virtual void RegisterItem(string keyword, ConfigurationItemHandler handler)
        {
            if (_dicItemMap == null || String.IsNullOrEmpty(keyword) ||
                handler == null)
            {
                return;
            }

            _dicItemMap[keyword] = handler;
        }

        #endregion
    }
}
