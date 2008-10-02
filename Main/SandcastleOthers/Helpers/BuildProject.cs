using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Conceptual;
using Sandcastle.References;
using Sandcastle.Configurations;

namespace Sandcastle
{
    public class BuildProject : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private bool             _isInitialized;
        private string           _logFile;
        private BuildLoggers      _logger;
        private BuildContext     _context;
        private BuildSettings    _settings;

        private ReferenceEngine  _referenceEngine;
        private ConceptualEngine _conceptualEngine;

        private BuildConfiguration _configuration;

        #endregion

        #region Constructors and Destructor

        public BuildProject()
        {
            _logger   = new BuildLoggers();
            _settings = new BuildSettings();
            _context  = new BuildContext();
            _configuration = new BuildConfiguration();

            _referenceEngine  = new ReferenceEngine(_settings, _logger,
                _context, _configuration);
            _conceptualEngine = new ConceptualEngine(_settings, _logger,
                _context, _configuration);
        }

        ~BuildProject()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsInitialize
        {
            get
            {
                return _isInitialized;
            }
        }

        public BuildLoggers Logger
        {
            get
            {
                return _logger;
            }
        }

        public BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        public BuildSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public ReferenceEngine Reference
        {
            get
            {
                return _referenceEngine;
            }
        }

        public ConceptualEngine Conceptual
        {
            get
            {
                return _conceptualEngine;
            }
        }

        public BuildConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        #endregion

        #region Public Methods

        #region AddGroup Method

        public bool AddGroup(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");
            ReferenceGroup apiGroup = group as ReferenceGroup;
            if (apiGroup != null)
            {
                if (_referenceEngine != null)
                {
                    _referenceEngine.AddGroup(apiGroup);
                }

                return true;
            }

            ConceptualGroup topicsGroup = group as ConceptualGroup;
            if (topicsGroup != null)
            {
                if (_conceptualEngine != null)
                {
                    _conceptualEngine.AddGroup(topicsGroup);
                }

                return true;
            }

            throw new ArgumentException("The specified group is not supported", "group");
        }

        public bool AddGroup(ReferenceGroup group)
        {
            BuildExceptions.NotNull(group, "group");
            if (_referenceEngine != null)
            {
                _referenceEngine.AddGroup(group);

                return true;
            }

            return false;
        }

        public bool AddGroup(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");
            if (_conceptualEngine != null)
            {
                _conceptualEngine.AddGroup(group);

                return true;
            }

            return false;
        }

        #endregion

        #region Initialize Method

        public virtual bool Initialize()
        {
            if (_isInitialized)
            {
                _logger.WriteLine("The project is already initialized", 
                    BuildLoggerLevel.Warn);

                return _isInitialized;
            }

            // 1. If there is no logger, we try creating a default logger...
            if (_logger.Count == 0)
            {
                BuildLogger logger = _context.CreateLogger(_settings);
                if (logger != null)
                {
                    _logger.Add(logger);
                }
            }

            // 2. If the logger is not initialized, we initialize it now...
            if (!_logger.IsInitialize)
            {
                _logger.Initialize(_settings);
            }

            _isInitialized = true;

            if (_isInitialized && _settings.BuildReferences)
            {
                try
                {
                    if (!_referenceEngine.Initialize(_settings))
                    {
                        _isInitialized = false;

                        _logger.WriteLine("Error in reference build initialization.",
                            BuildLoggerLevel.Error);
                    }
                }
                catch (Exception ex)
                {
                    _isInitialized = false;
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            if (_isInitialized && _settings.BuildConceptual)
            {
                try
                {
                    if (!_conceptualEngine.Initialize(_settings))
                    {
                        _isInitialized = false;

                        _logger.WriteLine("Error in reference build initialization.",
                            BuildLoggerLevel.Error);
                    }
                }
                catch (Exception ex)
                {
                    _isInitialized = false;
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            return _isInitialized;
        }

        #endregion

        #region Build Method

        public virtual bool Build()
        {
            bool buildResult = false;

            if (_isInitialized == false)
            {
                _logger.WriteLine("The project must be initialized before building.",
                    BuildLoggerLevel.Error);

                return buildResult;
            }

            if (!_logger.IsInitialize)
            {
                _logger.Initialize(_settings);
            }

            buildResult = true;

            if (buildResult && _settings.BuildReferences)
            {   
                try
                {
                    // Second, initialize the working (or your project directory)
                    // and build...
                    if (_referenceEngine.IsInitialized)
                    {   
                        // Third, build, only the batch build is implemented.
                        buildResult = _referenceEngine.Build();
                    }
                    else
                    {   
                        if (_referenceEngine.Initialize(_settings))
                        {
                            // Third, build, only the batch build is implemented.
                            buildResult = _referenceEngine.Build();
                        }
                        else
                        {
                            buildResult = false;

                            _logger.WriteLine("Error in reference build initialization.",
                                BuildLoggerLevel.Error);
                        }

                        _referenceEngine.Uninitialize();
                    }
                }
                catch (Exception ex)
                {
                    buildResult = false;

                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            if (buildResult && _settings.BuildConceptual)
            {   
                try
                {
                    // Second, initialize the working (or your project directory)
                    // and build...
                    if (_conceptualEngine.IsInitialized)
                    {   
                        // Third, build, only the batch build is implemented.
                        buildResult = _conceptualEngine.Build();
                    }
                    else
                    {
                        if (_conceptualEngine.Initialize(_settings))
                        {
                            // Third, build, only the batch build is implemented.
                            buildResult = _conceptualEngine.Build();
                        }
                        else
                        {
                            buildResult = false;

                            _logger.WriteLine("Error in reference build initialization.",
                                BuildLoggerLevel.Error);
                        }

                        _conceptualEngine.Uninitialize();
                    }
                }
                catch (Exception ex)
                {
                    buildResult = false;

                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            return buildResult;
        }

        #endregion

        #region Uninitialize Method

        public virtual void Uninitialize()
        {
            if (_logger.IsInitialize)
            {
                _logger.Initialize(_settings);
            }

            try
            {
                _referenceEngine.Uninitialize();
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex, BuildLoggerLevel.Error);
            }

            try
            {
                _conceptualEngine.Uninitialize();
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex, BuildLoggerLevel.Error);
            }

            _logger.Uninitialize();

            // Move the log file to the output directory...
            if (!String.IsNullOrEmpty(_logFile) && File.Exists(_logFile))
            {
                try
                {
                    if (_settings.KeepLogFile)
                    {
                        string outputDir = _settings.OutputDirectory;
                        if (String.IsNullOrEmpty(outputDir) == false)
                        {
                            outputDir = Environment.ExpandEnvironmentVariables(
                                outputDir);
                            outputDir = Path.GetFullPath(outputDir);
                            string logFile = Path.Combine(outputDir, 
                                _settings.LogFile);

                            File.SetAttributes(_logFile, FileAttributes.Normal);
                            File.Move(_logFile, logFile);
                        }
                    }
                    else
                    {
                        File.SetAttributes(_logFile, FileAttributes.Normal);
                        File.Delete(_logFile);
                    }
                }
                catch
                {
                }
            }

            _logFile = null;
        }

        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isInitialized = false;

            if (_referenceEngine != null)
            {
                _referenceEngine.Dispose();
                _referenceEngine = null;
            }
            if (_conceptualEngine != null)
            {
                _conceptualEngine.Dispose();
                _conceptualEngine = null;
            }

            if (_logger != null)
            {
                _logger.Dispose();
                _logger = null;
            }

            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        #endregion
    }
}
