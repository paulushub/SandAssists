using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Conceptual;
using Sandcastle.References;
using Sandcastle.Configurators;

namespace Sandcastle
{
    public class BuildProject : BuildObject, IDisposable
    {
        #region Private Fields

        private DateTime        _startTime;
        private DateTime        _endTime;

        private bool            _isInitialized;
        //private string          _logFile;
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
            DateTime startTime = DateTime.Now;

            _startTime = startTime;

            // 1. Prepare all the build directories...
            _documenter.BeginDocumentation(_context);

            _settings = _documenter.Settings;

            // 2. Initialize the logger...
            if (!_logger.IsInitialize)
            {
                _logger.Initialize(_context.BaseDirectory, _settings.HelpTitle);
            }

            try
            {
                _logger.WriteLine("Initialization of the documentation.",
                    BuildLoggerLevel.Started);
                _logger.WriteLine("Build started at: " +
                    startTime.ToString(), BuildLoggerLevel.Info);

                if (_isInitialized)
                {
                    _logger.WriteLine("The project is already initialized",
                        BuildLoggerLevel.Warn);

                    return _isInitialized;
                }

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
            finally
            {
                if (_isInitialized)
                {
                    DateTime endTime = DateTime.Now;
                    TimeSpan timeElapsed = endTime - startTime;

                    _logger.WriteLine("Successfully completed in: "
                        + timeElapsed.ToString(), BuildLoggerLevel.Info);
                }

                _logger.WriteLine("Initialization of the documentation.",
                    BuildLoggerLevel.Ended);

                _logger.WriteLine();
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

            buildResult = _documenter.Build();

            return buildResult;
        }

        #endregion

        #region Uninitialize Method

        public virtual void Uninitialize()
        {
            DateTime startTime = DateTime.Now;

            _logger.WriteLine("Completion of the documentation.",
                BuildLoggerLevel.Started);

            _documenter.EndDocumentation(_context);

            try
            {
                _documenter.Uninitialize();
                DateTime endTime = DateTime.Now;
                _endTime = endTime;

                TimeSpan timeElapsed = endTime - startTime;

                _logger.WriteLine("Successfully completed in: " + 
                    timeElapsed.ToString(), BuildLoggerLevel.Info);

                _logger.WriteLine("Build completed at: " +
                    _endTime.ToString(), BuildLoggerLevel.Info);

                timeElapsed = _endTime - _startTime;

                _logger.WriteLine("Total time of completion: " +
                    timeElapsed.ToString(), BuildLoggerLevel.Info);
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex, BuildLoggerLevel.Error);
            }
            finally
            {
                _logger.WriteLine("Completion of the documentation.",
                    BuildLoggerLevel.Ended);
            } 

            try
            {
                _logger.Uninitialize();
            }
            catch
            {              	
            }

            //if (_settings == null)
            //{
            //    return;
            //}

            // Move the log file to the output directory...
            //if (!String.IsNullOrEmpty(_logFile) && File.Exists(_logFile))
            //{
            //    try
            //    {
            //        if (_settings.KeepLogFile)
            //        {
            //            string outputDir = _context.OutputDirectory;
            //            if (String.IsNullOrEmpty(outputDir) == false)
            //            {
            //                outputDir = Environment.ExpandEnvironmentVariables(
            //                    outputDir);
            //                outputDir = Path.GetFullPath(outputDir);
            //                string logFile = Path.Combine(outputDir,
            //                    _settings.LogFile);

            //                File.SetAttributes(_logFile, FileAttributes.Normal);
            //                File.Move(_logFile, logFile);
            //            }
            //        }
            //        else
            //        {
            //            File.SetAttributes(_logFile, FileAttributes.Normal);
            //            File.Delete(_logFile);
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}

            //_logFile = null;
        }

        #endregion

        #endregion

        #region Private Methods

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
