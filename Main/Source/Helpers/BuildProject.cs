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

        private bool            _isInitialized;
        private string          _logFile;
        private BuildLoggers    _logger;
        private BuildContext    _context;
        private BuildSettings   _settings;
        private BuildDocumenter _documenter;

        #endregion

        #region Constructors and Destructor

        public BuildProject()
        {
            _logger     = new BuildLoggers();
            _context    = new BuildContext();
            _documenter = new BuildDocumenter();
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

        public BuildDocumenter Documenter
        {
            get
            {
                return _documenter;
            }
        }

        #endregion

        #region Public Methods

        #region Initialize Method

        public virtual bool Initialize(BuildDocumenter documenter)
        {
            if (documenter != null)
            {
                _documenter = documenter;
            }

            return this.Initialize();
        }

        public virtual bool Initialize()
        {
            if (_isInitialized)
            {
                _logger.WriteLine("The project is already initialized", 
                    BuildLoggerLevel.Warn);

                return _isInitialized;
            }

            _settings = _documenter.Settings;

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

            try
            {
                if (!_documenter.Initialize(_context, _logger))
                {
                    _isInitialized = false;

                    _logger.WriteLine("Error in reference build initialization.",
                        BuildLoggerLevel.Error);
                }
                else
                {
                    _isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                _isInitialized = false;
                _logger.WriteLine(ex, BuildLoggerLevel.Error);
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

            buildResult = _documenter.Build();

            return buildResult;
        }

        #endregion

        #region Uninitialize Method

        public virtual void Uninitialize()
        {
            try
            {
                _documenter.Uninitialize();
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex, BuildLoggerLevel.Error);
            }

            _logger.Uninitialize();

            if (_settings == null)
            {
                return;
            }

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
