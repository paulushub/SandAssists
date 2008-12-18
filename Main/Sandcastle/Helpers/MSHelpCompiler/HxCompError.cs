using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Sandcastle.MSHelpCompiler
{
    internal sealed class HxCompError : IHxCompError
    {
        #region Private Fields

        private int          _adviseCookie;
        private bool         _isSuccess;
        private BuildLogger _logger;
        private BuildLoggerVerbosity _verbosity;
        
        #endregion

        #region Constructors and Destructor

        public HxCompError(BuildLogger logger)
        {
            _isSuccess = true;
            _logger    = logger;
            if (logger != null)
            {
                _verbosity = logger.Verbosity;
            }
        }

        public HxCompError(BuildLogger logger, BuildLoggerVerbosity verbosity)
        {
            _isSuccess = true;
            _logger    = logger;
            _verbosity = verbosity;
        }

        #endregion

        #region Public Properties

        public bool IsSuccess
        {
            get
            {
                return _isSuccess;
            }
        }

        #endregion

        #region Public Methods

        public void Attach(HxComp hxCompiler)
        {
            BuildExceptions.NotNull(hxCompiler, "hxCompiler");

            _adviseCookie = hxCompiler.AdviseCompilerMessageCallback(this);
        }

        public void Detach(HxComp hxCompiler)
        {
            BuildExceptions.NotNull(hxCompiler, "hxCompiler");

            hxCompiler.UnadviseCompilerMessageCallback(_adviseCookie);
        }

        #endregion

        #region IHxCompError Members

        public void ReportError(string TaskItemString, string Filename, 
            int nLineNum, int nCharNum, HxCompErrorSeverity Severity, 
            string DescriptionString)
        {
            if (_logger == null)
            {
                return;
            }

            switch (Severity)
            {
                case HxCompErrorSeverity.Information:
                    if (_verbosity == BuildLoggerVerbosity.Normal   ||
                        _verbosity == BuildLoggerVerbosity.Detailed ||
                        _verbosity == BuildLoggerVerbosity.Diagnostic)
                    {
                        _logger.WriteLine(DescriptionString, BuildLoggerLevel.Info);
                    }
                    break;
                case HxCompErrorSeverity.Warning:
                    _logger.WriteLine(DescriptionString, BuildLoggerLevel.Warn);
                    break;
                case HxCompErrorSeverity.Error:
                case HxCompErrorSeverity.Fatal:
                    _isSuccess = false;
                    _logger.WriteLine(DescriptionString, BuildLoggerLevel.Error);
                    break;
            }
        }

        public void ReportMessage(HxCompErrorSeverity Severity, 
            string DescriptionString)
        {
            if (_logger == null)
            {
                return;
            }

            switch (Severity)
            {
                case HxCompErrorSeverity.Information:
                    if (_verbosity == BuildLoggerVerbosity.Normal   ||
                        _verbosity == BuildLoggerVerbosity.Detailed ||
                        _verbosity == BuildLoggerVerbosity.Diagnostic)
                    {
                        _logger.WriteLine(DescriptionString, BuildLoggerLevel.Info);
                    }
                    break;
                case HxCompErrorSeverity.Warning:
                    _logger.WriteLine(DescriptionString, BuildLoggerLevel.Warn);
                    break;
                case HxCompErrorSeverity.Error:
                case HxCompErrorSeverity.Fatal:
                    _isSuccess = false;
                    _logger.WriteLine(DescriptionString, BuildLoggerLevel.Error);
                    break;
            }
        }

        public HxCompStatus QueryStatus()
        {
            // TODO: For now, just continue...
            return HxCompStatus.Continue;
        }

        #endregion
    }
}
