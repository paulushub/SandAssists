using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.References;

namespace Sandcastle.Steps
{
    public class StepMrefBuilder : StepProcess
    {
        #region Private Fields

        private BuildGroup _group;

        private BuildLoggerLevel     _lastLevel;
        private BuildLoggerVerbosity _verbosity;

        #endregion

        #region Constructors and Destructor

        public StepMrefBuilder()
        {
            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        public StepMrefBuilder(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        public StepMrefBuilder(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        #endregion

        #region Public Properties

        public BuildGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            if (_group == null)
            {
                throw new BuildException(
                    "The build group for this step is required.");
            }

            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                _verbosity = logger.Verbosity;
            }

            ReferenceGroup group = _group as ReferenceGroup;
            if (group == null)
            {
                if (logger != null)
                {
                    logger.WriteLine("There is no reference group associated with this step.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            BuildSettings settings = context.Settings;
            BuildStyle outputStyle = settings.Style;
                                           
            string workingDir = context.WorkingDirectory;
            string configDir  = settings.ConfigurationDirectory;

            if (String.IsNullOrEmpty(workingDir))
            {
                if (logger != null)
                {
                    logger.WriteLine("The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            if (!this.CreateConfiguration(context, group, workingDir, configDir))
            {
                return false;
            } 

            bool buildResult = base.OnExecute(context);

            if (buildResult)
            {
                // For the unexpected case of no argument options to the
                // MRefBuilder tool, the exit code is still 0...
                if (_lastLevel == BuildLoggerLevel.Error)
                {
                    return false;
                }
            }

            return buildResult;
        }

        #endregion

        #region CreateConfiguration Method

        protected virtual bool CreateConfiguration(BuildContext context,
            ReferenceGroup group, string workingDir, string configDir)
        {
            BuildExceptions.NotNull(context, "context");
            BuildExceptions.NotNull(group, "group");

            string refBuilder         = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder    = String.Empty;
            if (String.IsNullOrEmpty(configDir) == false && 
                Directory.Exists(configDir))
            {
                refBuilder         = Path.Combine(configDir,  "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir,  "MRefBuilder.xml");
                finalRefBuilder    = Path.Combine(workingDir,
                    group["$ReflectionBuilderFile"]);
            }
            if (File.Exists(refBuilder) == false)
            {
                refBuilder = String.Empty;
            }

            ReferenceFilterConfigurator filterer = new ReferenceFilterConfigurator();

            try
            {
                filterer.Initialize(context, refBuilderDefAttrs);

                if (!String.IsNullOrEmpty(refBuilder))
                {
                    filterer.Configure(group, refBuilder, finalRefBuilder);
                }
            }
            finally
            {
                if (filterer != null)
                {
                    filterer.Uninitialize();
                }
            }

            return true;
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
                if (textData.StartsWith("MRefBuilder", StringComparison.OrdinalIgnoreCase))
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

                // 3. Check for valid target platform...
                if (textData.StartsWith("Unknown target", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }

                _logger.WriteLine(textData, BuildLoggerLevel.Info);
                _lastLevel = BuildLoggerLevel.Info;
                return;
            }

            string levelText = textData.Substring(0, findPos);
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
                else if (String.Equals(levelText, "out",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "config",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "dep",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "internal",
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
