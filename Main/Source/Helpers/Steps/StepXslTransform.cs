using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public class StepXslTransform : StepProcess
    {
        #region Private Fields

        private BuildLoggerLevel     _lastLevel;
        private BuildLoggerVerbosity _verbosity;

        #endregion

        #region Constructors and Destructor

        public StepXslTransform()
        {
            this.LogTitle = "Applying Transformation";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }                   

        public StepXslTransform(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.LogTitle = "Applying Transformation";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        public StepXslTransform(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.LogTitle = "Applying Transformation";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                _verbosity = logger.Verbosity;
            }

            bool buildResult = base.OnExecute(context);

            if (buildResult)
            {   
                // For the unexpected case of no argument options to the
                // XslTransformer tool, the exit code is still 0...
                if (_lastLevel == BuildLoggerLevel.Error)
                {
                    return false;
                }
            }

            return buildResult;
        }

        #endregion

        #region OnDataReceived Method

        protected override void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_logger == null || _verbosity == BuildLoggerVerbosity.Quiet)
            {
                return;
            }
            _messageCount++;

            if (_messageCount <= _copyright)
            {
                return;
            }

            string textData = e.Data;
            if (String.IsNullOrEmpty(textData))
            {
                if (!_ignoreWhitespace)
                {
                    _logger.WriteLine(String.Empty, BuildLoggerLevel.None);
                    _lastLevel = BuildLoggerLevel.None;
                }
                return;
            }

            int findPos = textData.IndexOf(':');
            if (findPos <= 0)
            {
                // 1. Check for no options/arguments...
                if (textData.StartsWith("XslTransformer", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }

                // 2. Check for missing or extra assembly directories...
                if (textData.StartsWith("Specify", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }      
                
                _logger.WriteLine(textData, BuildLoggerLevel.Info);
                _lastLevel = BuildLoggerLevel.Info;
                return;
            }

            string levelText   = textData.Substring(0, findPos);
            string messageText = textData.Substring(findPos + 1).Trim();
            if (String.Equals(levelText, "Info"))
            {
                if (_verbosity != BuildLoggerVerbosity.Minimal)
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Info);
                }
                _lastLevel = BuildLoggerLevel.Info;
            }
            else if (String.Equals(levelText, "Warn"))
            {
                _logger.WriteLine(messageText, BuildLoggerLevel.Warn);
                _lastLevel = BuildLoggerLevel.Warn;
            }
            else if (String.Equals(levelText, "Error"))
            {
                _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                _lastLevel = BuildLoggerLevel.Error;
            }
            else
            {
                // Check for invalid options...
                if (String.Equals(levelText, "?", 
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "xsl",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "arg",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "out",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "w",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Info);
                    _lastLevel = BuildLoggerLevel.None;
                }
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
