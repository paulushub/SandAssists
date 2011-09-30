using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.References;
using Sandcastle.Conceptual;

namespace Sandcastle.Steps
{
    public sealed class StepAssembler : StepProcess
    {
        #region Private Fields

        private string               _lastMessage;
        private BuildGroup           _group;
        private BuildLoggerLevel     _lastLevel;
        private BuildLoggerVerbosity _verbosity;

        #endregion

        #region Constructors and Destructor

        public StepAssembler()
        {
            this.ConstructorDefaults();
        }

        public StepAssembler(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.ConstructorDefaults();
        }

        public StepAssembler(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.ConstructorDefaults();
        }

        private void ConstructorDefaults()
        {
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
            this.LogTitle = "Assembling a documentation.";
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
            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                _verbosity = logger.Verbosity;
            }

            if (_group == null)
            {
                throw new BuildException(
                    "The build group for this step is required.");
            }

            if (_group.GroupType == BuildGroupType.Reference)
            {
                if (!CreateConfiguration((ReferenceGroup)_group))
                {
                    return false;
                }
            }
            else if (_group.GroupType == BuildGroupType.Conceptual)
            {
                if (!CreateConfiguration((ConceptualGroup)_group))
                {
                    return false;
                }
            }

            bool buildResult = base.OnExecute(context);

            if (buildResult && !String.IsNullOrEmpty(_lastMessage) &&
                _lastMessage.StartsWith("Processed", StringComparison.OrdinalIgnoreCase))
            {
                if (_logger != null && _lastLevel == BuildLoggerLevel.Info 
                    && _verbosity == BuildLoggerVerbosity.Normal 
                    && _verbosity != BuildLoggerVerbosity.Quiet)
                {
                    _logger.WriteLine(_lastMessage, BuildLoggerLevel.Info);
                }

                int startIndex = _lastMessage.IndexOf("topics");

                if (startIndex > 0)
                {
                    int processedTopics;
                    if (Int32.TryParse(_lastMessage.Substring(9,
                        startIndex - 9).Trim(), out processedTopics))
                    {
                        context.AddProcessedTopics(processedTopics);
                    }
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
                }
                return;
            }

            int findPos = textData.IndexOf(':');
            if (findPos <= 0)
            {
                _logger.WriteLine(textData, BuildLoggerLevel.None);
                return;
            }

            string levelText = textData.Substring(0, findPos);
            _lastMessage = textData.Substring(findPos + 1).Trim();
            if (String.Equals(levelText, "Info"))
            {
                if (_verbosity == BuildLoggerVerbosity.Detailed ||
                    _verbosity == BuildLoggerVerbosity.Diagnostic)
                {
                    _logger.WriteLine(_lastMessage, BuildLoggerLevel.Info);
                }
                _lastLevel = BuildLoggerLevel.Info;
            }
            else if (String.Equals(levelText, "Warn"))
            {
                _logger.WriteLine(_lastMessage, BuildLoggerLevel.Warn);
                _lastLevel = BuildLoggerLevel.Warn;
            }
            else if (String.Equals(levelText, "Error"))
            {
                _logger.WriteLine(_lastMessage, BuildLoggerLevel.Error);
                _lastLevel = BuildLoggerLevel.Error;
            }
            else
            {
                _logger.WriteLine(textData, BuildLoggerLevel.None);
                _lastLevel = BuildLoggerLevel.None;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region CreateConfiguration Methods

        private bool CreateConfiguration(ReferenceGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            BuildContext context     = this.Context;

            BuildGroupContext groupContext = context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            BuildLogger logger       = context.Logger;
            BuildSettings settings   = context.Settings;
            BuildStyle outputStyle   = settings.Style;
            BuildStyleType styleType = outputStyle.StyleType;

            string workingDir = context.WorkingDirectory;
            string configDir = settings.ConfigurationDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                if (logger != null)
                {
                    logger.WriteLine(
                        "The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            string configFile      = String.Empty;
            string finalConfigFile = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                configFile = Path.Combine(configDir, 
                    BuildStyle.StyleFolder(styleType) + ".config");
                finalConfigFile = Path.Combine(workingDir, groupContext["$ConfigurationFile"]);
            }
            if (!File.Exists(configFile))
            {
                configFile = String.Empty;
            }

            ReferenceConfigurator assembler = new ReferenceConfigurator();

            try
            {
                assembler.Initialize(context);

                // 3. Configure the build assembler...
                if (!String.IsNullOrEmpty(configFile))
                {
                    assembler.Configure(group, configFile, finalConfigFile);
                }
            }
            finally
            {
                if (assembler != null)
                {
                    assembler.Uninitialize();
                }
            }

            return true;
        }

        private bool CreateConfiguration(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            BuildContext context   = this.Context;
            BuildLogger logger     = context.Logger;
            BuildSettings settings = context.Settings;

            BuildGroupContext groupContext = context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string workingDir = context.WorkingDirectory;
            string configDir  = settings.ConfigurationDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                if (logger != null)
                {
                    logger.WriteLine(
                        "The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            ConceptualConfigurator assembler = new ConceptualConfigurator();

            try
            {
                assembler.Initialize(context);

                string configFile  = String.Empty;
                string finalConfig = String.Empty;
                if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
                {
                    configFile  = Path.Combine(configDir,  "Conceptual.config");
                    finalConfig = Path.Combine(workingDir, groupContext["$ConfigurationFile"]);
                }
                if (!File.Exists(configFile))
                {
                    configFile = String.Empty;
                }

                // 1. Configure the build assembler...
                if (!String.IsNullOrEmpty(configFile))
                {
                    assembler.Configure(group, configFile, finalConfig);
                }

            }
            finally
            {
                if (assembler != null)
                {
                    assembler.Uninitialize();
                }
            }

            return true;
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
