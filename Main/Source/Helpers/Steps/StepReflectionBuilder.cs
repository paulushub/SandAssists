using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.References;

namespace Sandcastle.Steps
{
    public sealed class StepReflectionBuilder : StepProcess
    {
        #region Private Fields

        private ReferenceGroup       _group;

        private BuildLoggerLevel     _lastLevel;
        private BuildLoggerVerbosity _verbosity;

        #endregion

        #region Constructors and Destructor

        public StepReflectionBuilder()
        {
            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        public StepReflectionBuilder(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        public StepReflectionBuilder(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        #endregion

        #region Public Properties

        public ReferenceGroup Group
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

            if (!this.OnBeginReflection(context, workingDir, configDir))
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

            if (!this.OnEndReflection(context, workingDir, configDir))
            {
                return false;
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

        #region Private Methods

        private bool OnBeginReflection(BuildContext context,
            string workingDir, string configDir)
        {   
            BuildGroupContext groupContext = context.GroupContexts[_group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string refBuilder         = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder    = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                refBuilder         = Path.Combine(configDir, "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                finalRefBuilder    = Path.Combine(workingDir,
                    groupContext["$ReflectionBuilderFile"]);
            }
            if (!File.Exists(refBuilder))
            {
                refBuilder = String.Empty;
            }

            ReferenceFilterConfigurator filterer = new ReferenceFilterConfigurator();

            try
            {
                filterer.Initialize(context, refBuilderDefAttrs);

                if (!String.IsNullOrEmpty(refBuilder))
                {
                    filterer.Configure(_group, refBuilder, finalRefBuilder);
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

        private bool OnEndReflection(BuildContext context,
            string workingDir, string configDir)
        {
            return true;
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
